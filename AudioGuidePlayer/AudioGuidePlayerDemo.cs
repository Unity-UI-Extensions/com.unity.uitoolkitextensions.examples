// Examples~/AudioGuidePlayer/AudioGuidePlayerDemo.cs
// Demonstrates: AudioGuidePlayer
//
// Scene setup:
//   A scene for this example is provided. The demo grabs the UIDocument from the
//   same GameObject via GetComponent<UIDocument>(), so no manual assignment is needed.
//      - A simulated 70 second guide: the demo plays the role of the audio source, answers
//        the player's requests and reports the position every frame.
//      - Drag the scrubber, skip 15 seconds, loop, change the rate; the transcript line
//        follows the position. At the end the play button offers a replay.

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUIToolkit.Extensions.Examples
{
    /// <summary>
    /// AudioGuidePlayer demo: a simulated source driving the controls and answering their requests.
    /// </summary>
    public class AudioGuidePlayerDemo : MonoBehaviour
    {
        private const float Length = 70f;

        private ComponentSamplePage page;
        private AudioGuidePlayer player;
        private float position;
        private bool playing;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("AudioGuidePlayerDemo requires a UIDocument on the same GameObject.", this);
                return;
            }

            page = new ComponentSamplePage(uiDocument.rootVisualElement, "Audio Guide Player",
                "Controls for a guide or a video. The owner reports the state and answers the requests.");
            page.Finish();

            player = new AudioGuidePlayer { TitleText = "Blade inspection", Duration = Length, ShowSkip = true };
            player.SetCues(new[]
            {
                new TranscriptCue(0f, "Welcome to the turbine hall."),
                new TranscriptCue(12f, "Each blade is forty metres long."),
                new TranscriptCue(30f, "Inspectors climb inside the blade to check the seams."),
                new TranscriptCue(52f, "That is the end of this stop. Walk on to stop five."),
            });

            player.PlayPauseRequested += play => { playing = play; page.Write(play ? "Play" : "Pause"); };
            player.ReplayRequested += () => { position = 0f; playing = true; page.Write("Replay"); };
            player.SeekRequested += seconds => { position = seconds; page.Write($"Seek to {AudioGuidePlayer.FormatTime(seconds)}"); };
            player.SkipRequested += delta => position = Mathf.Clamp(position + delta, 0f, Length);
            player.LoopToggled += on => page.Write($"Loop {(on ? "on" : "off")}");
            player.RateChanged += rate => page.Write($"Rate {rate}x");
            page.Stage.Add(player);
        }

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            if (playing)
            {
                position += Time.deltaTime * player.Rate;
                if (position >= Length)
                {
                    position = player.IsLooping ? 0f : Length;
                    playing = player.IsLooping;
                }
            }

            player.IsPlaying = playing;
            player.Position = position;
        }
    }
}
