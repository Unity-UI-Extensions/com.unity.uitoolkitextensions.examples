// Examples~/SegmentedControl/SegmentedControlDemo.cs
// Demonstrates: SegmentedControl (the extended QuadrantStepper)
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The first control uses equal sizing with an icon and a disabled segment.
//      - The second uses content sizing with more options than fit; drag or scroll it.
//      - The third sits on a light surface.
//      - Tab to a control and use the arrow keys, Home and End to move the selection.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// SegmentedControl demo: equal and content sizing, icons, a disabled segment and keyboard navigation.
    /// </summary>
    public class SegmentedControlDemo : MonoBehaviour
    {
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SegmentedControlDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Segmented Control",
                "N segments with a sliding indicator. Tap, drag the wide row, or use the arrow keys, Home and End.");
            page.Finish();

            var equal = new SegmentedControl(new[] { "Small", "Medium", "Large", "Huge" });
            equal.AddToClassList("segmentedDemo__control");
            equal.SetSegmentIcon(0, Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(32, Color.white)));
            equal.SetSegmentEnabled(3, false);
            equal.SelectionChanged += (index, text) => page.Write($"Caption size: {text} ({index})");
            page.Stage.Add(equal);

            var wide = new SegmentedControl(new[] { "All", "Characters", "Objects", "Places", "Audio guides", "Trails", "Drafts" })
            {
                Sizing = SegmentSizing.Content,
            };
            wide.AddToClassList("segmentedDemo__control");
            wide.SelectionChanged += (index, text) => page.Write($"Category: {text} ({index})");
            page.Stage.Add(wide);

            var lightCard = UIToolkitExtensions.CreateVisualElement(page.Stage, "segmentedDemo__lightCard");
            lightCard.SetSurface(ThemeSurface.Light);
            var onLight = new SegmentedControl(new[] { "Map", "List" });
            onLight.AddToClassList("segmentedDemo__control");
            onLight.SelectionChanged += (index, text) => page.Write($"View: {text} ({index})");
            lightCard.Add(onLight);
        }
    }
}
