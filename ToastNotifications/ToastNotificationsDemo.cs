// Examples~/ToastNotifications/ToastNotificationsDemo.cs
// Demonstrates: ToastSwipeDismissManipulator
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Tap "Add Toast" to spawn toast notifications (max 5; oldest auto-removed).
//      - Each toast fades away on its own after the default time if left untouched.
//      - Swipe left or right on any toast to dismiss it with an animated slide-out.
//      - Tap a toast (no swipe) to dismiss it with a fade-out.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Toast notification demo using <see cref="ToastSwipeDismissManipulator"/>.
    ///
    /// Architecture:
    ///   - A vertical container stacks active toasts.
    ///   - Each toast is a <see cref="ToastData"/> record holding its VisualElement
    ///     and its <see cref="ToastSwipeDismissManipulator"/> instance.
    ///   - The manipulator's callbacks handle swipe-dismiss (slide + fade) and
    ///     tap-dismiss (fade only) using UI Toolkit experimental animations.
    ///   - Every toast also auto-dismisses after the default time
    ///     (<see cref="AutoDismissSeconds"/>) unless the user taps or swipes it first;
    ///     the pending timer is cancelled when the toast is dismissed by any path.
    ///   - When dismissed the element is removed from the hierarchy and the
    ///     <see cref="ToastData"/> is removed from the active list.
    /// </summary>
    public class ToastNotificationsDemo : MonoBehaviour
    {
        private const int MaxToasts = 5;
        private const int FadeDurationMs = 260;

        private const float AutoDismissSeconds = 4f;

        private static readonly (string title, string subtitle)[] ToastTemplates =
        {
            ("Workout Complete",      "You finished your 30-min run. Great job!"),
            ("New Message",           "Alex sent you a new message."),
            ("Goal Reached",          "You hit your daily step target of 10,000."),
            ("Weekly Summary Ready",  "Your progress report is available to view."),
            ("Reminder",             "Don't forget to log your meals today."),
            ("Achievement Unlocked",  "7-day streak — keep it up!"),
            ("Sync Complete",         "Your data has been synced across devices."),
            ("Low Water Intake",      "You've only had 2 of your 8 glasses today."),
        };

        private static readonly Color[] ToastColors =
        {
            new Color(0.96f, 0.95f, 1.00f, 1f),
            new Color(0.95f, 1.00f, 0.97f, 1f),
            new Color(1.00f, 0.97f, 0.93f, 1f),
            new Color(0.93f, 0.97f, 1.00f, 1f),
            new Color(1.00f, 0.95f, 0.95f, 1f),
        };

        private UIDocument uiDocument;
        private VisualElement toastContainer;
        private int toastCounter = 0;
        private int templateIndex = 0;
        private int colorIndex = 0;

        /// <summary>
        /// Tracks each active toast alongside its manipulator.
        /// </summary>
        private sealed class ToastData
        {
            public VisualElement Root;
            public ToastSwipeDismissManipulator Manipulator;
            public bool IsDismissing;
            public IVisualElementScheduledItem AutoDismissTimer;
        }

        private readonly List<ToastData> activeToasts = new();

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ToastNotificationsDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "toastDemo__screen");

            VisualElement card = UIToolkitExtensions.CreateVisualElement(screen, "toastDemo__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "toastDemo__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label titleLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "toastDemo__headerTitle");
            titleLabel.text = "Toast Notifications";

            Label instructionLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "toastDemo__instruction");
            instructionLabel.text = "Add sample notifications below, dismiss them with a swipe, or tap a toast to clear it instantly.";

            VisualElement bottomBar = UIToolkitExtensions.CreateVisualElement(card, "toastDemo__bottomBar");

            PillButton addButton = UIToolkitExtensions.CreateVisualElement<PillButton>(bottomBar, "toastDemo__addButton");
            addButton.Text = "Add Toast";
            addButton.SetInnerColor("#4A90E2");
            addButton.SetOuterColor("#7B68EE");
            addButton.SetTextColor(Color.white);
            addButton.SetFontSize(15f);
            addButton.Clicked += OnAddToastClicked;

            VisualElement overlay = UIToolkitExtensions.CreateVisualElement(screen, "toastDemo__overlay");
            overlay.pickingMode = PickingMode.Ignore;

            toastContainer = UIToolkitExtensions.CreateVisualElement(overlay, "toastDemo__container");
            toastContainer.pickingMode = PickingMode.Ignore;
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
            toastCounter++;
            var (title, subtitle) = ToastTemplates[templateIndex % ToastTemplates.Length];
            templateIndex++;
            var bgColor = ToastColors[colorIndex % ToastColors.Length];
            colorIndex++;

            var toast = UIToolkitExtensions.CreateVisualElement(toastContainer, "toastDemo__toast");
            toast.style.backgroundColor = bgColor;

            var accentBar = UIToolkitExtensions.CreateVisualElement(toast, "toastDemo__accent");
            Color.RGBToHSV(bgColor, out var h, out var s, out var v);
            accentBar.style.backgroundColor = Color.HSVToRGB((h + 0.5f) % 1f, 0.65f, 0.65f);

            var textBlock = UIToolkitExtensions.CreateVisualElement(toast, "toastDemo__textBlock");

            var titleLbl = UIToolkitExtensions.CreateVisualElement<Label>(textBlock, "toastDemo__title");
            titleLbl.text = title;

            var subtitleLbl = UIToolkitExtensions.CreateVisualElement<Label>(textBlock, "toastDemo__subtitle");
            subtitleLbl.text = subtitle;

            var dismissIcon = UIToolkitExtensions.CreateVisualElement<Label>(toast, "toastDemo__dismiss");
            dismissIcon.text = "×";
            dismissIcon.pickingMode = PickingMode.Ignore;

            toast.style.translate = new Translate(0, 40, 0);
            toast.style.opacity = 0f;

            var toastData = new ToastData { Root = toast };

            toast.schedule.Execute(() =>
            {
                AnimateToastEntrance(toast);
                ScheduleAutoDismiss(toastData);
            }).StartingIn(10);

            var manipulator = new ToastSwipeDismissManipulator(
                canInteract: () => !toastData.IsDismissing,
                canStartAtPosition: _ => true,
                getHorizontalDismissTravelDistance: () => toast.resolvedStyle.width > 0
                    ? toast.resolvedStyle.width + 32f
                    : 400f,
                getVerticalDismissTravelDistance: () => toast.resolvedStyle.height > 0
                    ? toast.resolvedStyle.height + 16f
                    : 80f,
                getOffsetTarget: () => toast,
                onInteractionStarted: null,
                onInteractionAborted: null,
                onTapped: () => OnToastTapped(toastData),
                onDismissed: () => OnToastDismissedBySwipe(toastData)
            );

            toast.AddManipulator(manipulator);
            toastData.Manipulator = manipulator;
            activeToasts.Add(toastData);
        }

        private static void AnimateToastEntrance(VisualElement toast)
        {
            var propNames = new List<StylePropertyName>
            {
                new("opacity"),
                new("translate"),
            };

            var durations = new List<TimeValue>
            {
                new(300, TimeUnit.Millisecond),
                new(300, TimeUnit.Millisecond),
            };

            var easings = new List<EasingFunction>
            {
                new(EasingMode.EaseOut),
                new(EasingMode.EaseOut),
            };

            toast.style.transitionProperty = new StyleList<StylePropertyName>(propNames);
            toast.style.transitionDuration = new StyleList<TimeValue>(durations);
            toast.style.transitionTimingFunction = new StyleList<EasingFunction>(easings);

            toast.style.opacity = 1f;
            toast.style.translate = new Translate(0, 0, 0);
        }

        /// <summary>
        /// Arm the timer that fades the toast away after the default time.
        /// </summary>
        private void ScheduleAutoDismiss(ToastData toastData)
        {
            toastData.AutoDismissTimer = toastData.Root.schedule
                .Execute(() => AutoDismiss(toastData))
                .StartingIn((long)(AutoDismissSeconds * 1000));
        }

        private void AutoDismiss(ToastData toastData)
        {
            if (toastData.IsDismissing) return;

            OnToastTapped(toastData);
        }

        /// <summary>
        /// Stop a pending auto-dismiss timer so it never fires after removal.
        /// </summary>
        private static void CancelAutoDismiss(ToastData toastData)
        {
            toastData.AutoDismissTimer?.Pause();
            toastData.AutoDismissTimer = null;
        }

        private void OnToastTapped(ToastData toastData)
        {
            if (toastData.IsDismissing) return;
            toastData.IsDismissing = true;

            CancelAutoDismiss(toastData);
            StartCoroutine(FadeOutAndRemoveToast(toastData));
        }

        private IEnumerator FadeOutAndRemoveToast(ToastData toastData)
        {
            var toast = toastData.Root;

            toast.style.transitionProperty = new StyleList<StylePropertyName>(
                new List<StylePropertyName> { new("opacity") });
            toast.style.transitionDuration = new StyleList<TimeValue>(
                new List<TimeValue> { new(FadeDurationMs, TimeUnit.Millisecond) });
            toast.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new List<EasingFunction> { new(EasingMode.EaseOut) });
            toast.style.opacity = 0f;

            yield return new WaitForSeconds(FadeDurationMs / 1000f + 0.02f);

            RemoveToastFromHierarchy(toastData);
        }

        private void OnToastDismissedBySwipe(ToastData toastData)
        {
            if (toastData.IsDismissing) return;
            toastData.IsDismissing = true;

            CancelAutoDismiss(toastData);

            var toast = toastData.Root;
            var anim = toast.experimental.animation.Start(1f, 0f, FadeDurationMs, (_, v) =>
            {
                toast.style.opacity = v;
            });
            anim.KeepAlive();
            anim.onAnimationCompleted += () =>
            {
                RemoveToastFromHierarchy(toastData);
            };
        }

        private void RemoveToastFromHierarchy(ToastData toastData)
        {
            activeToasts.Remove(toastData);

            if (toastData.Root.parent != null)
            {
                toastData.Root.RemoveFromHierarchy();
            }
        }

        /// <summary>
        /// Remove the oldest toast immediately (when cap is exceeded).
        /// </summary>
        private void RemoveOldestToast()
        {
            if (activeToasts.Count == 0) return;

            var oldest = activeToasts[0];
            oldest.IsDismissing = true;
            CancelAutoDismiss(oldest);
            oldest.Manipulator?.ResetState();
            RemoveToastFromHierarchy(oldest);
        }
    }
}
