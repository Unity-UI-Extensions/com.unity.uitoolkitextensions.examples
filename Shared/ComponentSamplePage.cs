// Examples~/Shared/ComponentSamplePage.cs
// Shared page scaffold for the component samples: a themed screen with a title, a short
// description, rows for controls, an event log line and a stage for the component itself.

using System;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Builds the common sample page. Styles live in Shared/ComponentSampleStyles.uss and read
    /// ThemeTokens only.
    /// </summary>
    public sealed class ComponentSamplePage
    {
        public VisualElement Screen { get; }
        public VisualElement Stage { get; }
        public Label Log { get; }

        public ComponentSamplePage(VisualElement root, string title, string description)
        {
            root.Clear();
            Screen = UIToolkitExtensions.CreateVisualElement(root, "componentSample__screen");

            var titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(Screen, "componentSample__title");
            titleLabel.text = title;

            var body = UIToolkitExtensions.CreateVisualElement<Label>(Screen, "componentSample__body");
            body.text = description;

            Log = UIToolkitExtensions.CreateVisualElement<Label>("componentSample__log");
            Stage = UIToolkitExtensions.CreateVisualElement("componentSample__stage");
        }

        /// <summary>
        /// Adds a row for buttons and toggles above the stage.
        /// </summary>
        public VisualElement AddRow()
        {
            return UIToolkitExtensions.CreateVisualElement(Screen, "componentSample__row");
        }

        public Button AddButton(VisualElement row, string text, Action onClick, bool secondary = false)
        {
            var button = UIToolkitExtensions.CreateButton(row, text, onClick, className: "componentSample__button");
            button.EnableInClassList("componentSample__button--secondary", secondary);
            return button;
        }

        public Toggle AddToggle(VisualElement row, string text, bool value, Action<bool> onChanged)
        {
            var toggle = UIToolkitExtensions.CreateVisualElement<Toggle>(row, "componentSample__toggle");
            toggle.text = text;
            toggle.value = value;
            toggle.RegisterValueChangedCallback(evt => onChanged(evt.newValue));
            return toggle;
        }

        /// <summary>
        /// Adds the log line and the stage after the control rows.
        /// </summary>
        public void Finish()
        {
            Screen.Add(Log);
            Screen.Add(Stage);
        }

        public void Write(string message)
        {
            Log.text = message;
        }
    }
}
