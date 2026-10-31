// Examples~/SettingsRow/SettingsRowDemo.cs
// Demonstrates: SettingsRow
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A settings list on a light card: chevron, value, switch and custom trailing slots.
//      - Tapping a switch row toggles its switch; every row raises Clicked.
//      - The same rows on the deep ground sit below.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// SettingsRow demo: every trailing mode on light and deep surfaces.
    /// </summary>
    public class SettingsRowDemo : MonoBehaviour
    {
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SettingsRowDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Settings Row",
                "Icon, title, subtitle and a trailing slot: chevron, value, switch or custom content.");
            page.Finish();

            var light = UIToolkitExtensions.CreateVisualElement(page.Stage, "settingsRowDemo__card");
            light.SetSurface(ThemeSurface.Light);
            AddRows(light);

            var deep = UIToolkitExtensions.CreateVisualElement(page.Stage, "settingsRowDemo__card");
            AddRows(deep);
        }

        private void AddRows(VisualElement card)
        {
            var account = new SettingsRow("Account", "Signed in as guest");
            account.SetIcon(Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(48, Color.white)));
            account.Clicked += () => page.Write("Account tapped");
            card.Add(account);

            var language = new SettingsRow("Language") { Trailing = SettingsRowTrailing.Value, ValueText = "English (UK)" };
            language.Clicked += () => page.Write("Language tapped");
            card.Add(language);

            var captions = new SettingsRow("Captions", "Show speech as text", SettingsRowTrailing.Switch);
            captions.SwitchChanged += on => page.Write($"Captions: {(on ? "on" : "off")}");
            card.Add(captions);

            var downloads = new SettingsRow("Downloads") { Trailing = SettingsRowTrailing.Custom, ShowDivider = false };
            downloads.Add(new NotificationBadge { Count = 2 });
            downloads.Clicked += () => page.Write("Downloads tapped");
            card.Add(downloads);
        }
    }
}
