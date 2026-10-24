using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.PhysicsObjects;
using Raylib_cs;

namespace Physhicsh;

public class MainWindow
{
    private const int SCREEN_WIDTH = 1280;
    private const int SCREEN_HEIGHT = 640;
    private readonly List<PhysicsObject> _physicsObjects = [];
    private World? _world;

    public void Start()
    {
        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Physhicsh");
        Raylib.SetTargetFPS(60);

        _world = new World(new Vector2(0, 49.0500031F));

        //_physicsObjects.Add(new RectanglePhysicsObject(SCREEN_WIDTH / 2, (SCREEN_HEIGHT / 2) + 100, 50, 50, Color.Green, isStatic: true));
        _physicsObjects.Add(new CirclePhysicsObject((SCREEN_WIDTH / 2) - 5, SCREEN_HEIGHT / 2, 10, Color.Green));
        _physicsObjects.Add(new RectanglePhysicsObject((SCREEN_WIDTH / 2) - 20, SCREEN_HEIGHT, 250, 20, Color.DarkGray, isStatic: true));
        _physicsObjects.Add(new RectanglePhysicsObject(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, 50, 100, Color.DarkGray, isStatic: false));
        //_physicsObjects.Add(new RectanglePhysicsObject((SCREEN_WIDTH / 2) + 80, (SCREEN_HEIGHT / 2) + 200, 50, 100, Color.Green, isStatic: true));

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

        _world.Step(Raylib.GetFrameTime());
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
