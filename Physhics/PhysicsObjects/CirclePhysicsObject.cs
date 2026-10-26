using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects;

public class CirclePhysicsObject(float x, float y, float radius, Color color, float density = 1f, BodyType bodyType = BodyType.Dynamic) : PhysicsObject(x, y, color, bodyType)
{
    public float Radius { get { return Fixture.Shape.Radius; } set { Fixture.Shape.Radius = value; } }
    public override void Start(World world)
    {
        base.Start(world);

        Fixture = Body.CreateCircle(radius, density);
        Fixture.Restitution = 0.3f;
        Fixture.Friction = 0.5f;

        Fixture.Tag = this;

    }

    public override void PhysicsUpdate(World world)
    {
        base.PhysicsUpdate(world);

    }

    public override void Draw()
    {
        base.Draw();

        float cos = MathF.Cos(Body.Rotation);
        float sin = MathF.Sin(Body.Rotation);
        Vector2 local = Body.Position;

        Vector2 world = new(
            Body.Position.X + cos * local.X - sin * local.Y,
            Body.Position.Y + sin * local.X + cos * local.Y);

        Vector2 screenPos = WorldToScreen(world);
        float screenRadius = circle.Radius * PPM;

        Raylib.DrawCircleV(screenPos.ToNumericsVector2(), screenRadius, obj.Color);
    }
}
