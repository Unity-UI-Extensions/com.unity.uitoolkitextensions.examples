// Examples~/WorldAnchoredLabel/WorldAnchoredLabelDemo.cs
// Demonstrates: WorldAnchoredLabel
//
// Scene setup:
//   A scene for this example is provided. The demo uses the scene's main camera and
//   creates three cubes, so no manual assignment is needed.
//      - Each cube has a label that follows it through the camera; one shows its distance.
//      - The camera turns slowly, so labels leave the screen: one clamps to the safe area,
//        one hides.
//      - Tap a label to write the event below.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// WorldAnchoredLabel demo: tracking through a camera, tones, distance, clamping and hiding.
    /// </summary>
    public class WorldAnchoredLabelDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private Transform cameraTransform;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("WorldAnchoredLabelDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            page = new ComponentSamplePage(root, "World Anchored Label",
                "Labels that follow points in the world. The camera turns slowly so you can see clamping and hiding.");
            page.Finish();
            page.Screen.style.backgroundColor = Color.clear;

            var camera = Camera.main != null ? Camera.main : new GameObject("Demo Camera").AddComponent<Camera>();
            camera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.29f, 0.35f, 0.4f);
            cameraTransform = camera.transform;

            var layer = new VisualElement { pickingMode = PickingMode.Ignore };
            layer.style.position = Position.Absolute;
            layer.style.left = 0;
            layer.style.top = 0;
            layer.style.right = 0;
            layer.style.bottom = 0;
            root.Add(layer);

            AddLabel(layer, camera, new Vector3(0f, 0f, 4f), "Turbine cutaway", AnchoredLabelTone.Default, showDistance: true, clamp: true);
            AddLabel(layer, camera, new Vector3(-1.5f, -0.5f, 6f), "Tap to place", AnchoredLabelTone.Action, showDistance: false, clamp: true);
            AddLabel(layer, camera, new Vector3(2f, 0.5f, 5f), "Placed", AnchoredLabelTone.Success, showDistance: false, clamp: false);
        }

        private void AddLabel(VisualElement layer, Camera camera, Vector3 position, string text, AnchoredLabelTone tone, bool showDistance, bool clamp)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.transform.localScale = Vector3.one * 0.4f;

            var label = new WorldAnchoredLabel { Text = text, Tone = tone, ShowDistance = showDistance, ClampToSafeArea = clamp };
            label.SetSafeArea(0f, 48f, 0f, 34f);
            label.Tapped += () => page.Write($"Tapped: {text}");
            layer.Add(label);
            label.Track(camera, cube.transform, Vector3.up * 0.3f);
        }

        private void Update()
        {
            if (cameraTransform != null)
            {
                cameraTransform.rotation = Quaternion.Euler(0f, Mathf.Sin(Time.time * 0.4f) * 50f, 0f);
            }
        }
    }
}
