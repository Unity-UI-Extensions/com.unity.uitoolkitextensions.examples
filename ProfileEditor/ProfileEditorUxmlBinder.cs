using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    public class ProfileEditorUxmlBinder : MonoBehaviour
    {
        private static readonly string[] PaletteHex =
        {
            "#E74C3C",
            "#3498DB",
            "#2ECC71",
            "#F39C12",
            "#9B59B6",
        };

        private CircularImageButton avatarButton;
        private GrayscaleImage grayscaleImage;
        private ToggleButton bwToggle;
        private VisualElement colorSwatch;
        private Label colorLabel;
        private Label statusLabel;

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ProfileEditorUxmlBinder requires a UIDocument on the same GameObject.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.style.backgroundColor = new Color(0.035f, 0.059f, 0.106f, 1f);

            avatarButton = root.Q<CircularImageButton>("avatar-button");
            grayscaleImage = root.Q<GrayscaleImage>("sample-image");
            bwToggle = root.Q<ToggleButton>("bw-toggle");
            ColorToggleGroup colorGroup = root.Q<ColorToggleGroup>("color-group");
            colorSwatch = root.Q<VisualElement>("color-swatch");
            colorLabel = root.Q<Label>("color-label");
            statusLabel = root.Q<Label>("status-label");

            avatarButton.SetUploadLabel("Tap to upload");
            avatarButton.SetImage(CreatePlaceholderAvatarTexture(96), isDefault: true);
            avatarButton.Clicked += OnProfilePhotoClicked;

            grayscaleImage.TextureProperty = CreateGradientBannerTexture(400, 140);
            grayscaleImage.scaleMode = ScaleMode.ScaleAndCrop;

            bwToggle.SetImage(CreateCheckmarkTexture(32));
            bwToggle.OnClicked += OnBWToggleClicked;

            Color[] paletteColors = new Color[PaletteHex.Length];
            for (int index = 0; index < PaletteHex.Length; index++)
            {
                ColorUtility.TryParseHtmlString(PaletteHex[index], out paletteColors[index]);
            }

            colorGroup.Alignment = FlexDirection.Row;
            colorGroup.Colors = paletteColors;
            colorGroup.OnColorSelected += OnColorSelected;

            statusLabel.text = "Tap the photo, toggle greyscale, or choose a theme color.";
        }

        private void OnProfilePhotoClicked()
        {
            statusLabel.text = "Photo tapped — would open picker.";
            Debug.Log("[ProfileEditorUxmlBinder] CircularImageButton tapped.");
        }

        private void OnBWToggleClicked()
        {
            grayscaleImage.GreyscaleEnabled = bwToggle.IsSelected;
            bwToggle.style.backgroundColor = bwToggle.IsSelected
                ? new Color(0.28f, 0.56f, 0.89f, 1f)
                : new Color(0.88f, 0.88f, 0.92f, 1f);
        }

        private void OnColorSelected(Color selectedColor)
        {
            colorSwatch.style.backgroundColor = selectedColor;
            colorLabel.text = $"#{ColorUtility.ToHtmlStringRGB(selectedColor)}";
            statusLabel.text = $"Theme color selected: #{ColorUtility.ToHtmlStringRGB(selectedColor)}";
        }

        private static Texture2D CreatePlaceholderAvatarTexture(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            var pixels = new Color[size * size];
            var centre = size / 2f;
            var radius = centre - 1f;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - centre;
                    var dy = y - centre;
                    var dist = Mathf.Sqrt(dx * dx + dy * dy);
                    pixels[y * size + x] = dist > radius
                        ? Color.clear
                        : Color.Lerp(new Color(0.28f, 0.56f, 0.89f, 1f), new Color(0.48f, 0.76f, 1f, 1f), (float)y / size);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateGradientBannerTexture(int width, int height)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            var pixels = new Color[width * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var hue = (float)x / width;
                    var sat = 0.75f;
                    var val = 0.75f + 0.15f * ((float)y / height);
                    pixels[y * width + x] = Color.HSVToRGB(hue, sat, val);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateCheckmarkTexture(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            var pixels = new Color[size * size];

            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.clear;
            }

            int margin = size / 4;
            DrawLine(pixels, size, margin, size / 2, size / 2, size - margin, Color.white, 2);
            DrawLine(pixels, size, size / 2, size - margin, size - margin, margin, Color.white, 2);

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static void DrawLine(Color[] pixels, int texSize, int x0, int y0, int x1, int y1, Color color, int thickness)
        {
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                for (int ty = -thickness; ty <= thickness; ty++)
                {
                    for (int tx = -thickness; tx <= thickness; tx++)
                    {
                        int px = x0 + tx;
                        int py = y0 + ty;
                        if (px >= 0 && px < texSize && py >= 0 && py < texSize)
                        {
                            pixels[py * texSize + px] = color;
                        }
                    }
                }

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }
    }
}
