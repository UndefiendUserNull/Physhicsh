using Raylib_cs;

namespace Physhicsh;

public class MainWindow
{
    private const int GRAVITY = 1;
    private const int SCREEN_WIDTH = 1280;
    private const int SCREEN_HEIGHT = 640;
    private readonly List<PhysicsObject> _physicsObjects = [];
    public void Start()
    {
        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Physhicsh");
        Raylib.SetTargetFPS(59);

        _physicsObjects.Add(new(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, 50, 50));
        _physicsObjects.Add(new(SCREEN_WIDTH / 2, (SCREEN_HEIGHT / 2) + 100, 100, 50, true));

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
            if (!item.IsStatic) item.Position.Y += GRAVITY;

            item.PhysicsUpdate();
        }
    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        foreach (PhysicsObject item in _physicsObjects)
        {
            // TODO: Draw custom shape for each
            Vector2Int centerPivot = BoundsUtils.CenterPositionPivot(item.Position.X, item.Position.Y, item.Bounds.X, item.Bounds.Y);
            Raylib.DrawRectangle(centerPivot.X, centerPivot.Y, item.Bounds.X, item.Bounds.Y, Color.Green);
        }

        Raylib.EndDrawing();
    }
}
