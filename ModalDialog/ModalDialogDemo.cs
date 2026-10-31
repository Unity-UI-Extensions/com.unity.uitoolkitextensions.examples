// Examples~/ModalDialog/ModalDialogDemo.cs
// Demonstrates: ModalDialog
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Each button awaits ModalDialog.Show(options) and writes the result below.
//      - Tap the scrim or press Escape to dismiss; the "Required" dialog ignores both.
//      - Tab and Shift+Tab stay inside the dialog while it is open.
//      - "Light surface" switches the dialog to the page palette.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ModalDialog demo: confirm, destructive, input and choice fields, single action, required, light surface.
    /// </summary>
    public class ModalDialogDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private ModalDialog dialog;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ModalDialogDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Modal Dialog",
                "One dialog component for confirmations, destructive actions and short forms. Show() returns a Task with the outcome.");

            var row = page.AddRow();
            page.AddButton(row, "Confirm", () => Run(new DialogOptions
            {
                Title = "Leave this experience?",
                Body = "Placed content stays where it is for the next visit.",
                PrimaryText = "Leave",
                SecondaryText = "Stay",
            }));
            page.AddButton(row, "Destructive", () => Run(new DialogOptions
            {
                Title = "Delete this space?",
                Body = "Its anchors and placed content are removed from this device.",
                PrimaryText = "Delete",
                SecondaryText = "Cancel",
                Variant = DialogVariant.Destructive,
            }));
            page.AddButton(row, "Name and visibility", () => Run(new DialogOptions
            {
                Title = "Name this space",
                PrimaryText = "Save",
                SecondaryText = "Cancel",
                InputLabel = "Space name",
                InputValue = "Lobby",
                Choices = new[] { "Private", "Unlisted", "Public" },
                ChoicesLabel = "Visibility",
            }));

            var row2 = page.AddRow();
            page.AddButton(row2, "Single action", () => Run(new DialogOptions
            {
                Title = "Saved",
                Body = "Your space is saved as a draft.",
                PrimaryText = "Done",
            }), secondary: true);
            page.AddButton(row2, "Required", () => Run(new DialogOptions
            {
                Title = "Camera access needed",
                Body = "The app places content in the world through your camera.",
                PrimaryText = "Open settings",
                SecondaryText = "Not now",
                Dismissible = false,
            }), secondary: true);
            page.AddToggle(row2, "Light surface", false, value => dialog.SetSurface(value ? ThemeSurface.Light : ThemeSurface.Inherit));
            page.Finish();

            dialog = new ModalDialog();
            root.Add(dialog);
        }

        private async void Run(DialogOptions options)
        {
            var result = await dialog.Show(options);
            var details = options.InputLabel != null ? $", input \"{result.InputValue}\", choice {result.ChoiceIndex}" : string.Empty;
            page.Write($"{options.Title}: {result.Action}{details}");
        }
    }
}
