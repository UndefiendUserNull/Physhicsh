using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.PhysicsObjects;
using Raylib_cs;

namespace Physhicsh;

public class MainWindow
{
    private const int GRAVITY = 1;
    private const int SCREEN_WIDTH = 1280;
    private const int SCREEN_HEIGHT = 640;
    private readonly List<PhysicsObject> _physicsObjects = [];
    private World? _world;

    public void Start()
    {
        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Physhicsh");
        Raylib.SetTargetFPS(60);

        _world = new World();
        Vector2 temp = _world.Gravity;
        _world.Gravity = -temp;

        _physicsObjects.Add(new CirclePhysicsObject((SCREEN_WIDTH / 2) - 5, SCREEN_HEIGHT / 2, 50, Color.Green));
        _physicsObjects.Add(new RectanglePhysicsObject(SCREEN_WIDTH / 2, (SCREEN_HEIGHT / 2) + 100, 100, 50, Color.Green, isStatic: true));

        foreach (PhysicsObject obj in _physicsObjects)
        {
            obj.Start(_world);
        }

        while (!Raylib.WindowShouldClose())
        {
            GraphicsUpdate();
            Update();
        }

        Raylib.CloseWindow();
    }

    private void Update()
    {
        foreach (PhysicsObject item in _physicsObjects)
        {
            item.PhysicsUpdate();
        }

        float dt = 1f / Raylib.GetFPS() * 5;
        _world.Step(dt);
    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.DrawText(Raylib.GetFPS().ToString(), 10, 10, 30, Color.Magenta);
        Raylib.DrawText(Raylib.GetFrameTime().ToString(), 10, 40, 30, Color.Magenta);

        foreach (PhysicsObject item in _physicsObjects)
        {
            item.Draw();
        }

        Raylib.ClearBackground(Color.Black);
        Raylib.EndDrawing();
    }
}
