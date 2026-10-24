using nkast.Aether.Physics2D.Dynamics;
using Raylib_cs;
using System.Numerics;

namespace Physhicsh.PhysicsObjects
{
    public class RectanglePhysicsObject(float x, float y, float width, float height, Color color, float density = 1f, bool isStatic = false) : PhysicsObject(x, y, color, isStatic)
    {
        public Vector2 Size = new(width, height);
        public override void Start(World world)
        {
            base.Start(world);

            Fixture f = Body.CreateRectangle(width, height, density, nkast.Aether.Physics2D.Common.Vector2.One * 9);
            f.Restitution = 0.3f;
            f.Friction = 0.5f;
        }

        public override void Draw()
        {
            base.Draw();
            Raylib.DrawRectangleV(Position.ToNumericsVector2(), Size, Color);
        }
    }
}
