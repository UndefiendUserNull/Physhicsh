using nkast.Aether.Physics2D.Dynamics;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects;

public class PhysicsObject(float x, float y, Color color, bool isStatic = false)
{
    public Body Body = null!;
    public Color Color = color;
    public Fixture? Fixture { get; set; }
    public bool IsStatic = isStatic;

    public virtual void Start(World world)
    {
        Body = world.CreateBody(new(x, y), 0, IsStatic ? BodyType.Static : BodyType.Dynamic);
    }


    public virtual void PhysicsUpdate(World world)
    {

    }

    public virtual void Draw()
    {

    }
}
