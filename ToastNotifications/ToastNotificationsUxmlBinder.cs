using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    public class ToastNotificationsUxmlBinder : MonoBehaviour
    {
        private const int MaxToasts = 5;
        private const int FadeDurationMs = 260;

        private static readonly (string title, string subtitle)[] ToastTemplates =
        {
            ("Workout Complete", "You finished your 30-min run. Great job!"),
            ("New Message", "Alex sent you a new message."),
            ("Goal Reached", "You hit your daily step target of 10,000."),
            ("Weekly Summary Ready", "Your progress report is available to view."),
            ("Reminder", "Don't forget to log your meals today."),
            ("Achievement Unlocked", "7-day streak — keep it up!"),
            ("Sync Complete", "Your data has been synced across devices."),
            ("Low Water Intake", "You've only had 2 of your 8 glasses today."),
        };

        private static readonly Color[] ToastColors =
        {
            new(0.96f, 0.95f, 1.00f, 1f),
            new(0.95f, 1.00f, 0.97f, 1f),
            new(1.00f, 0.97f, 0.93f, 1f),
            new(0.93f, 0.97f, 1.00f, 1f),
            new(1.00f, 0.95f, 0.95f, 1f),
        };

        private VisualElement toastContainer;
        private int templateIndex;
        private int colorIndex;
        private readonly List<ToastData> activeToasts = new();

        private sealed class ToastData
        {
            public VisualElement Root;
            public ToastSwipeDismissManipulator Manipulator;
            public bool IsDismissing;
        }

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ToastNotificationsUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            toastContainer = root.Q<VisualElement>("toast-container");
            PillButton addButton = root.Q<PillButton>("add-button");
            addButton.SetTextColor(Color.white);
            addButton.Clicked += OnAddToastClicked;
        }

        private void OnAddToastClicked()
        {
            if (activeToasts.Count >= MaxToasts)
            {
                RemoveOldestToast();
            }

            SpawnToast();
        }

        private void SpawnToast()
        {
            var (title, subtitle) = ToastTemplates[templateIndex % ToastTemplates.Length];
            templateIndex++;
            var bgColor = ToastColors[colorIndex % ToastColors.Length];
            colorIndex++;

            var toast = new VisualElement();
            toast.AddToClassList("toastDemo__toast");
            toast.style.backgroundColor = bgColor;
            toastContainer.Add(toast);

            var accentBar = new VisualElement();
            accentBar.AddToClassList("toastDemo__accent");
            Color.RGBToHSV(bgColor, out var h, out _, out _);
            accentBar.style.backgroundColor = Color.HSVToRGB((h + 0.5f) % 1f, 0.65f, 0.65f);
            toast.Add(accentBar);

            var textBlock = new VisualElement();
            textBlock.AddToClassList("toastDemo__textBlock");
            toast.Add(textBlock);

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("toastDemo__title");
            textBlock.Add(titleLabel);

            var subtitleLabel = new Label(subtitle);
            subtitleLabel.AddToClassList("toastDemo__subtitle");
            textBlock.Add(subtitleLabel);

            var dismissIcon = new Label("×");
            dismissIcon.AddToClassList("toastDemo__dismiss");
            dismissIcon.pickingMode = PickingMode.Ignore;
            toast.Add(dismissIcon);

            toast.style.translate = new Translate(0, 40, 0);
            toast.style.opacity = 0f;
            toast.schedule.Execute(() => AnimateToastEntrance(toast)).StartingIn(10);

            var toastData = new ToastData { Root = toast };
            var manipulator = new ToastSwipeDismissManipulator(
                canInteract: () => !toastData.IsDismissing,
                canStartAtPosition: _ => true,
                getHorizontalDismissTravelDistance: () => toast.resolvedStyle.width > 0 ? toast.resolvedStyle.width + 32f : 400f,
                getVerticalDismissTravelDistance: () => toast.resolvedStyle.height > 0 ? toast.resolvedStyle.height + 16f : 80f,
                getOffsetTarget: () => toast,
                onInteractionStarted: null,
                onInteractionAborted: null,
                onTapped: () => OnToastTapped(toastData),
                onDismissed: () => OnToastDismissedBySwipe(toastData));

            toast.AddManipulator(manipulator);
            toastData.Manipulator = manipulator;
            activeToasts.Add(toastData);
        }

        private static void AnimateToastEntrance(VisualElement toast)
        {
            toast.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new("opacity"), new("translate") });
            toast.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new(300, TimeUnit.Millisecond), new(300, TimeUnit.Millisecond) });
            toast.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new(EasingMode.EaseOut), new(EasingMode.EaseOut) });
            toast.style.opacity = 1f;
            toast.style.translate = new Translate(0, 0, 0);
        }

        private void OnToastTapped(ToastData toastData)
        {
            if (toastData.IsDismissing) return;
            toastData.IsDismissing = true;
            StartCoroutine(FadeOutAndRemoveToast(toastData));
        }

        private IEnumerator FadeOutAndRemoveToast(ToastData toastData)
        {
            var toast = toastData.Root;
            toast.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new("opacity") });
            toast.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new(FadeDurationMs, TimeUnit.Millisecond) });
            toast.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new(EasingMode.EaseOut) });
            toast.style.opacity = 0f;

            yield return new WaitForSeconds(FadeDurationMs / 1000f + 0.02f);
            RemoveToastFromHierarchy(toastData);
        }

        private void OnToastDismissedBySwipe(ToastData toastData)
        {
            if (toastData.IsDismissing) return;
            toastData.IsDismissing = true;

            var toast = toastData.Root;
            var anim = toast.experimental.animation.Start(1f, 0f, FadeDurationMs, (_, value) =>
            {
                toast.style.opacity = value;
            });
            anim.KeepAlive();
            anim.onAnimationCompleted += () => RemoveToastFromHierarchy(toastData);
        }

        private void RemoveToastFromHierarchy(ToastData toastData)
        {
            activeToasts.Remove(toastData);
            if (toastData.Root.parent != null)
            {
                toastData.Root.RemoveFromHierarchy();
            }
        }

        private void RemoveOldestToast()
        {
            if (activeToasts.Count == 0) return;
            var oldest = activeToasts[0];
            oldest.IsDismissing = true;
            oldest.Manipulator?.ResetState();
            RemoveToastFromHierarchy(oldest);
        }
    }
}
