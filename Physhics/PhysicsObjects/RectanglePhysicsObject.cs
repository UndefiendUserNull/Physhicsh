using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Raylib_cs;
using System.Numerics;

namespace Physhicsh.PhysicsObjects
{
    public class RectanglePhysicsObject(float x, float y, float width, float height, Color color, float density = 1f, BodyType bodyType = BodyType.Dynamic) : PhysicsObject(x, y, color, bodyType)
    {
        public Vector2 Size = new(width, height);
        public override void Start(World world)
        {
            base.Start(world);
            Fixture = Body.CreateRectangle(Size.X, Size.Y, density, Vector2.Zero.ToAetherVector2());
            Fixture.Restitution = 0.3f;
            Fixture.Friction = 0.5f;

            Fixture.Tag = this;
        }
    }
}
