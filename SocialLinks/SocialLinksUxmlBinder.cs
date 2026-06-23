using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    public class SocialLinksUxmlBinder : MonoBehaviour
    {
        private SocialLinkContainer socialLinks;
        private Label statusLabel;

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SocialLinksUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;

            statusLabel = root.Q<Label>("status-label");

            socialLinks = root.Q<SocialLinkContainer>("social-links");
            socialLinks.PlatformPlaceholderResolver = platform => $"Enter your {platform} handle or URL";

            // Edit mode is enabled in the UXML, so seeded rows also get their remove buttons.
            socialLinks.CreateSocial(SocialLinkContainer.SocialPlatform.Instagram, "@uitoolkit.extensions");
            socialLinks.CreateSocial(SocialLinkContainer.SocialPlatform.YouTube, "UnityUIExtensions");

            PillButton showButton = root.Q<PillButton>("show-button");
            showButton.SetTextColor(Color.white);
            showButton.Clicked += OnShowClicked;
        }

        private void OnShowClicked()
        {
            List<SocialLink> socials = socialLinks.GetSocials();
            if (socials.Count == 0)
            {
                statusLabel.text = "No links entered yet.";
                return;
            }

            var builder = new StringBuilder();
            foreach (SocialLink social in socials)
            {
                builder.AppendLine($"{social.platform}: {social.url}");
            }

            statusLabel.text = builder.ToString().TrimEnd();
        }
    }
}
