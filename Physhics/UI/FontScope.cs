using ImGuiNET;
using System;

namespace Physhicsh.UI;

public readonly struct FontScope : IDisposable
{
    public FontScope(ImFontPtr font) => ImGui.PushFont(font);
    public void Dispose() => ImGui.PopFont();
}
