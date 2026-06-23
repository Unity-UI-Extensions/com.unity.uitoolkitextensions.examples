// Examples~/ScrollSnapAndDots/ScrollSnapAndDotsDemo.cs
// Demonstrates: ScrollSnap, PageDotIndicator, ComingSoonMessage
//
// Scene setup:
//   1. A scene is provided for this example.
//   2. This component lives on a GameObject alongside a UIDocument; the demo
//      retrieves it via GetComponent<UIDocument>() on the same GameObject.
//   3. The Panel Settings are already configured in the provided scene
//      (Scale With Screen Size, 1080x1920).
//   4. Press Play — five swipeable pages appear with a dot indicator and prev/next buttons.

using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Full demo of the <see cref="ScrollSnap"/> paging control paired with
    /// <see cref="PageDotIndicator"/> and <see cref="ComingSoonMessage"/>.
    ///
    /// What you will see:
    /// - A header label at the top.
    /// - Five horizontally swipeable pages (solid background colors + centered labels).
    /// - Page 4 (index 3) is replaced by a <see cref="ComingSoonMessage"/> widget.
    /// - A row at the bottom containing a "back" arrow, the dot indicator, and a "next" arrow.
    /// - The dot indicator updates in real time as pages change.
    /// </summary>
    public class ScrollSnapAndDotsDemo : MonoBehaviour
    {
        // ── Page configuration ────────────────────────────────────────────────────

        /// <summary>Background colors for each page (hex strings).</summary>
        private static readonly string[] PageColors =
        {
            "#E74C3C", // red
            "#3498DB", // blue
            "#2ECC71", // green
            "#9B59B6", // purple  → ComingSoonMessage page
            "#F39C12", // orange
        };

        /// <summary>Labels shown on pages that are NOT the coming-soon page.</summary>
        private static readonly string[] PageLabels =
        {
            "Welcome",
            "Explore",
            "Discover",
            "Coming Soon", // replaced at runtime — label is set but hidden under the widget
            "Get Started",
        };

        /// <summary>Index of the page that should show a <see cref="ComingSoonMessage"/>.</summary>
        private const int ComingSoonPageIndex = 3;

        // ── Runtime references ────────────────────────────────────────────────────
        private UIDocument uiDocument;
        private ScrollSnap scrollSnap;
        private PageDotIndicator dotIndicator;
        private PillButton prevButton;
        private PillButton nextButton;

        // ──────────────────────────────────────────────────────────────────────────
        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ScrollSnapAndDotsDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        // ──────────────────────────────────────────────────────────────────────────
        private void BuildUI(VisualElement root)
        {
            // ── Root container — full screen, dark background ─────────────────────
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "scrollSnapDemo__screen");

            // ── Header ────────────────────────────────────────────────────────────
            VisualElement header = UIToolkitExtensions.CreateVisualElement(screen, "scrollSnapDemo__header");

            Label titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "scrollSnapDemo__title");
            titleLabel.text = "ScrollSnap Demo";

            Label subtitleLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "scrollSnapDemo__subtitle");
            subtitleLabel.text = "Swipe left/right or use the arrows below";

            // ── ScrollSnap ────────────────────────────────────────────────────────
            scrollSnap = new ScrollSnap();
            scrollSnap.AddToClassList("scrollSnapDemo__scrollSnap");
            scrollSnap.ManualMovementEnabled = true;
            screen.Add(scrollSnap);

            // ── Build pages ───────────────────────────────────────────────────────
            for (var i = 0; i < PageColors.Length; i++)
            {
                var page = BuildPage(i);
                scrollSnap.Add(page);
            }

            // ── Bottom bar ─────────────────────────────────────────────────────────
            VisualElement bottomBar = UIToolkitExtensions.CreateVisualElement(screen, "scrollSnapDemo__bottomBar");

            // Previous button
            prevButton = new PillButton();
            prevButton.AddToClassList("scrollSnapDemo__navButton");
            prevButton.Text = "←";
            prevButton.SetInnerColor("#555566");
            prevButton.SetOuterColor("#777788");
            prevButton.Clicked += OnPreviousClicked;
            bottomBar.Add(prevButton);

            // Dot indicator
            dotIndicator = new PageDotIndicator();
            dotIndicator.AddToClassList("scrollSnapDemo__dotIndicator");
            dotIndicator.SetColors("#FFFFFF", "#555566");
            dotIndicator.SetProgress(0, PageColors.Length);
            bottomBar.Add(dotIndicator);

            // Next button
            nextButton = new PillButton();
            nextButton.AddToClassList("scrollSnapDemo__navButton");
            nextButton.Text = "→";
            nextButton.SetInnerColor("#4A90E2");
            nextButton.SetOuterColor("#7B68EE");
            nextButton.Clicked += OnNextClicked;
            bottomBar.Add(nextButton);

            // ── Wire up PageChanged ────────────────────────────────────────────────
            scrollSnap.PageChanged += OnPageChanged;

            // Seed the initial dot state
            UpdateNavButtons();
        }

        // ── Page factory ──────────────────────────────────────────────────────────
        private VisualElement BuildPage(int index)
        {
            // Outer container — fills the page slot assigned by ScrollSnap
            var page = new VisualElement();
            page.AddToClassList("scrollSnapDemo__page");

            // Parse the page background color (data-driven — stays in C#)
            Color bg = Color.gray;
            ColorUtility.TryParseHtmlString(PageColors[index], out bg);
            page.style.backgroundColor = bg;

            if (index == ComingSoonPageIndex)
            {
                // Replace page content with the ComingSoonMessage widget
                var comingSoon = new ComingSoonMessage();
                comingSoon.AddToClassList("scrollSnapDemo__comingSoon");
                comingSoon.Title = "Coming Soon";
                comingSoon.SetMessage("More controls are on their way! Stay tuned for the next update.");
                page.Add(comingSoon);
            }
            else
            {
                // Page number badge
                VisualElement badge = UIToolkitExtensions.CreateVisualElement(page, "scrollSnapDemo__badge");

                Label badgeLabel = UIToolkitExtensions.CreateVisualElement<Label>(badge, "scrollSnapDemo__badgeLabel");
                badgeLabel.text = (index + 1).ToString();

                // Main label
                Label pageLabel = UIToolkitExtensions.CreateVisualElement<Label>(page, "scrollSnapDemo__pageLabel");
                pageLabel.text = PageLabels[index];

                // Sub-label
                Label subLabel = UIToolkitExtensions.CreateVisualElement<Label>(page, "scrollSnapDemo__pageSubLabel");
                subLabel.text = $"Page {index + 1} of {PageColors.Length}";
            }

            return page;
        }

        // ── Event handlers ────────────────────────────────────────────────────────

        private void OnPageChanged(int newIndex)
        {
            dotIndicator.SetProgress(newIndex, PageColors.Length);
            UpdateNavButtons();
        }

        private void OnPreviousClicked()
        {
            scrollSnap.MovePrevious();
        }

        private void OnNextClicked()
        {
            scrollSnap.MoveNext();
        }

        /// <summary>Dim the prev/next arrow to hint at boundary conditions.</summary>
        private void UpdateNavButtons()
        {
            bool atStart = scrollSnap.CurrentPageIndex <= 0;
            bool atEnd = scrollSnap.CurrentPageIndex >= scrollSnap.PageCount - 1;

            prevButton.SetInnerColor(atStart ? "#333344" : "#555566");
            prevButton.SetOuterColor(atStart ? "#444455" : "#777788");

            nextButton.SetInnerColor(atEnd ? "#2A5090" : "#4A90E2");
            nextButton.SetOuterColor(atEnd ? "#4A449A" : "#7B68EE");
        }
    }
}
