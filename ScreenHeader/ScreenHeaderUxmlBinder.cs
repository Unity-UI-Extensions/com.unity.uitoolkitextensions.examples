using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    public class ScreenHeaderUxmlBinder : MonoBehaviour
    {
        private Label statusLabel;

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ScreenHeaderUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            statusLabel = root.Q<Label>("status-label");

            ScreenHeader header = root.Q<ScreenHeader>("screen-header");
            header.Action1Clicked += () => SetStatus("Back tapped — a real app would pop this screen.");
            header.Action3Clicked += () => SetStatus("Info tapped — show help or an about panel here.");
            header.Action4Toggled += isMuted => SetStatus(isMuted ? "Sound muted." : "Sound on.");
        }

        private void SetStatus(string message)
        {
            if (statusLabel != null)
            {
                statusLabel.text = message;
            }
        }
    }
}
