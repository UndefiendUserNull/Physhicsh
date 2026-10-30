using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.PhysicsObjects;
using System.Collections.Generic;

namespace Physhicsh.UI
{
    public static class ImGuiStaticPopups
    {
        public static void DrawQuickSpawnMenu(World world, Vector2 spawnPos, MenuBuilder mb, List<PhysicsObject> physicsObjects)
        {
            mb.Item("Rectangle", "r", () => Spawn.SpawnRectangle(world, spawnPos, physicsObjects));
            mb.Item("Circle", "c", () => Spawn.SpawnCircle(world, spawnPos, physicsObjects));
        }
    }
}
