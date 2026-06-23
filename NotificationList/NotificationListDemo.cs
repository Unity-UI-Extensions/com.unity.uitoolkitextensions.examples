// Examples~/NotificationList/NotificationListDemo.cs
// Demonstrates: ElasticListView, NotificationBadge
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the same
//   GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A header row shows a title and an unread NotificationBadge (top right).
//      - The ElasticListView below holds the notifications. It bounces elastically and,
//        on device, a swipe-up past the bottom edge loads 3 more (the control notes that
//        swipe input is unreliable in the editor — use the Add button there).
//      - "Add Notification" appends a randomly generated notification and bumps the badge.
//      - Tap the badge to mark everything read (the count returns to zero and the badge hides).

using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Notification feed demo combining <see cref="ElasticListView"/> and <see cref="NotificationBadge"/>.
    ///
    /// The badge tracks the unread count: "Add Notification" appends one item and increments it,
    /// while tapping the badge clears it. Swipe-up overscroll raises
    /// <see cref="ElasticListView.LoadMoreRequested"/>, which the demo answers by simulating a short
    /// fetch (via <c>schedule</c>) and appending three more items between
    /// <see cref="ElasticListView.BeginLoadMore"/> and <see cref="ElasticListView.EndLoadMore"/>.
    /// </summary>
    public class NotificationListDemo : MonoBehaviour
    {
        private const int SeedCount = 4;
        private const int LoadMoreBatch = 3;
        private const long SimulatedFetchMs = 700;

        private static readonly string[] MessageTemplates =
        {
            "Alex sent you a new message.",
            "Your order has shipped and is on its way.",
            "Jordan started following you.",
            "Your weekly summary is ready to view.",
            "You hit your daily step goal of 10,000.",
            "A new comment was added to your post.",
            "Backup completed successfully.",
            "Your subscription renews in 3 days.",
            "Taylor mentioned you in a thread.",
            "New login from an unrecognised device.",
            "Payment received — thank you!",
            "Reminder: stand-up starts in 15 minutes.",
        };

        private static readonly Color[] AccentColors =
        {
            new(0.29f, 0.78f, 0.65f, 1f),
            new(0.51f, 0.68f, 1.00f, 1f),
            new(0.78f, 0.55f, 1.00f, 1f),
            new(1.00f, 0.72f, 0.42f, 1f),
            new(1.00f, 0.54f, 0.54f, 1f),
        };

        private ElasticListView listView;
        private NotificationBadge badge;
        private int unreadCount;
        private int accentIndex;
        private bool loadingMore;

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("NotificationListDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "notificationListDemo__screen");

            VisualElement cardWrap = UIToolkitExtensions.CreateVisualElement(screen, "notificationListDemo__cardWrap");
            VisualElement card = UIToolkitExtensions.CreateVisualElement(cardWrap, "notificationListDemo__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "notificationListDemo__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label title = UIToolkitExtensions.CreateVisualElement<Label>(card, "notificationListDemo__title");
            title.text = "Notifications";

            Label instruction = UIToolkitExtensions.CreateVisualElement<Label>(card, "notificationListDemo__instruction");
            instruction.text = "Add notifications to grow the list and bump the unread badge. Swipe up past the " +
                "bottom to load more (best on device). Tap the badge to mark all read.";

            listView = UIToolkitExtensions.CreateVisualElement<ElasticListView>(card, "notificationListDemo__list");
            listView.EmptyStateText = "You're all caught up.";
            listView.EnableTouchElasticity(0.12f);
            listView.EnableLoadMore();
            listView.LoadMoreRequested += OnLoadMoreRequested;

            for (int i = 0; i < SeedCount; i++)
            {
                listView.AddItem(CreateNotificationItem(RandomMessage()));
            }

            VisualElement bottomBar = UIToolkitExtensions.CreateVisualElement(card, "notificationListDemo__bottomBar");

            PillButton addButton = UIToolkitExtensions.CreateVisualElement<PillButton>(bottomBar, "notificationListDemo__addButton");
            addButton.Text = "Add Notification";
            addButton.SetInnerColor("#4A90E2");
            addButton.SetOuterColor("#7B68EE");
            addButton.SetTextColor(Color.white);
            addButton.SetFontSize(15f);
            addButton.Clicked += OnAddClicked;

            badge = UIToolkitExtensions.CreateVisualElement<NotificationBadge>(cardWrap, "notificationListDemo__badge");
            badge.RegisterCallback<ClickEvent>(_ => MarkAllRead());
        }

        private void OnAddClicked()
        {
            listView.AddItem(CreateNotificationItem(RandomMessage()));
            unreadCount++;
            badge.SetCount(unreadCount);
        }

        private void OnLoadMoreRequested()
        {
            if (loadingMore)
            {
                return;
            }

            loadingMore = true;
            listView.BeginLoadMore();

            listView.schedule.Execute(() =>
            {
                for (int i = 0; i < LoadMoreBatch; i++)
                {
                    listView.AddItem(CreateNotificationItem(RandomMessage()));
                }

                listView.EndLoadMore();
                loadingMore = false;
            }).StartingIn(SimulatedFetchMs);
        }

        private void MarkAllRead()
        {
            unreadCount = 0;
            badge.SetCount(0);
        }

        private VisualElement CreateNotificationItem(string message)
        {
            VisualElement item = UIToolkitExtensions.CreateVisualElement("notificationListDemo__item");

            VisualElement accent = UIToolkitExtensions.CreateVisualElement(item, "notificationListDemo__itemAccent");
            accent.style.backgroundColor = AccentColors[accentIndex % AccentColors.Length];
            accentIndex++;

            VisualElement textBlock = UIToolkitExtensions.CreateVisualElement(item, "notificationListDemo__itemText");

            Label messageLabel = UIToolkitExtensions.CreateVisualElement<Label>(textBlock, "notificationListDemo__itemMessage");
            messageLabel.text = message;

            Label metaLabel = UIToolkitExtensions.CreateVisualElement<Label>(textBlock, "notificationListDemo__itemMeta");
            metaLabel.text = "Just now";

            return item;
        }

        private static string RandomMessage()
        {
            return MessageTemplates[Random.Range(0, MessageTemplates.Length)];
        }
    }
}
