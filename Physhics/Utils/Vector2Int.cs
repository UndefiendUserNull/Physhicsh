using System.Numerics;

namespace Physhicsh.Utils;

public class Vector2Int(int p_x = 0, int p_y = 0)
{
    public int X { get; set; } = p_x;
    public int Y { get; set; } = p_y;

    public static Vector2Int operator +(Vector2Int og, Vector2Int other) =>
        new(og.X + other.X, og.Y + other.Y);

    public static Vector2Int operator +(Vector2Int og, int value) =>
        new(og.X + value, og.Y + value);

    public static Vector2Int operator /(Vector2Int og, int value) =>
        new(og.X / value, og.Y / value);

    public Vector2 ToVector2()
    {
        return new(this.X, this.Y);
    }

    public nkast.Aether.Physics2D.Common.Vector2 ToAetherVector2()
    {
        return new(this.X, this.Y);
    }
}
