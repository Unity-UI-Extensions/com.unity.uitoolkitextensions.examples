// Examples~/ContentGrid/ContentGridDemo.cs
// Demonstrates: ContentGrid
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - 200 generated items in a virtualised grid; the column count follows the width.
//      - Paging: the grid shows 24 items and asks for the next 24 (LoadMoreRequested) as the end
//        comes into view, or at once when they all fit. A short delay stands in for a server fetch.
//      - The segmented control switches between no selection, single and multiple.
//      - Hold a tile, then move, to start a drag-out (written below).
//      - "Loading" and "Empty" show the built-in states.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ContentGrid demo: virtualised tiles, paging, selection modes, drag-out, loading and empty states.
    /// </summary>
    public class ContentGridDemo : MonoBehaviour
    {
        private const int PageSize = 24;
        private const long SimulatedFetchMs = 600;
        private static readonly string[] Types = { "3D", "Audio", "Video", null };

        private ComponentSamplePage page;
        private ContentGrid grid;
        private readonly List<ContentGridItem> items = new();
        private int shown;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ContentGridDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Content Grid",
                "A virtualised tile grid with selection, long-press drag-out, and loading and empty states.");
            var row = page.AddRow();
            page.AddButton(row, "Loading", () => grid.IsLoading = !grid.IsLoading, secondary: true);
            page.AddButton(row, "Empty", ShowEmpty, secondary: true);
            page.AddButton(row, "Fill", ShowFirstPage, secondary: true);
            page.Finish();

            var modes = new SegmentedControl(new[] { "Tap", "Single", "Multiple" });
            modes.SelectionChanged += (index, _) => grid.SelectionMode = (GridSelectionMode)index;
            page.Stage.Add(modes);

            var swatches = new[]
            {
                ProceduralTextureUtility.CreateHorizontalGradient(new Color(0.24f, 0.51f, 0.6f), new Color(0.08f, 0.23f, 0.29f), 64),
                ProceduralTextureUtility.CreateHorizontalGradient(new Color(0.5f, 0.78f, 0.85f), new Color(0.24f, 0.51f, 0.6f), 64),
            };

            for (var i = 0; i < 200; i++)
            {
                items.Add(new ContentGridItem($"item{i}", $"Experience {i + 1}", $"{(i * 7) % 90 + 4} MB", Types[i % Types.Length])
                {
                    Thumbnail = Background.FromTexture2D(swatches[i % swatches.Length]),
                });
            }

            grid = new ContentGrid { EmptyTitle = "No experiences", EmptyBody = "Nothing is published nearby yet.", EmptyActionText = "Fill" };
            grid.AddToClassList("contentGridDemo__grid");
            grid.ItemTapped += item => page.Write($"ItemTapped: {item.Name}");
            grid.SelectionChanged += ids => page.Write($"Selection: {ids.Count} item(s)");
            grid.DragStarted += (item, position) => page.Write($"DragStarted: {item.Name} at {position}");
            grid.EmptyActionClicked += ShowFirstPage;
            grid.LoadMoreRequested += OnLoadMoreRequested;
            page.Stage.Add(grid);
            ShowFirstPage();
        }

        private void ShowFirstPage()
        {
            shown = Mathf.Min(PageSize, items.Count);
            grid.SetItems(items.GetRange(0, shown));
            // Starts paging over, as a host app does for a new search.
            grid.EnableLoadMore();
        }

        private void ShowEmpty()
        {
            grid.DisableLoadMore();
            grid.SetItems(new List<ContentGridItem>());
        }

        private void OnLoadMoreRequested()
        {
            page.Write($"LoadMoreRequested at {shown} items");
            grid.BeginLoadMore();
            // A host app fetches the next page from its server here.
            grid.schedule.Execute(() =>
            {
                shown = Mathf.Min(shown + PageSize, items.Count);
                // All the items so far: the grid keeps the scroll offset and the rows on screen.
                grid.SetItems(items.GetRange(0, shown));
                grid.EndLoadMore();
                if (shown >= items.Count)
                {
                    grid.DisableLoadMore();
                    page.Write("All items loaded");
                }
            }).StartingIn(SimulatedFetchMs);
        }
    }
}
