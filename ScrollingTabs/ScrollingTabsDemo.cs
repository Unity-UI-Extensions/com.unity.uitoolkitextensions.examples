// Examples~/ScrollingTabs/ScrollingTabsDemo.cs
// Demonstrates: ScrollingTabs
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The top row is the tabs variant with a search tab; the bottom row is chips.
//      - "Reload categories" replaces the items, as a content browser does when the
//        catalogue changes; the selection is kept when its id survives.
//      - Drag or scroll a row; the edge fades show where more tabs are hidden.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ScrollingTabs demo: tabs and chips variants, counts, search tab, dynamic categories.
    /// </summary>
    public class ScrollingTabsDemo : MonoBehaviour
    {
        private static readonly string[] Names = { "All", "Characters", "Objects", "Places", "Audio guides", "Trails", "Drafts", "Favourites", "Shared with me" };

        private ComponentSamplePage page;
        private ScrollingTabs tabs;
        private ScrollingTabs chips;
        private int reloads;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ScrollingTabsDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Scrolling Tabs",
                "Dynamic categories as tabs or chips, with an animated indicator and edge fades.");
            var row = page.AddRow();
            page.AddButton(row, "Reload categories", Reload);
            page.Finish();

            tabs = new ScrollingTabs { ShowSearch = true };
            tabs.SelectionChanged += (id, index) => page.Write($"Tabs: {id} ({index})");
            tabs.SearchRequested += () => page.Write("Search requested");
            page.Stage.Add(tabs);

            chips = new ScrollingTabs { Variant = ScrollingTabsVariant.Chips };
            chips.AddToClassList("scrollingTabsDemo__chips");
            chips.SelectionChanged += (id, index) => page.Write($"Chips: {id} ({index})");
            page.Stage.Add(chips);

            Reload();
        }

        private void Reload()
        {
            var items = new List<TabItem>();
            var offset = reloads++ % 3;
            for (var i = 0; i < Names.Length - offset; i++)
            {
                var name = Names[(i + offset) % Names.Length];
                items.Add(new TabItem(name.ToLowerInvariant(), name, (i * 7 + reloads) % 23));
            }

            tabs.SetItems(items);
            chips.SetItems(items);
        }
    }
}
