// Examples~/QRCodeView/QRCodeViewDemo.cs
// Demonstrates: QRCodeView
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A visitor link drawn as a QR code; scan it with a phone to check it.
//      - "Next link" changes the content; longer content needs a larger version.
//      - "Save PNG" writes a copy at 8 pixels per module to the persistent data path.

using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// QRCodeView demo: encode a link, change it, and save the code as an image.
    /// </summary>
    public class QRCodeViewDemo : MonoBehaviour
    {
        private static readonly string[] Links =
        {
            "https://example.com/go/sample",
            "https://example.com/go/walkthrough?stop=4&lang=en",
            "4471",
        };

        private ComponentSamplePage page;
        private QRCodeView view;
        private Label caption;
        private int index;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("QRCodeViewDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "QR Code View",
                "A QR code for a link, with a quiet zone, drawn without a barcode library.");
            var row = page.AddRow();
            page.AddButton(row, "Next link", NextLink);
            page.AddButton(row, "Save PNG", SavePng, secondary: true);
            page.Finish();

            var card = UIToolkitExtensions.CreateVisualElement(page.Stage, "qrDemo__card");
            view = new QRCodeView(Links[0], 220f);
            card.Add(view);
            caption = UIToolkitExtensions.CreateVisualElement<Label>(card, "qrDemo__caption");
            ShowCaption();
        }

        private void NextLink()
        {
            index = (index + 1) % Links.Length;
            view.Text = Links[index];
            ShowCaption();
        }

        private void ShowCaption()
        {
            caption.text = view.Code == null ? view.Text : $"{view.Text}\nVersion {view.Code.Version}, {view.Code.Size} modules";
        }

        private void SavePng()
        {
            var texture = view.CreateTexture(8);
            if (texture == null)
            {
                return;
            }

            var path = Path.Combine(Application.persistentDataPath, "qr-code.png");
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Destroy(texture);
            page.Write($"Saved {path}");
        }
    }
}
