// Examples~/DropDownPhoneEntry/DropDownPhoneEntryDemo.cs
// Demonstrates: DropDownControl, PillInputField, PillButton
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the same
//   GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A dial-code DropDownControl and a PillInputField sit side by side in a single
//        composite input row.
//      - Pick a dial code from the dropdown, then type a phone number (the digits are
//        grouped and formatted as you type).
//      - The "Send code" PillButton stays disabled until at least 10 digits are entered;
//        the status line reports readiness and confirms the simulated send.

using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Demonstrates <see cref="DropDownControl"/> paired with <see cref="PillInputField"/>
    /// </summary>
    public class DropDownPhoneEntryDemo : MonoBehaviour
    {
        private static readonly string[] DialCodeOptions =
        {
            "+1",
            "+33",
            "+34",
            "+44",
            "+49",
            "+61",
            "+81",
            "+91",
        };

        private const string WaitingStatusText = "Enter at least 10 digits to enable the button.";

        private UIDocument uiDocument;
        private DropDownControl dialCodePicker;
        private PillInputField phoneNumberInput;
        private PillButton sendCodeButton;
        private Label statusLabel;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("DropDownPhoneEntryDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
            UpdateSendButtonState(phoneNumberInput?.Value);
        }

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "phoneVerificationExample__screen");
            VisualElement card = UIToolkitExtensions.CreateVisualElement(screen, "phoneVerificationExample__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "phoneVerificationExample__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label title = UIToolkitExtensions.CreateVisualElement<Label>(card, "phoneVerificationExample__title");
            title.text = "YOUR PHONE NUMBER";

            Label description = UIToolkitExtensions.CreateVisualElement<Label>(card, "phoneVerificationExample__description");
            description.text = "To protect this space, we verify each person once. We'll send a verification code by text message.";

            VisualElement compositeInput = UIToolkitExtensions.CreateVisualElement(card, "phoneVerificationExample__phoneCompositeInput");

            dialCodePicker = UIToolkitExtensions.CreateVisualElement<DropDownControl>(compositeInput, "phoneVerificationExample__dialCodePicker");
            dialCodePicker.Items = DialCodeOptions;
            dialCodePicker.SetDefault("+44");
            dialCodePicker.OpenStateChanged += HandlePickerOpenStateChanged;
            dialCodePicker.ValueChanged += HandleDialCodeChanged;

            UIToolkitExtensions.CreateVisualElement(compositeInput, "phoneVerificationExample__inputDivider");

            phoneNumberInput = UIToolkitExtensions.CreateVisualElement<PillInputField>(compositeInput, "phoneVerificationExample__phoneInput");
            phoneNumberInput.SetPlaceholder("Type your number here");
            phoneNumberInput.KeyboardType = TouchScreenKeyboardType.PhonePad;
            phoneNumberInput.OnValueChanged += UpdateSendButtonState;
            phoneNumberInput.OnValidation += UpdateSendButtonState;

            Label info = UIToolkitExtensions.CreateVisualElement<Label>(card, "phoneVerificationExample__info");
            info.text = "Your phone number is used only for verification and connection, never shared.";

            sendCodeButton = UIToolkitExtensions.CreateVisualElement<PillButton>(card, "phoneVerificationExample__sendButton");
            sendCodeButton.Text = "Send code";
            sendCodeButton.SetInnerColor("#4A90E2");
            sendCodeButton.SetOuterColor("#7B68EE");
            sendCodeButton.SetTextColor(Color.white);
            sendCodeButton.Clicked += SendCode;

            statusLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "phoneVerificationExample__status");
            statusLabel.text = WaitingStatusText;
        }

        private void HandlePickerOpenStateChanged(bool isOpen)
        {
            if (statusLabel == null)
            {
                return;
            }

            if (isOpen)
            {
                statusLabel.text = "Choose a dial code.";
                return;
            }

            UpdateSendButtonState(phoneNumberInput?.Value);
        }

        private void HandleDialCodeChanged(string value)
        {
            UpdateSendButtonState(phoneNumberInput?.Value);
        }

        private void UpdateSendButtonState(string rawValue)
        {
            if (sendCodeButton == null)
            {
                return;
            }

            bool isValid = ExtractDigits(rawValue).Length >= 10;
            sendCodeButton.SetEnabled(isValid);
            sendCodeButton.style.opacity = isValid ? 1f : 0.42f;

            if (statusLabel == null)
            {
                return;
            }

            if (!isValid)
            {
                statusLabel.text = WaitingStatusText;
                return;
            }

            statusLabel.text = $"Ready to send a code to {dialCodePicker?.Value} {FormatReadableNumber(rawValue)}.";
        }

        private void SendCode()
        {
            string digits = ExtractDigits(phoneNumberInput?.Value);
            if (digits.Length < 10)
            {
                UpdateSendButtonState(phoneNumberInput?.Value);
                return;
            }

            statusLabel.text = $"Sample send complete: code sent to {dialCodePicker?.Value} {digits}.";
        }

        private static string ExtractDigits(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(value.Length);
            for (int index = 0; index < value.Length; index++)
            {
                char currentCharacter = value[index];
                if (char.IsDigit(currentCharacter))
                {
                    builder.Append(currentCharacter);
                }
            }

            return builder.ToString();
        }

        private static string FormatReadableNumber(string value)
        {
            string digits = ExtractDigits(value);
            if (digits.Length <= 3)
            {
                return digits;
            }

            if (digits.Length <= 6)
            {
                return $"{digits[..3]} {digits[3..]}";
            }

            if (digits.Length <= 10)
            {
                return $"{digits[..3]} {digits[3..6]} {digits[6..]}";
            }

            return $"{digits[..3]} {digits[3..6]} {digits[6..10]} {digits[10..]}";
        }
    }
}