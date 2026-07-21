using DaniDojo.Assets;
using DaniDojo.Data;
using DaniDojo.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.ResultsScreen
{
    internal class SongsView : MonoBehaviour
    {
#if IL2CPP
            static SongsView() => ClassInjector.RegisterTypeInIl2Cpp<SongsView>();
#endif
        private List<GameObject> songPanels = new List<GameObject>();

        private DaniResultsPlayer parent;

        private Coroutine introAnimation;
        private Coroutine autoAdvanceTimer;

        public void Initialize(DaniResultsPlayer newParent, GameObject bg, DaniCourse course, PlayData currentPlay)
        {
            parent = newParent;
            var songPanel = ResultsAssets.CreateSongPanel(bg);
            songPanels = ResultsAssets.CreateEachSongBg(songPanel, course, currentPlay, SaveDataManager.GetCourseRecord(course.Hash), true);
        }

        public void HandleUpDown(ControllerManager.Dir dir)
        {
            switch (dir)
            {
                case ControllerManager.Dir.Up:
                    break;
                case ControllerManager.Dir.Down:
                    break;
            }
        }

        public void StartIntro()
        {
            introAnimation = Plugin.Instance.StartCoroutine(PlayIntro());
        }

        private void StartAutoAdvanceTimer()
        {
            autoAdvanceTimer = Plugin.Instance.StartCoroutine(AutoAdvanceTimer());
        }

        private void SnapToEndPosition()
        {
            for (int i = 0; i < songPanels.Count; i++)
            {
                var newPos = ResultsAssets.GetSongPanelPosition(i, false);

                songPanels[i].transform.localPosition = newPos;
            }

            parent.TakeScreenshot(DaniResultScreen.Songs);
        }

        public void StopIntro()
        {
            ModLogger.Log("Stop Songs Intro Animation", LogType.Debug);
            if (introAnimation != null)
            {
                Plugin.Instance.StopCoroutine(introAnimation);
            }
        }

        public bool IsInIntro()
        {
            return introAnimation != null;
        }

        public void StopAutoAdvance()
        {
            ModLogger.Log("Stop Songs AutoAdvance", LogType.Debug);
            if (introAnimation != null)
            {
                Plugin.Instance.StopCoroutine(autoAdvanceTimer);
            }
        }

        public bool IsAutoAdvanceEnabled()
        {
            return autoAdvanceTimer != null;
        }

        private IEnumerator PlayIntro()
        {
            try
            {
                ModLogger.Log("Play Songs -> PlayResults Intro Animation", LogType.Debug);

                yield return new WaitForSeconds(1);

                // Slide each panel in 1-by-1 with a slight delay between them
                float slideDuration = 0.3f;
                float delayBetweenPanels = 0.08f;

                for (int i = 0; i < songPanels.Count; i++)
                {
                    if (i != 0)
                    {
                        // Wait for the delay if it isn't the first panel
                        yield return new WaitForSeconds(delayBetweenPanels);
                    }

                    //ModLogger.Log("PlaySongPanelsIntro songPanel[" + i + "]", LogType.Debug);
                    // TODO: Play slide sound file
                    DaniSoundManager.PlaySound("se_daniresult_partial_plate.bin");
                    var newPos = ResultsAssets.GetSongPanelPosition(i, false);
                    yield return MoveSongPanel(songPanels[i], newPos);
                }
            }
            finally
            {
                SnapToEndPosition();
                introAnimation = null;
                StartAutoAdvanceTimer();
            }
        }

        private IEnumerator AutoAdvanceTimer()
        {
            try
            {
                ModLogger.Log("AutoAdvanceTimer Songs -> PlayResults", LogType.Debug);
                // Maybe have this set in the config file?
                yield return new WaitForSeconds(10.0f);
            }
            finally
            {
                autoAdvanceTimer = null;
                parent.HandleAutoAdvance();
            }
        }

        private IEnumerator MoveSongPanel(GameObject objectToMove, Vector3 endLocalPos, float easingFactor = 0.3f, float threshold = 0.5f)
        {
            // Make sure we use localPosition throughout
            Vector3 currentPos = objectToMove.transform.localPosition;

            // Loop until the panel is close enough to its final resting spot
            while (Vector3.Distance(currentPos, endLocalPos) > threshold)
            {
                // Calculate the distance left to go
                Vector3 distanceRemaining = endLocalPos - currentPos;

                // Move a percentage of that remaining distance
                // Frame-rate independent easing scales the factor by DeltaTime relative to a 60fps frame target
                float frameScaledEase = easingFactor * (Time.deltaTime / 0.01666f);

                // Clamp it so it never overshoots if the game hitches/stutters
                frameScaledEase = Mathf.Clamp01(frameScaledEase);

                currentPos += distanceRemaining * frameScaledEase;
                objectToMove.transform.localPosition = currentPos;

                yield return null; // Wait for the next frame
            }

            // Snap to the exact pixel destination when the loop finishes
            objectToMove.transform.localPosition = endLocalPos;
        }
    }
}
