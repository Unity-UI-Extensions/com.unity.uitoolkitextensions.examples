// Examples~/PromptChoices/PromptChoicesDemo.cs
// Demonstrates: PromptChoices, AvatarPortrait and HoldToTalk
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The character's questions: tap a prompt, or press 1 to 4 while the list has focus.
//        A used prompt dims and cannot be picked again.
//      - Hold the microphone row to talk; a quick tap counts as "type instead".

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// PromptChoices demo: numbered prompts, used prompts, keyboard picks, and the hold-to-talk row.
    /// </summary>
    public class PromptChoicesDemo : MonoBehaviour
    {
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("PromptChoicesDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Prompt Choices",
                "What a visitor can ask a character, plus hold to ask in their own words.");
            page.Finish();

            var header = UIToolkitExtensions.CreateVisualElement(page.Stage, "promptDemo__header");
            header.Add(new AvatarPortrait("Mara Okafor", AvatarSize.Medium));
            var name = UIToolkitExtensions.CreateVisualElement<Label>(header, "promptDemo__name");
            name.text = "Mara, site engineer";

            var prompts = new PromptChoices();
            prompts.SetItems(new List<PromptItem>
            {
                new("about", "Tell me about yourself"),
                new("here", "What is going on here?"),
                new("cool", "Show me something cool"),
                new("photo", "Pose for a photo"),
            });
            prompts.ChoiceSelected += id =>
            {
                page.Write($"ChoiceSelected: {id}");
                prompts.SetItemEnabled(id, false);
            };
            page.Stage.Add(prompts);

            var hold = new HoldToTalk { TitleText = "Hold to ask in your own words", Subtitle = "or type a question, captions stay on" };
            hold.AddToClassList("promptDemo__hold");
            hold.PressStarted += () => page.Write("Listening...");
            hold.PressEnded += released => page.Write(released ? "PressEnded" : "Cancelled");
            hold.Tapped += () => page.Write("Tapped: type instead");
            page.Stage.Add(hold);
        }
    }
}
