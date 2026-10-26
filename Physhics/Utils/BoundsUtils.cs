using System.Numerics;

namespace Physhicsh.Utils
{
    public static class BoundsUtils
    {
        public static Vector2 CenterPositionPivot(float x, float y, float w, float h)
        {
            return new Vector2(x - (w / 2), y - (h / 2));
        }
        public static Vector2 CenterPositionPivot(Vector2 pos, Vector2 size)
        {
            return CenterPositionPivot(pos.X, pos.Y, size.X, size.Y);
        }
    }
}
