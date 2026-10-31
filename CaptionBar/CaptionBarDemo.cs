// Examples~/CaptionBar/CaptionBarDemo.cs
// Demonstrates: CaptionBar
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The captions advance every few seconds with a short cross-fade.
//      - "Larger text" applies the 1.3 text scale; the clamp to three lines follows it.
//      - "Reduce motion" swaps captions without the fade.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// CaptionBar demo: cues with a speaker, the cross-fade, text scale and reduce motion.
    /// </summary>
    public class CaptionBarDemo : MonoBehaviour
    {
        private static readonly (string speaker, string text)[] Cues =
        {
            ("Mara", "Welcome to the gallery. I look after the stage-two line."),
            ("Mara", "Ask me anything about what you can see. The turbines here are built one blade at a time, and each blade takes three days to finish and check."),
            ("Guide", "Walk towards the big doors when you are ready for the next stop."),
            (null, "Music plays softly in the hall."),
        };

        private ComponentSamplePage page;
        private int index;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("CaptionBarDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Caption Bar",
                "Captions with a speaker column, clamped to three lines, cross-fading between cues.");
            var row = page.AddRow();
            page.AddToggle(row, "Larger text", false, value => root.SetTextScale(value ? 1.3f : 1f));
            page.AddToggle(row, "Reduce motion", false, value => root.SetReduceMotion(value));
            page.Finish();

            var bar = new CaptionBar();
            page.Stage.Add(bar);
            bar.SetCue(Cues[0].speaker, Cues[0].text);
            bar.schedule.Execute(() =>
            {
                index = (index + 1) % Cues.Length;
                bar.SetCue(Cues[index].speaker, Cues[index].text);
            }).Every(3000).StartingIn(3000);
        }
    }
}
