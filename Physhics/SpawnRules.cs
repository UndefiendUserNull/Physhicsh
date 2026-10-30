using Raylib_cs;
using SysVec2 = System.Numerics.Vector2;

namespace Physhicsh;

public sealed class SpawnRules
{
    private static readonly Lazy<SpawnRules> _instance = new(() => new SpawnRules());
    public static SpawnRules Instance => _instance.Value;
    private SysVec2 _circlesRadius = new(0.2f, 0.5f);
    public SysVec2 CirclesRadius
    {
        get { return _circlesRadius; }
        set => _circlesRadius = MinMax(value);
    }
    public SysVec2 RectSize { get; set; } = new(1f, 1f);
    public bool UseRandomColors = true;
    public Color SpawnColor = Color.White;
    private static SysVec2 MinMax(SysVec2 value)
    {
        return new(MathF.Min(value.X, value.Y), MathF.Max(value.X, value.Y));
    }
    private SpawnRules()
    {

    }
}
