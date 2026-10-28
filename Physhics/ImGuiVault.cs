namespace Physhicsh;

public enum ImGuiWindow
{
    None,
    Settings,
    Properties,
}

public class ImGuiVault
{
    private readonly HashSet<ImGuiWindow> _open = [];
    public readonly string IMGUI_FONT_PATH = "fonts/arial.ttf";
    public bool IsOpen(ImGuiWindow p) => _open.Contains(p);
    public void Open(ImGuiWindow p) => _open.Add(p);
    public void Close(ImGuiWindow p) => _open.Remove(p);
    public void Toggle(ImGuiWindow p) { if (!_open.Add(p)) _open.Remove(p); }
    public void CloseAll() => _open.Clear();
}
