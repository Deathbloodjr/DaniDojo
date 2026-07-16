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
    internal class AdvancementView : MonoBehaviour
    {
#if IL2CPP
            static AdvancementView() => ClassInjector.RegisterTypeInIl2Cpp<AdvancementView>();
#endif

        private DaniResultsPlayer parent;

        private Coroutine introAnimation;
        private Coroutine screenChangeAnimation;

        public void Initialize(DaniResultsPlayer newParent, GameObject bg, DaniCourse course, PlayData currentPlay)
        {
        }


        public void StartIntro()
        {
            introAnimation = Plugin.Instance.StartCoroutine(PlayIntro());
        }

        public void ScreenEnter()
        {
            // This has some bounce-back animation to it
            if (screenChangeAnimation != null)
            {
                Plugin.Instance.StopCoroutine(screenChangeAnimation);
            }
            //screenChangeAnimation = Plugin.Instance.StartCoroutine(SlideMainScreenTo(PlayRecordParent.GetComponent<RectTransform>(), new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337, 0))));
        }

        public void ScreenExit()
        {
            if (screenChangeAnimation != null)
            {
                Plugin.Instance.StopCoroutine(screenChangeAnimation);
            }
            //screenChangeAnimation = Plugin.Instance.StartCoroutine(SlideMainScreenTo(PlayRecordParent.GetComponent<RectTransform>(), new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337 + 1920, 0))));
        }

        private void SnapToEndPosition()
        {

            parent.TakeScreenshot(DaniResultScreen.PlayResults);
        }

        public void StopIntro()
        {
            ModLogger.Log("Stop Advancement Intro Animation", LogType.Debug);
            if (introAnimation != null)
            {
                Plugin.Instance.StopCoroutine(introAnimation);
            }
        }

        public bool IsInIntro()
        {
            return introAnimation != null;
        }

        private IEnumerator PlayIntro()
        {
            try
            {
                ModLogger.Log("Play Advancement Intro Animation", LogType.Debug);

                yield return new WaitForSeconds(1);

            }
            finally
            {
                SnapToEndPosition();
                introAnimation = null;
            }
        }
    }
}
