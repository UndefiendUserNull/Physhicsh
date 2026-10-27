using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects
{
    public class RectanglePhysicsObject(float x, float y, float width, float height, Color color, float density = 1f, BodyType bodyType = BodyType.Dynamic) : PhysicsObject(x, y, color, bodyType)
    {
        public Vector2 Size = new(width, height);
        private float _widthPx;
        private float _heightPx;
        public override void Start(World world)
        {
            base.Start(world);
            BuildFixture();

        }

        public override void Draw()
        {
            base.Draw();

            (_widthPx, _heightPx) = MeasurePolygon(Fixture.Shape as PolygonShape);
            Vector2 screenPos = Body.Position.WorldToScreen();

            Rectangle rec = new(screenPos.X, screenPos.Y, _widthPx, _heightPx);
            Vector2 origin = new(_widthPx / 2f, _heightPx / 2f);
            float rotationDegrees = -Body.Rotation * (180f / MathF.PI);

            Raylib.DrawRectanglePro(rec, origin.ToNumericsVector2(), rotationDegrees, Color);
        }

        public void Resize(Vector2 newSize)
        {
            if (newSize == Vector2.Zero) return;
            if (newSize == Size) return;

            Size = newSize;

            float friction = Fixture.Friction;
            float restit = Fixture.Restitution;
            object tag = Fixture.Tag;

            Body.Remove(Fixture);

            BuildFixture();

            Fixture.Friction = friction;
            Fixture.Restitution = restit;
            Fixture.Tag = tag;

        }

        private void BuildFixture()
        {
            Fixture = Body.CreateRectangle(Size.X, Size.Y, 1f, Vector2.Zero);
            Fixture.Restitution = 0.3f;
            Fixture.Friction = 0.5f;
            Fixture.Tag = this;
        }

        // Vibe coded
        private static (float width, float height) MeasurePolygon(PolygonShape polygon)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (Vector2 v in polygon.Vertices)
            {
                minX = MathF.Min(minX, v.X);
                maxX = MathF.Max(maxX, v.X);
                minY = MathF.Min(minY, v.Y);
                maxY = MathF.Max(maxY, v.Y);
            }

            return ((maxX - minX) * Consts.PPM, (maxY - minY) * Consts.PPM);
        }
    }
}
