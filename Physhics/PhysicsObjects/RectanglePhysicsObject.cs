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
            Fixture = Body.CreateRectangle(Size.X, Size.Y, density, Vector2.Zero.ToAetherVector2());
            Fixture.Restitution = 0.3f;
            Fixture.Friction = 0.5f;
        }

        public override void Draw()
        {
            base.Draw();

            Raylib.DrawRectanglePro(new Rectangle(Body.Position.ToNumericsVector2(), Size), Size, Body.Rotation, Color);
            //Raylib.DrawRectangleV(BoundsUtils.CenterPositionPivot(Body.Position.ToNumericsVector2(), Size), Size, Color);


        }
    }
}
