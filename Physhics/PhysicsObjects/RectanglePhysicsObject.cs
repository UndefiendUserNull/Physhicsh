using nkast.Aether.Physics2D.Dynamics;
using Raylib_cs;
using System.Numerics;

namespace Physhicsh.PhysicsObjects
{
    public class RectanglePhysicsObject(float x, float y, float width, float height, Color color, float density = 1f, bool isStatic = false) : PhysicsObject(x, y, color, isStatic)
    {
        public Vector2 Size = new(width, height);
        private Fixture f;
        public override void Start(World world)
        {
            base.Start(world);
            f = Body.CreateRectangle(Size.X, Size.Y, density, Vector2.Zero.ToAetherVector2());

            f.Restitution = 0.3f;
            f.Friction = 0.5f;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (Raylib.IsMouseButtonDown(MouseButton.Left) && !IsStatic) Body.Position = new(Body.Position.X + .1f, Body.Position.Y);
        }

        public override void Draw()
        {
            base.Draw();


            Raylib.DrawRectangleV(BoundsUtils.CenterPositionPivot(Body.Position.ToNumericsVector2(), Size), Size, Color);

        }
    }
}
