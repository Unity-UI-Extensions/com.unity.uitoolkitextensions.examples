// Examples~/RegistrationForm/RegistrationFormUxmlBinder.cs
// UXML counterpart of RegistrationFormDemo.cs. Layout lives in RegistrationFormUxmlView.uxml;
// this binder wires validation (with shake feedback) and the success transition.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>Behaviour binder for the UXML-authored RegistrationForm demo.</summary>
    public class RegistrationFormUxmlBinder : MonoBehaviour
    {
        private static readonly string[] CategoryOptions =
        {
            "Fitness",
            "Nutrition",
            "Wellness",
            "Mindfulness",
        };

        private int currentCategoryIndex;

        private UIDocument uiDocument;
        private PillInputField nameField;
        private PillInputField emailField;
        private PillInputField passwordField;
        private RoundedInputField bioField;
        private PillSelector categorySelector;
        private PillButton submitButton;
        private VisualElement formContainer;
        private VisualElement successContainer;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("RegistrationFormUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            nameField = root.Q<PillInputField>("name-field");
            emailField = root.Q<PillInputField>("email-field");
            passwordField = root.Q<PillInputField>("password-field");
            bioField = root.Q<RoundedInputField>("bio-field");
            categorySelector = root.Q<PillSelector>("category-selector");
            submitButton = root.Q<PillButton>("submit-button");
            formContainer = root.Q<VisualElement>("form-container");
            successContainer = root.Q<VisualElement>("success-container");

            if (nameField == null || emailField == null || passwordField == null || submitButton == null)
            {
                Debug.LogWarning("RegistrationFormUxmlBinder: one or more named elements were not found in the UXML.", this);
                return;
            }

            // Field appearance is configured through the controls' runtime API.
            ApplyPillInputStyle(nameField);
            ApplyPillInputStyle(emailField);
            ApplyPillInputStyle(passwordField);

            if (bioField != null)
            {
                bioField.SetBackgroundColor(new Color(1f, 1f, 1f, 0.03f));
                bioField.SetTextColor(Color.white);
                bioField.SetPlaceholderColor(new Color(0.71f, 0.75f, 0.86f, 0.65f));
                bioField.SetFontSize(14f);
            }

            if (categorySelector != null)
            {
                categorySelector.Clicked += OnCategorySelectorClicked;
            }

            submitButton.SetTextColor(Color.white);
            submitButton.SetFontSize(16f);
            submitButton.Clicked += OnSubmitClicked;
        }

        private static void ApplyPillInputStyle(PillInputField field)
        {
            field.SetBackgroundColor(new Color(1f, 1f, 1f, 0.03f));
            field.SetTextColor(Color.white);
            field.SetFontSize(15f);
        }

        private void OnCategorySelectorClicked()
        {
            currentCategoryIndex = (currentCategoryIndex + 1) % CategoryOptions.Length;
            categorySelector.Value = CategoryOptions[currentCategoryIndex];
        }

        private void OnSubmitClicked()
        {
            if (!ValidateForm())
            {
                return;
            }

            StartCoroutine(ShowSuccessTransition());
        }

        private bool ValidateForm()
        {
            var isValid = true;

            if (string.IsNullOrWhiteSpace(nameField.Value))
            {
                VisualElementShakeUtility.Shake(nameField, wobbleCount: 4, wobbleDurationMs: 65, amplitudePixels: 10f);
                nameField.SetBackgroundColor(new Color(0.46f, 0.14f, 0.22f, 0.9f));
                isValid = false;
            }
            else
            {
                nameField.SetBackgroundColor(new Color(1f, 1f, 1f, 0.03f));
            }

            if (!emailField.Value.Contains("@"))
            {
                VisualElementShakeUtility.Shake(emailField, wobbleCount: 4, wobbleDurationMs: 65, amplitudePixels: 10f);
                emailField.SetBackgroundColor(new Color(0.46f, 0.14f, 0.22f, 0.9f));
                isValid = false;
            }
            else
            {
                emailField.SetBackgroundColor(new Color(1f, 1f, 1f, 0.03f));
            }

            if (passwordField.Value.Length < 6)
            {
                VisualElementShakeUtility.Shake(passwordField, wobbleCount: 4, wobbleDurationMs: 65, amplitudePixels: 10f);
                passwordField.SetBackgroundColor(new Color(0.46f, 0.14f, 0.22f, 0.9f));
                isValid = false;
            }
            else
            {
                passwordField.SetBackgroundColor(new Color(1f, 1f, 1f, 0.03f));
            }

            return isValid;
        }

        private IEnumerator ShowSuccessTransition()
        {
            if (formContainer != null)
            {
                formContainer.style.transitionProperty = new StyleList<StylePropertyName>(
                    new List<StylePropertyName> { new("opacity") });
                formContainer.style.transitionDuration = new StyleList<TimeValue>(
                    new List<TimeValue> { new(400, TimeUnit.Millisecond) });
                formContainer.style.transitionTimingFunction = new StyleList<EasingFunction>(
                    new List<EasingFunction> { new(EasingMode.EaseInOut) });
                formContainer.style.opacity = 0f;
            }

            yield return new WaitForSeconds(0.45f);

            if (formContainer != null)
            {
                formContainer.style.display = DisplayStyle.None;
            }

            if (successContainer == null)
            {
                yield break;
            }

            successContainer.style.display = DisplayStyle.Flex;
            successContainer.style.opacity = 0f;
            successContainer.style.transitionProperty = new StyleList<StylePropertyName>(
                new List<StylePropertyName> { new("opacity") });
            successContainer.style.transitionDuration = new StyleList<TimeValue>(
                new List<TimeValue> { new(500, TimeUnit.Millisecond) });
            successContainer.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new List<EasingFunction> { new(EasingMode.EaseOut) });

            yield return null;

            successContainer.style.opacity = 1f;
        }
    }
}
