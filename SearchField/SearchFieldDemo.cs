// Examples~/SearchField/SearchFieldDemo.cs
// Demonstrates: SearchField, EmptyState and SkeletonTile together
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Type to filter a small catalogue; results update after a short pause.
//      - While a query is "loading", skeleton rows stand in for the results.
//      - A query with no matches shows an EmptyState with one action that clears it.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// SearchField demo: debounced query, result count, submit, clear, loading skeletons and an empty state.
    /// </summary>
    public class SearchFieldDemo : MonoBehaviour
    {
        private static readonly string[] Catalogue =
        {
            "Lobby greeter", "Planet", "Founders' trail", "Night market", "Harbour audio guide",
            "Lighthouse", "Museum map", "Sculpture garden", "Station welcome", "Treasure hunt",
        };

        private ComponentSamplePage page;
        private SearchField search;
        private VisualElement results;
        private IVisualElementScheduledItem loading;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SearchFieldDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Search Field",
                "Debounced search with a result count, loading skeletons and an empty state.");
            page.Finish();

            search = new SearchField { Placeholder = "Search experiences", DebounceMs = 250 };
            search.QueryChanged += OnQuery;
            search.Submitted += query => page.Write($"Submitted: \"{query}\"");
            search.Cleared += () => page.Write("Cleared");
            page.Stage.Add(search);

            results = UIToolkitExtensions.CreateVisualElement(page.Stage, "searchDemo__results");
            ShowResults(string.Empty);
        }

        private void OnQuery(string query)
        {
            page.Write($"QueryChanged: \"{query}\"");
            loading?.Pause();
            results.Clear();
            results.Add(new SkeletonTile(SkeletonShape.Row, 3));
            loading = results.schedule.Execute(() => ShowResults(query)).StartingIn(600);
        }

        private void ShowResults(string query)
        {
            results.Clear();
            var matches = Catalogue.Where(c => c.ToLowerInvariant().Contains(query.ToLowerInvariant())).ToList();
            search.ResultCount = query.Length > 0 ? matches.Count : -1;

            if (matches.Count == 0)
            {
                var empty = new EmptyState("No matches", $"Nothing in the catalogue matches \"{query}\".", "Clear search");
                empty.ActionClicked += () => search.ClearQuery();
                results.Add(empty);
                return;
            }

            foreach (var match in matches)
            {
                var row = UIToolkitExtensions.CreateVisualElement<Label>(results, "searchDemo__row");
                row.text = match;
            }
        }
    }
}
