using nkast.Aether.Physics2D.Common;
using Physhicsh.PConfig;

namespace Physhicsh.Utils
{
    public static class VectorUtils
    {
        public static Vector2 Px2M(System.Numerics.Vector2 px) =>
    new(px.X / Config.Instance.Screen.PPM, px.Y / Config.Instance.Screen.PPM);

        public static System.Numerics.Vector2 M2Px(Vector2 m) =>
     new(m.X * Config.Instance.Screen.PPM, m.Y * Config.Instance.Screen.PPM);
    }
}
