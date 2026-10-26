using ImGuiNET;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Physhicsh.PhysicsObjects;
using Raylib_cs;
using rlImGui_cs;

namespace Physhicsh;

public class MainWindow
{
    private const int SCREEN_WIDTH = 1280;
    private const int SCREEN_HEIGHT = 640;
    private const int MAX_FPS = 60;
    private float PPM = 32f;
    private const float FIXED_DT = 1f / 60f;

    private readonly List<PhysicsObject> _physicsObjects = [];
    private readonly Random _rng = new();

    private World _world = null!;
    private float _accumulator;
    private float _gravityScale = 1f;
    private FixedMouseJoint? _mouseJoint;
    private Fixture _heldFixture = null!;
    private Vector2 _mousePosWorld;

    private static Vector2 ScreenCenter => new(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
    private readonly string _imGuiFontPath = "fonts/arial.ttf";
    private ImFontPtr _imGuiFont;
    private Vector2 _contextMenuWorldPos;
    private bool _quickSpawnMenuPending = false;
    private Vector2 _quickSpawnMenuMousePos;
    private Vector2 _quickSpawnMenuScreenPos;

    public void Start()
    {
        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Physhicsh");
        Raylib.SetTargetFPS(MAX_FPS);

        // Load font
        rlImGui.SetupUserFonts += io =>
        {
            _imGuiFont = io.Fonts.AddFontFromFileTTF(
            _imGuiFontPath,
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
        _physicsObjects.Add(new RectanglePhysicsObject(0, 0, 1, 1, Color.DarkGray, 1, isStatic: false));
        _physicsObjects.Add(new RectanglePhysicsObject(5, 0, 1, 1, Color.DarkGray, 1, isStatic: false));
        _physicsObjects.Add(new RectanglePhysicsObject(0, -5, 10, 2, Color.SkyBlue, isStatic: true));
        _physicsObjects.Add(new RectanglePhysicsObject(0, -10, 100, 2, Color.SkyBlue, isStatic: true));
        _physicsObjects.Add(new RectanglePhysicsObject(20, -10, 1, 100, Color.SkyBlue, isStatic: true));
        _physicsObjects.Add(new RectanglePhysicsObject(-20, -10, 1, 100, Color.SkyBlue, isStatic: true));

        foreach (PhysicsObject obj in _physicsObjects)
            obj.Start(_world);
    }


    private void Update()
    {
        _mousePosWorld = ScreenToWorld(Raylib.GetMousePosition().ToAetherVector2());

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
            _quickSpawnMenuPending = true;
            _quickSpawnMenuMousePos = _mousePosWorld;
            _quickSpawnMenuScreenPos = Raylib.GetMousePosition().ToAetherVector2();
        }
        if (Raylib.IsKeyDown(KeyboardKey.Minus)) PPM -= 0.5F;
        if (Raylib.IsKeyDown(KeyboardKey.Equal)) PPM += 0.5F;
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

        #endregion

        if (_quickSpawnMenuPending)
        {
            _quickSpawnMenuPending = false;
            ImGui.SetNextWindowPos(_quickSpawnMenuMousePos.ToNumericsVector2());
            ImGui.OpenPopup("##quick_spawn");

            if (ImGui.BeginPopup("##quick_spawn"))
            {
                ImGui.PushFont(_imGuiFont);

                DrawQuickSpawnMenu(_quickSpawnMenuMousePos);

                ImGui.PopFont();
                ImGui.EndPopup();
            }

        }

        DrawPhysicsObjects();
        DrawDebugText();

        ImGui.End();
        rlImGui.End();
        Raylib.EndDrawing();
    }

    private void ImGuiQuickSpawnPopup()
    {
        if (_quickSpawnMenuPending)
        {
            _quickSpawnMenuPending = false;
            ImGui.SetNextWindowPos(_quickSpawnMenuScreenPos.ToNumericsVector2());
            ImGui.OpenPopup("##quick_spawn");
        }

        // BeginPopup is called EVERY frame so the popup stays alive.
        if (ImGui.BeginPopup("##quick_spawn"))
        {
            ImGui.PushFont(_imGuiFont);
            DrawQuickSpawnMenu(_mousePosWorld);   // world pos for spawning
            ImGui.PopFont();
            ImGui.EndPopup();
        }
    }

    private void ImGuiContextMenu()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            _contextMenuWorldPos = ScreenToWorld(Raylib.GetMousePosition().ToAetherVector2());

            ImGui.SetNextWindowPos(Raylib.GetMousePosition());
            ImGui.OpenPopup("##world_context");
        }

        if (ImGui.BeginPopup("##world_context"))
        {
            ImGui.PushFont(_imGuiFont);

            DrawQuickSpawnMenu(_contextMenuWorldPos);

            if (_heldFixture is not null)
            {
                if (ImGui.MenuItem("Delete"))
                {

                }

                if (ImGui.MenuItem("Properties"))
                {

                }
            }


            ImGui.PopFont();
            ImGui.EndPopup();
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
        _accumulator += frameTime;
        while (_accumulator >= FIXED_DT)
        {
            _world.Step(FIXED_DT);
            _accumulator -= FIXED_DT;
        }
    }

    private void UpdatePhysicsObjects()
    {
        foreach (PhysicsObject item in _physicsObjects)
            item.PhysicsUpdate(_world);
    }

    private void DebugScaling()
    {
        if (_heldFixture?.Shape is CircleShape circle)
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
            RandomColor(), 1, isStatic: false);

        _physicsObjects.Add(newRect);
        newRect.Start(_world);
    }

