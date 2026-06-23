// Examples~/ScrollSnapSplit/ScrollSnapSplitUxmlView.cs
// Demonstrates: ScrollSnap — authored in UXML, driven from C# (the UI Document path).
//
// This is the RIGHT half of the ScrollSnapSplitDemo scene. The layout (the ScrollSnap and
// its five colour pages) lives entirely in ScrollSnapSplitView.uxml. This script does only
// what UXML cannot: it positions the document on the right 50% of the screen, then queries
// the cloned tree to wire the live "current page" readout — the classic
// "layout in UXML, behaviour in C#" division of labour.
//
// Scene setup:
//   1. Add this component to a GameObject alongside a UIDocument.
//   2. Set the UIDocument's Source Asset to ScrollSnapSplitView.uxml.
//   3. Assign that UIDocument to the 'uiDocument' field.

using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Binds behaviour onto a <see cref="ScrollSnap"/> that was instantiated from UXML,
    /// proving the same control is fully usable through a UI Document.
    /// </summary>
    public class ScrollSnapSplitUxmlView : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private ScrollSnap scrollSnap;
        private Label readoutLabel;

        private void Start()
        {
            var root = uiDocument.rootVisualElement;

            ScrollSnapSplitLayout.OccupyHalf(root, leftHalf: false);

            scrollSnap = root.Q<ScrollSnap>();
            readoutLabel = root.Q<Label>("page-readout");

            if (scrollSnap == null)
            {
                Debug.LogWarning($"{nameof(ScrollSnapSplitUxmlView)}: no ScrollSnap found in the UXML source asset.");
                return;
            }

            scrollSnap.PageChanged += UpdateReadout;
            UpdateReadout(scrollSnap.CurrentPageIndex);
        }

        private void OnDisable()
        {
            if (scrollSnap != null)
            {
                scrollSnap.PageChanged -= UpdateReadout;
            }
        }

        private void UpdateReadout(int pageIndex)
        {
            if (readoutLabel != null)
            {
                readoutLabel.text = $"Showing page {pageIndex + 1} of {scrollSnap.PageCount}";
            }
        }
    }
}
