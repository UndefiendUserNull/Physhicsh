using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Physhicsh.UI;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects;

public class CirclePhysicsObject(float x, float y, float radius, Color color, float density = 0f, BodyType bodyType = BodyType.Dynamic) : PhysicsObject(x, y, color, bodyType)
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


    public override void Draw()
    {
        base.Draw();
        float screenRadius = Radius * Config.Instance.PPM;

        Raylib.DrawCircleV(Body.Position.WorldToScreen().ToNumericsVector2(), screenRadius, Color);
    }

    public override void Properties(ImGuiUI ui)
    {
        base.Properties(ui);

        float r = Radius;
        if (ImGui.SliderFloat("Radius", ref r, 0f, 3f))
        {
            Radius = r;
        }
    }
}
