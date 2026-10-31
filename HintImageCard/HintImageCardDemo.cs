// Examples~/HintImageCard/HintImageCardDemo.cs
// Demonstrates: HintImageCard
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A hint card with the FAR / NEAR / FOUND strip; "Closer" moves along it.
//      - "Picture" swaps the placeholder for an image; the tag sits on a chip.
//      - The embedded variant below sits inside another card, without its own chrome.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// HintImageCard demo: the proximity strip, a picture, and the embedded variant.
    /// </summary>
    public class HintImageCardDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private HintImageCard card;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("HintImageCardDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Hint Image Card",
                "A picture of what to face, a line of guidance and how close you are.");
            var row = page.AddRow();
            page.AddButton(row, "Closer", () =>
            {
                card.SetProximity(Mathf.Min(card.Proximity + 1, card.ProximitySegments.Count - 1));
                page.Write($"Proximity: {card.Proximity}");
            });
            page.AddButton(row, "Reset", () => card.SetProximity(0), secondary: true);
            page.AddToggle(row, "Picture", false, value => card.SetImage(value
                ? Background.FromTexture2D(ProceduralTextureUtility.CreateHorizontalGradient(new Color(0.85f, 0.8f, 0.72f), new Color(0.3f, 0.36f, 0.4f), 64))
                : default));
            page.Finish();

            card = new HintImageCard("Face the big doors like the picture and hold still. Closer is better.");
            card.SetProximity(0);
            page.Stage.Add(card);

            var host = UIToolkitExtensions.CreateVisualElement(page.Stage, "hintDemo__host");
            var heading = UIToolkitExtensions.CreateVisualElement<Label>(host, "hintDemo__heading");
            heading.text = "WHILE IT LOADS";
            host.Add(new HintImageCard("Go to the hall entrance and face the big doors, like the picture.") { Embedded = true });
        }
    }
}
