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
        /// <summary>
        /// Per-page background modifier classes (colors defined in the USS).
        /// </summary>
        private static readonly string[] PageModifiers =
        {
            "scrollSnapDemo__page--one",
            "scrollSnapDemo__page--two",
            "scrollSnapDemo__page--three",
            "scrollSnapDemo__page--four",
            "scrollSnapDemo__page--five",
        };

        /// <summary>
        /// Labels shown on pages that are NOT the coming-soon page.
        /// </summary>
        private static readonly string[] PageLabels =
        {
            "Welcome",
            "Explore",
            "Discover",
            "Coming Soon",
            "Get Started",
        };

        /// <summary>
        /// Index of the page that should show a <see cref="ComingSoonMessage"/>.
        /// </summary>
        private const int ComingSoonPageIndex = 3;

        private UIDocument uiDocument;
        private ScrollSnap scrollSnap;
        private PageDotIndicator dotIndicator;
        private PillButton prevButton;
        private PillButton nextButton;

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

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "scrollSnapDemo__screen");

            VisualElement card = UIToolkitExtensions.CreateVisualElement(screen, "scrollSnapDemo__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "scrollSnapDemo__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "scrollSnapDemo__title");
            titleLabel.text = "Scroll Snap And Dots";

            Label subtitleLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "scrollSnapDemo__subtitle");
            subtitleLabel.text = "Swipe through the sample pages below or use the controls to move through the demo content.";

            VisualElement carouselSurface = UIToolkitExtensions.CreateVisualElement(card, "scrollSnapDemo__carouselSurface");

            scrollSnap = new ScrollSnap();
            scrollSnap.AddToClassList("scrollSnapDemo__scrollSnap");
            scrollSnap.ManualMovementEnabled = true;
            carouselSurface.Add(scrollSnap);

            for (var i = 0; i < PageModifiers.Length; i++)
            {
                var page = BuildPage(i);
                scrollSnap.Add(page);
            }

            VisualElement bottomBar = UIToolkitExtensions.CreateVisualElement(card, "scrollSnapDemo__bottomBar");

            prevButton = new PillButton();
            prevButton.AddToClassList("scrollSnapDemo__navButton");
            prevButton.Text = "←";
            prevButton.SetInnerColor("#465062");
            prevButton.SetOuterColor("#5C667A");
            prevButton.Clicked += OnPreviousClicked;
            bottomBar.Add(prevButton);

            dotIndicator = new PageDotIndicator();
            dotIndicator.AddToClassList("scrollSnapDemo__dotIndicator");
            dotIndicator.SetColors("#FFFFFF", "#44506A");
            dotIndicator.SetProgress(0, PageModifiers.Length);
            bottomBar.Add(dotIndicator);

            nextButton = new PillButton();
            nextButton.AddToClassList("scrollSnapDemo__navButton");
            nextButton.Text = "→";
            nextButton.SetInnerColor("#4A90E2");
            nextButton.SetOuterColor("#7B68EE");
            nextButton.Clicked += OnNextClicked;
            bottomBar.Add(nextButton);

            scrollSnap.PageChanged += OnPageChanged;

            UpdateNavButtons();
        }

        private VisualElement BuildPage(int index)
        {
            var page = new VisualElement();
            page.AddToClassList("scrollSnapDemo__page");
            page.AddToClassList(PageModifiers[index]);

            if (index == ComingSoonPageIndex)
            {
                var comingSoon = new ComingSoonMessage();
                comingSoon.AddToClassList("scrollSnapDemo__comingSoon");
                comingSoon.Title = "Coming Soon";
                comingSoon.SetMessage("More controls are on their way! Stay tuned for the next update.");
                page.Add(comingSoon);
            }
            else
            {
                VisualElement badge = UIToolkitExtensions.CreateVisualElement(page, "scrollSnapDemo__badge");

                Label badgeLabel = UIToolkitExtensions.CreateVisualElement<Label>(badge, "scrollSnapDemo__badgeLabel");
                badgeLabel.text = (index + 1).ToString();

                Label pageLabel = UIToolkitExtensions.CreateVisualElement<Label>(page, "scrollSnapDemo__pageLabel");
                pageLabel.text = PageLabels[index];

                Label subLabel = UIToolkitExtensions.CreateVisualElement<Label>(page, "scrollSnapDemo__pageSubLabel");
                subLabel.text = $"Page {index + 1} of {PageModifiers.Length}";
            }

            return page;
        }

        private void OnPageChanged(int newIndex)
        {
            dotIndicator.SetProgress(newIndex, PageModifiers.Length);
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

        /// <summary>
        /// Dim the prev/next arrow to hint at boundary conditions.
        /// </summary>
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
