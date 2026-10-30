using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Physhicsh.PhysicsObjects;

namespace Physhicsh;

public static class Spawn
{
    public static void SpawnRectangle(World world, Vector2 pos, List<PhysicsObject>? list = null)
    {
        RectanglePhysicsObject newRect = new(
            pos.X, pos.Y, 1, 1,
            Random.Shared.RandomColor(), 1);

        list?.Add(newRect);
        newRect.Start(world);
    }

    public static void SpawnCircle(World world, Vector2 pos, List<PhysicsObject>? list = null)
    {
        CirclePhysicsObject newBall = new(
            pos.X, pos.Y, Random.Shared.NextFloat(SpawnRules.Instance.CirclesRadius.X, SpawnRules.Instance.CirclesRadius.Y),
            Random.Shared.RandomColor(), 1);

        list?.Add(newBall);
        newBall.Start(world);
    }

}
