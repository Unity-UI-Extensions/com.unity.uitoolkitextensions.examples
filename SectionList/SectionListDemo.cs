// Examples~/SectionList/SectionListDemo.cs
// Demonstrates: SectionList
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A list-first browser: sections with counts, rows with a size or distance.
//      - "Single open" keeps one section open at a time.
//      - Tap a row to raise RowAction (written below).

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// SectionList demo: data-bound sections, counts, rows and single-open mode.
    /// </summary>
    public class SectionListDemo : MonoBehaviour
    {
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SectionListDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Section List",
                "Expandable sections with counts and rows, built on CollapsibleSection and SettingsRow.");
            var list = new SectionList();
            var row = page.AddRow();
            page.AddToggle(row, "Single open", false, value => list.SingleOpen = value);
            page.Finish();

            list.SetSections(new List<SectionListSection>
            {
                new("characters", "Characters", new[]
                {
                    new SectionListRow("greeter", "Lobby greeter", "Talks about the building", "12 MB"),
                    new SectionListRow("guide", "Harbour guide", "Audio tour, 20 minutes", "48 MB"),
                }),
                new("objects", "Objects", new[]
                {
                    new SectionListRow("planet", "Planet", meta: "1 MB"),
                    new SectionListRow("lantern", "Paper lantern", meta: "3 MB"),
                    new SectionListRow("statue", "Founders' statue", meta: "22 MB"),
                }),
                new("places", "Places nearby", new[]
                {
                    new SectionListRow("market", "Night market", meta: "120 m"),
                    new SectionListRow("garden", "Sculpture garden", meta: "450 m"),
                }),
            });
            list.SectionToggled += (id, open) => page.Write($"{id}: {(open ? "open" : "closed")}");
            list.RowAction += (section, item) => page.Write($"RowAction: {section}/{item.Title}");

            var scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.style.flexGrow = 1;
            scroll.Add(list);
            page.Stage.Add(scroll);
        }
    }
}
