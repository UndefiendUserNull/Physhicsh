using ImGuiNET;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Physhicsh.Extensions;
using Physhicsh.PhysicsObjects;
using Physhicsh.UI;
using Physhicsh.Utils;
using Raylib_cs;

namespace Physhicsh;

public class Input(World world, List<PhysicsObject> objs)
{
    private readonly World _world = world;
    private readonly List<PhysicsObject> _physicsObjects = objs;
    public bool ShowDebugWindow { get; set; } = false;
    public Vector2 MousePosWorld { get; private set; } = new();
    public FixedMouseJoint? MouseJoint { get; private set; }
    private bool _isRotating = false;
    private BodyType _prevRotatingBodyType;
    public void UpdateMouse()
    {
        MousePosWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();
    }

    public void HandleShortcuts(ImGuiUI ui, ref PhysicsObject pickedObject, Action<PhysicsObject> DeletePhysicsObject)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        if (io.WantCaptureKeyboard) return;

        if (Raylib.IsKeyPressedRepeat(KeyboardKey.R) || Raylib.IsKeyPressed(KeyboardKey.R)) Spawn.SpawnRectangle(_world, MousePosWorld, _physicsObjects);
        if (Raylib.IsKeyPressedRepeat(KeyboardKey.C) || Raylib.IsKeyPressed(KeyboardKey.C)) Spawn.SpawnCircle(_world, MousePosWorld, _physicsObjects);
        if (Raylib.IsKeyPressed(KeyboardKey.Tab))
        {
            Vector2 mousePos = MousePosWorld;
            ui.ShowPopup("##quick_spawn", Raylib.GetMousePosition(), () => ImGuiStaticPopups.DrawQuickSpawnMenu(_world, mousePos, new(), _physicsObjects));
        }
        if (Raylib.IsKeyPressed(KeyboardKey.V) && pickedObject is not null)
        {
            if (pickedObject.BodyType == BodyType.Static) pickedObject.BodyType = BodyType.Dynamic;
            else if (pickedObject.BodyType == BodyType.Dynamic) pickedObject.BodyType = BodyType.Static;

        }
        if (Raylib.IsKeyPressed(KeyboardKey.X) && pickedObject is not null) DeletePhysicsObject(pickedObject);
        if (Raylib.IsKeyPressed(KeyboardKey.F3)) ShowDebugWindow = !ShowDebugWindow;

        // Rotation

        if ((Raylib.IsKeyDown(KeyboardKey.Q) || Raylib.IsKeyDown(KeyboardKey.E)) && pickedObject is not null)
        {
            _isRotating = true;
            _prevRotatingBodyType = pickedObject.Body.BodyType;
            //pickedObject.Body.ResetDynamics();
            pickedObject.Body.BodyType = BodyType.Static;
        }
        else if (_isRotating && !Raylib.IsMouseButtonDown(MouseButton.Left) && pickedObject is not null)
        {
            pickedObject.Body.BodyType = _prevRotatingBodyType;
            _isRotating = false;
        }

        if (_isRotating && pickedObject is not null)
        {
            pickedObject.Body.Enabled = false;
            if (Raylib.IsKeyDown(KeyboardKey.Q))
            {
                pickedObject.Body.Rotation += -Consts.ROTATION_SPEED;
            }
            if (Raylib.IsKeyDown(KeyboardKey.E))
            {
                pickedObject.Body.Rotation += Consts.ROTATION_SPEED;
            }
        }
        else if (!_isRotating && pickedObject is not null)
        {
            pickedObject.Body.Enabled = true;

        }

        if (pickedObject is CirclePhysicsObject circle)
        {
            if (Raylib.IsKeyDown(KeyboardKey.H))
                circle.Radius += 0.01f;
            if (Raylib.IsKeyDown(KeyboardKey.J))
                circle.Radius -= 0.01f;
        }
    }

    public void HandleMouseInput(ImGuiUI ui, World world, ref PhysicsObject pickedObject)
    {
        if (ui.AnyPopupsOpen() || ImGui.GetIO().WantCaptureMouse)
        {
            if (MouseJoint != null)
                EndDrag(world);

            return;
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            AetherUtils.PickFixture(world, MousePosWorld, ref pickedObject);
            BeginDrag(world, ref pickedObject);
        }
        else if (MouseJoint != null && _isRotating == false && Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            UpdateDrag();
        }
        else if (MouseJoint != null && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            EndDrag(world);
        }
    }

    public void BeginDrag(World world, ref PhysicsObject? pickedObject)
    {
        if (pickedObject == null) return;

        Vector2 mouseWorld = MousePosWorld;

        MouseJoint = new FixedMouseJoint(pickedObject.Body, mouseWorld)
        {
            MaxForce = 100000f,
        };
        world.Add(MouseJoint);
    }

    public void UpdateDrag()
    {
        MouseJoint!.WorldAnchorB = MousePosWorld;
    }

    public void EndDrag(World world)
    {
        if (MouseJoint is not null)
            world.Remove(MouseJoint);

        MouseJoint = null;
    }
}
