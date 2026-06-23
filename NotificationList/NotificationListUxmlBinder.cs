using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    public class NotificationListUxmlBinder : MonoBehaviour
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
                Debug.LogError("NotificationListUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;

            badge = root.Q<NotificationBadge>("badge");
            badge.RegisterCallback<ClickEvent>(_ => MarkAllRead());

            listView = root.Q<ElasticListView>("list");
            listView.EnableTouchElasticity(0.12f);
            listView.EnableLoadMore();
            listView.LoadMoreRequested += OnLoadMoreRequested;

            for (int i = 0; i < SeedCount; i++)
            {
                listView.AddItem(CreateNotificationItem(RandomMessage()));
            }

            PillButton addButton = root.Q<PillButton>("add-button");
            addButton.SetTextColor(Color.white);
            addButton.Clicked += OnAddClicked;
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
            var item = new VisualElement();
            item.AddToClassList("notificationListDemo__item");

            var accent = new VisualElement();
            accent.AddToClassList("notificationListDemo__itemAccent");
            accent.style.backgroundColor = AccentColors[accentIndex % AccentColors.Length];
            accentIndex++;
            item.Add(accent);

            var textBlock = new VisualElement();
            textBlock.AddToClassList("notificationListDemo__itemText");
            item.Add(textBlock);

            var messageLabel = new Label(message);
            messageLabel.AddToClassList("notificationListDemo__itemMessage");
            textBlock.Add(messageLabel);

            var metaLabel = new Label("Just now");
            metaLabel.AddToClassList("notificationListDemo__itemMeta");
            textBlock.Add(metaLabel);

            return item;
        }

        private static string RandomMessage()
        {
            return MessageTemplates[Random.Range(0, MessageTemplates.Length)];
        }
    }
}
