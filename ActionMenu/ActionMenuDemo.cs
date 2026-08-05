// Examples~/ActionMenu/ActionMenuDemo.cs
// Demonstrates: DropDownMenuControl, PillButton
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the same
//   GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Three content rows each expose a "···" overflow button; tapping it opens a
//        DropDownMenuControl anchored to the button (AnchorRight) with View / Edit /
//        Remove options. Remove really removes the row.
//      - The "Card actions" PillButton opens the same menu centered on itself
//        (CenteredOnAnchor) with card-level actions.
//      - The status line reports the chosen action, or notes when a menu is
//        dismissed by tapping the backdrop.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Demonstrates <see cref="DropDownMenuControl"/> opened from per-row overflow
    /// buttons and from a centered card-level trigger.
    /// </summary>
    public class ActionMenuDemo : MonoBehaviour
    {
        private static readonly string[] ItemNames =
        {
            "Quarterly report.pdf",
            "Team photo.png",
            "Product roadmap.fig",
        };

        private const string IdleStatusText = "Open an item's ··· menu, or the card actions menu.";

        private UIDocument uiDocument;
        // Constructed in Start — VisualElements must not be created from a MonoBehaviour
        // constructor or field initializer.
        private DropDownMenuControl actionMenu;
        private VisualElement itemList;
        private PillButton cardActionsButton;
        private Label statusLabel;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ActionMenuDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            actionMenu = new DropDownMenuControl();

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "actionMenuExample__screen");
            VisualElement card = UIToolkitExtensions.CreateVisualElement(screen, "actionMenuExample__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "actionMenuExample__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label title = UIToolkitExtensions.CreateVisualElement<Label>(card, "actionMenuExample__title");
            title.text = "ACTION MENU";

            Label description = UIToolkitExtensions.CreateVisualElement<Label>(card, "actionMenuExample__description");
            description.text = "Each item hides its actions behind a ··· overflow menu. Choosing an option runs that option's callback; tapping the backdrop dismisses the menu without acting.";

            itemList = UIToolkitExtensions.CreateVisualElement(card, "actionMenuExample__itemList");
            foreach (string itemName in ItemNames)
            {
                AddItemRow(itemName);
            }

            cardActionsButton = UIToolkitExtensions.CreateVisualElement<PillButton>(card, "actionMenuExample__centeredButton");
            cardActionsButton.Text = "Card actions";
            cardActionsButton.SetInnerColor("#4A90E2");
            cardActionsButton.SetOuterColor("#7B68EE");
            cardActionsButton.SetTextColor(Color.white);
            cardActionsButton.Clicked += OpenCardMenu;

            statusLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "actionMenuExample__status");
            statusLabel.text = IdleStatusText;
        }

        private void AddItemRow(string itemName)
        {
            VisualElement row = UIToolkitExtensions.CreateVisualElement(itemList, "actionMenuExample__itemRow");

            Label itemLabel = UIToolkitExtensions.CreateVisualElement<Label>(row, "actionMenuExample__itemLabel");
            itemLabel.text = itemName;

            Label menuButton = UIToolkitExtensions.CreateVisualElement<Label>(row, "actionMenuExample__itemMenuButton");
            menuButton.text = "···";
            menuButton.RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopPropagation();
                OpenItemMenu(menuButton, row, itemName);
            });
        }

        private void OpenItemMenu(VisualElement anchor, VisualElement row, string itemName)
        {
            actionMenu.Open(anchor, new List<DropDownMenuControl.DropDownOption>
            {
                new DropDownMenuControl.DropDownOption("View", () => statusLabel.text = $"Viewing '{itemName}'."),
                new DropDownMenuControl.DropDownOption("Edit", () => statusLabel.text = $"Editing '{itemName}'."),
                new DropDownMenuControl.DropDownOption("Remove", () => RemoveItem(row, itemName)),
            },
            DropDownMenuControl.Placement.AnchorRight,
            onDismissed: () => statusLabel.text = "Menu dismissed — no action taken.");
        }

        private void OpenCardMenu()
        {
            actionMenu.Open(cardActionsButton, new List<DropDownMenuControl.DropDownOption>
            {
                new DropDownMenuControl.DropDownOption("Share card", () => statusLabel.text = "Sharing this card."),
                new DropDownMenuControl.DropDownOption("Duplicate card", () => statusLabel.text = "Card duplicated."),
                new DropDownMenuControl.DropDownOption("Archive card", () => statusLabel.text = "Card archived."),
            },
            DropDownMenuControl.Placement.CenteredOnAnchor,
            onDismissed: () => statusLabel.text = "Menu dismissed — no action taken.");
        }

        private void RemoveItem(VisualElement row, string itemName)
        {
            row.RemoveFromHierarchy();
            statusLabel.text = itemList.childCount == 0
                ? $"Removed '{itemName}'. All items removed — restart Play Mode to reset."
                : $"Removed '{itemName}'.";
        }
    }
}
