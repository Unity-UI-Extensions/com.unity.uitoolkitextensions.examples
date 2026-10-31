// Examples~/ToolRail/ToolRailDemo.cs
// Demonstrates: ToolRail and GestureHint
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - An action rail (duplicate, undo, inspect, delete) with the gesture hint above it,
//        as when an object is selected. "Undo" starts disabled.
//      - A mode rail below keeps one tool active and shows labels.
//      - "Simulate gesture" reports a manipulation, which dismisses the hint.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ToolRail demo: an action rail and a mode rail, with the GestureHint chip.
    /// </summary>
    public class ToolRailDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private GestureHint hint;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ToolRailDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Tool Rail",
                "Icon tools in a pill: an action rail for a selected object, and a mode rail with one active tool.");
            var row = page.AddRow();
            page.AddButton(row, "Show hint", () => { hint.ShowOnce = false; hint.Show(); });
            page.AddButton(row, "Simulate gesture", () => hint.NotifyGesture(Gesture.Move), secondary: true);
            page.Finish();

            var dot = Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(32, Color.white));

            hint = new GestureHint { InspectorText = "Inspector" };
            hint.AddToClassList("toolRailDemo__hint");
            hint.Dismissed += () => page.Write("Hint dismissed");
            hint.OpenInspector += () => page.Write("OpenInspector");
            page.Stage.Add(hint);
            hint.Show();

            var actions = new ToolRail();
            actions.SetItems(new List<ToolItem>
            {
                new("duplicate", "Duplicate", dot),
                new("undo", "Undo", dot),
                new("inspect", "Inspect", dot),
                new("delete", "Delete", dot, warn: true),
            });
            actions.SetItemEnabled("undo", false);
            actions.ToolSelected += id => page.Write($"ToolSelected: {id}");
            page.Stage.Add(actions);

            var modes = new ToolRail { Selectable = true, ShowLabels = true, ActiveId = "library" };
            modes.AddToClassList("toolRailDemo__modes");
            modes.SetItems(new List<ToolItem>
            {
                new("library", "Library", dot),
                new("scan", "Scan", dot),
                new("capture", "Capture", dot),
                new("map", "Map", dot),
            });
            modes.ToolSelected += id => page.Write($"Mode: {id}");
            page.Stage.Add(modes);
        }
    }
}
