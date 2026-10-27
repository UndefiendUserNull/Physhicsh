using ImGuiNET;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Physhicsh.Extensions;
using Physhicsh.PhysicsObjects;
using Physhicsh.UI;
using Raylib_cs;
using rlImGui_cs;

namespace Physhicsh;

public class MainWindow
{
    private readonly List<PhysicsObject> _physicsObjects = [];
    private readonly Random _rng = new();
    private readonly ImGuiVault _imgVault = new();

    private World _world = null!;
    private float _accumulator;
    private float _gravityScale = 1f;
    private FixedMouseJoint? _mouseJoint;
    private PhysicsObject _pickedObject = null!;
    private Vector2 _mousePosWorld;
    private PhysicsObject? _propertiesObject;
    private bool _showUglyDebug = false;
    private System.Numerics.Vector3 _cachedColor;
    private bool _hasCachedColor;
    private ImGuiUI _ui;

    public void Start()
    {
        Raylib.InitWindow(Consts.SCREEN_WIDTH, Consts.SCREEN_HEIGHT, "Physhicsh");
        Raylib.SetWindowFocused();
        Raylib.SetTargetFPS(Consts.MAX_FPS);

        // Load font
        rlImGui.SetupUserFonts += io =>
        {
            _ui = new(io.Fonts.AddFontFromFileTTF(
            _imgVault.IMGUI_FONT_PATH,
            18.0f));
        };

        rlImGui.Setup(true);

        _world = new World(new Vector2(0, -9.81f * _gravityScale));

        SpawnDemoObjects();

        while (!Raylib.WindowShouldClose())
        {
            Update();
            GraphicsUpdate();
        }

        Raylib.CloseWindow();
    }

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
        _mousePosWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();

