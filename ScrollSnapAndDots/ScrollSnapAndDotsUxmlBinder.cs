using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    public class ScrollSnapAndDotsUxmlBinder : MonoBehaviour
    {
        private ScrollSnap scrollSnap;
        private PageDotIndicator dotIndicator;
        private PillButton prevButton;
        private PillButton nextButton;

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ScrollSnapAndDotsUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            scrollSnap = root.Q<ScrollSnap>("scroll-snap");
            dotIndicator = root.Q<PageDotIndicator>("dot-indicator");
            prevButton = root.Q<PillButton>("prev-button");
            nextButton = root.Q<PillButton>("next-button");

            scrollSnap.PageChanged += OnPageChanged;
            prevButton.SetTextColor(Color.white);
            nextButton.SetTextColor(Color.white);
            prevButton.Clicked += OnPreviousClicked;
            nextButton.Clicked += OnNextClicked;

            dotIndicator.SetProgress(0, scrollSnap.PageCount);
            UpdateNavButtons();
        }

        private void OnPageChanged(int newIndex)
        {
            dotIndicator.SetProgress(newIndex, scrollSnap.PageCount);
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

        private void UpdateNavButtons()
        {
            bool atStart = scrollSnap.CurrentPageIndex <= 0;
            bool atEnd = scrollSnap.CurrentPageIndex >= scrollSnap.PageCount - 1;

            prevButton.SetInnerColor(atStart ? "#374152" : "#465062");
            prevButton.SetOuterColor(atStart ? "#465062" : "#5C667A");
            nextButton.SetInnerColor(atEnd ? "#305E9A" : "#4A90E2");
            nextButton.SetOuterColor(atEnd ? "#5B57B2" : "#7B68EE");
        }
    }
}
