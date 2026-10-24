using System.Numerics;

namespace Physhicsh;

public static class NumericsVectorExtension
{
    public static Vector2Int ToVector2Int(this Vector2 v) => new((int)v.X, (int)v.Y);
    public static nkast.Aether.Physics2D.Common.Vector2 ToAetherVector2(this Vector2 v) => new(v.X, v.Y);
}
