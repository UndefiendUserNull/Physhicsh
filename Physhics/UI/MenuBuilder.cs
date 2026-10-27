using ImGuiNET;

namespace Physhicsh.UI;

public class MenuBuilder
{
    public MenuBuilder Item(string label, string? shortcut, Action onClick)
    {
        if (ImGui.MenuItem(label, shortcut)) onClick();
        return this;
    }

    public MenuBuilder Separator()
    {
        ImGui.Separator();
        return this;
    }

    public MenuBuilder Disabled(string label, string? shortcut = null)
    {
        ImGui.MenuItem(label, shortcut, false, false);
        return this;
    }

    public MenuBuilder Item(string label, Action onClick) => Item(label, null, onClick);
}