// Examples~/CharacterExtras/CharacterExtrasDemo.cs
// Demonstrates: AvatarPortrait, LinearProgress, AchievementRow and ShutterButton
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - Portraits at 40, 48 and 56 px, with initials and with an image.
//      - An achievement row whose progress fills as you press "Meet a character".
//      - The shutter: tap it, or start the countdown, which disables it until zero.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// Small character components: portraits, linear progress, an achievement row and the shutter.
    /// </summary>
    public class CharacterExtrasDemo : MonoBehaviour
    {
        private ComponentSamplePage page;
        private AchievementRow achievement;
        private int met = 1;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("CharacterExtrasDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Character Extras",
                "Portraits, progress, an achievement row and the camera shutter.");
            var row = page.AddRow();
            page.AddButton(row, "Meet a character", MeetOne);
            page.Finish();

            var portraits = UIToolkitExtensions.CreateVisualElement(page.Stage, "characterDemo__row");
            portraits.Add(new AvatarPortrait("Mara Okafor", AvatarSize.Small));
            portraits.Add(new AvatarPortrait("Test Visitor", AvatarSize.Medium));
            var withImage = new AvatarPortrait("Guide", AvatarSize.Large);
            withImage.SetImage(Background.FromTexture2D(ProceduralTextureUtility.CreateHorizontalGradient(new Color(0.91f, 0.83f, 0.75f), new Color(0.24f, 0.51f, 0.6f), 64)));
            portraits.Add(withImage);

            var progress = new LinearProgress(0.4f);
            progress.AddToClassList("characterDemo__progress");
            page.Stage.Add(progress);

            achievement = new AchievementRow();
            achievement.SetIcon(Background.FromTexture2D(ProceduralTextureUtility.CreateSolidCircle(32, Color.white)));
            page.Stage.Add(achievement);
            UpdateAchievement();

            var shutter = new ShutterButton();
            shutter.AddToClassList("characterDemo__shutter");
            shutter.Pressed += () =>
            {
                page.Write("Pressed: photo taken");
                shutter.SetEnabled(false);
                shutter.schedule.Execute(() => shutter.SetEnabled(true)).StartingIn(1500);
            };
            page.Stage.Add(shutter);
        }

        private void MeetOne()
        {
            met = Mathf.Min(3, met + 1);
            UpdateAchievement();
        }

        private void UpdateAchievement()
        {
            achievement.TitleText = $"Conversationalist \u00B7 {met} of 3 characters met";
            achievement.Progress = met / 3f;
        }
    }
}
