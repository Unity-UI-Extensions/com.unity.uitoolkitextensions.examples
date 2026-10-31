// Examples~/Switch/SwitchDemo.cs
// Demonstrates: Switch
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Tap a switch, or focus it and press Enter or Space, to toggle it.
//      - "Reduce motion" applies the reduce-motion class to the page, so the thumbs jump.
//      - The last switch is disabled.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Switch demo: labelled switches, the change event, reduce motion and the disabled state.
    /// </summary>
    public class SwitchDemo : MonoBehaviour
    {
        private ComponentSamplePage page;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("SwitchDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "Switch",
                "A labelled on/off field. It is a BaseField<bool>, so it raises ChangeEvent<bool> and binds like Toggle.");
            page.Finish();

            var captions = new Switch("Captions") { value = true };
            captions.ValueChanged += on => page.Write($"Captions: {(on ? "on" : "off")}");
            page.Stage.Add(captions);

            var reduceMotion = new Switch("Reduce motion");
            reduceMotion.ValueChanged += on =>
            {
                root.SetReduceMotion(on);
                page.Write($"Reduce motion: {(on ? "on" : "off")}");
            };
            page.Stage.Add(reduceMotion);

            var locked = new Switch("Offline maps (needs storage)");
            locked.SetEnabled(false);
            page.Stage.Add(locked);
        }
    }
}
