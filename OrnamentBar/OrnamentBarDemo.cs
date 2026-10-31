// Examples~/OrnamentBar/OrnamentBarDemo.cs
// Demonstrates: OrnamentBar
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The bar floats at the bottom centre in portrait and on the right edge in
//        landscape (resize the Game view to see the Auto anchor switch).
//      - "Place" is the primary item; the buttons move the primary or disable an item.
//      - "Labels" hides or shows the item labels.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// OrnamentBar demo: auto anchoring, primary item, disabled item, labels on and off.
    /// </summary>
    public class OrnamentBarDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private OrnamentBar ornament;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("OrnamentBarDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Ornament Bar",
                "A floating action bar that never stretches. Bottom centre in portrait, right edge in landscape.");

            var row = page.AddRow();
            page.AddButton(row, "Primary: Place", () => ornament.PrimaryIndex = 1);
            page.AddButton(row, "Primary: Capture", () => ornament.PrimaryIndex = 2);
            page.AddButton(row, "Toggle Library", () => ornament.SetItemEnabled(0, !ornament.Items[0].Enabled), secondary: true);
            var options = page.AddRow();
            page.AddToggle(options, "Labels", true, value => ornament.ShowLabels = value);
            page.Finish();

            var dot = ProceduralTextureUtility.CreateSolidCircle(48, Color.white);
            ornament = new OrnamentBar { EdgeInset = 8f };
            ornament.SetItems(new List<OrnamentItem>
            {
                new("library", "Library", Background.FromTexture2D(dot)),
                new("place", "Place", Background.FromTexture2D(dot)),
                new("capture", "Capture", Background.FromTexture2D(dot)),
                new("map", "Map", Background.FromTexture2D(dot)),
            }, primary: 1);
            ornament.ItemSelected += (index, id) => page.Write($"ItemSelected: {id} ({index}), anchor {ornament.ResolvedAnchor}");
            root.Add(ornament);
        }
    }
}
