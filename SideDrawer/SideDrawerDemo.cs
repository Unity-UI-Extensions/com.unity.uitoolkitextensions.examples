// Examples~/SideDrawer/SideDrawerDemo.cs
// Demonstrates: SideDrawer
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Swipe in from the left edge, or tap "Open", to open the drawer.
//      - Tap the scrim, swipe the panel back, or press Escape to close it.
//      - "Right" anchors it to the right edge; "Permanent" shows the wide-layout variant.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// SideDrawer demo: edge swipe, scrim, sections, right anchor and the permanent variant.
    /// </summary>
    public class SideDrawerDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private SideDrawer drawer;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SideDrawerDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Side Drawer",
                "An options drawer with an edge swipe, scrim, sections and a permanent wide-layout variant.");
            var row = page.AddRow();
            page.AddButton(row, "Open", () => drawer.Open());
            page.AddToggle(row, "Right", false, value => drawer.Anchor = value ? DrawerAnchor.Right : DrawerAnchor.Left);
            page.AddToggle(row, "Permanent", false, value => drawer.Variant = value ? DrawerVariant.Permanent : DrawerVariant.Temporary);
            page.Finish();

            drawer = new SideDrawer { TopInset = 24f };
            drawer.Opened += () => page.Write("Opened");
            drawer.Closed += () => page.Write("Closed");

            var account = drawer.AddSection("ACCOUNT");
            account.Add(new SettingsRow("Profile", "Signed in as guest"));
            account.Add(new SettingsRow("Downloads") { Trailing = SettingsRowTrailing.Value, ValueText = "3" });

            var settings = drawer.AddSection("SETTINGS");
            settings.Add(new SettingsRow("Captions", trailingMode: SettingsRowTrailing.Switch));
            settings.Add(new SettingsRow("Language") { Trailing = SettingsRowTrailing.Value, ValueText = "English (UK)" });
            settings.Add(new SettingsRow("About this app") { ShowDivider = false });

            root.Add(drawer);
            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape && drawer.HandleBack())
                {
                    evt.StopPropagation();
                }
            });
        }
    }
}
