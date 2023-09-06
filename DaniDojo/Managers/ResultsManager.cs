using DaniDojo.Assets;
using DaniDojo.Patches;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Managers
{
    internal class ResultsManager
    {
        internal class DaniResultsPlayer : MonoBehaviour
        {
            enum DaniResultScreen
            {
                Songs,
                PlayResults,
                Advancement,
            }

            DaniResultScreen CurrentScreen;

            GameObject ResultsParent;
            GameObject PlayRecordParent;
            public void Start()
            {
                ResultsParent = this.gameObject;
                InitializeAssets();
                CurrentScreen = DaniResultScreen.Songs;
            }

            public void Update()
            {
                GetInput();
            }


            public void GetInput()
            {
                ControllerManager.Dir dir = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionButton(ControllerManager.ControllerPlayerNo.Player1, ControllerManager.Prio.None, false);
                bool okDown = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetOkDown(ControllerManager.ControllerPlayerNo.Player1);
                bool cancelDown = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetCancelDown(ControllerManager.ControllerPlayerNo.Player1);
                if ((dir == ControllerManager.Dir.Left || cancelDown) && CurrentScreen == DaniResultScreen.PlayResults)
                {
                    CurrentScreen = DaniResultScreen.Songs;
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);

                    StartCoroutine(MoveOverSeconds(PlayRecordParent, new Vector2(337 + 1920, 44), 0.1f));
                }
                else if ((dir == ControllerManager.Dir.Right || okDown) && CurrentScreen == DaniResultScreen.Songs)
                {
                    CurrentScreen = DaniResultScreen.PlayResults;
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);

                    StartCoroutine(MoveOverSeconds(PlayRecordParent, new Vector2(337, 44), 0.1f));
                }
                else if ((okDown) && CurrentScreen == DaniResultScreen.PlayResults)
                {
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);
                    DaniDojoDaniCourseSelect.ChangeSceneDaniDojo();
                }
            }

            private void InitializeAssets()
            {
                ResultsAssets.CreateBg(ResultsParent);

                var songPanel = ResultsAssets.CreateSongPanel(ResultsParent);
                ResultsAssets.CreateEachSongBg(songPanel, DaniPlayManager.GetCurrentCourse());

                PlayRecordParent = ResultsAssets.CreatePlayRecordBg(ResultsParent);
                var scoreBg = ResultsAssets.CreatePlayRecordScoreBg(PlayRecordParent);
                var playRecord1Bg = ResultsAssets.CreatePlayRecordGoodOkBadBg(PlayRecordParent);
                var playRecord2Bg = ResultsAssets.CreatePlayRecordDrumrollComboTotalHitsBg(PlayRecordParent);
            }

            public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds, bool deleteAfter = false)
            {
                float elapsedTime = 0;
                Vector3 startingPos = objectToMove.transform.position;
                while (elapsedTime < seconds)
                {
                    objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                objectToMove.transform.position = end;
                if (deleteAfter)
                {
                    GameObject.Destroy(objectToMove);
                }
            }
        }
    }
}
