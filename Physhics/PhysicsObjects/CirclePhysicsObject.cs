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
        float screenRadius = Radius * Consts.PPM;

        Raylib.DrawCircleV(Body.Position.WorldToScreen().ToNumericsVector2(), screenRadius, Color);
    }
}
