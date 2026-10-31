// Examples~/BottomSheet/BottomSheetDemo.cs
// Demonstrates: BottomSheet
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The buttons snap the sheet to peek, half or full, or close it.
//      - Drag the grabber or the header; a fast flick moves one detent in its direction.
//      - "Modal" adds a scrim at half and full height; tap it to close the sheet.
//      - "Light surface" switches the sheet to the page palette; "Docked" shows the
//        tablet layout, a right-hand column.
//      - Escape (or Android back) follows the navigation rule: full to half, then close.
//      - "Footer" pins a footer to the visible bottom edge at half and full height.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// BottomSheet demo: detents, drag and fling, modal scrim, light surface, docked layout and back handling.
    /// </summary>
    public class BottomSheetDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private BottomSheet sheet;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("BottomSheetDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Bottom Sheet",
                "Peek, half and full detents with drag, fling, an optional scrim and a docked tablet layout.");

            var detents = page.AddRow();
            page.AddButton(detents, "Peek", () => sheet.SnapTo(SheetDetent.Peek));
            page.AddButton(detents, "Half", () => sheet.SnapTo(SheetDetent.Half));
            page.AddButton(detents, "Full", () => sheet.SnapTo(SheetDetent.Full));
            page.AddButton(detents, "Close", () => sheet.Close(), secondary: true);

            var options = page.AddRow();
            page.AddToggle(options, "Modal", false, value => sheet.Modal = value);
            page.AddToggle(options, "Light surface", false, value => sheet.SetSurface(value ? ThemeSurface.Light : ThemeSurface.Inherit));
            page.AddToggle(options, "Docked", false, value => sheet.Docked = value);
            page.AddToggle(options, "Footer", false, value =>
            {
                var footer = new Label("Showing 12 experiences nearby");
                footer.AddToClassList("bottomSheetDemo__footer");
                sheet.SetFooter(value ? footer : null);
            });
            page.Finish();

            sheet = new BottomSheet { PeekHeight = 96f, FullTopOffset = 56f };
            sheet.DetentChanged += detent => page.Write($"DetentChanged: {detent}");
            sheet.ScrimClicked += () => page.Write("ScrimClicked");

            var title = UIToolkitExtensions.CreateVisualElement<Label>(sheet.Header, "bottomSheetDemo__title");
            title.text = "Nearby content";

            for (var i = 1; i <= 12; i++)
            {
                var row = UIToolkitExtensions.CreateVisualElement<Label>(sheet, "bottomSheetDemo__row");
                row.text = $"Experience {i}";
            }

            // The sheet is an overlay layer over the whole screen.
            root.Add(sheet);
            sheet.Open(SheetDetent.Peek);

            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape && sheet.HandleBack())
                {
                    evt.StopPropagation();
                }
            });
        }
    }
}
