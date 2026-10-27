using ImGuiNET;
using nkast.Aether.Physics2D.Common;

namespace Physhicsh;

/// <summary>
/// Stores all ImGui variables and stuff
/// </summary>
public class ImGuiVault
{
    public readonly string IMGUI_FONT_PATH = "fonts/arial.ttf";
    public ImFontPtr ImGuiFont { get; set; }
    public Vector2 ContextMenuWorldPos { get; set; }
    public bool QuickSpawnMenuPending { get; set; } = false;
    public Vector2 QuickSpawnMenuMousePos { get; set; }
    public Vector2 QuickSpawnMenuScreenPos { get; set; }
    public bool ContextMenuOpen { get; set; } = false;
    public bool ShowProperties { get; set; } = false;
}
