// Examples~/RegistrationForm/RegistrationFormDemo.cs
// Demonstrates: PillInputField, RoundedInputField, PillSelector, PillButton, VisualElementShakeUtility
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the same
//   GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A registration card holds Full Name / Email / Password PillInputFields, an
//        optional multiline RoundedInputField bio, and a PillSelector that cycles
//        categories each time it is tapped.
//      - "Create Account" validates the fields (name not empty, email contains '@',
//        password at least 6 characters); each invalid field shakes via
//        VisualElementShakeUtility.
//      - On success the form fades out and a confirmation message fades in.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Registration form demo showcasing <see cref="PillInputField"/>,
    /// <see cref="RoundedInputField"/>, <see cref="PillButton"/>, <see cref="PillSelector"/>,
    /// and <see cref="VisualElementShakeUtility"/> for validation feedback.
    ///
    /// Validation rules:
    ///   Full Name  — must not be empty.
    ///   Email      — must contain '@'.
    ///   Password   — must be at least 6 characters.
    ///   Bio        — optional (no validation).
    ///   Category   — cycles through options when the selector is tapped.
    ///
    /// On successful validation the form fades out and a success message is shown
    /// after a short coroutine delay.
    /// </summary>
    public class RegistrationFormDemo : MonoBehaviour
    {
        private static readonly string[] CategoryOptions =
        {
            "Fitness",
            "Nutrition",
            "Wellness",
            "Mindfulness",
        };

        private int currentCategoryIndex = 0;

        private UIDocument uiDocument;
        private PillInputField nameField;
        private PillInputField emailField;
        private PillInputField passwordField;
        private RoundedInputField bioField;
        private PillSelector categorySelector;
        private VisualElement formContainer;
        private VisualElement successContainer;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("RegistrationFormDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "registrationForm__screen");

            VisualElement card = UIToolkitExtensions.CreateVisualElement(screen, "registrationForm__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "registrationForm__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "registrationForm__title");
            titleLabel.text = "Registration Form";

            Label subtitleLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "registrationForm__subtitle");
            subtitleLabel.text = "Complete the sample form below to review the shared example card style in a more complex layout.";

            formContainer = UIToolkitExtensions.CreateVisualElement(card, "registrationForm__formContainer");

            nameField = new PillInputField();
            nameField.Label = "Full Name";
            nameField.SetPlaceholder("Your full name");
            nameField.KeyboardType = TouchScreenKeyboardType.Default;
            nameField.AddToClassList("registrationForm__nameField");
            ApplyPillInputStyle(nameField);
            formContainer.Add(nameField);

            emailField = new PillInputField();
            emailField.Label = "Email Address";
            emailField.SetPlaceholder("you@example.com");
            emailField.KeyboardType = TouchScreenKeyboardType.EmailAddress;
            emailField.AddToClassList("registrationForm__emailField");
            ApplyPillInputStyle(emailField);
            formContainer.Add(emailField);

            passwordField = new PillInputField();
            passwordField.Label = "Password";
            passwordField.SetPlaceholder("Min. 6 characters");
            passwordField.SetPasswordMode(true);
            passwordField.SetMaxLength(64);
            passwordField.AddToClassList("registrationForm__passwordField");
            ApplyPillInputStyle(passwordField);
            formContainer.Add(passwordField);

            Label bioSectionLabel = UIToolkitExtensions.CreateVisualElement<Label>(formContainer, "registrationForm__sectionLabel");
            bioSectionLabel.text = "Bio (optional)";

            bioField = new RoundedInputField();
            bioField.Placeholder = "Tell us a little about yourself…";
            bioField.Multiline = true;
            bioField.MaxLength = 280;
            bioField.SetTextColor(Color.white);
            bioField.SetFontSize(14f);
            bioField.AddToClassList("registrationForm__bioField");
            formContainer.Add(bioField);

            categorySelector = new PillSelector();
            categorySelector.Label = "Category";
            categorySelector.Value = CategoryOptions[currentCategoryIndex];
            categorySelector.AddToClassList("registrationForm__categorySelector");
            categorySelector.Clicked += OnCategorySelectorClicked;
            formContainer.Add(categorySelector);

            PillButton submitButton = new PillButton();
            submitButton.Text = "Create Account";
            submitButton.SetInnerColor("#4A90E2");
            submitButton.SetOuterColor("#7B68EE");
            submitButton.SetTextColor(Color.white);
            submitButton.SetFontSize(16f);
            submitButton.AddToClassList("registrationForm__submitButton");
            submitButton.Clicked += OnSubmitClicked;
            formContainer.Add(submitButton);

            BuildSuccessContainer(card);
        }

        private void BuildSuccessContainer(VisualElement card)
        {
            successContainer = UIToolkitExtensions.CreateVisualElement(card, "registrationForm__successContainer");

            VisualElement icon = UIToolkitExtensions.CreateVisualElement(successContainer, "registrationForm__successIcon");

            Label checkLabel = UIToolkitExtensions.CreateVisualElement<Label>(icon, "registrationForm__successCheck");
            checkLabel.text = "✓";

            Label successTitle = UIToolkitExtensions.CreateVisualElement<Label>(successContainer, "registrationForm__successTitle");
            successTitle.text = "Account Created!";

            Label successMessage = UIToolkitExtensions.CreateVisualElement<Label>(successContainer, "registrationForm__successMessage");
            successMessage.text = "Welcome aboard! Your account has been successfully created. You can now explore all features.";
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

        /// <summary>
        /// Validates all required fields.
        /// Shakes each invalid field using <see cref="VisualElementShakeUtility"/>.
        /// </summary>
        /// <returns>True when all rules pass.</returns>
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

        /// <summary>
        /// Fades the form out, waits briefly, then reveals the success message.
        /// </summary>
        private IEnumerator ShowSuccessTransition()
        {
            formContainer.style.transitionProperty = new StyleList<StylePropertyName>(
                new List<StylePropertyName> { new("opacity") });
            formContainer.style.transitionDuration = new StyleList<TimeValue>(
                new List<TimeValue> { new(400, TimeUnit.Millisecond) });
            formContainer.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new List<EasingFunction> { new(EasingMode.EaseInOut) });
            formContainer.style.opacity = 0f;

            yield return new WaitForSeconds(0.45f);

            formContainer.style.display = DisplayStyle.None;
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