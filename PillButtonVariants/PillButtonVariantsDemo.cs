// Examples~/PillButtonVariants/PillButtonVariantsDemo.cs
// Demonstrates: PillButton variants (Primary, Ghost, Warn, Danger, Compact, disabled, icons)
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Each row shows a variant at full and compact height.
//      - "Disable" toggles SetEnabled(false) on every button.
//      - "Icons" adds a leading icon (a stand-in disc); it takes the label colour.
//      - The last row keeps the original inner and outer colour gradient for comparison.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// PillButton variants demo: token-styled primary, ghost, warn and danger buttons beside the original gradient.
    /// </summary>
    public class PillButtonVariantsDemo : MonoBehaviour
    {
        private readonly List<PillButton> buttons = new();
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("PillButtonVariantsDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Pill Button Variants",
                "Primary, ghost, warn and danger read ThemeTokens; compact is the shorter height. Custom keeps the inner and outer colours.");
            var options = page.AddRow();
            page.AddToggle(options, "Disable", false, disabled => buttons.ForEach(b => b.SetEnabled(!disabled)));
            var icon = Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(40, Color.white));
            page.AddToggle(options, "Icons", false, on => buttons.ForEach(b => b.SetIcon(on ? icon : default)));
            page.Finish();

            AddRow(PillButtonVariant.Primary, "Continue");
            AddRow(PillButtonVariant.Ghost, "Maybe later");
            AddRow(PillButtonVariant.Warn, "Skip scanning");
            AddRow(PillButtonVariant.Danger, "Stop and save");

            var custom = new PillButton { Text = "Custom gradient" };
            custom.SetInnerColor("#4A90E2");
            custom.SetOuterColor("#7B68EE");
            custom.AddToClassList("pillVariantsDemo__button");
            custom.Clicked += () => page.Write("Clicked: Custom");
            buttons.Add(custom);
            page.Stage.Add(custom);
        }

        private void AddRow(PillButtonVariant variant, string text)
        {
            var row = UIToolkitExtensions.CreateVisualElement(page.Stage, "pillVariantsDemo__row");
            foreach (var compact in new[] { false, true })
            {
                var button = new PillButton { Text = text, Variant = variant, Compact = compact };
                button.AddToClassList("pillVariantsDemo__button");
                button.Clicked += () => page.Write($"Clicked: {variant}{(compact ? " (compact)" : string.Empty)}");
                buttons.Add(button);
                row.Add(button);
            }
        }
    }
}
