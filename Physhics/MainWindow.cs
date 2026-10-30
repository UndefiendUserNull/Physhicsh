using ImGuiNET;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Physhicsh.PhysicsObjects;
using Physhicsh.UI;
using Raylib_cs;
using rlImGui_cs;
using System.Diagnostics;

namespace Physhicsh;

public class MainWindow
{
    private readonly List<PhysicsObject> _physicsObjects = [];
    private readonly Random _rng = new();
    private readonly ImGuiVault _imgVault = new();
    private World _world = null!;
    private float _accumulator;
    private float _gravityScale = 5f;
    private PhysicsObject _pickedObject = null!;
    private PhysicsObject? _propertiesObject;
    private TextureLoader _textureLoader = new();
    private ImGuiUI _ui = null!;
    private Color _currentBgColor = Color.Black;
    private List<Action> _imGuiWindows = [];
    private Input _input;
    private SolverIterations _solver = new();
    private readonly HashSet<Timer> _timers = [];
    private Stopwatch _sw;
    private double _physicsTime;
    private double _inputTime;
    private double _updateObjTime;

    public void Start()
    {
        _sw = Stopwatch.StartNew();
        _imGuiWindows = [DrawPropertiesWindow, DrawSettingsWindow];

        Raylib.InitWindow(Config.Instance.SCREEN_WIDTH, Config.Instance.SCREEN_HEIGHT, "Physhicsh");
        Raylib.SetConfigFlags(ConfigFlags.VSyncHint);
        Raylib.SetWindowFocused();
        Raylib.SetTargetFPS(Config.Instance.MAX_FPS);

        LoadFont();

        rlImGui.Setup(true);

        _world = new World(new Vector2(0, -9.81f * _gravityScale));

        _world.ContactManager.VelocityConstraintsMultithreadThreshold = 128;
        _world.ContactManager.PositionConstraintsMultithreadThreshold = 128;

        _input = new(_world, _physicsObjects);
        _solver = new()
        {
            VelocityIterations = 8,
            PositionIterations = 8,
        };
        SpawnDemoObjects();

        while (!Raylib.WindowShouldClose())
        {
            Update();
            GraphicsUpdate();
        }

        Raylib.CloseWindow();
    }

    private void LoadFont()
    {
        // Try to load Arial, if doesn't exist, load whatever font in there
        rlImGui.SetupUserFonts += io =>
        {
            if (File.Exists(_imgVault.IMGUI_FONT_PATH))
            {
                _ui = new(io.Fonts.AddFontFromFileTTF(
                _imgVault.IMGUI_FONT_PATH,
                18.0f));
            }
            else
            {
                if (!Directory.Exists("fonts")) throw new FileNotFoundException("No fonts folder were found");

                IEnumerable<string> fontsDir = Directory.EnumerateFiles("fonts");

                if (!fontsDir.Any()) throw new FileNotFoundException("Empty fonts folder");

                for (int i = 0; i < fontsDir.Count(); i++)
                {
                    string foundFile = fontsDir.ElementAt(i);
                    if (foundFile.EndsWith(".ttf"))
                    {
                        _ui = new(io.Fonts.AddFontFromFileTTF(
                        foundFile,
                        18.0f));
                        break;
                    }
                }
            }

        };
    }

    // Scene -> SpawnObjects
    private void SpawnDemoObjects()
    {
        _physicsObjects.Add(new CirclePhysicsObject(0, 10, 0.5f, Color.Green));
        _physicsObjects.Add(new RectanglePhysicsObject(0, 0, 1, 1, Color.DarkGray, 1));
        _physicsObjects.Add(new RectanglePhysicsObject(5, 0, 1, 1, Color.DarkGray, 1));
        _physicsObjects.Add(new RectanglePhysicsObject(0, -5, 10, 2, Color.SkyBlue, bodyType: BodyType.Static));
        _physicsObjects.Add(new RectanglePhysicsObject(0, -10, 100, 2, Color.SkyBlue, bodyType: BodyType.Static));
        _physicsObjects.Add(new RectanglePhysicsObject(20, -10, 1, 100, Color.SkyBlue, bodyType: BodyType.Static));
        _physicsObjects.Add(new RectanglePhysicsObject(-20, -10, 1, 100, Color.SkyBlue, bodyType: BodyType.Static));

        foreach (PhysicsObject obj in _physicsObjects)
            obj.Start(_world);
    }


