// Examples~/ThemedVariants/ThemedVariantsDemo.cs
// Demonstrates: the ThemeTokens variants of existing controls
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A light page with a compact ScreenHeader (back chevron, eyebrow, title) and
//        themed RoundedInputFields, one of them invalid.
//      - The same fields on the deep ground below, to compare the two surfaces.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// ThemeTokens variants demo: ScreenHeader compact and RoundedInputField themed, on light and deep grounds.
    /// </summary>
    public class ThemedVariantsDemo : MonoBehaviour
    {
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ThemedVariantsDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Themed Variants",
                "Existing controls with token-styled variants: ScreenHeader Compact and RoundedInputField Themed.");
            page.Finish();

            var light = UIToolkitExtensions.CreateVisualElement(page.Stage, "themedDemo__card");
            light.SetSurface(ThemeSurface.Light);

            var header = new ScreenHeader { Variant = ScreenHeaderVariant.Compact, Eyebrow = "OPTIONS", Title = "Account" };
            header.Configure(showAction1: true, showTitle: true, showAction2: false, showAction3: false, showAction4Toggle: false);
            header.Action1Clicked += () => page.Write("Back");
            light.Add(header);
            AddFields(light);

            var deep = UIToolkitExtensions.CreateVisualElement(page.Stage, "themedDemo__card");
            AddFields(deep);
        }

        private static void AddFields(VisualElement parent)
        {
            var email = new RoundedInputField { Variant = RoundedInputFieldVariant.Themed, Placeholder = "Email", Value = "mara@example.org" };
            email.AddToClassList("themedDemo__field");
            parent.Add(email);

            var password = new RoundedInputField { Variant = RoundedInputFieldVariant.Themed, Placeholder = "Password", IsPassword = true, Invalid = true };
            password.AddToClassList("themedDemo__field");
            parent.Add(password);
        }
    }
}
