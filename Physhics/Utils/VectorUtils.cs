using nkast.Aether.Physics2D.Common;

namespace Physhicsh.Utils
{
    public static class VectorUtils
    {
        public static Vector2 Px2M(System.Numerics.Vector2 px) =>
    new(px.X / Config.Instance.PPM, px.Y / Config.Instance.PPM);

        public static System.Numerics.Vector2 M2Px(Vector2 m) =>
     new(m.X * Config.Instance.PPM, m.Y * Config.Instance.PPM);
    }
}
