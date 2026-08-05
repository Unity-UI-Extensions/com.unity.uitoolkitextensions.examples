// Examples~/StepWizard/StepWizardUxmlBinder.cs
// UXML counterpart of StepWizardDemo.cs. Layout lives in StepWizardUxmlView.uxml;
// this binder drives navigation, panel visibility, the stepper and the progress bar.

using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Behaviour binder for the UXML-authored StepWizard demo.
    /// </summary>
    public class StepWizardUxmlBinder : MonoBehaviour
    {
        private const int StepCount = 4;

        private UIDocument uiDocument;
        private QuadrantStepper stepper;
        private StepProgressBar progressBar;
        private Label stepCounterLabel;
        private PillButton backButton;
        private PillButton nextButton;
        private readonly VisualElement[] panels = new VisualElement[StepCount];

        private int currentStep;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("StepWizardUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            stepper = root.Q<QuadrantStepper>("stepper");
            progressBar = root.Q<StepProgressBar>("progress-bar");
            stepCounterLabel = root.Q<Label>("step-counter");
            backButton = root.Q<PillButton>("back-button");
            nextButton = root.Q<PillButton>("next-button");
            for (var i = 0; i < StepCount; i++)
            {
                panels[i] = root.Q<VisualElement>($"panel-{i}");
            }

            if (stepper == null || progressBar == null || backButton == null || nextButton == null)
            {
                Debug.LogWarning("StepWizardUxmlBinder: one or more named elements were not found in the UXML.", this);
                return;
            }

            root.Query<RoundedInputField>().ForEach(field =>
            {
                field.SetBackgroundColor(new Color(1f, 1f, 1f, 0.03f));
                field.SetTextColor(Color.white);
                field.SetPlaceholderColor(new Color(0.71f, 0.75f, 0.86f, 0.65f));
                field.SetFontSize(14f);
            });

            backButton.SetTextColor(Color.white);
            nextButton.SetTextColor(Color.white);

            stepper.SelectionChanged += OnStepperSelectionChanged;
            backButton.Clicked += OnBackClicked;
            nextButton.Clicked += OnNextClicked;

            GoToStep(0, notify: false);
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
            if (currentStep < StepCount - 1)
            {
                GoToStep(currentStep + 1, notify: true);
            }
            else
            {
                Debug.Log("[StepWizardUxmlBinder] Wizard finished.");
                GoToStep(0, notify: true);
            }
        }

        private void GoToStep(int index, bool notify)
        {
            index = Mathf.Clamp(index, 0, StepCount - 1);
            currentStep = index;

            for (var i = 0; i < panels.Length; i++)
            {
                if (panels[i] != null)
                {
                    panels[i].style.display = i == currentStep ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }

            stepper.SetSelectedIndex(currentStep, notify: false, animate: notify);
            progressBar.SetProgress(currentStep + 1, StepCount);

            if (stepCounterLabel != null)
            {
                stepCounterLabel.text = $"Step {currentStep + 1} of {StepCount}";
            }

            backButton.style.display = currentStep == 0 ? DisplayStyle.None : DisplayStyle.Flex;

            bool isLast = currentStep == StepCount - 1;
            nextButton.Text = isLast ? "Finish" : "Next";
            nextButton.SetInnerColor("#4A90E2");
            nextButton.SetOuterColor("#7B68EE");
        }
    }
}
