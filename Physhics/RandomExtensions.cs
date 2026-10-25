namespace Physhicsh;

public static class RandomExtensions
{
    public static float NextFloat(this Random random, float min, float max)
    {
        if (random is null) throw new ArgumentNullException(nameof(random));
        if (min > max) throw new ArgumentException("min must be <= max");
        return min + (float)random.NextDouble() * (max - min);
    }
}