    private void SpawnCircle(Vector2 pos)
    {
        CirclePhysicsObject newBall = new(
            pos.X, pos.Y, _rng.NextFloat(0.2f, 0.5f),
            RandomColor(), 1, isStatic: false);

        _physicsObjects.Add(newBall);
        newBall.Start(_world);
    }

    private Color RandomColor() => new(
        (byte)_rng.Next(0, 254),
        (byte)_rng.Next(0, 254),
        (byte)_rng.Next(0, 254));

    private void HandleMouseInput()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            BeginDrag();
        else if (_mouseJoint != null && Raylib.IsMouseButtonDown(MouseButton.Left))
            UpdateDrag();
        else if (_mouseJoint != null && Raylib.IsMouseButtonReleased(MouseButton.Left))
            EndDrag();
    }

    private void BeginDrag()
    {
        Vector2 mouseWorld = ScreenToWorld(Raylib.GetMousePosition().ToAetherVector2());
        _heldFixture = _world.TestPoint(mouseWorld);

        if (_heldFixture == null || _heldFixture.Body.BodyType != BodyType.Dynamic)
            return;

        _mouseJoint = new FixedMouseJoint(_heldFixture.Body, mouseWorld)
        {
            MaxForce = 1000f * _heldFixture.Body.Mass
        };
        _world.Add(_mouseJoint);
    }

    private void UpdateDrag()
    {
        _mouseJoint!.WorldAnchorB = ScreenToWorld(Raylib.GetMousePosition().ToAetherVector2());
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
        if (_heldFixture is not null)
            Raylib.DrawText($"{_heldFixture.Body.Position}", 10, 70, 30, Color.Magenta);
    }

    private void DrawPhysicsObjects()
    {
        foreach (PhysicsObject item in _physicsObjects)
            DrawBody(item);
    }

    private void DrawBody(PhysicsObject obj)
    {
        switch (obj.Fixture.Shape)
        {
            case CircleShape circle:
                DrawCircleShape(obj, circle);
                break;
            case PolygonShape polygon:
                DrawPolygonShape(obj, polygon);
                break;
        }
    }

    private void DrawCircleShape(PhysicsObject obj, CircleShape circle)
    {
        float cos = MathF.Cos(obj.Body.Rotation);
        float sin = MathF.Sin(obj.Body.Rotation);
        Vector2 local = circle.Position;

        Vector2 world = new(
            obj.Body.Position.X + cos * local.X - sin * local.Y,
            obj.Body.Position.Y + sin * local.X + cos * local.Y);

        Vector2 screenPos = WorldToScreen(world);
        float screenRadius = circle.Radius * PPM;

        Raylib.DrawCircleV(screenPos.ToNumericsVector2(), screenRadius, obj.Color);
    }

    private void DrawPolygonShape(PhysicsObject obj, PolygonShape polygon)
    {
        (float widthPixels, float heightPixels) = MeasurePolygon(polygon);

        Vector2 screenPos = WorldToScreen(obj.Body.Position);

        Rectangle rec = new(screenPos.X, screenPos.Y, widthPixels, heightPixels);
        Vector2 origin = new(widthPixels / 2f, heightPixels / 2f);
        float rotationDegrees = -obj.Body.Rotation * (180f / MathF.PI);

        Raylib.DrawRectanglePro(rec, origin.ToNumericsVector2(), rotationDegrees, obj.Color);
    }

    private (float width, float height) MeasurePolygon(PolygonShape polygon)
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (Vector2 v in polygon.Vertices)
        {
            minX = MathF.Min(minX, v.X);
            maxX = MathF.Max(maxX, v.X);
            minY = MathF.Min(minY, v.Y);
            maxY = MathF.Max(maxY, v.Y);
        }

        return ((maxX - minX) * PPM, (maxY - minY) * PPM);
    }

    private Vector2 WorldToScreen(Vector2 world) => new(
        ScreenCenter.X + world.X * PPM,
        ScreenCenter.Y - world.Y * PPM);

    private Vector2 ScreenToWorld(Vector2 screen) => new(
        (screen.X - ScreenCenter.X) / PPM,
        (ScreenCenter.Y - screen.Y) / PPM);
}