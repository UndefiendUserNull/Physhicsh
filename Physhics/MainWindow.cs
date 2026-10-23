using Raylib_cs;

namespace Physhicsh;

public class MainWindow
{
    public void Start()
    {
        Raylib.InitWindow(1280, 640, "Physhicsh");

        while (!Raylib.WindowShouldClose())
        {
            GraphicsUpdate();
            Update();
        }

        Raylib.CloseWindow();
    }

    private void Update()
    {

    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        Raylib.EndDrawing();
    }
}
