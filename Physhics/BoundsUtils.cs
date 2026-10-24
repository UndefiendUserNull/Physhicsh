namespace Physhicsh
{
    public static class BoundsUtils
    {
        public static Vector2Int CenterPositionPivot(int x, int y, int w, int h)
        {
            int center = Math.Abs(w - h) / 2;
            return new Vector2Int(x - center, y - center);
        }
    }
}
