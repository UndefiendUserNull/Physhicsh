using ImGuiNET;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Physhicsh.Extensions;
using Physhicsh.PhysicsObjects;
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

    public void Start()
    {
        Raylib.InitWindow(Consts.SCREEN_WIDTH, Consts.SCREEN_HEIGHT, "Physhicsh");
        Raylib.MaximizeWindow();
        Raylib.SetWindowFocused();
        Raylib.SetTargetFPS(Consts.MAX_FPS);

        // Load font
        rlImGui.SetupUserFonts += io =>
        {
            _imgVault.ImGuiFont = io.Fonts.AddFontFromFileTTF(
            _imgVault.IMGUI_FONT_PATH,
            18.0f);
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
    }

    private void HandleShortcuts()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.R)) SpawnRectangle(_mousePosWorld);
        if (Raylib.IsKeyPressed(KeyboardKey.C)) SpawnCircle(_mousePosWorld);
        if (Raylib.IsKeyPressed(KeyboardKey.Q))
        {
            _imgVault.QuickSpawnMenuPending = true;
            _imgVault.QuickSpawnMenuMousePos = _mousePosWorld;
            _imgVault.QuickSpawnMenuScreenPos = Raylib.GetMousePosition().ToAetherVector2();
        }
        if (Raylib.IsKeyPressed(KeyboardKey.V) && _pickedObject is not null)
        {
            if (_pickedObject.BodyType == BodyType.Static) _pickedObject.BodyType = BodyType.Dynamic;
            else if (_pickedObject.BodyType == BodyType.Dynamic) _pickedObject.BodyType = BodyType.Static;

        }
    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        rlImGui.Begin();
        ImGuiIOPtr io = ImGui.GetIO();

        StartImGui(io);

        #region ImGui menus

        ImGuiContextMenu();
        ImGuiQuickSpawnPopup();
        DrawPropertiesWindow();

        #endregion

        DrawPhysicsObjects();
        DrawDebugText();

        ImGui.End();
        rlImGui.End();
        Raylib.EndDrawing();
    }

    private void ImGuiQuickSpawnPopup()
    {
        if (_imgVault.QuickSpawnMenuPending)
        {
            _imgVault.QuickSpawnMenuPending = false;
            ImGui.SetNextWindowPos(_imgVault.QuickSpawnMenuScreenPos.ToNumericsVector2());
            ImGui.OpenPopup("##quick_spawn");
        }

        if (ImGui.BeginPopup("##quick_spawn"))
        {
            ImGui.PushFont(_imgVault.ImGuiFont);
            DrawQuickSpawnMenu(_mousePosWorld);
            ImGui.PopFont();
            ImGui.EndPopup();
        }
    }

    private void ImGuiContextMenu()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            if (_pickedObject == null) PickFixture();
            _imgVault.ContextMenuOpen = true;
            _imgVault.ContextMenuWorldPos = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();

            ImGui.SetNextWindowPos(Raylib.GetMousePosition());
            ImGui.OpenPopup("##world_context");
        }

        if (ImGui.BeginPopup("##world_context"))
        {
            ImGui.PushFont(_imgVault.ImGuiFont);

            DrawQuickSpawnMenu(_imgVault.ContextMenuWorldPos);

            if (_pickedObject is not null)
            {
                if (ImGui.MenuItem("Delete"))
                {
                    _pickedObject.Delete();
                    _physicsObjects.Remove(_pickedObject);
                    _pickedObject = null!;
                }

                if (ImGui.MenuItem("Properties"))
                {
                    _imgVault.ShowProperties = true;
                    _propertiesObject = _pickedObject;
                }
            }


            ImGui.PopFont();
            ImGui.EndPopup();
        }
        else
        {
            _imgVault.ContextMenuOpen = false;
        }
    }

    private void DrawQuickSpawnMenu(Vector2 spawnPos)
    {
        if (ImGui.BeginMenu("Quick Spawn"))
        {
            if (ImGui.MenuItem("Rectangle", "r"))
            {
                SpawnRectangle(spawnPos);

            }

            if (ImGui.MenuItem("Circle", "c"))
            {
                SpawnCircle(spawnPos);
            }

            ImGui.EndMenu();
        }
    }

    private void DrawPropertiesWindow()
    {
        if (!_imgVault.ShowProperties) return;

        ImGui.PushFont(_imgVault.ImGuiFont);

        if (_imgVault.ShowProperties)
        {
            if (ImGui.Begin("Properties"))
            {
                if (_propertiesObject is not null)
                {
                    ImGui.Text($"{_propertiesObject.Color}");
                    ImGui.Text($"Mass: {_propertiesObject.Body.Mass}");
                }
                else
                {
                    ImGui.Text("No object.");
                }
            }
            ImGui.End();

            ImGui.PopFont();
        }

    }

    private static void StartImGui(ImGuiIOPtr io)
    {
        ImGui.SetNextWindowPos(Vector2.Zero.ToNumericsVector2());
        ImGui.SetNextWindowSize(io.DisplaySize);
        ImGui.Begin("##world_context_host",
        ImGuiWindowFlags.NoDecoration |
        ImGuiWindowFlags.NoBackground |
        ImGuiWindowFlags.NoInputs |
        ImGuiWindowFlags.NoSavedSettings |
        ImGuiWindowFlags.NoBringToFrontOnFocus);
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
        if (!_imgVault.ContextMenuOpen)
        {
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
        Raylib.DrawText(Raylib.GetFPS().ToString(), 10, 10, 30, Color.Magenta);
        Raylib.DrawText(Raylib.GetFrameTime().ToString(), 10, 40, 30, Color.Magenta);
        if (_pickedObject is not null)
        {
            Raylib.DrawText($"{_pickedObject.Body.Position}", 10, 70, 30, Color.Magenta);
            Raylib.DrawText($"{_pickedObject.Body.BodyType} | {_pickedObject.BodyType}", 10, 100, 30, Color.Magenta);

        }

        Raylib.DrawText($"{_accumulator}", 10, 130, 30, Color.Magenta);
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