// Examples~/ContentExplorer/ContentExplorerUxmlBinder.cs
// UXML counterpart of ContentExplorerDemo.cs. Layout lives in ContentExplorerUxmlView.uxml;
// this binder wires the loading sequence and item-selection behaviour.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Behaviour binder for the UXML-authored ContentExplorer demo.
    /// </summary>
    public class ContentExplorerUxmlBinder : MonoBehaviour
    {
        private UIDocument uiDocument;
        private LoadingIcon loadingIcon;
        private VisualElement loadingOverlay;
        private VisualElement contentScroll;
        private Label statusLabel;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ContentExplorerUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            loadingIcon = root.Q<LoadingIcon>("loading-icon");
            loadingOverlay = root.Q<VisualElement>("loading-overlay");
            contentScroll = root.Q<VisualElement>("content-scroll");
            statusLabel = root.Q<Label>("status-label");

            root.Query<IconLabelButton>().ForEach(button =>
            {
                button.Clicked += () => OnItemClicked(button.Text);
            });

            loadingIcon?.PlayLoading(customSpeed: 0.9f, blockInteraction: true);
            StartCoroutine(SimulateAsyncLoad());
        }

        private IEnumerator SimulateAsyncLoad()
        {
            yield return new WaitForSeconds(1.5f);

            loadingIcon?.StopLoading();

            if (loadingOverlay != null)
            {
                loadingOverlay.style.display = DisplayStyle.None;
            }

            if (contentScroll != null)
            {
                contentScroll.style.transitionProperty = new StyleList<StylePropertyName>(
                    new List<StylePropertyName> { new("opacity") });
                contentScroll.style.transitionDuration = new StyleList<TimeValue>(
                    new List<TimeValue> { new(500, TimeUnit.Millisecond) });
                contentScroll.style.transitionTimingFunction = new StyleList<EasingFunction>(
                    new List<EasingFunction> { new(EasingMode.EaseOut) });
                contentScroll.style.opacity = 1f;
            }

            if (statusLabel != null)
            {
                statusLabel.text = "Tap any item to select it.";
            }
        }

        private void OnItemClicked(string itemText)
        {
            if (statusLabel != null)
            {
                statusLabel.text = $"Selected: {itemText}";
            }

            Debug.Log($"[ContentExplorerUxmlBinder] Item selected: {itemText}");
        }
    }
}
