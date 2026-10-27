using ImGuiNET;
using System.Numerics;

namespace Physhicsh.UI;

public sealed class ImGuiUI(ImFontPtr font)
{
    private readonly ImFontPtr _font = font;

    private sealed class PopupState
    {
        public string Id = "";
        public Vector2 ScreenPos = new();
        public Action? Body = null;
        public bool PendingOpen;
        public bool ShouldClose;
    }

    private readonly Dictionary<string, PopupState> _popups = [];


    public FontScope UseFont() => new(_font);

    public void BeginFrame()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);
        ImGui.SetNextWindowSize(io.DisplaySize);
        ImGui.Begin("##ui_host",
            ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground |
            ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoSavedSettings |
            ImGuiWindowFlags.NoBringToFrontOnFocus);
    }

    public void EndFrame()
    {
        RenderPopups();
        ImGui.End();
    }

    public void ShowPopup(string id, Vector2 screenPos, Action body)
    {
        // if the popup doesn't already exist, create a new one
        if (!_popups.TryGetValue(id, out PopupState? p))
        {
            _popups[id] = p = new PopupState { Id = id };
        }

        p.ScreenPos = screenPos;
        p.Body = body;
        p.PendingOpen = true;
        p.ShouldClose = false;
    }

    public void ClosePopup(string id)
    {
        if (_popups.TryGetValue(id, out PopupState? p)) p.ShouldClose = true;
    }

    public bool AnyPopupsOpen() => _popups.Count > 0;

    private void RenderPopups()
    {
        List<string>? toRemove = null;

        foreach ((string? id, PopupState? p) in _popups)
        {
            if (p.PendingOpen)
            {
                ImGui.SetNextWindowPos(p.ScreenPos);
                ImGui.OpenPopup(id);
                p.PendingOpen = false;
            }

            if (!ImGui.BeginPopup(id))
            {
                (toRemove ??= new()).Add(id);
                continue;
            }

            try
            {
                if (p.ShouldClose)
                {
                    ImGui.CloseCurrentPopup();
                }
                else
                {
                    using (UseFont())
                    {
                        if (p.Body != null)
                            p.Body();
                        else throw new NullReferenceException($"{p.Id} doesn't contain a body");
                    }
                }
            }
            finally
            {
                ImGui.EndPopup();
            }

            if (p.ShouldClose)
                (toRemove ??= new()).Add(id);
        }

        if (toRemove is not null)
            foreach (string id in toRemove)
                _popups.Remove(id);
    }

    public static void Menu(string label, Action<MenuBuilder> build)
    {
        if (!ImGui.BeginMenu(label)) return;
        build(new());
        ImGui.EndMenu();
    }

    public void BeginWindow(string label, ImGuiWindowFlags windowFlags, Action body)
    {
        if (ImGui.Begin(label, windowFlags))
        {
            using (UseFont())
            {
                body();
            }
            ImGui.End();
        }
    }

}
