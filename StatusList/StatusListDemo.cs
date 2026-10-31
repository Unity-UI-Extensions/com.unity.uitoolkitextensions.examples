// Examples~/StatusList/StatusListDemo.cs
// Demonstrates: StatusList
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - "Run" walks through an entry sequence: each step goes active, then done.
//      - "Run with failure" fails the positioning step with a plain-language detail.
//      - AllDone is written below when every step completes.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// StatusList demo: an entry sequence with active, done and failed steps and the overall bar.
    /// </summary>
    public class StatusListDemo : MonoBehaviour
    {
        private static readonly string[] Ids = { "download", "locate", "place" };

        private ComponentSamplePage page;
        private StatusList list;
        private IVisualElementScheduledItem run;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("StatusListDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Status List",
                "Structured loading: one row per step and an overall bar.");
            var row = page.AddRow();
            page.AddButton(row, "Run", () => Run(fail: false));
            page.AddButton(row, "Run with failure", () => Run(fail: true), secondary: true);
            page.Finish();

            list = new StatusList();
            list.AllDone += () => page.Write("AllDone");
            list.StepChanged += (id, state) => page.Write($"{id}: {state}");
            page.Stage.Add(list);
            Reset();
        }

        private void Reset()
        {
            list.SetSteps(new List<StatusStep>
            {
                new("download", "Downloading the experience"),
                new("locate", "Finding where you are"),
                new("place", "Placing the content"),
            });
        }

        private void Run(bool fail)
        {
            run?.Pause();
            Reset();
            var tick = 0;
            run = list.schedule.Execute(() =>
            {
                var index = tick / 2;
                if (index >= Ids.Length)
                {
                    run.Pause();
                    return;
                }

                var id = Ids[index];
                if (tick % 2 == 0)
                {
                    list.SetState(id, StepState.Active, index == 0 ? "18 of 40 MB" : null);
                }
                else if (fail && id == "locate")
                {
                    list.SetState(id, StepState.Failed, "No match nearby. Move to the marked spot and try again.");
                    run.Pause();
                }
                else
                {
                    list.SetState(id, StepState.Done, string.Empty);
                }

                tick++;
            }).Every(700);
        }
    }
}
