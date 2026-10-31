// Examples~/ScanFrame/ScanFrameDemo.cs
// Demonstrates: ScanFrame
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The brackets pulse while searching; "Reduce motion" holds them still.
//      - "Found", "Not ours" and "Searching" switch the tone.
//      - "Track marker" moves the brackets around a moving card, as a tracked image would.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ScanFrame demo: tones, the searching pulse and following a target rectangle.
    /// </summary>
    public class ScanFrameDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private ScanFrame frame;
        private VisualElement marker;
        private bool tracking;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ScanFrameDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Scan Frame",
                "Corner brackets that frame where to point, or hug a tracked marker.");
            var row = page.AddRow();
            page.AddButton(row, "Searching", () => SetTone(ScanFrameTone.Neutral, true));
            page.AddButton(row, "Found", () => SetTone(ScanFrameTone.Success, false));
            page.AddButton(row, "Not ours", () => SetTone(ScanFrameTone.Warn, false), secondary: true);
            var options = page.AddRow();
            page.AddToggle(options, "Reduce motion", false, value => root.SetReduceMotion(value));
            page.AddToggle(options, "Track marker", false, value =>
            {
                tracking = value;
                marker.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
                if (!value)
                {
                    frame.ClearTarget();
                }
            });
            page.Finish();

            page.Stage.AddToClassList("scanFrameDemo__stage");
            marker = UIToolkitExtensions.CreateVisualElement(page.Stage, "scanFrameDemo__marker");
            marker.style.display = DisplayStyle.None;
            frame = new ScanFrame { Searching = true };
            page.Stage.Add(frame);
        }

        private void Update()
        {
            if (!tracking || marker == null || marker.panel == null)
            {
                return;
            }

            // Drift the stand-in marker and keep the brackets on it.
            var t = Time.time;
            marker.style.left = 70 + Mathf.Sin(t * 0.8f) * 40;
            marker.style.top = 60 + Mathf.Cos(t * 0.6f) * 30;
            frame.SetTargetRect(marker.worldBound);
        }

        private void SetTone(ScanFrameTone tone, bool searching)
        {
            frame.Tone = tone;
            frame.Searching = searching;
            page.Write($"Tone: {tone}");
        }
    }
}
