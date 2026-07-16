using DaniDojo.Assets;
using DaniDojo.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.ResultsScreen
{
    internal class PlayResultsView : MonoBehaviour
    {
#if IL2CPP
            static PlayResultsView() => ClassInjector.RegisterTypeInIl2Cpp<PlayResultsView>();
#endif

        private DaniResultsPlayer parent;
        private GameObject PlayRecordParent;

        private Coroutine introAnimation;
        private Coroutine autoAdvanceTimer;
        private Coroutine screenChangeAnimation;

        public void Initialize(DaniResultsPlayer newParent, GameObject playRecordParent, DaniCourse course, PlayData currentPlay)
        {
            parent = newParent;
            PlayRecordParent = playRecordParent;

            var scoreBg = ResultsAssets.CreatePlayRecordScoreBg(playRecordParent, currentPlay);
            var playRecord1Bg = ResultsAssets.CreatePlayRecordGoodOkBad(playRecordParent, currentPlay);
            var playRecord2Bg = ResultsAssets.CreatePlayRecordDrumrollComboTotalHits(playRecordParent, currentPlay);
            var borders = ResultsAssets.CreateBorderPanels(playRecordParent, course, currentPlay);
        }


        public void StartIntro()
        {
            introAnimation = Plugin.Instance.StartCoroutine(PlayIntro());
        }

        private void StartAutoAdvanceTimer()
        {
            autoAdvanceTimer = Plugin.Instance.StartCoroutine(AutoAdvanceTimer());
        }

        public void ScreenEnter()
        {
            // This has some bounce-back animation to it
            if (screenChangeAnimation != null)
            {
                Plugin.Instance.StopCoroutine(screenChangeAnimation);
            }
            screenChangeAnimation = Plugin.Instance.StartCoroutine(SlideMainScreenTo(PlayRecordParent.GetComponent<RectTransform>(), new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337, 0))));
        }

        public void ScreenExit()
        {
            if (screenChangeAnimation != null)
            {
                Plugin.Instance.StopCoroutine(screenChangeAnimation);
            }
            screenChangeAnimation = Plugin.Instance.StartCoroutine(SlideMainScreenTo(PlayRecordParent.GetComponent<RectTransform>(), new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337 + 1920, 0))));
        }

        private void SnapToEndPosition()
        {
            parent.TakeScreenshot(DaniResultScreen.PlayResults);
        }

        public void StopIntro()
        {
            ModLogger.Log("Stop PlayResults Intro Animation", LogType.Debug);
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
            ModLogger.Log("Stop PlayResults -> Advancement AutoAdvance", LogType.Debug);
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
                ModLogger.Log("Play PlayResults Intro Animation", LogType.Debug);

                ScreenEnter();
                yield return null;

            }
            finally
            {
                SnapToEndPosition();
                introAnimation = null;
            }
        }

        private IEnumerator AutoAdvanceTimer()
        {
            try
            {
                ModLogger.Log("AutoAdvanceTimer PlayResults -> Advancement", LogType.Debug);
                yield return new WaitForSeconds(10.0f);
            }
            finally
            {
                autoAdvanceTimer = null;
                parent.HandleAutoAdvance();
            }
        }

        private IEnumerator SlideMainScreenTo(RectTransform rectTransform, Vector2 targetPosition)
        {
            float elapsed = 0f;
            float duration = 0.1f;
            Vector3 startPos = rectTransform.localPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                rectTransform.localPosition = Vector3.Lerp(startPos, targetPosition, elapsed / duration);
                yield return null;
            }
            rectTransform.localPosition = targetPosition;
        }
    }
}
