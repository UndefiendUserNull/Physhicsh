using nkast.Aether.Physics2D.Common;
using Physhicsh.Utils;
using Raylib_cs;

namespace Physhicsh.Extensions;

public static class AetherVectorsExtension
{
    public static Vector2Int ToVector2Int(this Vector2 v) => new((int)v.X, (int)v.Y);
    public static Vector2 ToAetherVector2(this System.Numerics.Vector2 v) => new(v.X, v.Y);

    public static Vector2 WorldToScreen(this Vector2 world) => new(
        Consts.ScreenCenter.X + (world.X * Consts.PPM),
        Consts.ScreenCenter.Y - (world.Y * Consts.PPM));

    public static Vector2 ScreenToWorld(this Vector2 screen) => new(
        (screen.X - Consts.ScreenCenter.X) / Consts.PPM,
        (Consts.ScreenCenter.Y - screen.Y) / Consts.PPM);

    public static Vector3 ToVector3(this Color c) => new(c.R, c.B, c.G);
    public static Color ToColor(this Vector3 c) => new(c.X, c.Y, c.Z);

    public static Vector3 ToAetherVector3(this System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z);
    public static System.Numerics.Vector3 ToNumericsVector3(this Vector3 v) => new(v.X, v.Y, v.Z);
}
