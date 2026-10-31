// Examples~/CollapsibleSectionCard/CollapsibleSectionCardDemo.cs
// Demonstrates: CollapsibleSection card variant, slots and static mode
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A permissions-style accordion: each card has an icon tile, a title with a
//        REQUIRED or OPTIONAL subtitle, and a status badge in place of the chevron.
//      - The cards toggle on a completed tap, so scrolling past them is safe.
//      - "Static" stops taps; the buttons then open and close the cards from state.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// CollapsibleSection demo: the card variant with leading and trailing slots, subtitle and static mode.
    /// </summary>
    public class CollapsibleSectionCardDemo : MonoBehaviour
    {
        private readonly List<CollapsibleSection> sections = new();
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("CollapsibleSectionCardDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Collapsible Section Card",
                "The card variant: an icon tile, a subtitle and a trailing badge, read from ThemeTokens.");
            var row = page.AddRow();
            page.AddToggle(row, "Static", false, value => sections.ForEach(s => s.Interactive = !value));
            page.AddButton(row, "Open all", () => sections.ForEach(s => s.IsExpanded = true), secondary: true);
            page.AddButton(row, "Close all", () => sections.ForEach(s => s.IsExpanded = false), secondary: true);
            page.Finish();

            var scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.style.flexGrow = 1;
            page.Stage.Add(scroll);

            Add(scroll, "Camera", "REQUIRED", "Explorer places content in the world through your camera.", new StatusChip("Allowed", StatusState.Ok));
            Add(scroll, "Location", "REQUIRED", "Used to find experiences near you and to position content.", new StatusChip("Needed", StatusState.Warn));
            Add(scroll, "Microphone", "OPTIONAL", "Lets you talk to characters. You can type instead.", new StatusChip("Skipped", StatusState.Info));
        }

        private void Add(VisualElement parent, string title, string subtitle, string body, VisualElement badge)
        {
            var section = new CollapsibleSection
            {
                TitleText = title,
                SubtitleText = subtitle,
                BodyText = body,
                Variant = CollapsibleSectionVariant.Card,
                ToggleOnPointerDown = false,
            };

            var tile = UIToolkitExtensions.CreateVisualElement("collapsibleCardDemo__tile");
            section.SetLeading(tile);
            section.SetTrailing(badge);
            section.OnExpandedChanged += expanded => page.Write($"{title}: {(expanded ? "open" : "closed")}");
            sections.Add(section);
            parent.Add(section);
        }
    }
}
