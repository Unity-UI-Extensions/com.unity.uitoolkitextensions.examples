// Examples~/ProfileEditor/ProfileEditorDemo.cs
// Demonstrates: CircularImageButton, GrayscaleImage, ToggleButton, ColorToggleGroup, ColorToggleButton
//
// Scene setup (already configured in the provided ProfileEditorDemo.unity scene):
//   - A single GameObject holds a UIDocument (using the shared example Panel Settings:
//     Scale With Screen Size, 1080x1920) and this component.
//   - The demo builds its UI entirely in C# via GetComponent<UIDocument>().
//   - Press Play.

using UnityEngine;
using UnityEngine.UIElements;
using UnityUIToolkit.Extensions;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Profile editor demo combining avatar display, image processing, and theme color
    /// selection, mirroring <c>Documentation~/Examples/ProfileEditor.md</c>.
    ///
    /// Controls featured:
    ///   <see cref="CircularImageButton"/> — the profile avatar; tapping it logs a message
    ///   (a real app would open an image picker).
    ///   <see cref="GrayscaleImage"/> — a sample gradient image that switches between full
    ///   colour and black-and-white.
    ///   <see cref="ToggleButton"/> — the "B&amp;W" two-state toggle that drives the grayscale switch.
    ///   <see cref="ColorToggleGroup"/> / <see cref="ColorToggleButton"/> — a five-colour,
    ///   single-selection theme picker that updates a swatch and a colour-name label.
    ///
    /// The package ships no grayscale shader/material, so the grayscale effect is demonstrated
    /// by swapping the <see cref="GrayscaleImage.TextureProperty"/> between a colour texture and a
    /// runtime-desaturated copy (the <see cref="GrayscaleImage.GreyscaleEnabled"/> flag is also set,
    /// which takes effect automatically if a grayscale material is assigned later).
    /// </summary>
    public class ProfileEditorDemo : MonoBehaviour
    {
        private readonly struct ThemeColor
        {
            public readonly string Name;
            public readonly Color Value;

            public ThemeColor(string name, Color value)
            {
                Name = name;
                Value = value;
            }
        }

        private static readonly ThemeColor[] ThemeColors =
        {
            new("Coral", new Color(0.90f, 0.45f, 0.45f)),
            new("Amber", new Color(1.00f, 0.72f, 0.30f)),
            new("Lemon", new Color(0.98f, 0.86f, 0.36f)),
            new("Mint", new Color(0.51f, 0.78f, 0.52f)),
            new("Sky", new Color(0.39f, 0.71f, 0.96f)),
        };

        private UIDocument uiDocument;
        private CircularImageButton avatarButton;
        private GrayscaleImage sampleImage;
        private ToggleButton blackWhiteToggle;
        private ColorToggleGroup colorGroup;
        private VisualElement swatch;
        private Label colorNameLabel;
        private Label statusLabel;

        private Texture2D avatarTexture;
        private Texture2D colorSampleTexture;
        private Texture2D grayscaleSampleTexture;
        private Texture2D circleTexture;

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("ProfileEditorDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            CreateTextures();

            VisualElement root = uiDocument.rootVisualElement;
            root.Clear();
            BuildUI(root);
        }

        private void OnDestroy()
        {
            DestroyTexture(avatarTexture);
            DestroyTexture(colorSampleTexture);
            DestroyTexture(grayscaleSampleTexture);
            DestroyTexture(circleTexture);
        }

        private void CreateTextures()
        {
            avatarTexture = ProceduralTextureUtility.CreateHorizontalGradient(
                new Color(0.45f, 0.55f, 0.95f), new Color(0.12f, 0.78f, 0.72f), 256);
            colorSampleTexture = ProceduralTextureUtility.CreateHorizontalGradient(
                new Color(0.95f, 0.45f, 0.55f), new Color(0.35f, 0.45f, 0.95f), 256);
            grayscaleSampleTexture = CreateDesaturatedCopy(colorSampleTexture);
            circleTexture = ProceduralTextureUtility.CreateSolidCircle(64, Color.white);
        }

        private void BuildUI(VisualElement root)
        {
            VisualElement screen = UIToolkitExtensions.CreateVisualElement(root, "profileEditor__screen");
            VisualElement card = UIToolkitExtensions.CreateVisualElement(screen, "profileEditor__card");

            Label eyebrow = UIToolkitExtensions.CreateVisualElement<Label>(card, "profileEditor__eyebrow");
            eyebrow.text = "Toolkit Sample";

            Label title = UIToolkitExtensions.CreateVisualElement<Label>(card, "profileEditor__title");
            title.text = "PROFILE EDITOR";

            Label description = UIToolkitExtensions.CreateVisualElement<Label>(card, "profileEditor__description");
            description.text = "Avatar display, image processing, and theme color selection working together in one screen.";

            VisualElement avatarRow = UIToolkitExtensions.CreateVisualElement(card, "profileEditor__avatarRow");

            avatarButton = UIToolkitExtensions.CreateVisualElement<CircularImageButton>(avatarRow, "profileEditor__avatar");
            avatarButton.SetImage(avatarTexture);
            avatarButton.Clicked += OnAvatarClicked;

            Label avatarHelp = UIToolkitExtensions.CreateVisualElement<Label>(avatarRow, "profileEditor__avatarHelp");
            avatarHelp.text = "Tap the avatar to change your profile picture. In this demo it logs to the Console.";

            Label imageLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "profileEditor__sectionLabel");
            imageLabel.text = "Sample Image";

            VisualElement imageRow = UIToolkitExtensions.CreateVisualElement(card, "profileEditor__imageRow");

            sampleImage = UIToolkitExtensions.CreateVisualElement<GrayscaleImage>(imageRow, "profileEditor__sampleImage");
            sampleImage.scaleMode = ScaleMode.ScaleAndCrop;
            sampleImage.GreyscaleEnabled = false;
            sampleImage.TextureProperty = colorSampleTexture;

            VisualElement bwBlock = UIToolkitExtensions.CreateVisualElement(imageRow, "profileEditor__bwBlock");

            Label bwLabel = UIToolkitExtensions.CreateVisualElement<Label>(bwBlock, "profileEditor__bwLabel");
            bwLabel.text = "B&W";

            blackWhiteToggle = UIToolkitExtensions.CreateVisualElement<ToggleButton>(bwBlock, "profileEditor__bwToggle");
            blackWhiteToggle.OnClicked += OnBlackWhiteToggled;

            Label colorLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "profileEditor__sectionLabel");
            colorLabel.text = "Theme Color";

            colorGroup = UIToolkitExtensions.CreateVisualElement<ColorToggleGroup>(card, "profileEditor__colorGroup");
            colorGroup.Alignment = FlexDirection.Row;

            var colorValues = new Color[ThemeColors.Length];
            for (int index = 0; index < ThemeColors.Length; index++)
            {
                colorValues[index] = ThemeColors[index].Value;
            }
            colorGroup.Colors = colorValues;

            colorGroup.Query(className: "toggleButton__icon")
                .ForEach(icon => icon.style.backgroundImage = new StyleBackground(circleTexture));

            colorGroup.OnColorSelected += OnThemeColorSelected;

            VisualElement swatchRow = UIToolkitExtensions.CreateVisualElement(card, "profileEditor__swatchRow");

            swatch = UIToolkitExtensions.CreateVisualElement(swatchRow, "profileEditor__swatch");

            colorNameLabel = UIToolkitExtensions.CreateVisualElement<Label>(swatchRow, "profileEditor__colorName");
            colorNameLabel.text = "No color selected";

            statusLabel = UIToolkitExtensions.CreateVisualElement<Label>(card, "profileEditor__status");
            statusLabel.text = "Pick a theme color, toggle B&W, or tap the avatar.";

            colorGroup.SelectColor(ThemeColors[0].Value);
        }

        private void OnAvatarClicked()
        {
            statusLabel.text = "Avatar tapped — a real app would open an image picker here.";
            Debug.Log("[ProfileEditorDemo] Avatar clicked.");
        }

        private void OnBlackWhiteToggled()
        {
            bool blackAndWhite = blackWhiteToggle.IsSelected;
            sampleImage.GreyscaleEnabled = blackAndWhite;
            sampleImage.TextureProperty = blackAndWhite ? grayscaleSampleTexture : colorSampleTexture;
            statusLabel.text = blackAndWhite
                ? "Sample image switched to black & white."
                : "Sample image switched to full color.";
        }

        private void OnThemeColorSelected(Color color)
        {
            swatch.style.backgroundColor = color;
            colorNameLabel.text = $"Theme: {ResolveColorName(color)}";
            statusLabel.text = "Theme color updated.";
        }

        private static string ResolveColorName(Color color)
        {
            foreach (ThemeColor themeColor in ThemeColors)
            {
                if (themeColor.Value == color)
                {
                    return themeColor.Name;
                }
            }

            return "Custom";
        }

        private static Texture2D CreateDesaturatedCopy(Texture2D source)
        {
            var copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
            };

            Color[] pixels = source.GetPixels();
            for (int index = 0; index < pixels.Length; index++)
            {
                Color pixel = pixels[index];
                float luminance = (0.299f * pixel.r) + (0.587f * pixel.g) + (0.114f * pixel.b);
                pixels[index] = new Color(luminance, luminance, luminance, pixel.a);
            }

            copy.SetPixels(pixels);
            copy.Apply(updateMipmaps: false, makeNoLongerReadable: false);
            return copy;
        }

        private static void DestroyTexture(Texture2D texture)
        {
            if (texture != null)
            {
                Destroy(texture);
            }
        }
    }
}