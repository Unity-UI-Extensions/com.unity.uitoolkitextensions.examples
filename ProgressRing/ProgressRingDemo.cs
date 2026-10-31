// Examples~/ProgressRing/ProgressRingDemo.cs
// Demonstrates: ProgressRing
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The first ring shows a percentage, the second a "4 of 12" count, the third a
//        small download ring; "Step" advances them, and they turn green when complete.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ProgressRing demo: percentage, count text, sizes and the complete state.
    /// </summary>
    public class ProgressRingDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private ProgressRing coverage;
        private ProgressRing count;
        private ProgressRing download;
        private int step;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ProgressRingDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Progress Ring",
                "A determinate ring with a centre value and caption, drawn in the token colours.");
            var row = page.AddRow();
            page.AddButton(row, "Step", Advance);
            page.AddButton(row, "Reset", () => { step = -1; Advance(); }, secondary: true);
            page.Finish();

            var rings = UIToolkitExtensions.CreateVisualElement(page.Stage, "progressRingDemo__row");
            coverage = new ProgressRing { Size = 120f, Thickness = 10f, Label = "coverage" };
            count = new ProgressRing { Size = 96f, Label = "stops" };
            download = new ProgressRing { Size = 56f, Thickness = 5f };
            rings.Add(coverage);
            rings.Add(count);
            rings.Add(download);
            Advance();
        }

        private void Advance()
        {
            step = Mathf.Min(step + 1, 12);
            coverage.Value = step / 12f;
            count.Value = step / 12f;
            count.ValueText = $"{step} of 12";
            download.Value = Mathf.Min(1f, step / 6f);
            page.Write($"Step {step} of 12");
        }
    }
}