    private void Update()
    {
        _sw.Restart();
        StepPhysics(Raylib.GetFrameTime());
        _physicsTime = _sw.Elapsed.TotalMilliseconds;

        _sw.Restart();
        _input.UpdateMouse();
        _input.HandleMouseInput(_ui, _world, ref _pickedObject);
        _input.HandleShortcuts(_ui, ref _pickedObject, DeletePhysicsObject);
        _inputTime = _sw.Elapsed.TotalMilliseconds;

        _sw.Restart();
        UpdatePhysicsObjects();
        _updateObjTime = _sw.Elapsed.TotalMilliseconds;


        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            Vector2 worldPos = _input.MousePosWorld;

            _ui.ShowPopup("##world_context", Raylib.GetMousePosition(), mb =>
            {
                mb.Item("Settings", () => _imgVault.Open(ImGuiWindow.Settings));

                mb.Separator();

                mb.SubMenu("Quick spawn", (x) => ImGuiStaticPopups.DrawQuickSpawnMenu(_world, worldPos, x, _physicsObjects));

                if (_pickedObject is not null)
                {
                    mb.Separator();
                    mb.Item("Delete", () => DeletePhysicsObject(_pickedObject));
                    mb.Item("Properties", () =>
                    {
                        _propertiesObject = _pickedObject;
                        _imgVault.Open(ImGuiWindow.Properties);
                    });
                }

            });
        }
    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(_currentBgColor);

        rlImGui.Begin();

        _ui.BeginFrame();

        foreach (Action window in _imGuiWindows)
        {
            window();
        }

        if (_input.ShowDebugWindow)
            DrawDebugWindow();

        _ui.EndFrame();

        DrawPhysicsObjects();


