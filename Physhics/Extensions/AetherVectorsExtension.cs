using nkast.Aether.Physics2D.Common;
using Physhicsh.Utils;

namespace Physhicsh.Extensions;

public static class AetherVectorsExtension
{
    public static Vector2Int ToVector2Int(this Vector2 v) => new((int)v.X, (int)v.Y);
    public static Vector2 ToAetherVector2(this System.Numerics.Vector2 v) => new(v.X, v.Y);

}
