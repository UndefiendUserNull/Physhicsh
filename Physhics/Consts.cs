using nkast.Aether.Physics2D.Common;

namespace Physhicsh
{
    public static class Consts
    {
        public const float PPM = 32f;
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 640;
        public const int MAX_FPS = 60;
        public const int MAX_SUBSTEPS = 5;
        public const float FIXED_DT = 1f / 60f;
        public const float MAX_FRAME_TIME = 0.25f;
        public static Vector2 ScreenCenter => new(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);

    }
}
