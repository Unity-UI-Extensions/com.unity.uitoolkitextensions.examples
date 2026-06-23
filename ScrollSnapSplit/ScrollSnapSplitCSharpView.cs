// Examples~/ScrollSnapSplit/ScrollSnapSplitCSharpView.cs
// Demonstrates: ScrollSnap — built entirely in C# (the "classic" instantiation path).
//
// This is the LEFT half of the ScrollSnapSplitDemo scene. The RIGHT half
// (ScrollSnapSplitUxmlView) builds the *identical* control from a UXML asset, so the
// two panels prove that the same ScrollSnap control is usable from both C# and UXML.
//
// Scene setup:
//   1. Add this component to a GameObject alongside a UIDocument.
//   2. Assign that UIDocument to the 'uiDocument' field.
//   3. The UIDocument shares the example PanelSettings with the UXML panel; this script
//      sizes its own root to the LEFT 50% of the screen.

using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Builds a five-page <see cref="ScrollSnap"/> in pure C#. Each page is a solid colour
    /// with a centred "Page N" label. A header label reports the current page live via the
    /// <see cref="ScrollSnap.PageChanged"/> event.
    /// </summary>
    public class ScrollSnapSplitCSharpView : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private static readonly string[] PageColors =
        {
            "#E74C3C",
            "#3498DB",
            "#2ECC71",
            "#9B59B6",
            "#F39C12",
        };

        private ScrollSnap scrollSnap;
        private Label readoutLabel;

        private void Start()
        {
            var root = uiDocument.rootVisualElement;
            root.Clear();

            ScrollSnapSplitLayout.OccupyHalf(root, leftHalf: true);

            BuildUI(root);
        }

        private void BuildUI(VisualElement root)
        {
            var screen = UIToolkitExtensions.CreateVisualElement(root, "screen");
            screen.style.flexGrow = 1;
            screen.style.flexDirection = FlexDirection.Column;
            screen.style.backgroundColor = new Color(0.13f, 0.13f, 0.16f, 1f);

            var header = UIToolkitExtensions.CreateVisualElement(screen, "header");
            header.style.paddingTop = 28;
            header.style.paddingBottom = 12;
            header.style.paddingLeft = 20;
            header.style.paddingRight = 20;
            header.style.alignItems = Align.Center;

            var titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "header__title");
            titleLabel.text = "Built in C#";
            titleLabel.style.fontSize = 22;
            titleLabel.style.color = Color.white;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

            readoutLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "header__readout");
            readoutLabel.style.fontSize = 13;
            readoutLabel.style.color = new Color(0.7f, 0.7f, 0.78f, 1f);
            readoutLabel.style.marginTop = 4;

            scrollSnap = new ScrollSnap();
            scrollSnap.style.flexGrow = 1;
            scrollSnap.ManualMovementEnabled = true;
            screen.Add(scrollSnap);

            for (var i = 0; i < PageColors.Length; i++)
            {
                scrollSnap.Add(BuildPage(i, PageColors.Length));
            }

            var hint = UIToolkitExtensions.CreateVisualElement<Label>(screen, "footer__hint");
            hint.text = "Swipe ← / →";
            hint.style.unityTextAlign = TextAnchor.MiddleCenter;
            hint.style.fontSize = 12;
            hint.style.color = new Color(0.5f, 0.5f, 0.56f, 1f);
            hint.style.paddingTop = 8;
            hint.style.paddingBottom = 20;

            scrollSnap.PageChanged += UpdateReadout;
            UpdateReadout(scrollSnap.CurrentPageIndex);
        }

        private static VisualElement BuildPage(int index, int total)
        {
            var page = new VisualElement();
            page.style.justifyContent = Justify.Center;
            page.style.alignItems = Align.Center;

            ColorUtility.TryParseHtmlString(PageColors[index], out var bg);
            page.style.backgroundColor = bg;

            var label = UIToolkitExtensions.CreateVisualElement<Label>(page, "page__label");
            label.text = $"Page {index + 1}";
            label.style.fontSize = 40;
            label.style.color = Color.white;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;

            var subLabel = UIToolkitExtensions.CreateVisualElement<Label>(page, "page__sub");
            subLabel.text = $"{index + 1} of {total}";
            subLabel.style.fontSize = 14;
            subLabel.style.color = new Color(1f, 1f, 1f, 0.8f);
            subLabel.style.marginTop = 6;

            return page;
        }

        private void UpdateReadout(int pageIndex)
        {
            readoutLabel.text = $"Showing page {pageIndex + 1} of {scrollSnap.PageCount}";
        }
    }
}
