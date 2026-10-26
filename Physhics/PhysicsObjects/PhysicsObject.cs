using nkast.Aether.Physics2D.Dynamics;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects;

public class PhysicsObject(float x, float y, Color color, BodyType bodyType = BodyType.Dynamic)
{
    public Body Body = null!;
    public Color Color = color;
    public Fixture? Fixture { get; set; }
    public BodyType BodyType = bodyType;

    public virtual void Start(World world)
    {
        Body = world.CreateBody(new(x, y), 0, BodyType);
        Body.Tag = this;
    }


    public virtual void PhysicsUpdate(World world)
    {

    }

    public virtual void Draw()
    {

    }
}
