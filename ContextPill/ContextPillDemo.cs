// Examples~/ContextPill/ContextPillDemo.cs
// Demonstrates: ContextPill
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Pills for a tour, a scanner and a space being edited, with and without a logo.
//      - "Dim" fades them as if a dialog were open; a dimmed pill ignores taps.
//      - Tapping a pill writes to the log.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ContextPill demo: a logo, a title and an eyebrow in a dark pill, and the dimmed state.
    /// </summary>
    public class ContextPillDemo : MonoBehaviour
    {
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ContextPillDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Context Pill",
                "Where you are: a logo, a title and an eyebrow in a dark pill at the top left.");
            var row = page.AddRow();
            var pills = new[]
            {
                new ContextPill("Sample Tour", "Stop 4 of 12"),
                new ContextPill("Scan a code", "Posters, signs, markers"),
                new ContextPill("Sample Location \u00B7 Stage two", "Editing \u00B7 6 items"),
            };
            page.AddToggle(row, "Dim", false, value =>
            {
                foreach (var pill in pills)
                {
                    pill.Dimmed = value;
                }
            });
            page.Finish();

            // A stand-in logo: a white disc. The app passes its white logo.
            var logo = Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(44, Color.white));
            for (var i = 0; i < pills.Length; i++)
            {
                var pill = pills[i];
                if (i < 2)
                {
                    pill.SetLogo(logo);
                }

                pill.AddToClassList("contextPillDemo__pill");
                pill.Tapped += () => page.Write($"Tapped: {pill.Title}");
                page.Stage.Add(pill);
            }

            var noSubtitle = new ContextPill("Sample Tour");
            noSubtitle.SetLogo(logo);
            noSubtitle.AddToClassList("contextPillDemo__pill");
            page.Stage.Add(noSubtitle);
        }
    }
}
