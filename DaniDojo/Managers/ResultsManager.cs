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

            GameObject donCommon;
            GameObject playerName;

            public void Start()
            {
                ResultsParent = this.gameObject;
                InitializeAssets();
                CurrentScreen = DaniResultScreen.Songs;

                DaniSoundManager.SetupBgm("results_primal_loop.bin", true);
                DaniSoundManager.PlayBgm();



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
                    DaniPlayManager.SetStartResult(false);
                    DaniDojoDaniCourseSelect.ChangeSceneDaniDojo();
                }
            }

            private void InitializeAssets()
            {
                var bg = ResultsAssets.CreateBg(ResultsParent);

                var songPanel = ResultsAssets.CreateSongPanel(bg);

                var currentCourse = DaniPlayManager.GetCurrentCourse();
                var currentPlay = DaniPlayManager.GetCurrentPlay();

                ResultsAssets.CreateEachSongBg(songPanel, currentCourse, currentPlay, SaveDataManager.GetCourseRecord(currentCourse.Hash));

                PlayRecordParent = ResultsAssets.CreatePlayRecordBg(bg);
                var scoreBg = ResultsAssets.CreatePlayRecordScoreBg(PlayRecordParent, currentPlay);
                var playRecord1Bg = ResultsAssets.CreatePlayRecordGoodOkBad(PlayRecordParent, currentPlay);
                var playRecord2Bg = ResultsAssets.CreatePlayRecordDrumrollComboTotalHits(PlayRecordParent, currentPlay);
                var borders = ResultsAssets.CreateBorderPanels(PlayRecordParent, currentCourse, currentPlay);

                var danCourseIcon = ResultsAssets.CreateCourseIcon(bg, currentCourse);

                donCommon = Instantiate(DaniDojoSongSelect.donCommonObject);
                playerName = Instantiate(DaniDojoSongSelect.playerNameObject);
                donCommon.transform.SetParent(bg.transform);
                playerName.transform.SetParent(bg.transform);

                donCommon.transform.localPosition = new Vector3(202, 289, 0);
                playerName.transform.localPosition = new Vector3(194, 120, 0);

                var danResultAsset = ResultsAssets.CreateDanResult(bg, currentPlay);



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
