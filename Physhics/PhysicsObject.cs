namespace Physhicsh;

public class PhysicsObject(int x, int y, int w, int h, bool isStatic = false)
{
    public Vector2Int Position { get; set; } = new Vector2Int(x, y);
    public Vector2Int Bounds { get; set; } = new Vector2Int(w, h);
    public bool IsStatic = isStatic;

    public virtual void PhysicsUpdate()
    {

    }
}
