// Examples~/EmptyState/EmptyStateDemo.cs
// Demonstrates: EmptyState
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Three empty states: with an icon and an action, text only, and on a light card.
//      - Tapping an action writes the event below.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// EmptyState demo: icon and action, text only, and the light surface.
    /// </summary>
    public class EmptyStateDemo : MonoBehaviour
    {
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("EmptyStateDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Empty State",
                "What a list shows when it has nothing to show: an icon, a title, a short body and one action.");
            page.Finish();

            var downloads = new EmptyState("No downloads", "Experiences you save for offline use appear here.", "Browse experiences");
            downloads.SetIcon(Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(64, Color.white)));
            downloads.ActionClicked += () => page.Write("ActionClicked: Browse experiences");
            page.Stage.Add(downloads);

            page.Stage.Add(new EmptyState("No notifications", "You are all caught up."));

            var card = UIToolkitExtensions.CreateVisualElement(page.Stage, "emptyStateDemo__card");
            card.SetSurface(ThemeSurface.Light);
            var spaces = new EmptyState("No saved spaces", "Scan a room to save it as a space.", "Start scanning");
            spaces.ActionClicked += () => page.Write("ActionClicked: Start scanning");
            card.Add(spaces);
        }
    }
}
