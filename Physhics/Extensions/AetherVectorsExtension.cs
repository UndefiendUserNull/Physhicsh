using nkast.Aether.Physics2D.Common;
using Physhicsh.Utils;
using Raylib_cs;

namespace Physhicsh.Extensions;

public static class AetherVectorsExtension
{
    public static Vector2Int ToVector2Int(this Vector2 v) => new((int)v.X, (int)v.Y);
    public static Vector2 ToAetherVector2(this System.Numerics.Vector2 v) => new(v.X, v.Y);

    public static Vector2 WorldToScreen(this Vector2 world) => new(
        Config.ScreenCenter.X + (world.X * Config.Instance.PPM),
        Config.ScreenCenter.Y - (world.Y * Config.Instance.PPM));

    public static Vector2 ScreenToWorld(this Vector2 screen) => new(
        (screen.X - Config.ScreenCenter.X) / Config.Instance.PPM,
        (Config.ScreenCenter.Y - screen.Y) / Config.Instance.PPM);

    public static Vector3 ToVector3(this Color c) =>
        new(c.R / 255f, c.G / 255f, c.B / 255f);

    public static System.Numerics.Vector3 ToSysVector3(this Color c) =>
        new(c.R / 255f, c.G / 255f, c.B / 255f);

    public static System.Numerics.Vector4 ToVector4(this Color c) =>
        new(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);

    public static Color ToColor(this Vector3 c) => new(
        (byte)(c.X * 255f),
        (byte)(c.Y * 255f),
        (byte)(c.Z * 255f),
        (byte)255);

    public static Color ToColor(this System.Numerics.Vector3 c) => new(
        (byte)(c.X * 255f),
        (byte)(c.Y * 255f),
        (byte)(c.Z * 255f),
        (byte)255);

    public static Color ToColor(this System.Numerics.Vector4 c) => new(
        (byte)(c.X * 255f),
        (byte)(c.Y * 255f),
        (byte)(c.Z * 255f),
        (byte)(c.W * 255f));

    public static Vector3 ToAetherVector3(this System.Numerics.Vector3 v) =>
        new(v.X, v.Y, v.Z);

    public static System.Numerics.Vector3 ToNumericsVector3(this Vector3 v) =>
        new(v.X, v.Y, v.Z);
}