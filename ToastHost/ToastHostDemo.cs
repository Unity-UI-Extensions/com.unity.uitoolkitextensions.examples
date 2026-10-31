// Examples~/ToastHost/ToastHostDemo.cs
// Demonstrates: ToastHost
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The buttons show plain, toned, action and undo toasts.
//      - At most three toasts are on screen; the rest queue and appear as others leave.
//      - Tap a toast to dismiss it, or swipe it away; touching one pauses its timer.
//      - "Bottom" anchors the stack to the bottom edge.
//      - "Download" shows a progress toast with an icon and a detail line that updates in
//        place and stays until the download finishes.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ToastHost demo: queue, tones, one action, undo, swipe and tap to dismiss, top or bottom anchor.
    /// </summary>
    public class ToastHostDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private ToastHost host;
        private int count;
        private Texture2D download;

        private void StartDownload()
        {
            var handle = host.Show(new ToastOptions
            {
                Message = "Getting Turbine cutaway",
                Detail = "0.0 of 4.2 MB, keeps going if you browse",
                Progress = 0f,
                Icon = Background.FromTexture2D(download),
                DurationMs = 0,
            });

            var received = 0f;
            IVisualElementScheduledItem tick = null;
            tick = host.schedule.Execute(() =>
            {
                received = Mathf.Min(4.2f, received + 0.3f);
                handle.SetProgress(received / 4.2f);
                handle.SetMessage("Getting Turbine cutaway", $"{received:0.0} of 4.2 MB, keeps going if you browse");
                if (received >= 4.2f)
                {
                    tick.Pause();
                    handle.Dismiss();
                    host.Show(new ToastOptions { Message = "Turbine cutaway is ready", Tone = ToastTone.Success });
                }
            }).Every(250);
        }

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ToastHostDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Toast Host",
                "Short messages with a queue, one optional action and auto-dismiss. Tap or swipe a toast to dismiss it.");

            var row = page.AddRow();
            page.AddButton(row, "Message", () => host.Show($"Saved as a draft ({++count})"));
            page.AddButton(row, "Success", () => host.Show(new ToastOptions { Message = "Space published", Tone = ToastTone.Success }));
            page.AddButton(row, "Error", () => host.Show(new ToastOptions
            {
                Message = "Upload failed. Check your connection.",
                Tone = ToastTone.Error,
                ActionText = "Retry",
                OnAction = () => page.Write("Retry tapped"),
            }));
            page.AddButton(row, "Undo", () => host.ShowUndo("Placed object removed", () => page.Write("Undo tapped")));
            page.AddButton(row, "Download", StartDownload);

            var options = page.AddRow();
            page.AddToggle(options, "Bottom", false, value => host.Anchor = value ? ToastAnchor.Bottom : ToastAnchor.Top);
            page.AddButton(options, "Dismiss all", () => host.DismissAll(), secondary: true);
            page.Finish();

            host = new ToastHost { TopInset = 8f, BottomInset = 16f };
            download = ProceduralTextureUtility.CreateSolidCircle(32, Color.white);
            host.ToastDismissed += (handle, reason) => page.Write($"Dismissed \"{handle.Options.Message}\": {reason}");
            root.Add(host);
        }
    }
}
