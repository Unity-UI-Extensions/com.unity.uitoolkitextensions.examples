// Examples~/ActionMenu/ActionMenuUxmlBinder.cs
// UXML counterpart of ActionMenuDemo.cs.
//
// The layout lives in ActionMenuUxmlView.uxml (loaded by the UIDocument on this
// GameObject). This binder only queries the cloned tree and wires the interactive
// behaviour — the "layout in UXML, behaviour in C#" division of labour.
// DropDownMenuControl is purely programmatic, so the binder constructs it here and
// opens it from the ··· triggers and the card actions button declared in the UXML.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Behaviour binder for the UXML-authored ActionMenu demo.
    /// </summary>
    public class ActionMenuUxmlBinder : MonoBehaviour
    {
        // Constructed in Start — VisualElements must not be created from a MonoBehaviour
        // constructor or field initializer.
        private DropDownMenuControl actionMenu;

        private UIDocument uiDocument;
        private VisualElement itemList;
        private PillButton cardActionsButton;
        private Label statusLabel;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ActionMenuUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            actionMenu = new DropDownMenuControl();

            VisualElement root = uiDocument.rootVisualElement;

            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            itemList = root.Q<VisualElement>("item-list");
            cardActionsButton = root.Q<PillButton>("card-actions-button");
            statusLabel = root.Q<Label>("status-label");

            if (itemList == null || cardActionsButton == null || statusLabel == null)
            {
                Debug.LogWarning("ActionMenuUxmlBinder: one or more named elements were not found in the UXML.", this);
                return;
            }

            foreach (VisualElement row in itemList.Query<VisualElement>(className: "actionMenuExample__itemRow").ToList())
            {
                WireItemRow(row);
            }

            cardActionsButton.SetTextColor(Color.white);
            cardActionsButton.Clicked += OpenCardMenu;
        }

        private void WireItemRow(VisualElement row)
        {
            Label itemLabel = row.Q<Label>(className: "actionMenuExample__itemLabel");
            Label menuButton = row.Q<Label>(className: "actionMenuExample__itemMenuButton");
            if (itemLabel == null || menuButton == null)
            {
                return;
            }

            string itemName = itemLabel.text;
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
