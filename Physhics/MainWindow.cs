using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Physhicsh.PhysicsObjects;
using Raylib_cs;

namespace Physhicsh;

public class MainWindow
{
    private const int SCREEN_WIDTH = 1280;
    private const int SCREEN_HEIGHT = 640;
    private const float PPM = 64f; // Pixles per meter
    private const float FIXED_DT = 1f / 60f;

    private List<PhysicsObject> _physicsObjects = [];
    private World _world = null!;
    private float _accumulator;
    private float _gravityScale = 1f;
    private FixedMouseJoint? _mouseJoint;
    private static Vector2 ScreenCenter => new(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
    private Fixture _heldFixture = null!;
    private Random _rng = new();
    private Vector2 _mousePosWorld = new();
    public void Start()
    {
        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Physhicsh");
        Raylib.SetTargetFPS(60);

        _world = new World(new Vector2(0, -9.81f * _gravityScale));


        //_physicsObjects.Add(new CirclePhysicsObject(0, 0, 10, Color.Green));

        _physicsObjects.Add(new CirclePhysicsObject(0, 10, 0.5f, Color.Green));
        _physicsObjects.Add(new RectanglePhysicsObject(0, 0, 1, 1, Color.DarkGray, 1, isStatic: false));
        _physicsObjects.Add(new RectanglePhysicsObject(5, 0, 1, 1, Color.DarkGray, 1, isStatic: false));
        _physicsObjects.Add(new RectanglePhysicsObject(0, -5, 10, 2, Color.SkyBlue, isStatic: true));

        foreach (PhysicsObject obj in _physicsObjects)
        {
            obj.Start(_world);
        }


        while (!Raylib.WindowShouldClose())
        {
            Update();
            GraphicsUpdate();
        }

        Raylib.CloseWindow();
    }

    private void Update()
    {
        float frameTime = Raylib.GetFrameTime();
        _accumulator += frameTime;
        _mousePosWorld = ScreenToWorld(Raylib.GetMousePosition().ToAetherVector2());
        while (_accumulator >= FIXED_DT)
        {
            _world.Step(FIXED_DT);
            _accumulator -= FIXED_DT;
        }

        foreach (PhysicsObject item in _physicsObjects)
        {
            item.PhysicsUpdate(_world);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.F))
        {
            RectanglePhysicsObject newRect = new(_mousePosWorld.X, _mousePosWorld.Y, 1, 1, new(_rng.Next(0, 254), _rng.Next(0, 254), _rng.Next(0, 254)), 1, isStatic: false);
            _physicsObjects.Add(newRect);
            newRect.Start(_world);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.B))
        {
            CirclePhysicsObject newBall = new(_mousePosWorld.X, _mousePosWorld.Y, 0.6f, new(_rng.Next(0, 254), _rng.Next(0, 254), _rng.Next(0, 254)), 1, isStatic: false);
            _physicsObjects.Add(newBall);
            newBall.Start(_world);
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Vector2 mouseWorld = ScreenToWorld(Raylib.GetMousePosition().ToAetherVector2());
            _heldFixture = _world.TestPoint(mouseWorld);

            if (_heldFixture != null && _heldFixture.Body.BodyType == BodyType.Dynamic)
            {
                // Create a soft joint that pulls the body toward the cursor.
                _mouseJoint = new FixedMouseJoint(_heldFixture.Body, mouseWorld)
                {
                    // Optional: make it strong enough to lift the body quickly.
                    // 1000 * mass is a common starting point; raise if it feels sluggish.
                    MaxForce = 1000f * _heldFixture.Body.Mass
                };

                _world.Add(_mouseJoint);   // register the joint so it is solved each step
            }
        }

        if (_mouseJoint != null && Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            _mouseJoint.WorldAnchorB = ScreenToWorld(Raylib.GetMousePosition().ToAetherVector2());
        }

        if (_mouseJoint != null && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            _world.Remove(_mouseJoint);
            _mouseJoint = null;
        }
    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        Raylib.DrawText(Raylib.GetFPS().ToString(), 10, 10, 30, Color.Magenta);
        Raylib.DrawText(Raylib.GetFrameTime().ToString(), 10, 40, 30, Color.Magenta);
        if (_heldFixture is not null) Raylib.DrawText($"{_heldFixture.Body.Position}", 10, 70, 30, Color.Magenta);

        foreach (PhysicsObject item in _physicsObjects)
        {
            DrawBody(item);
        }

        Raylib.EndDrawing();

    }

    // Vibe coded
    private static void DrawBody(PhysicsObject obj)
    {
        if (obj.Fixture.Shape is CircleShape circle)
        {
            float cos = MathF.Cos(obj.Body.Rotation);
            float sin = MathF.Sin(obj.Body.Rotation);
            Vector2 local = circle.Position;

            Vector2 world = new Vector2(
                obj.Body.Position.X + cos * local.X - sin * local.Y,
                obj.Body.Position.Y + sin * local.X + cos * local.Y);

            Vector2 screenPos = new Vector2(
            ScreenCenter.X + world.X * PPM,
            ScreenCenter.Y - world.Y * PPM);

            float screenRadius = circle.Radius * PPM;

            Raylib.DrawCircleV(screenPos.ToNumericsVector2(), screenRadius, obj.Color);
        }

        else if (obj.Fixture.Shape is PolygonShape polygon)
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

            float widthPixels = (maxX - minX) * PPM;
            float heightPixels = (maxY - minY) * PPM;

            // Body position (metres, Y-up) → screen (pixels, Y-down)
            Vector2 screenPos = new Vector2(
                ScreenCenter.X + obj.Body.Position.X * PPM,
                ScreenCenter.Y - obj.Body.Position.Y * PPM);

            // Rec position IS where the origin point lands → pass screenPos directly.
            Rectangle rec = new Rectangle(screenPos.X, screenPos.Y, widthPixels, heightPixels);
            Vector2 origin = new Vector2(widthPixels / 2f, heightPixels / 2f);
            float rotationDegrees = -obj.Body.Rotation * (180f / MathF.PI);

            Raylib.DrawRectanglePro(rec, origin.ToNumericsVector2(), rotationDegrees, obj.Color);
        }
    }

    private static Vector2 ScreenToWorld(Vector2 screen)
    {
        return new Vector2(
            (screen.X - ScreenCenter.X) / PPM,
            (ScreenCenter.Y - screen.Y) / PPM);
    }
}
