using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Physhicsh.UI;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects;

public class PhysicsObject(float x, float y, Color color, BodyType bodyType = BodyType.Dynamic)
{
    public Body Body = null!;
    public Color Color = color;
    public Fixture? Fixture { get; set; }
    public Texture2D? Texture;
    private bool _hasPropCachedColor;
    private System.Numerics.Vector3 _cachedPropColor;

    private readonly TextureLoader _texLoader = new();
    public virtual void Start(World world)
    {
        Body = world.CreateBody(new(x, y), 0, bodyType);
        Body.Tag = this;
    }


    public virtual void PhysicsUpdate(World world)
    {

    }

    public virtual void Draw()
    {

    }
    public virtual void Delete()
    {
        Body.Remove(Fixture);
        Body.Tag = null;
        Body = null!;
    }

    public virtual void Properties(ImGuiUI ui)
    {
        Color currentColor = Color;

        if (!_hasPropCachedColor || currentColor != _cachedPropColor.ToAetherVector3().ToColor())
        {
            _cachedPropColor = currentColor.ToVector3().ToNumericsVector3() / 255f;
            _hasPropCachedColor = true;
        }

        if (ImGui.ColorEdit3("Color", ref _cachedPropColor))
        {
            Color = _cachedPropColor.ToAetherVector3().ToColor();
        }

        float massRefSlider = Body.Mass;

        if (ImGui.SliderFloat("Mass", ref massRefSlider, 0f, 100f))
        {
            Body.Mass = massRefSlider;
        }

        float res = Fixture.Restitution;
        if (ImGui.SliderFloat("Boingness", ref res, 0f, 2f))
        {
            Fixture.Restitution = res;
        }

        float slippy = Fixture.Friction;
        if (ImGui.SliderFloat("Slippiness", ref slippy, 0f, 2f))
        {
            Fixture.Friction = slippy;
        }

        //if (ImGui.Button("Set texture"))
        //{
        //    byte[] buff = [];
        //    if (ImGui.InputText("Path", buff, 1024))
        //    {

        //    }
        //}
        ImGui.Separator();
        //ImGui.Text($"{Texture}");
    }
}
