// Examples~/WayfinderCard/WayfinderCardDemo.cs
// Demonstrates: WayfinderCard
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The arrow turns as the bearing sweeps, as it would while walking.
//      - "Arrive" swaps the arrow for a check and changes the text.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// WayfinderCard demo: a rotating arrow, distance text and the arrived state.
    /// </summary>
    public class WayfinderCardDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private WayfinderCard card;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("WayfinderCardDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Wayfinder Card",
                "Directions to the next stop: an arrow, the distance and a line of detail.");
            var row = page.AddRow();
            page.AddToggle(row, "Arrive", false, value =>
            {
                card.Arrived = value;
                card.Title = value ? "Stop 3 \u00B7 you are here" : "Stop 3 \u00B7 18 m";
                card.Detail = value ? "Look inside the window of the test cell." : "Ahead on the left, past the test cell. Appears within 10 m.";
            });
            page.Finish();

            card = new WayfinderCard("Stop 3 \u00B7 18 m", "Ahead on the left, past the test cell. Appears within 10 m.");
            page.Stage.Add(card);
        }

        private void Update()
        {
            if (card != null && !card.Arrived)
            {
                card.SetBearing(Mathf.Sin(Time.time * 0.7f) * 120f);
            }
        }
    }
}
