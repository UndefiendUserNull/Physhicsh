using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.PhysicsObjects;

namespace Physhicsh.Utils;

public static class AetherUtils
{
    public static Fixture? PickFixture(World world, Vector2 mouseWorld, ref PhysicsObject? pickedObject)
    {
        Fixture fixture = world.TestPoint(mouseWorld);

        if (fixture != null)
            pickedObject = fixture.Tag as PhysicsObject;
        else
            pickedObject = null;

        return fixture == null ? null : fixture;
    }
}
