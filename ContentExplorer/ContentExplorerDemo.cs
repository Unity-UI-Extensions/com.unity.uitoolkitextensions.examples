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
        // ── Section data ──────────────────────────────────────────────────────────
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

        // ── Runtime references ────────────────────────────────────────────────────
        private UIDocument uiDocument;
        private LoadingIcon loadingIcon;
        private VisualElement contentScroll;
        private Label statusLabel;
        private Texture2D spinnerTexture;

        // ──────────────────────────────────────────────────────────────────────────
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

        // ──────────────────────────────────────────────────────────────────────────
        private void BuildUI(VisualElement root)
        {
            // ── Screen wrapper ────────────────────────────────────────────────────
            // position: relative is needed for the absolute loading overlay
            var screen = UIToolkitExtensions.CreateVisualElement(root, "contentExplorer__screen");

            // ── Header ────────────────────────────────────────────────────────────
            var header = UIToolkitExtensions.CreateVisualElement(screen, "contentExplorer__header");

            var titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "contentExplorer__title");
            titleLabel.text = "Content Explorer";

            var subtitleLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "contentExplorer__subtitle");
            subtitleLabel.text = "Tap a section to expand it, then tap an item to select it.";

            // ── Scrollable content ────────────────────────────────────────────────
            contentScroll = UIToolkitExtensions.CreateVisualElement(screen, "contentExplorer__contentScroll");
            // Start invisible — revealed after load finishes (runtime state)
            contentScroll.style.opacity = 0f;

            // Build collapsible sections
            foreach (var (sectionTitle, items) in SectionData)
            {
                BuildSection(contentScroll, sectionTitle, items);
            }

            // ── Status bar ────────────────────────────────────────────────────────
            var statusBar = UIToolkitExtensions.CreateVisualElement(screen, "contentExplorer__statusBar");

            statusLabel = UIToolkitExtensions.CreateVisualElement<Label>(statusBar, "contentExplorer__statusLabel");
            statusLabel.text = "Loading content…";

            // ── Loading overlay (absolute, full-screen, on top) ───────────────────
            BuildLoadingOverlay(screen);

            // ── Kick off async load simulation ────────────────────────────────────
            StartCoroutine(SimulateAsyncLoad());
        }

        // ── Section builder ───────────────────────────────────────────────────────

        private void BuildSection(VisualElement parent, string sectionTitle, string[] items)
        {
            var section = new CollapsibleSection();
            section.TitleText = sectionTitle;
            section.AddToClassList("contentExplorer__section");

            foreach (var itemText in items)
            {
                // Capture loop variable for the lambda
                var capturedText = itemText;

                var btn = new IconLabelButton();
                btn.Text = capturedText;
                btn.AddToClassList("contentExplorer__itemButton");
                btn.Clicked += () => OnItemClicked(capturedText);

                section.AddBodyContent(btn);
            }

            parent.Add(section);
        }

        // ── Loading overlay ───────────────────────────────────────────────────────

        private void BuildLoadingOverlay(VisualElement screen)
        {
            // Semi-transparent backdrop positioned absolutely over the full screen
            var overlay = UIToolkitExtensions.CreateVisualElement(screen, "contentExplorer__loadingOverlay");

            // LoadingIcon — centered inside the overlay.
            // Uses ProceduralTextureUtility to generate a runtime spinner arc texture,
            // demonstrating the utility. The package USS default (LoadingSpinner.svg) would
            // also work if SetIcon is not called.
            loadingIcon = new LoadingIcon();
            loadingIcon.AddToClassList("contentExplorer__loadingIcon");
            spinnerTexture = ProceduralTextureUtility.CreateSpinnerArc(64, new Color(0.27f, 0.55f, 0.87f));
            loadingIcon.SetIcon(spinnerTexture);
            overlay.Add(loadingIcon);

            var loadingLabel = UIToolkitExtensions.CreateVisualElement<Label>(overlay, "contentExplorer__loadingLabel");
            loadingLabel.text = "Loading content…";

            // Start the spinner — block pointer events while loading
            loadingIcon.PlayLoading(customSpeed: 0.9f, blockInteraction: true);
        }

        // ── Async load simulation ─────────────────────────────────────────────────

        private IEnumerator SimulateAsyncLoad()
        {
            yield return new WaitForSeconds(1.5f);

            // Stop and hide the loading icon
            loadingIcon.StopLoading();

            // Fade the loading overlay's parent — hide the overlay's parent visually
            // by setting its display to none after the spinner fades
            var overlay = loadingIcon.parent;
            if (overlay != null)
            {
                overlay.style.display = DisplayStyle.None;
            }

            // Fade in the content scroll area (runtime-driven transition)
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

            // Update status label
            statusLabel.text = "Tap any item to select it.";
        }

        // ── Event handlers ────────────────────────────────────────────────────────

        private void OnItemClicked(string itemText)
        {
            statusLabel.text = $"Selected: {itemText}";
            Debug.Log($"[ContentExplorerDemo] Item selected: {itemText}");
        }

    }
}
