// Examples~/SkeletonTile/SkeletonTileDemo.cs
// Demonstrates: SkeletonTile and EmptyState
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The segmented control switches between the four skeleton shapes.
//      - "Reduce motion" stops the pulse; the placeholders hold still.
//      - "Empty" swaps the skeletons for an EmptyState with an icon and one action.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// SkeletonTile demo: tile, row, text and circle placeholders, the pulse, reduce motion, and an empty state.
    /// </summary>
    public class SkeletonTileDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private VisualElement stage;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SkeletonTileDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Skeleton Tile",
                "Placeholders in the shape of the content to come. They pulse, and hold still with reduce motion.");
            var row = page.AddRow();
            page.AddToggle(row, "Reduce motion", false, value => root.SetReduceMotion(value));
            page.AddButton(row, "Empty", ShowEmpty, secondary: true);
            page.Finish();

            var shapes = new SegmentedControl(new[] { "Tile", "Row", "Text", "Circle" });
            shapes.SelectionChanged += (index, _) => ShowShape((SkeletonShape)index);
            page.Stage.Add(shapes);

            stage = UIToolkitExtensions.CreateVisualElement(page.Stage, "skeletonDemo__stage");
            ShowShape(SkeletonShape.Tile);
        }

        private void ShowShape(SkeletonShape shape)
        {
            stage.Clear();
            var count = shape == SkeletonShape.Circle ? 8 : 4;
            stage.Add(new SkeletonTile(shape, count) { Columns = shape == SkeletonShape.Circle ? 4 : 2 });
            page.Write($"Shape: {shape}");
        }

        private void ShowEmpty()
        {
            stage.Clear();
            var empty = new EmptyState("No downloads", "Experiences you save for offline use appear here.", "Browse experiences");
            empty.SetIcon(Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(64, Color.white)));
            empty.ActionClicked += () => ShowShape(SkeletonShape.Tile);
            stage.Add(empty);
            page.Write("EmptyState");
        }
    }
}
