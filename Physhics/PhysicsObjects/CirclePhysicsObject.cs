using nkast.Aether.Physics2D.Dynamics;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects;

public class CirclePhysicsObject(float x, float y, float radius, Color color, float density = 1f, bool isStatic = false) : PhysicsObject(x, y, color, isStatic)
{
    public float Radius = radius;
    public override void Start(World world)
    {
        base.Start(world);

        Fixture = Body.CreateCircle(Radius, density);
        Fixture.Restitution = 0.3f;
        Fixture.Friction = 0.5f;
    }

    public override void PhysicsUpdate(World world)
    {
        base.PhysicsUpdate(world);

    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawCircleV(VectorUtils.M2Px(Body.Position), Radius, Color);
    }
}
