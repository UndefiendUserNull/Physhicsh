using nkast.Aether.Physics2D.Common;

namespace Physhicsh
{
    public static class Consts
    {
        public const float PPM = 32f;
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 640;
        public const int MAX_FPS = 60;
        public const bool USE_VSYNC = true;
        public const int MAX_SUBSTEPS = 3;
        public const float FIXED_DT = 1f / MAX_FPS;
        public const float MAX_FRAME_TIME = 0.060f;
        public const float ROTATION_SPEED = 10f;
        public const string TEXTURES_PATH = "textures";
        public static Vector2 ScreenCenter => new(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);

    }
}
