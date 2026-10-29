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
    private Body? _kinematicDragBody;
    private BodyType _prevKinematicBodyType;
    private Vector2 _prevKinematicLinearVelocity;
    private bool _isKinematicDragging;

    public void UpdateMouse()
    {
        MousePosWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();
    }

    public void HandleShortcuts(ImGuiUI ui, ref PhysicsObject pickedObject, Action<PhysicsObject> DeletePhysicsObject)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        if (io.WantCaptureKeyboard) return;

        float dt = Raylib.GetFrameTime();

        if (Raylib.IsKeyPressedRepeat(KeyboardKey.R) || Raylib.IsKeyPressed(KeyboardKey.R)) Spawn.SpawnRectangle(_world, MousePosWorld, _physicsObjects);
        if (Raylib.IsKeyPressedRepeat(KeyboardKey.C) || Raylib.IsKeyPressed(KeyboardKey.C)) Spawn.SpawnCircle(_world, MousePosWorld, _physicsObjects);
        if (Raylib.IsKeyPressed(KeyboardKey.Tab))
        {
            Vector2 mousePos = MousePosWorld;
            ui.ShowPopup("##quick_spawn", Raylib.GetMousePosition(), () => ImGuiStaticPopups.DrawQuickSpawnMenu(_world, mousePos, new(), _physicsObjects));
        }
        if (Raylib.IsKeyPressed(KeyboardKey.V) && pickedObject is not null)
        {
            if (pickedObject.Body.BodyType == BodyType.Static) pickedObject.Body.BodyType = BodyType.Dynamic;
            else if (pickedObject.Body.BodyType == BodyType.Dynamic) pickedObject.Body.BodyType = BodyType.Static;
        }
        if (Raylib.IsKeyPressed(KeyboardKey.X) && pickedObject is not null) DeletePhysicsObject(pickedObject);
        if (Raylib.IsKeyPressed(KeyboardKey.F3)) ShowDebugWindow = !ShowDebugWindow;

        if (pickedObject is not null)
        {
            float direction = 0f;
            float rotationSpeed = 0f;
            if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
            {
                rotationSpeed = Consts.ROTATION_SPEED / 2;
            }
            else
            {
                rotationSpeed = Consts.ROTATION_SPEED;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Q)) direction += rotationSpeed;
            if (Raylib.IsKeyDown(KeyboardKey.E)) direction -= rotationSpeed;

            if (direction != 0f)
            {
                pickedObject.Body.Rotation += direction * Consts.ROTATION_SPEED * 60f * dt;
                pickedObject.Body.AngularVelocity = 0f;
                pickedObject.Body.Awake = true;
            }
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
            if (MouseJoint != null || _isKinematicDragging)
                EndDrag(world);

            return;
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            AetherUtils.PickFixture(world, MousePosWorld, ref pickedObject);
            BeginDrag(world, ref pickedObject);
        }
        else if ((MouseJoint != null || _isKinematicDragging) && Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            UpdateDrag(ref pickedObject);
        }
        else if ((MouseJoint != null || _isKinematicDragging) && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            EndDrag(world);
        }
    }

    public void BeginDrag(World world, ref PhysicsObject? pickedObject)
    {
        if (pickedObject == null) return;

        Body body = pickedObject.Body;

        if (body.BodyType != BodyType.Dynamic)
        {
            _kinematicDragBody = body;
            _prevKinematicBodyType = body.BodyType;
            _prevKinematicLinearVelocity = body.LinearVelocity;
            body.BodyType = BodyType.Kinematic;
            body.Enabled = true;
            body.Awake = true;
            _isKinematicDragging = true;
        }
        else
        {
            MouseJoint = new FixedMouseJoint(body, MousePosWorld)
            {
                MaxForce = 100000f,
                Frequency = 10.0f,
            };

            world.Add(MouseJoint);
        }
    }

    public void UpdateDrag(ref PhysicsObject? pickedObject)
    {
        if (_isKinematicDragging && _kinematicDragBody != null)
        {
            const float stiffness = 20.0f;
            const float maxSpeed = 50.0f;

            Vector2 desiredVelocity = (MousePosWorld - _kinematicDragBody.Position) * stiffness;
            if (desiredVelocity.LengthSquared() > maxSpeed * maxSpeed)
                (desiredVelocity *= maxSpeed).Normalize();

            _kinematicDragBody.LinearVelocity = desiredVelocity;
            _kinematicDragBody.Awake = true;
        }
        else if (MouseJoint != null)
        {
            MouseJoint.WorldAnchorB = MousePosWorld;
        }
    }

    public void EndDrag(World world)
    {
        if (MouseJoint is not null)
        {
            world.Remove(MouseJoint);
            MouseJoint = null;
        }

        if (_isKinematicDragging && _kinematicDragBody != null)
        {
            _kinematicDragBody.LinearVelocity = _prevKinematicLinearVelocity;
            _kinematicDragBody.BodyType = _prevKinematicBodyType;
            _kinematicDragBody = null;
            _isKinematicDragging = false;
        }
    }
}