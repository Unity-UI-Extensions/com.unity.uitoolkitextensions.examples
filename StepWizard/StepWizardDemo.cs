// Examples~/StepWizard/StepWizardDemo.cs
// Demonstrates: QuadrantStepper, StepProgressBar, PillButton
//
// Scene setup:
//   1. Open the bundled scene (or add this component to a GameObject that also
//      has a UIDocument). The demo calls GetComponent<UIDocument>() on the same
//      GameObject at runtime.
//   2. Press Play — a 4-step wizard with a progress bar, animated stepper overlay,
//      and Back/Next/Finish buttons appears.

using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Four-step onboarding wizard demonstrating the <see cref="QuadrantStepper"/>,
    /// <see cref="StepProgressBar"/>, and <see cref="PillButton"/> controls.
    ///
    /// Steps:
    ///   0 — About      : name and role selection.
    ///   1 — Goals      : checkbox-style goal cards.
    ///   2 — Settings   : notification and privacy toggles.
    ///   3 — Done       : confirmation summary.
    ///
    /// Tapping a stepper segment navigates directly to that step.
    /// The progress bar fills proportionally as steps advance.
    /// Back is hidden on step 0; Next becomes Finish on step 3.
    /// </summary>
    public class StepWizardDemo : MonoBehaviour
    {
        private static readonly string[] StepNames = { "About", "Goals", "Settings", "Done" };

        private static readonly string[] StepDescriptions =
        {
            "Tell us a bit about yourself so we can personalise your experience.",
            "Pick the goals that matter most to you right now.",
            "Configure notifications and privacy to suit your preferences.",
            "You're all set! Review your choices below before finishing.",
        };

        private UIDocument uiDocument;
        private QuadrantStepper stepper;
        private StepProgressBar progressBar;
        private VisualElement contentArea;
        private VisualElement[] stepPanels;
        private PillButton backButton;
        private PillButton nextButton;
        private Label stepCounterLabel;

        private int currentStep = 0;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("StepWizardDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void BuildUI(VisualElement root)
        {
            var screen = UIToolkitExtensions.CreateVisualElement(root, "stepWizard__screen");

            var card = UIToolkitExtensions.CreateVisualElement(screen, "stepWizard__card");

            // Everything down to the progress bar lives in a non-shrinking header so tall
            // step panels can never compress the stepper or progress bar.
            var header = UIToolkitExtensions.CreateVisualElement(card, "stepWizard__header");

            var eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(header, "stepWizard__eyebrow");
            eyebrow.text = "Toolkit Sample";

            var titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "stepWizard__title");
            titleLabel.text = "Step Wizard Demo";

            var subtitleLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "stepWizard__subtitle");
            subtitleLabel.text = "Move through the sample steps below, adjust your preferences, and finish with a compact summary.";

            stepCounterLabel = UIToolkitExtensions.CreateVisualElement<Label>(header, "stepWizard__counter");

            var stepperWrapper = UIToolkitExtensions.CreateVisualElement(header, "stepWizard__stepperWrapper");

            stepper = new QuadrantStepper(StepNames);
            stepper.AddToClassList("stepWizard__stepper");
            stepper.SelectionChanged += OnStepperSelectionChanged;
            stepperWrapper.Add(stepper);

            var progressWrapper = UIToolkitExtensions.CreateVisualElement(header, "stepWizard__progressWrapper");

            progressBar = new StepProgressBar();
            progressBar.AddToClassList("stepWizard__progressBar");
            progressBar.SetGradientColors("#4A90E2", "#7B68EE");
            progressWrapper.Add(progressBar);

            contentArea = UIToolkitExtensions.CreateVisualElement(card, "stepWizard__content");

            stepPanels = new VisualElement[StepNames.Length];
            stepPanels[0] = BuildAboutPanel();
            stepPanels[1] = BuildGoalsPanel();
            stepPanels[2] = BuildSettingsPanel();
            stepPanels[3] = BuildDonePanel();

            foreach (var panel in stepPanels)
            {
                panel.style.display = DisplayStyle.None;
                contentArea.Add(panel);
            }

            var navBar = UIToolkitExtensions.CreateVisualElement(card, "stepWizard__navBar");

            backButton = new PillButton();
            backButton.AddToClassList("stepWizard__navButton");
            backButton.Text = "Back";
            backButton.SetInnerColor("#465062");
            backButton.SetOuterColor("#5C667A");
            backButton.SetTextColor(Color.white);
            backButton.Clicked += OnBackClicked;
            navBar.Add(backButton);

            nextButton = new PillButton();
            nextButton.AddToClassList("stepWizard__navButton");
            nextButton.Text = "Next";
            nextButton.SetInnerColor("#4A90E2");
            nextButton.SetOuterColor("#7B68EE");
            nextButton.SetTextColor(Color.white);
            nextButton.Clicked += OnNextClicked;
            navBar.Add(nextButton);

            GoToStep(0, notify: false);
        }

        private VisualElement BuildAboutPanel()
        {
            var panel = CreateStepPanel("About You", StepDescriptions[0]);

            AddFieldRow(panel, "Full Name", "e.g. Alex Johnson");
            AddFieldRow(panel, "Role", "e.g. Athlete, Coach, Student…");

            var roleHint = UIToolkitExtensions.CreateVisualElement<Label>(panel, "stepWizard__hint");
            roleHint.text = "This helps us show you the most relevant content.";

            return panel;
        }

        private VisualElement BuildGoalsPanel()
        {
            var panel = CreateStepPanel("Your Goals", StepDescriptions[1]);

            string[] goals =
            {
                "Improve fitness & strength",
                "Eat healthier every day",
                "Reduce stress and anxiety",
                "Build better sleep habits",
            };

            foreach (var goal in goals)
            {
                AddGoalCard(panel, goal);
            }

            return panel;
        }

        private VisualElement BuildSettingsPanel()
        {
            var panel = CreateStepPanel("Preferences", StepDescriptions[2]);

            AddToggleRow(panel, "Push Notifications", "Receive reminders and tips");
            AddToggleRow(panel, "Weekly Summary", "Get a progress report each Monday");
            AddToggleRow(panel, "Personalised Ads", "Help support the app");
            AddToggleRow(panel, "Data Sharing",      "Anonymous usage analytics");

            return panel;
        }

        private VisualElement BuildDonePanel()
        {
            var panel = CreateStepPanel("All Done!", StepDescriptions[3]);

            var summaryBox = UIToolkitExtensions.CreateVisualElement(panel, "stepWizard__summaryBox");

            AddSummaryRow(summaryBox, "Steps completed", "4 / 4");
            AddSummaryRow(summaryBox, "Profile",          "Ready");
            AddSummaryRow(summaryBox, "Goals",            "Selected");
            AddSummaryRow(summaryBox, "Preferences",      "Saved");

            return panel;
        }

        private VisualElement CreateStepPanel(string heading, string description)
        {
            var panel = UIToolkitExtensions.CreateVisualElement("stepWizard__panel");

            var h = UIToolkitExtensions.CreateVisualElement<Label>(panel, "stepWizard__panelHeading");
            h.text = heading;

            var desc = UIToolkitExtensions.CreateVisualElement<Label>(panel, "stepWizard__panelDescription");
            desc.text = description;

            return panel;
        }

        private void AddFieldRow(VisualElement parent, string label, string placeholder)
        {
            var row = UIToolkitExtensions.CreateVisualElement(parent, "stepWizard__fieldRow");

            var lbl = UIToolkitExtensions.CreateVisualElement<Label>(row, "stepWizard__fieldLabel");
            lbl.text = label;

            var field = new RoundedInputField();
            field.AddToClassList("stepWizard__field");
            field.Placeholder = placeholder;
            field.SetBackgroundColor(Color.white);
            field.SetTextColor(new Color(0.15f, 0.15f, 0.2f, 1f));
            field.SetPlaceholderColor(new Color(0.65f, 0.65f, 0.7f, 1f));
            field.SetFontSize(14f);
            row.Add(field);
        }

        private void AddGoalCard(VisualElement parent, string goalText)
        {
            var card = UIToolkitExtensions.CreateVisualElement(parent, "stepWizard__goalCard");

            UIToolkitExtensions.CreateVisualElement(card, "stepWizard__goalCheckbox");

            var lbl = UIToolkitExtensions.CreateVisualElement<Label>(card, "stepWizard__goalText");
            lbl.text = goalText;
        }

        private void AddToggleRow(VisualElement parent, string label, string detail)
        {
            var row = UIToolkitExtensions.CreateVisualElement(parent, "stepWizard__toggleRow");

            var textBlock = UIToolkitExtensions.CreateVisualElement(row, "stepWizard__toggleText");

            var lbl = UIToolkitExtensions.CreateVisualElement<Label>(textBlock, "stepWizard__toggleLabel");
            lbl.text = label;

            var detailLbl = UIToolkitExtensions.CreateVisualElement<Label>(textBlock, "stepWizard__toggleDetail");
            detailLbl.text = detail;

            UIToolkitExtensions.CreateVisualElement(row, "stepWizard__toggleIndicator");
        }

        private void AddSummaryRow(VisualElement parent, string key, string value)
        {
            var row = UIToolkitExtensions.CreateVisualElement(parent, "stepWizard__summaryRow");

            var keyLbl = UIToolkitExtensions.CreateVisualElement<Label>(row, "stepWizard__summaryKey");
            keyLbl.text = key;

            var valueLbl = UIToolkitExtensions.CreateVisualElement<Label>(row, "stepWizard__summaryValue");
            valueLbl.text = value;
        }

        private void OnStepperSelectionChanged(int index, string text)
        {
            GoToStep(index, notify: false);
        }

        private void OnBackClicked()
        {
            if (currentStep > 0)
            {
                GoToStep(currentStep - 1, notify: true);
            }
        }

        private void OnNextClicked()
        {
            if (currentStep < StepNames.Length - 1)
            {
                GoToStep(currentStep + 1, notify: true);
            }
            else
            {
                Debug.Log("[StepWizardDemo] Wizard finished.");
                GoToStep(0, notify: true);
            }
        }

        /// <summary>
        /// Switch the active step panel and synchronise the stepper and progress bar.
        /// </summary>
        private void GoToStep(int index, bool notify)
        {
            index = Mathf.Clamp(index, 0, StepNames.Length - 1);
            currentStep = index;

            for (var i = 0; i < stepPanels.Length; i++)
            {
                stepPanels[i].style.display = i == currentStep ? DisplayStyle.Flex : DisplayStyle.None;
            }

            stepper.SetSelectedIndex(currentStep, notify: false, animate: notify);

            progressBar.SetProgress(currentStep + 1, StepNames.Length);

            stepCounterLabel.text = $"Step {currentStep + 1} of {StepNames.Length}";

            backButton.style.display = currentStep == 0 ? DisplayStyle.None : DisplayStyle.Flex;

            nextButton.Text = currentStep == StepNames.Length - 1 ? "Finish" : "Next";
        }
    }
}
