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
        set => _circlesRadius = new(MathF.Min(value.X, value.Y), MathF.Max(value.X, value.Y));
    }
    public bool UseRandomColors = true;
    public Color SpawnColor = Color.White;
    private SpawnRules()
    {

    }
}
