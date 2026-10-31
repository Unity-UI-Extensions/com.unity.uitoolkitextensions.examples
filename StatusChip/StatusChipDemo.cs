// Examples~/StatusChip/StatusChipDemo.cs
// Demonstrates: StatusChip and StatusChipHost
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - "Start upload" shows a busy chip that counts up, then resolves to ok and lingers.
//      - "Go offline" shows a warn chip until "Back online" resolves it.
//      - "Fail" shows how a failed operation reads before it is resolved.
//      - The row below shows every state as a standalone StatusChip.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// StatusChip demo: persistent statuses keyed by id, updated in place and resolved with a final state.
    /// </summary>
    public class StatusChipDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private StatusChipHost host;
        private IVisualElementScheduledItem upload;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("StatusChipDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Status Chip",
                "A status that stays until it is resolved. The host keeps one chip per id and updates it in place.");

            var row = page.AddRow();
            page.AddButton(row, "Start upload", StartUpload);
            page.AddButton(row, "Go offline", () => host.ShowStatus("network", "Offline", StatusState.Warn));
            page.AddButton(row, "Back online", () => host.Resolve("network", StatusState.Ok, "Back online"), secondary: true);
            page.AddButton(row, "Fail", () => host.ShowStatus("sync", "Sync failed. Tap to retry.", StatusState.Fail), secondary: true);
            page.Finish();

            var gallery = UIToolkitExtensions.CreateVisualElement(page.Stage, "statusChipDemo__gallery");
            gallery.Add(new StatusChip("Info", StatusState.Info));
            gallery.Add(new StatusChip("Busy", StatusState.Busy));
            gallery.Add(new StatusChip("Ok", StatusState.Ok));
            gallery.Add(new StatusChip("Warn", StatusState.Warn));
            gallery.Add(new StatusChip("Fail", StatusState.Fail));

            host = new StatusChipHost { TopInset = 8f };
            host.StatusResolved += id => page.Write($"Resolved: {id}");
            host.ChipClicked += id =>
            {
                page.Write($"Tapped: {id}");
                if (id == "sync")
                {
                    host.Resolve("sync");
                }
            };
            root.Add(host);
        }

        private void StartUpload()
        {
            upload?.Pause();
            var step = 0;
            host.ShowStatus("upload", "Uploading 1 of 4", StatusState.Busy);
            upload = host.schedule.Execute(() =>
            {
                step++;
                if (step < 4)
                {
                    host.ShowStatus("upload", $"Uploading {step + 1} of 4", StatusState.Busy);
                }
                else
                {
                    host.Resolve("upload", StatusState.Ok, "Uploaded");
                    upload.Pause();
                }
            }).Every(800).StartingIn(800);
        }
    }
}
