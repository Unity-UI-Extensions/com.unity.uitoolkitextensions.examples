// Examples~/ScrollSnapSplit/ScrollSnapSplitLayout.cs
// Small shared helper used by both halves of the ScrollSnapSplitDemo scene.

using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Helper that pins a UIDocument root to one half of a shared panel.
    ///
    /// Both demo panels share a single PanelSettings, so without this each document's
    /// root would stretch across the whole screen and overlap the other. Sizing each
    /// root to an exact 50% slot keeps them side-by-side with no overlap (and therefore
    /// no pointer-event contention between the two panels).
    /// </summary>
    internal static class ScrollSnapSplitLayout
    {
        public static void OccupyHalf(VisualElement root, bool leftHalf)
        {
            root.style.position = Position.Absolute;
            root.style.top = 0;
            root.style.bottom = 0;
            root.style.width = Length.Percent(50);

            if (leftHalf)
            {
                root.style.left = 0;
                root.style.right = StyleKeyword.Auto;
            }
            else
            {
                root.style.left = Length.Percent(50);
                root.style.right = StyleKeyword.Auto;
            }
        }
    }
}
