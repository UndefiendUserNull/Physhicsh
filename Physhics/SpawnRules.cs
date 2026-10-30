using SysVec2 = System.Numerics.Vector2;

namespace Physhicsh;

public sealed class SpawnRules
{
    private static readonly Lazy<SpawnRules> _instance = new(() => new SpawnRules());
    public static SpawnRules Instance => _instance.Value;

    public SysVec2 CirclesRadius = new(0.2f, 0.5f);

    private SpawnRules()
    {

    }
}
