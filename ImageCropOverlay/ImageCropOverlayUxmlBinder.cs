using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    public class ImageCropOverlayUxmlBinder : MonoBehaviour
    {
        private UIDocument uiDocument;
        private CircularImageButton imageButton;
        private VisualElement savedPreview;
        private Label statusLabel;
        private PillButton resetButton;

        private Texture2D defaultTexture;
        private Texture2D croppedTexture;

        private Texture2D ActiveTexture => croppedTexture != null ? croppedTexture : defaultTexture;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ImageCropOverlayUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            defaultTexture = CreateDefaultPortraitTexture(512);

            VisualElement root = uiDocument.rootVisualElement;
            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            imageButton = root.Q<CircularImageButton>("image-button");
            savedPreview = root.Q<VisualElement>("preview-image");
            statusLabel = root.Q<Label>("status-label");
            resetButton = root.Q<PillButton>("reset-button");
            PillButton editButton = root.Q<PillButton>("edit-button");

            imageButton.Clicked += OpenCropOverlay;
            if (editButton != null)
            {
                editButton.SetTextColor(Color.white);
                editButton.Clicked += OpenCropOverlay;
            }

            if (resetButton != null)
            {
                resetButton.SetTextColor(Color.white);
                resetButton.Clicked += ResetImage;
            }

            RefreshPreview("Default image loaded. Tap Edit Image to crop and save.");
        }

        private void OnDestroy()
        {
            DestroyRuntimeTexture(croppedTexture);
            DestroyRuntimeTexture(defaultTexture);
        }

        private void OpenCropOverlay()
        {
            if (ActiveTexture == null)
            {
                statusLabel.text = "No source image is available for cropping.";
                return;
            }

            var configuration = new ImageCropOverlayControl.Configuration
            {
                Title = "Move and Scale",
                CancelLabel = "Cancel",
                SaveLabel = "Save",
                ExportSize = 512,
                ScreenMarginPx = 48f,
                MaxViewportWidthRatio = 0.9f,
                CornerRadiusPercent = ImageCropOverlayControl.CircleCornerRadiusPercent,
            };

            bool overlayShown = ImageCropOverlayControl.Show(
                imageButton ?? uiDocument.rootVisualElement,
                ActiveTexture,
                configuration,
                OnCropConfirmed,
                () => RefreshPreview("Crop cancelled. The current preview was left unchanged.")) != null;

            if (overlayShown)
            {
                statusLabel.text = "Drag to move the image. Pinch or use the mouse wheel to zoom, then press Save.";
            }
        }

        private void OnCropConfirmed(Texture2D newTexture)
        {
            ReplaceCroppedTexture(newTexture);
            RefreshPreview($"Crop saved at {newTexture.width}x{newTexture.height}. The preview now reflects the exported texture.");
        }

        private void ResetImage()
        {
            ReplaceCroppedTexture(null);
            RefreshPreview("Reset to the original generated portrait.");
        }

        private void RefreshPreview(string statusMessage)
        {
            Texture2D previewTexture = ActiveTexture;
            if (previewTexture != null)
            {
                imageButton.SetImage(previewTexture);
                savedPreview.style.backgroundImage = new StyleBackground(previewTexture);
            }
            else
            {
                imageButton.ClearImage();
                savedPreview.style.backgroundImage = StyleKeyword.Null;
            }

            bool canReset = croppedTexture != null;
            resetButton.SetEnabled(canReset);
            resetButton.style.opacity = canReset ? 1f : 0.42f;
            statusLabel.text = statusMessage;
        }

        private void ReplaceCroppedTexture(Texture2D newTexture)
        {
            if (croppedTexture != null && croppedTexture != newTexture)
            {
                DestroyRuntimeTexture(croppedTexture);
            }

            croppedTexture = newTexture;
        }

        private static void DestroyRuntimeTexture(Texture2D texture)
        {
            if (texture != null)
            {
                Destroy(texture);
            }
        }

        private static Texture2D CreateDefaultPortraitTexture(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
            };

            var pixels = new Color[size * size];
            Vector2 center = new(size * 0.5f, size * 0.5f);
            float backgroundRadius = size * 0.49f;
            Vector2 torsoCenter = new(size * 0.5f, size * 0.72f);
            Vector2 torsoSize = new(size * 0.24f, size * 0.18f);
            Vector2 headCenter = new(size * 0.5f, size * 0.38f);
            float headRadius = size * 0.12f;
            Vector2 hairCenter = new(size * 0.5f, size * 0.34f);
            Vector2 hairSize = new(size * 0.16f, size * 0.11f);

            Color backgroundTop = new(0.31f, 0.55f, 0.95f, 1f);
            Color backgroundBottom = new(0.12f, 0.78f, 0.72f, 1f);
            Color torsoColor = new(0.13f, 0.21f, 0.37f, 1f);
            Color skinColor = new(0.97f, 0.81f, 0.67f, 1f);
            Color hairColor = new(0.17f, 0.12f, 0.1f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int pixelIndex = (y * size) + x;
                    Vector2 currentPoint = new(x, y);
                    float distanceFromCenter = Vector2.Distance(currentPoint, center);
                    if (distanceFromCenter > backgroundRadius)
                    {
                        pixels[pixelIndex] = Color.clear;
                        continue;
                    }

                    float verticalLerp = Mathf.Clamp01((float)y / size);
                    Color pixelColor = Color.Lerp(backgroundTop, backgroundBottom, verticalLerp);

                    if (IsInsideEllipse(currentPoint, torsoCenter, torsoSize))
                    {
                        pixelColor = torsoColor;
                    }

                    if (Vector2.Distance(currentPoint, headCenter) <= headRadius)
                    {
                        pixelColor = skinColor;
                    }

                    if (IsInsideEllipse(currentPoint, hairCenter, hairSize) && currentPoint.y >= size * 0.26f)
                    {
                        pixelColor = hairColor;
                    }

                    pixels[pixelIndex] = pixelColor;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
            return texture;
        }

        private static bool IsInsideEllipse(Vector2 point, Vector2 ellipseCenter, Vector2 ellipseRadii)
        {
            if (ellipseRadii.x <= 0f || ellipseRadii.y <= 0f)
            {
                return false;
            }

            float normalizedX = (point.x - ellipseCenter.x) / ellipseRadii.x;
            float normalizedY = (point.y - ellipseCenter.y) / ellipseRadii.y;
            return (normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1f;
        }
    }
}
