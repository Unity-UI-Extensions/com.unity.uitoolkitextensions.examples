// Examples~/SocialLinks/SocialLinksDemo.cs
// Demonstrates: SocialLinkContainer
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the same
//   GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The SocialLinkContainer is shown in edit mode, so the "+ Add Social" button and a
//        per-row remove (✕) button are visible.
//      - Tap "+ Add Social", pick a platform from the wheel picker, then type a handle/URL.
//        Only platforms not already in the list are offered.
//      - "Show Entered Links" reads the rows back via GetSocials() and lists them below.

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Social links demo using <see cref="SocialLinkContainer"/> "as is": an editable list of
    /// platform-labelled URL fields. The demo turns on edit mode so rows can be added and removed,
    /// supplies a placeholder resolver, seeds a couple of rows, and reads the entered links back
    /// out with <see cref="SocialLinkContainer.GetSocials"/>.
    /// </summary>
    public class SocialLinksDemo : MonoBehaviour
    {
        private SocialLinkContainer socialLinks;
        private Label statusLabel;

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SocialLinksDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "socialLinksDemo__screen");
            VisualElement card = UIToolkitExtensions.CreateVisualElement(screen, "socialLinksDemo__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "socialLinksDemo__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label title = UIToolkitExtensions.CreateVisualElement<Label>(card, "socialLinksDemo__title");
            title.text = "Social Links";

            Label instruction = UIToolkitExtensions.CreateVisualElement<Label>(card, "socialLinksDemo__instruction");
            instruction.text = "Add as many social links as you like. Tap \"+ Add Social\", pick a platform from the " +
                "wheel, then type your handle. Remove a row with its ✕ button.";

            socialLinks = UIToolkitExtensions.CreateVisualElement<SocialLinkContainer>(card, "socialLinksDemo__container");
            socialLinks.Label = "Your Profiles";
            socialLinks.IsInEditMode = true;
            socialLinks.PlatformPlaceholderResolver = platform => $"Enter your {platform} handle or URL";

            socialLinks.CreateSocial(SocialLinkContainer.SocialPlatform.Instagram, "@uitoolkit.extensions");
            socialLinks.CreateSocial(SocialLinkContainer.SocialPlatform.YouTube, "UnityUIExtensions");

            VisualElement bottomBar = UIToolkitExtensions.CreateVisualElement(card, "socialLinksDemo__bottomBar");

            PillButton showButton = UIToolkitExtensions.CreateVisualElement<PillButton>(bottomBar, "socialLinksDemo__showButton");
            showButton.Text = "Show Entered Links";
            showButton.SetInnerColor("#4A90E2");
            showButton.SetOuterColor("#7B68EE");
            showButton.SetTextColor(Color.white);
            showButton.SetFontSize(15f);
            showButton.Clicked += OnShowClicked;

            statusLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "socialLinksDemo__status");
            statusLabel.text = "Your entered links will be listed here.";
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
