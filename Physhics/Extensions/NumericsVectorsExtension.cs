using nkast.Aether.Physics2D.Common;
using Physhicsh.Utils;

namespace Physhicsh.Extensions;

public static class NumericsVectorsExtension
{
    public static Vector2Int ToVector2Int(this Vector2 v) => new((int)v.X, (int)v.Y);
    public static System.Numerics.Vector2 ToNumericsVector2(this Vector2 v) => new(v.X, v.Y);
    public static System.Numerics.Vector2 AddInt(this System.Numerics.Vector2 v, int x) => new(v.X + x, v.Y + x);
    public static System.Numerics.Vector2 AddFloat(this System.Numerics.Vector2 v, float x) => new(v.X + x, v.Y + x);
    public static Vector2 WorldToScreen(this System.Numerics.Vector2 world) => new(
        Consts.ScreenCenter.X + (world.X * Consts.PPM),
        Consts.ScreenCenter.Y - (world.Y * Consts.PPM));

    public static Vector2 ScreenToWorld(this System.Numerics.Vector2 screen) => new(
        (screen.X - Consts.ScreenCenter.X) / Consts.PPM,
        (Consts.ScreenCenter.Y - screen.Y) / Consts.PPM);
}
