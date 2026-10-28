using ImGuiNET;

namespace Physhicsh.UI;

public class MenuBuilder
{
    public MenuBuilder Item(string label, string? shortcut, Action onClick)
    {
        if (ImGui.MenuItem(label, shortcut)) onClick();
        return this;
    }
    public bool Item(string label) => ImGui.MenuItem(label);

    public MenuBuilder Separator()
    {
        ImGui.Separator();
        return this;
    }

    public bool Checkbox(string label, Func<bool> get, Action<bool> set)
    {
        bool v = get();
        if (!ImGui.MenuItem(label, "", ref v)) return false;
        set(v);
        return true;
    }

    public MenuBuilder Disabled(string label, string? shortcut = null)
    {
        ImGui.MenuItem(label, shortcut, false, false);
        return this;
    }

    public MenuBuilder SubMenu(string label, Action<MenuBuilder> build)
    {
        if (ImGui.BeginMenu(label))
        {
            build(new MenuBuilder());
            ImGui.EndMenu();
        }
        return this;
    }

    public MenuBuilder Item(string label, Action onClick) => Item(label, null, onClick);
}