        StepPhysics(Raylib.GetFrameTime());
        DebugScaling();
        HandleMouseInput();
        HandleShortcuts();
        UpdatePhysicsObjects();

        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            if (_pickedObject is null) PickFixture();
            Vector2 worldPos = _mousePosWorld;
            _ui.ShowPopup("##world_context", Raylib.GetMousePosition(), () =>
            {
                if (ImGui.BeginMenu("Quick spawn"))
                {
                    DrawQuickSpawnMenu(worldPos);
                    ImGui.EndMenu();
                }
                if (_pickedObject is not null)
                {
                    ImGui.Separator();
                    if (ImGui.MenuItem("Delete")) DeletePhysicsObject(_pickedObject);
                    if (ImGui.MenuItem("Properties"))
                    {
                        _propertiesObject = _pickedObject;
                        _imgVault.ShowProperties = true;
                    }
                }
            });
        }
    }

    private void HandleShortcuts()
    {
        if (Raylib.IsKeyPressedRepeat(KeyboardKey.R) || Raylib.IsKeyPressed(KeyboardKey.R)) SpawnRectangle(_mousePosWorld);
        if (Raylib.IsKeyPressedRepeat(KeyboardKey.C) || Raylib.IsKeyPressed(KeyboardKey.C)) SpawnCircle(_mousePosWorld);
        if (Raylib.IsKeyPressed(KeyboardKey.Tab))
        {
            Vector2 mousePos = _mousePosWorld;
            _ui.ShowPopup("##quick_spawn", Raylib.GetMousePosition(), () => DrawQuickSpawnMenu(mousePos));
        }
        if (Raylib.IsKeyPressed(KeyboardKey.V) && _pickedObject is not null)
        {
            if (_pickedObject.BodyType == BodyType.Static) _pickedObject.BodyType = BodyType.Dynamic;
            else if (_pickedObject.BodyType == BodyType.Dynamic) _pickedObject.BodyType = BodyType.Static;

        }
        if (Raylib.IsKeyPressed(KeyboardKey.X) && _pickedObject is not null) DeletePhysicsObject(_pickedObject);
        if (Raylib.IsKeyPressed(KeyboardKey.F3)) _showUglyDebug = !_showUglyDebug;

        // Rotation

        if (Raylib.IsKeyDown(KeyboardKey.Q) && _pickedObject is not null)
        {
            _pickedObject.Body.Rotation -= 0.05f;
            _pickedObject.Body.ResetDynamics();
        }
        if (Raylib.IsKeyDown(KeyboardKey.E) && _pickedObject is not null)
        {
            _pickedObject.Body.Rotation += 0.05f;
            _pickedObject.Body.ResetDynamics();
        }
    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        rlImGui.Begin();

        #region ImGui menus

        _ui.BeginFrame();

        DrawPropertiesWindow();

        if (_showUglyDebug)
            DrawDebugText();

        _ui.EndFrame();

        #endregion

        DrawPhysicsObjects();


        rlImGui.End();
        Raylib.EndDrawing();
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

    private void DrawQuickSpawnMenu(Vector2 spawnPos)
    {
        if (ImGui.MenuItem("Rectangle", "r"))
        {
            SpawnRectangle(spawnPos);

        }

        if (ImGui.MenuItem("Circle", "c"))
        {
            SpawnCircle(spawnPos);
        }

    }

    private void DrawPropertiesWindow()
    {
        if (!_imgVault.ShowProperties) return;

        using (_ui.UseFont())
        {
            if (ImGui.Begin("Properties", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar))
            {
                if (ImGui.SmallButton("X"))
                {
                    _imgVault.ShowProperties = false;
                    _propertiesObject = null;
                    ImGui.End();
                    return;
                }
                if (_propertiesObject is not null)
                {
                    Color currentColor = _propertiesObject.Color;

                    if (!_hasCachedColor || currentColor != _cachedColor.ToAetherVector3().ToColor())
                    {
                        _cachedColor = currentColor.ToVector3().ToNumericsVector3() / 255f;
                        _hasCachedColor = true;
                    }

                    if (ImGui.ColorEdit3("Color", ref _cachedColor))
                    {
                        _propertiesObject.Color = _cachedColor.ToAetherVector3().ToColor();
                    }

                    float massRefSlider = _propertiesObject.Body.Mass;

                    if (ImGui.SliderFloat("Mass", ref massRefSlider, 0f, 100f))
                    {
                        _propertiesObject.Body.Mass = massRefSlider;
                    }

                    if (_propertiesObject is CirclePhysicsObject circle)
                    {
                        float r = circle.Radius;
                        if (ImGui.SliderFloat("Radius", ref r, 0f, 3f))
                        {
                            circle.Radius = r;
                        }
                    }
                    else if (_propertiesObject is RectanglePhysicsObject rect)
                    {
                        System.Numerics.Vector2 sizeRefSlider = rect.Size.ToNumericsVector2();
                        if (ImGui.SliderFloat2("Size", ref sizeRefSlider, 0.001f, 100f))
                        {
                            rect.Resize(sizeRefSlider.ToAetherVector2());
                        }
                    }
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
        frameTime = Math.Min(frameTime, Consts.MAX_FRAME_TIME);
        _accumulator += frameTime;

        int stepCount = 0;

        while (_accumulator >= Consts.FIXED_DT && stepCount < Consts.MAX_SUBSTEPS)
        {
            _world.Step(Consts.FIXED_DT);
            _accumulator -= Consts.FIXED_DT;
            stepCount++;
        }

        // if while loop blocked
        if (_accumulator >= Consts.FIXED_DT)
        {
            _accumulator = 0f;
        }
    }

    private void UpdatePhysicsObjects()
    {
        foreach (PhysicsObject item in _physicsObjects)
            item.PhysicsUpdate(_world);
    }

    private void DebugScaling()
    {
        if (_pickedObject is CirclePhysicsObject circle)
        {
            if (Raylib.IsKeyDown(KeyboardKey.H))
                circle.Radius += 0.01f;
            if (Raylib.IsKeyDown(KeyboardKey.J))
                circle.Radius -= 0.01f;
        }
    }

    private void SpawnRectangle(Vector2 pos)
    {
        RectanglePhysicsObject newRect = new(
            pos.X, pos.Y, 1, 1,
            RandomColor(), 1);

        _physicsObjects.Add(newRect);
        newRect.Start(_world);
    }

    private void SpawnCircle(Vector2 pos)
    {
        CirclePhysicsObject newBall = new(
            pos.X, pos.Y, _rng.NextFloat(0.2f, 0.5f),
            RandomColor(), 1);

        _physicsObjects.Add(newBall);
        newBall.Start(_world);
    }

    private Color RandomColor() => new(
        (byte)_rng.Next(0, 254),
        (byte)_rng.Next(0, 254),
        (byte)_rng.Next(0, 254));

    private void HandleMouseInput()
    {
        if (_ui.AnyPopupsOpen()) return;

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            PickFixture();
            BeginDrag();
        }
        else if (_mouseJoint != null && Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            UpdateDrag();
        }
        else if (_mouseJoint != null && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            EndDrag();
        }
    }

    private Fixture? PickFixture()
    {
        Vector2 mouseWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();
        Fixture fixture = _world.TestPoint(mouseWorld);

        if (fixture != null)
            _pickedObject = fixture.Tag as PhysicsObject;
        else
            _pickedObject = null;

        return fixture == null ? null : fixture;
    }

    private void BeginDrag()
    {
        if (_pickedObject == null) return;

        Vector2 mouseWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();

        _mouseJoint = new FixedMouseJoint(_pickedObject.Body, mouseWorld)
        {
            MaxForce = 1000f * _pickedObject.Body.Mass
        };
        _world.Add(_mouseJoint);
    }

    private void UpdateDrag()
    {
        _mouseJoint!.WorldAnchorB = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();
    }

    private void EndDrag()
    {
        _world.Remove(_mouseJoint!);
        _mouseJoint = null;
    }

    private void DrawDebugText()
    {
        _ui.BeginWindow("##debug", ImGuiWindowFlags.NoTitleBar, () =>
        {
            ImGui.Text($"FPS: {Raylib.GetFPS().ToString()}");
            ImGui.Text($"Frametime: {Raylib.GetFrameTime().ToString()}");
            ImGui.Text($"Accumulator: {_accumulator}");
            ImGui.Text($"Count: {_physicsObjects.Count}");
            ImGui.Separator();

            if (_pickedObject is not null)
            {
                ImGui.Text($"Holding Position: {_pickedObject.Body.Position}");
                ImGui.Text($"BodyType: {_pickedObject.Body.BodyType}");
            }
        });
    }

    private void DrawPhysicsObjects()
    {
        foreach (PhysicsObject item in _physicsObjects)
            item.Draw();
    }

    private void DrawPolygonShape(PhysicsObject obj, PolygonShape polygon)
    {

    }




}