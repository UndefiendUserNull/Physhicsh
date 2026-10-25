using nkast.Aether.Physics2D.Common;

namespace Physhicsh;

public static class AetherVectorsExtension
{
    public static Vector2Int ToVector2Int(this Vector2 v) => new((int)v.X, (int)v.Y);
    public static System.Numerics.Vector2 ToNumericsVector2(this Vector2 v) => new(v.X, v.Y);
    static System.Numerics.Vector2 M2Px(Vector2 m) =>
    new(m.X * Consts.PPM, m.Y * Consts.PPM);
}
