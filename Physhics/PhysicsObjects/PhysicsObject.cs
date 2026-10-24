using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects;

public class PhysicsObject(float x, float y, Color color, bool isStatic = false)
{
    public Body Body;
    public Color Color = color;
    public Vector2 Position { get { return Body.Position; } set { Body.Position = value; } }

    public bool IsStatic = isStatic;

    public virtual void Start(World world)
    {
        Body = world.CreateBody(new(x, y), 0, IsStatic ? BodyType.Static : BodyType.Dynamic);
        Position = Body.Position;
    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Draw()
    {

    }
}