        rlImGui.End();
        Raylib.EndDrawing();
    }

    private void DrawSettingsWindow()
    {
        if (!_imgVault.IsOpen(ImGuiWindow.Settings)) return;

        _ui.BeginWindow("Settings", ImGuiWindowFlags.NoCollapse, () =>
        {
            if (ImGui.SmallButton("X"))
            {
                _imgVault.Close(ImGuiWindow.Settings);
                return;
            }

            System.Numerics.Vector3 currentBgColor = _currentBgColor.ToSysVector3();

            if (ImGui.ColorEdit3("Background Color", ref currentBgColor))
            {
                _currentBgColor = currentBgColor.ToColor();
            }

            int gravityRefSlider = (int)_gravityScale;

            if (ImGui.SliderInt("Gravity scale", ref gravityRefSlider, -10, 10))
            {
                _gravityScale = gravityRefSlider;
                _world.Gravity = new(0, -9.81f * _gravityScale);
            }

            if (ImGui.CollapsingHeader("Spawn Settings"))
            {
                System.Numerics.Vector2 circlesRadiusRefSlider = SpawnRules.Instance.CirclesRadius;
                System.Numerics.Vector2 rectanglesSizeRefSlider = SpawnRules.Instance.RectSize;
                bool useRandomColorsRefBool = SpawnRules.Instance.UseRandomColors;
                System.Numerics.Vector3 spawnColorRefColor = SpawnRules.Instance.SpawnColor.ToSysVector3();

                if (ImGui.SliderFloat2("Circles radius", ref circlesRadiusRefSlider, 0.01f, 2f))
                {
                    SpawnRules.Instance.CirclesRadius = circlesRadiusRefSlider;
                }

                if (ImGui.SliderFloat2("Rectangles size", ref rectanglesSizeRefSlider, 0.1f, 5f))
                {
                    SpawnRules.Instance.RectSize = rectanglesSizeRefSlider;
                }

                if (ImGui.Checkbox("Random Colors", ref useRandomColorsRefBool))
                {
                    SpawnRules.Instance.UseRandomColors = useRandomColorsRefBool;
                }

                if (!SpawnRules.Instance.UseRandomColors && ImGui.ColorEdit3("Spawn Color", ref spawnColorRefColor))
                {
                    SpawnRules.Instance.SpawnColor = spawnColorRefColor.ToAetherVector3().ToColor();
                }

            }

            if (ImGui.CollapsingHeader("VERY DANGEROUS !!!"))
            {
                ImGui.TextColored(new(255, 0, 0, 255), "All buttons here doesn't have confirmation.");
                if (ImGui.Button("Clear All"))
                {
                    _physicsObjects.Clear();
                }
            }
        });
    }

    private void DeletePhysicsObject(PhysicsObject obj)
    {
        obj.Delete();
        _physicsObjects.Remove(obj);

        if (_pickedObject == _propertiesObject)
            _propertiesObject = null;
        if (ReferenceEquals(_pickedObject, obj))
            _pickedObject = null!;
    }



    private void DrawPropertiesWindow()
    {
        if (!_imgVault.IsOpen(ImGuiWindow.Properties)) return;

        using (_ui.UseFont())
        {
            if (ImGui.Begin("Properties", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar))
            {
                if (ImGui.SmallButton("X"))
                {
                    _imgVault.Close(ImGuiWindow.Properties);
                    _propertiesObject = null;
                    ImGui.End();
                    return;
                }
                if (_propertiesObject is not null)
                {
                    _propertiesObject.Properties(_ui);
                }
                else
                {
                    ImGui.Text("No object.");
                }

                ImGui.End();
            }

        }

    }

    private void StepPhysics(float frameTime)
    {
        frameTime = Math.Min(frameTime, Config.Instance.MAX_FRAME_TIME);
        _accumulator += frameTime;

        int stepCount = 0;

        while (_accumulator >= Config.Instance.FIXED_DT && stepCount < Config.Instance.MAX_SUBSTEPS)
        {
            _world.Step(Config.Instance.FIXED_DT, ref _solver);
            _accumulator -= Config.Instance.FIXED_DT;
            stepCount++;
        }

        // if while loop blocked
        if (_accumulator >= Config.Instance.FIXED_DT)
        {
            _accumulator = 0f;
        }
    }

    private void UpdatePhysicsObjects()
    {
        foreach (PhysicsObject item in _physicsObjects)
            item.PhysicsUpdate(_world);
    }

    private void DrawDebugWindow()
    {
        _ui.BeginWindow("##debug", ImGuiWindowFlags.NoTitleBar, () =>
        {
            ImGui.Text($"FPS: {Raylib.GetFPS().ToString()}");
            ImGui.Text($"Frametime: {Raylib.GetFrameTime().ToString()}");
            ImGui.Text($"Accumulator: {_accumulator}");
            ImGui.Text($"Count: {_physicsObjects.Count}");
            ImGui.Text($"Physics: {_physicsTime}ms, Update: {_updateObjTime}ms, Input: {_inputTime}ms");
            ImGui.Separator();

            if (_pickedObject is not null)
            {
                ImGui.Text($"Picked: {_pickedObject}");
                ImGui.Text($"Holding Position: {_pickedObject.Body.Position}");
                ImGui.Text($"BodyType: {_pickedObject.Body.BodyType}");
            }

            if (_propertiesObject is not null)
            {
                ImGui.Text($"Properties object: {_propertiesObject}");
            }
        });
    }

    private void DrawPhysicsObjects()
    {
        foreach (PhysicsObject obj in _physicsObjects)
        {
            obj.Draw();
        }
    }

}