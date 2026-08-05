using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
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

            ApplyPillInputStyle(nameField);
            ApplyPillInputStyle(emailField);
            ApplyPillInputStyle(passwordField);

            if (bioField != null)
            {
                bioField.SetTextColor(Color.white);
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
                nameField.EnableInClassList("registrationForm__field--error", true);
                isValid = false;
            }
            else
            {
                nameField.EnableInClassList("registrationForm__field--error", false);
            }

            if (!emailField.Value.Contains("@"))
            {
                VisualElementShakeUtility.Shake(emailField, wobbleCount: 4, wobbleDurationMs: 65, amplitudePixels: 10f);
                emailField.EnableInClassList("registrationForm__field--error", true);
                isValid = false;
            }
            else
            {
                emailField.EnableInClassList("registrationForm__field--error", false);
            }

            if (passwordField.Value.Length < 6)
            {
                VisualElementShakeUtility.Shake(passwordField, wobbleCount: 4, wobbleDurationMs: 65, amplitudePixels: 10f);
                passwordField.EnableInClassList("registrationForm__field--error", true);
                isValid = false;
            }
            else
            {
                passwordField.EnableInClassList("registrationForm__field--error", false);
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