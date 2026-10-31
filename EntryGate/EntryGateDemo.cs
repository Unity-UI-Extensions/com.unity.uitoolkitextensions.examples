// Examples~/EntryGate/EntryGateDemo.cs
// Demonstrates: EntryGate (with StatusList in its content slot)
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - The tabs at the bottom switch between four launch screens built from one gate:
//        a safety notice, a permission failure, an experience entry and a could-not-load.
//      - On the entry screen the loading steps advance and Start enables when they finish.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// EntryGate demo: message and hero layouts, navy and deep backdrops, card and plain panels.
    /// </summary>
    public class EntryGateDemo : MonoBehaviour
    {
        private VisualElement root;
        private EntryGate gate;
        private Label log;
        private IVisualElementScheduledItem loading;
        private Background icon;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("EntryGateDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            root = uiDocument.rootVisualElement;
            root.Clear();
            icon = Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(48, Color.white));

            var tabs = new SegmentedControl(new[] { "Notice", "Denied", "Entry", "Offline" });
            tabs.AddToClassList("entryGateDemo__tabs");
            tabs.SelectionChanged += (index, _) => Show(index);

            log = new Label();
            log.AddToClassList("entryGateDemo__log");

            Show(0);
            root.Add(log);
            root.Add(tabs);
        }

        private void Show(int screen)
        {
            loading?.Pause();
            gate?.RemoveFromHierarchy();
            gate = new EntryGate { TopInset = 24f, BottomInset = 60f };
            gate.SetLogo(icon);
            gate.Started += () => Write("Started");
            gate.SecondaryClicked += () => Write("SecondaryClicked");
            gate.LinkClicked += () => Write("LinkClicked");
            gate.Skipped += () => Write("Skipped");
            gate.ConsentLinkClicked += id => Write($"Consent: {id}");

            switch (screen)
            {
                case 0:
                    gate.IconTone = EntryGateIconTone.Warn;
                    gate.SetIcon(icon);
                    gate.TitleText = "Look up from the screen";
                    gate.BodyText = "Augmented reality places things in the real world. Stay aware of people, steps, traffic and edges.\n\nStop and look around whenever something is in your way.";
                    gate.FooterNote = "AR SAFETY NOTICE";
                    gate.FooterEndText = "2 OF 2";
                    break;
                case 1:
                    gate.PanelCard = false;
                    gate.IconTone = EntryGateIconTone.WarnOutline;
                    gate.SetIcon(icon);
                    gate.TitleText = "The camera is off for this app";
                    gate.BodyText = "This experience places content in the room around you, so it needs the camera. Nothing is recorded or sent without you pressing the shutter.";
                    gate.StartText = "Allow the camera";
                    gate.SecondaryText = "Open settings";
                    gate.LinkText = "Quit for now";
                    gate.SetConsentLinks(new Dictionary<string, string> { ["privacy"] = "Privacy notice" });
                    break;
                case 2:
                    gate.Layout = EntryGateLayout.Hero;
                    gate.Backdrop = EntryGateBackdrop.Deep;
                    gate.PanelCard = false;
                    gate.Eyebrow = "OPENING EXPERIENCE";
                    gate.TitleText = "Sample Museum Tour";
                    gate.Subtitle = "12 stops · about 25 min";
                    gate.StartText = "Start the tour";
                    gate.StartEnabled = false;
                    gate.CancelText = "Cancel";
                    gate.FooterEndText = "STEP 2 OF 2";
                    AddLoadingSteps();
                    break;
                default:
                    gate.Backdrop = EntryGateBackdrop.Deep;
                    gate.IconTone = EntryGateIconTone.Warn;
                    gate.SetIcon(icon);
                    gate.TitleText = "Could not load Sample Museum Tour";
                    gate.BodyText = "The phone is offline and only part of this tour is saved. You can start with what is here, or try again once you are connected.";
                    gate.StartText = "Retry";
                    gate.SecondaryText = "Use the saved copy (7 of 12 stops)";
                    gate.LinkText = "Go to the home screen";
                    break;
            }

            root.Insert(0, gate);
        }

        private void AddLoadingSteps()
        {
            var steps = new StatusList();
            steps.AddToClassList("entryGateDemo__steps");
            steps.SetSteps(new List<StatusStep>
            {
                new("content", "Content loading"),
                new("position", "Finding your position"),
                new("ready", "Ready to start"),
            });
            steps.AllDone += () => gate.StartEnabled = true;
            gate.Add(steps);

            var ids = new[] { "content", "position", "ready" };
            var tick = 0;
            loading = steps.schedule.Execute(() =>
            {
                var index = tick / 2;
                if (index >= ids.Length)
                {
                    loading.Pause();
                    return;
                }

                steps.SetState(ids[index], tick % 2 == 0 ? StepState.Active : StepState.Done);
                tick++;
            }).Every(700);
        }

        private void Write(string message)
        {
            log.text = message;
        }
    }
}
