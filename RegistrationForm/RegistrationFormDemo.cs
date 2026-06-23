// Examples~/RegistrationForm/RegistrationFormDemo.cs
// Demonstrates: PillInputField, RoundedInputField, PillButton, PillSelector,
//               VisualElementShakeUtility (validation shake)
//
// Scene setup:
//   A companion scene already wires this component to a GameObject that carries a
//   UIDocument. The demo resolves that UIDocument via GetComponent<UIDocument>()
//   on the same GameObject, so no manual assignment is required.
//   Press Play — fill in the form and tap "Create Account".
//   Invalid fields shake and highlight. Valid submission shows a success screen.

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
        // ── Category options ──────────────────────────────────────────────────────
        private static readonly string[] CategoryOptions =
        {
            "Fitness",
            "Nutrition",
            "Wellness",
            "Mindfulness",
        };

        private int currentCategoryIndex = 0;

        // ── Runtime field references ───────────────────────────────────────────────
        private UIDocument uiDocument;
        private PillInputField nameField;
        private PillInputField emailField;
        private PillInputField passwordField;
        private RoundedInputField bioField;
        private PillSelector categorySelector;
        private VisualElement formContainer;
        private VisualElement successContainer;

        // ──────────────────────────────────────────────────────────────────────────
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

        // ──────────────────────────────────────────────────────────────────────────
        private void BuildUI(VisualElement root)
        {
            // Full-screen scroll container
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "registrationForm__screen");

            // ── Form container ────────────────────────────────────────────────────
            formContainer = UIToolkitExtensions.CreateVisualElement(screen, "registrationForm__formContainer");

            // Title
            Label titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(formContainer, "registrationForm__title");
            titleLabel.text = "Create Account";

            Label subtitleLabel = UIToolkitExtensions.CreateVisualElement<Label>(formContainer, "registrationForm__subtitle");
            subtitleLabel.text = "Join us today — it only takes a moment.";

            // ── Full Name — PillInputField ─────────────────────────────────────────
            nameField = new PillInputField();
            nameField.Label = "Full Name";
            nameField.SetPlaceholder("Your full name");
            nameField.KeyboardType = TouchScreenKeyboardType.Default;
            nameField.AddToClassList("registrationForm__nameField");
            ApplyPillInputStyle(nameField);
            formContainer.Add(nameField);

            // ── Email — PillInputField ────────────────────────────────────────────
            emailField = new PillInputField();
            emailField.Label = "Email Address";
            emailField.SetPlaceholder("you@example.com");
            emailField.KeyboardType = TouchScreenKeyboardType.EmailAddress;
            emailField.AddToClassList("registrationForm__emailField");
            ApplyPillInputStyle(emailField);
            formContainer.Add(emailField);

            // ── Password — PillInputField (password mode) ─────────────────────────
            passwordField = new PillInputField();
            passwordField.Label = "Password";
            passwordField.SetPlaceholder("Min. 6 characters");
            passwordField.SetPasswordMode(true);
            passwordField.SetMaxLength(64);
            passwordField.AddToClassList("registrationForm__passwordField");
            ApplyPillInputStyle(passwordField);
            formContainer.Add(passwordField);

            // ── Bio — RoundedInputField (multiline) ───────────────────────────────
            Label bioSectionLabel = UIToolkitExtensions.CreateVisualElement<Label>(formContainer, "registrationForm__sectionLabel");
            bioSectionLabel.text = "Bio (optional)";

            bioField = new RoundedInputField();
            bioField.Placeholder = "Tell us a little about yourself…";
            bioField.Multiline = true;
            bioField.MaxLength = 280;
            bioField.SetBackgroundColor(Color.white);
            bioField.SetTextColor(new Color(0.15f, 0.15f, 0.2f, 1f));
            bioField.SetPlaceholderColor(new Color(0.65f, 0.65f, 0.7f, 1f));
            bioField.SetFontSize(14f);
            bioField.AddToClassList("registrationForm__bioField");
            formContainer.Add(bioField);

            // ── Category — PillSelector ───────────────────────────────────────────
            categorySelector = new PillSelector();
            categorySelector.Label = "Category";
            categorySelector.Value = CategoryOptions[currentCategoryIndex];
            categorySelector.AddToClassList("registrationForm__categorySelector");
            categorySelector.Clicked += OnCategorySelectorClicked;
            formContainer.Add(categorySelector);

            // ── Submit button — PillButton ────────────────────────────────────────
            PillButton submitButton = new PillButton();
            submitButton.Text = "Create Account";
            submitButton.SetInnerColor("#4A90E2");
            submitButton.SetOuterColor("#7B68EE");
            submitButton.SetTextColor(Color.white);
            submitButton.SetFontSize(16f);
            submitButton.AddToClassList("registrationForm__submitButton");
            submitButton.Clicked += OnSubmitClicked;
            formContainer.Add(submitButton);

            // ── Success container (hidden until form submitted) ────────────────────
            BuildSuccessContainer(screen);
        }

        // ── Success screen ─────────────────────────────────────────────────────────
        private void BuildSuccessContainer(VisualElement screen)
        {
            successContainer = UIToolkitExtensions.CreateVisualElement(screen, "registrationForm__successContainer");

            VisualElement icon = UIToolkitExtensions.CreateVisualElement(successContainer, "registrationForm__successIcon");

            Label checkLabel = UIToolkitExtensions.CreateVisualElement<Label>(icon, "registrationForm__successCheck");
            checkLabel.text = "✓";

            Label successTitle = UIToolkitExtensions.CreateVisualElement<Label>(successContainer, "registrationForm__successTitle");
            successTitle.text = "Account Created!";

            Label successMessage = UIToolkitExtensions.CreateVisualElement<Label>(successContainer, "registrationForm__successMessage");
            successMessage.text = "Welcome aboard! Your account has been successfully created. You can now explore all features.";
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        /// <summary>Apply consistent control-API styles to a PillInputField.</summary>
        private static void ApplyPillInputStyle(PillInputField field)
        {
            field.SetBackgroundColor(Color.white);
            field.SetTextColor(new Color(0.15f, 0.15f, 0.2f, 1f));
            field.SetFontSize(15f);
        }

        // ── Event handlers ────────────────────────────────────────────────────────

        private void OnCategorySelectorClicked()
        {
            // Advance through options in a cycle — simulates a picker result
            currentCategoryIndex = (currentCategoryIndex + 1) % CategoryOptions.Length;
            categorySelector.Value = CategoryOptions[currentCategoryIndex];
        }

        private void OnSubmitClicked()
        {
            if (!ValidateForm())
            {
                // Validation failed — individual field shakes were applied inside ValidateForm
                return;
            }

            // All valid — transition to success
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

            // Full Name — must not be empty
            if (string.IsNullOrWhiteSpace(nameField.Value))
            {
                VisualElementShakeUtility.Shake(nameField, wobbleCount: 4, wobbleDurationMs: 65, amplitudePixels: 10f);
                nameField.SetBackgroundColor(new Color(1f, 0.93f, 0.93f, 1f));
                isValid = false;
            }
            else
            {
                nameField.SetBackgroundColor(Color.white);
            }

            // Email — must contain '@'
            if (!emailField.Value.Contains("@"))
            {
                VisualElementShakeUtility.Shake(emailField, wobbleCount: 4, wobbleDurationMs: 65, amplitudePixels: 10f);
                emailField.SetBackgroundColor(new Color(1f, 0.93f, 0.93f, 1f));
                isValid = false;
            }
            else
            {
                emailField.SetBackgroundColor(Color.white);
            }

            // Password — minimum 6 characters
            if (passwordField.Value.Length < 6)
            {
                VisualElementShakeUtility.Shake(passwordField, wobbleCount: 4, wobbleDurationMs: 65, amplitudePixels: 10f);
                passwordField.SetBackgroundColor(new Color(1f, 0.93f, 0.93f, 1f));
                isValid = false;
            }
            else
            {
                passwordField.SetBackgroundColor(Color.white);
            }

            return isValid;
        }

        /// <summary>
        /// Fades the form out, waits briefly, then reveals the success message.
        /// </summary>
        private IEnumerator ShowSuccessTransition()
        {
            // Animate form opacity to 0 via inline style transitions
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

            // Fade success container in
            successContainer.style.opacity = 0f;
            successContainer.style.transitionProperty = new StyleList<StylePropertyName>(
                new List<StylePropertyName> { new("opacity") });
            successContainer.style.transitionDuration = new StyleList<TimeValue>(
                new List<TimeValue> { new(500, TimeUnit.Millisecond) });
            successContainer.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new List<EasingFunction> { new(EasingMode.EaseOut) });

            // Force a frame so the initial opacity=0 is applied before we set it to 1
            yield return null;

            successContainer.style.opacity = 1f;
        }
    }
}
