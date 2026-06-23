// Examples~/DropDownPhoneEntry/DropDownPhoneEntryUxmlBinder.cs
// UXML counterpart of DropDownPhoneEntryDemo.cs.
//
// The layout lives in DropDownPhoneEntryUxmlView.uxml (loaded by the UIDocument on this
// GameObject). This binder only queries the cloned tree and wires the interactive
// behaviour — the "layout in UXML, behaviour in C#" division of labour.

using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>Behaviour binder for the UXML-authored DropDownPhoneEntry demo.</summary>
    public class DropDownPhoneEntryUxmlBinder : MonoBehaviour
    {
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
                Debug.LogError("DropDownPhoneEntryUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;

            // Robust dark backdrop even if the stylesheet fails to resolve.
            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            dialCodePicker = root.Q<DropDownControl>("dial-code-picker");
            phoneNumberInput = root.Q<PillInputField>("phone-input");
            sendCodeButton = root.Q<PillButton>("send-button");
            statusLabel = root.Q<Label>("status-label");

            if (dialCodePicker == null || phoneNumberInput == null || sendCodeButton == null)
            {
                Debug.LogWarning("DropDownPhoneEntryUxmlBinder: one or more named elements were not found in the UXML.", this);
                return;
            }

            // Items are declared in UXML; just pick a sensible default and subscribe.
            dialCodePicker.SetDefault("+44");
            dialCodePicker.OpenStateChanged += HandlePickerOpenStateChanged;
            dialCodePicker.ValueChanged += HandleDialCodeChanged;

            phoneNumberInput.OnValueChanged += UpdateSendButtonState;
            phoneNumberInput.OnValidation += UpdateSendButtonState;

            sendCodeButton.SetTextColor(Color.white);
            sendCodeButton.Clicked += SendCode;

            UpdateSendButtonState(phoneNumberInput.Value);
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
            foreach (char currentCharacter in value)
            {
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
