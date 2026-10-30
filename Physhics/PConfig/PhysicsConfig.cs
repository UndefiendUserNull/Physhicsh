namespace Physhicsh.PConfig;

public class PhysicsConfig
{
    public int MAX_SUBSTEPS { get; set; } = 3;
    public float FIXED_DT { get; set; } = 1f / 60f;
    public float MAX_FRAME_TIME { get; set; } = 0.060f;
}
