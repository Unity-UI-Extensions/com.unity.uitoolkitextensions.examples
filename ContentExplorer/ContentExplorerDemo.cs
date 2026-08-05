// Examples~/ContentExplorer/ContentExplorerDemo.cs
// Demonstrates: CollapsibleSection, IconLabelButton, LoadingIcon
//
// Scene setup:
//   1. Open the provided ContentExplorer scene (the UIDocument and this
//      ContentExplorerDemo component live on the same GameObject; the demo
//      resolves the UIDocument via GetComponent<UIDocument>() at runtime).
//   2. Press Play — a loading spinner blocks interaction for 1.5 s, then three
//      collapsible accordion sections appear. Tap any row to see which item was selected.

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Content explorer demo showcasing <see cref="CollapsibleSection"/>,
    /// <see cref="IconLabelButton"/>, and <see cref="LoadingIcon"/>.
    ///
    /// On Start a full-screen loading overlay is displayed using <see cref="LoadingIcon"/>
    /// which blocks pointer events (<c>blockInteraction: true</c>).
    /// After 1.5 seconds the spinner is stopped and the three collapsible sections
    /// are revealed with a simple fade-in.
    ///
    /// Each section contains several <see cref="IconLabelButton"/> rows.
    /// Clicking a row writes the selection into the status label at the bottom.
    /// </summary>
    public class ContentExplorerDemo : MonoBehaviour
    {
        private static readonly (string section, string[] items)[] SectionData =
        {
            (
                "Getting Started",
                new[]
                {
                    "Introduction to UI Toolkit",
                    "Your first VisualElement",
                    "Styling with USS classes",
                    "Working with events",
                }
            ),
            (
                "Controls",
                new[]
                {
                    "PillButton — gradient actions",
                    "PillInputField — mobile-ready input",
                    "ScrollSnap — paging container",
                    "QuadrantStepper — segmented navigation",
                }
            ),
            (
                "Utilities",
                new[]
                {
                    "VisualElementShakeUtility",
                    "UIToolkitExtensions helpers",
                    "ToastSwipeDismissManipulator",
                }
            ),
        };

        private UIDocument uiDocument;
        private LoadingIcon loadingIcon;
        private VisualElement contentScroll;
        private Label statusLabel;
        private Texture2D spinnerTexture;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ContentExplorerDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void OnDestroy()
        {
            if (spinnerTexture != null)
                Destroy(spinnerTexture);
        }

        private void BuildUI(VisualElement root)
        {
            var screen = UIToolkitExtensions.CreateVisualElement(root, "contentExplorer__screen");

            var card = UIToolkitExtensions.CreateVisualElement(screen, "contentExplorer__card");

            var eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "contentExplorer__eyebrow");
            eyebrow.text = "Toolkit Sample";

            var titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "contentExplorer__title");
            titleLabel.text = "Content Explorer";

            var subtitleLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "contentExplorer__subtitle");
            subtitleLabel.text = "Browse the demo content below, expand any section, and tap a row to inspect the current selection.";

            contentScroll = UIToolkitExtensions.CreateVisualElement(card, "contentExplorer__contentScroll");
            contentScroll.style.opacity = 0f;

            foreach (var (sectionTitle, items) in SectionData)
            {
                BuildSection(contentScroll, sectionTitle, items);
            }

            statusLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "contentExplorer__statusLabel");
            statusLabel.text = "Loading content…";

            BuildLoadingOverlay(screen);

            StartCoroutine(SimulateAsyncLoad());
        }

        private void BuildSection(VisualElement parent, string sectionTitle, string[] items)
        {
            var section = new CollapsibleSection();
            section.TitleText = sectionTitle;
            section.AddToClassList("contentExplorer__section");

            foreach (var itemText in items)
            {
                var capturedText = itemText;

                var btn = new IconLabelButton();
                btn.Text = capturedText;
                btn.AddToClassList("contentExplorer__itemButton");
                btn.Clicked += () => OnItemClicked(capturedText);

                section.AddBodyContent(btn);
            }

            parent.Add(section);
        }

        private void BuildLoadingOverlay(VisualElement screen)
        {
            var overlay = UIToolkitExtensions.CreateVisualElement(screen, "contentExplorer__loadingOverlay");

            loadingIcon = new LoadingIcon();
            loadingIcon.AddToClassList("contentExplorer__loadingIcon");
            spinnerTexture = ProceduralTextureUtility.CreateSpinnerArc(64, new Color(0.27f, 0.55f, 0.87f));
            loadingIcon.SetIcon(spinnerTexture);
            overlay.Add(loadingIcon);

            var loadingLabel = UIToolkitExtensions.CreateVisualElement<Label>(overlay, "contentExplorer__loadingLabel");
            loadingLabel.text = "Loading content…";

            loadingIcon.PlayLoading(customSpeed: 0.9f, blockInteraction: true);
        }

        private IEnumerator SimulateAsyncLoad()
        {
            yield return new WaitForSeconds(1.5f);

            loadingIcon.StopLoading();

            var overlay = loadingIcon.parent;
            if (overlay != null)
            {
                overlay.style.display = DisplayStyle.None;
            }

            contentScroll.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName>
                {
                    new("opacity")
                });
            contentScroll.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue>
                {
                    new(500, TimeUnit.Millisecond)
                });
            contentScroll.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new System.Collections.Generic.List<EasingFunction>
                {
                    new(EasingMode.EaseOut)
                });
            contentScroll.style.opacity = 1f;

            statusLabel.text = "Tap any item to select it.";
        }

        private void OnItemClicked(string itemText)
        {
            statusLabel.text = $"Selected: {itemText}";
            Debug.Log($"[ContentExplorerDemo] Item selected: {itemText}");
        }

    }
}
