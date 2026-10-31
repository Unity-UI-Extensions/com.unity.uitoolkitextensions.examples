// Examples~/SpeechInputPanel/SpeechInputPanelDemo.cs
// Demonstrates: SpeechInputPanel
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - "Simulate" walks through listening (a partial transcript), a final transcript
//        and a match; "No match" and "Unavailable" show the other states.
//      - The meter animates by itself while listening and holds still with reduce motion.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// SpeechInputPanel demo: listening, partial and final transcripts, match, no match and unavailable.
    /// </summary>
    public class SpeechInputPanelDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private SpeechInputPanel panel;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SpeechInputPanelDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Speech Input Panel",
                "What the phone heard, what it matched, and what to do next.");
            var row = page.AddRow();
            page.AddButton(row, "Simulate", Simulate);
            page.AddButton(row, "No match", () =>
            {
                panel.SetTranscript("Where is the cafe?", true);
                panel.SetMatch("Try one of the questions below");
                panel.SetState(SpeechState.NoMatch);
            }, secondary: true);
            page.AddButton(row, "Unavailable", () => panel.SetState(SpeechState.Unavailable), secondary: true);
            var options = page.AddRow();
            page.AddToggle(options, "Reduce motion", false, value => root.SetReduceMotion(value));
            page.Finish();

            panel = new SpeechInputPanel
            {
                PrimaryText = "Ask Mara",
                SecondaryText = "Cancel",
                Footnote = "Speech is recognised on the phone. We show what we heard before anything is sent.",
            };
            panel.PrimaryClicked += () => page.Write("PrimaryClicked");
            panel.SecondaryClicked += () => page.Write("SecondaryClicked");
            page.Stage.Add(panel);
            Simulate();
        }

        private void Simulate()
        {
            panel.SetState(SpeechState.Listening);
            panel.SetMatch(null);
            panel.SetTranscript("What's going on", false);
            panel.schedule.Execute(() => panel.SetTranscript("What's going on over here?", false)).StartingIn(900);
            panel.schedule.Execute(() =>
            {
                panel.SetTranscript("What's going on over here?", true);
                panel.SetMatch("2 \u00B7 What is going on here?");
                panel.SetState(SpeechState.Matched);
            }).StartingIn(1800);
        }
    }
}
