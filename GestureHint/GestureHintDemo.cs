// Examples~/GestureHint/GestureHintDemo.cs
// Demonstrates: GestureHint
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The touch hint shows once; "Simulate gesture" or a tap dismisses it, and "Show"
//        does nothing afterwards until "Show once" is turned off.
//      - The mouse hint below uses the desktop wording and offers the inspector link.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// GestureHint demo: touch and mouse wording, show once, dismiss by gesture or tap, inspector link.
    /// </summary>
    public class GestureHintDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private GestureHint touch;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("GestureHintDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Gesture Hint",
                "Teaches the manipulation gestures on first selection and gets out of the way.");
            var row = page.AddRow();
            page.AddButton(row, "Show", () => page.Write(touch.Show() ? "Shown" : "Already dismissed once"));
            page.AddButton(row, "Simulate gesture", () => touch.NotifyGesture(Gesture.Rotate), secondary: true);
            page.AddToggle(row, "Show once", true, value => touch.ShowOnce = value);
            page.Finish();

            touch = new GestureHint();
            touch.Dismissed += () => page.Write("Dismissed");
            page.Stage.Add(touch);
            touch.Show();

            var mouse = new GestureHint { Platform = GestureHintPlatform.Mouse, InspectorText = "Inspector", ShowOnce = false };
            mouse.AddToClassList("gestureHintDemo__mouse");
            mouse.OpenInspector += () => page.Write("OpenInspector");
            page.Stage.Add(mouse);
            mouse.Show();
        }
    }
}
