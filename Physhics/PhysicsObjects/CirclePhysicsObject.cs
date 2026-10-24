using nkast.Aether.Physics2D.Dynamics;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects;

public class CirclePhysicsObject(float x, float y, int radius, Color color, float density = 1f, bool isStatic = false) : PhysicsObject(x, y, color, isStatic)
{
    public int Radius = radius;
    public override void Start(World world)
    {
        base.Start(world);

        Fixture fFixture = Body.CreateCircle(radius, density);
        fFixture.Restitution = 0.3f;
        fFixture.Friction = 0.5f;
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawCircleV(Position.ToNumericsVector2(), Radius, Color);
    }
}
