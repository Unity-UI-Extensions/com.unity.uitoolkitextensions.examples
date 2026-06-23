// Examples~/ScreenHeader/ScreenHeaderDemo.cs
// Demonstrates: ScreenHeader
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A full-width header bar sits at the top: a back button (left edge), a centered
//        title, and a sound on/off toggle (right edge). An info button sits beside it.
//      - Tap the back or info button to log the action to the status line below.
//      - Tap the sound toggle to flip between the volume-on / volume-off icons; the
//        status line reports the muted state.
//
// The header icons are supplied entirely through USS (ScreenHeaderStyles.uss) using the
// package's SVG icon assets, so the demo assigns no Texture2D references in code. The
// toggle glyph swaps automatically via the control's "toggleButton--selected" state class.

using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Screen header demo using <see cref="ScreenHeader"/>.
    ///
    /// The control owns no application state: it raises <see cref="ScreenHeader.Action1Clicked"/>
    /// (back), <see cref="ScreenHeader.Action3Clicked"/> (info) and
    /// <see cref="ScreenHeader.Action4Toggled"/> (sound on/off), which the demo turns into
    /// status-line updates. The host stays responsible for any behaviour behind those events.
    /// </summary>
    public class ScreenHeaderDemo : MonoBehaviour
    {
        private ScreenHeader header;
        private Label statusLabel;

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ScreenHeaderDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "screenHeaderDemo__screen");

            header = UIToolkitExtensions.CreateVisualElement<ScreenHeader>(screen, "screenHeaderDemo__header");
            header.Title = "header";
            header.Configure(showAction1: true, showTitle: true, showAction2: false,
                showAction3: true, showAction4Toggle: true);

            header.Action1Clicked += () => SetStatus("Back tapped — a real app would pop this screen.");
            header.Action3Clicked += () => SetStatus("Info tapped — show help or an about panel here.");
            header.Action4Toggled += isMuted => SetStatus(isMuted ? "Sound muted." : "Sound on.");

            VisualElement body = UIToolkitExtensions.CreateVisualElement(screen, "screenHeaderDemo__body");
            VisualElement card = UIToolkitExtensions.CreateVisualElement(body, "screenHeaderDemo__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "screenHeaderDemo__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label title = UIToolkitExtensions.CreateVisualElement<Label>(card, "screenHeaderDemo__title");
            title.text = "Screen Header";

            Label instruction = UIToolkitExtensions.CreateVisualElement<Label>(card, "screenHeaderDemo__instruction");
            instruction.text = "A configurable top bar with edge action buttons. Tap the back or info buttons, " +
                "or flip the sound toggle, to see the header report each interaction below.";

            statusLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "screenHeaderDemo__status");
            statusLabel.text = "Interact with the header above.";
        }

        private void SetStatus(string message)
        {
            if (statusLabel != null)
            {
                statusLabel.text = message;
            }
        }
    }
}
