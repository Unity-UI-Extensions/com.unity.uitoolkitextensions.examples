// Examples~/CoachingOverlay/CoachingOverlayDemo.cs
// Demonstrates: CoachingOverlay
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The buttons show each coaching mode; the warning modes use the warn colour.
//      - "Reduce motion" stops the sway. "Dismissible" lets a tap on the cue hide it.
//      - "Top card" shows the card placement with an icon disc and a detail line; "Bottom card"
//        puts the same card above a 92 px inset, where scan actions would sit.
//      - "Two actions" adds a ghost and a primary action side by side; "Progress" adds the thin
//        progress line under the detail, which fills over a few seconds.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// CoachingOverlay demo: the four modes, reduce motion and tap to dismiss.
    /// </summary>
    public class CoachingOverlayDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private CoachingOverlay overlay;
        private float progressStart;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("CoachingOverlayDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Coaching Overlay",
                "A non-blocking cue for AR tracking with one line of plain-language guidance.");
            var row = page.AddRow();
            page.AddButton(row, "Look around", () => overlay.Show(CoachingMode.LookAround));
            page.AddButton(row, "Find surface", () => overlay.Show(CoachingMode.FindSurface));
            page.AddButton(row, "Too fast", () => overlay.Show(CoachingMode.MoveSlower), secondary: true);
            page.AddButton(row, "Too dark", () => overlay.Show(CoachingMode.TooDark), secondary: true);
            var options = page.AddRow();
            page.AddToggle(options, "Reduce motion", false, value => root.SetReduceMotion(value));
            page.AddToggle(options, "Dismissible", false, value => overlay.Dismissible = value);
            page.AddToggle(options, "Top card", false, value =>
            {
                overlay.Placement = value ? CoachingPlacement.Top : CoachingPlacement.Centre;
                overlay.SetIcon(value ? Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(44, Color.white)) : default);
                overlay.Detail = value ? "Move the phone slowly over the floor or a table. Content appears once the room is understood." : null;
            });
            page.AddButton(options, "Hide", () => overlay.Hide(), secondary: true);
            var extras = page.AddRow();
            page.AddToggle(extras, "Two actions", false, value =>
            {
                overlay.ActionText = value ? "Rescan" : null;
                overlay.SecondaryActionText = value ? "Keep trying" : null;
            });
            page.AddToggle(extras, "Progress", false, value =>
            {
                progressStart = Time.time;
                overlay.Progress = value ? 0f : -1f;
            });
            page.AddToggle(extras, "Indeterminate", false, value => overlay.ProgressIndeterminate = value);
            page.AddToggle(extras, "Bottom card", false, value =>
            {
                overlay.Placement = value ? CoachingPlacement.Bottom : CoachingPlacement.Centre;
                overlay.BottomInset = value ? 92f : 0f;
                overlay.SetIcon(value ? Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(44, Color.white)) : default);
                overlay.Detail = value ? "Walk slowly along the walls and keep the floor in view." : null;
            });
            page.Finish();

            overlay = new CoachingOverlay();
            overlay.Dismissed += () => page.Write("Dismissed");
            overlay.ActionInvoked += () => page.Write("Action: Rescan");
            overlay.SecondaryActionInvoked += () => page.Write("Secondary action: Keep trying");
            root.Add(overlay);
            overlay.Show(CoachingMode.LookAround);
        }

        private void Update()
        {
            if (overlay != null && overlay.Progress >= 0f)
            {
                overlay.Progress = Mathf.Repeat((Time.time - progressStart) / 6f, 1f);
            }
        }
    }
}
