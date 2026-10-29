using Raylib_cs;

namespace Physhicsh.Extensions;

public static class RandomExtensions
{
    public static float NextFloat(this Random random, float min, float max)
    {
        if (random is null) throw new ArgumentNullException(nameof(random));
        if (min > max) throw new ArgumentException("min must be <= max");
        return min + (float)random.NextDouble() * (max - min);
    }

    public static Color RandomColor(this Random rng)
    {
        return new(
    (byte)rng.Next(0, 254),
    (byte)rng.Next(0, 254),
    (byte)rng.Next(0, 254));
    }
}
