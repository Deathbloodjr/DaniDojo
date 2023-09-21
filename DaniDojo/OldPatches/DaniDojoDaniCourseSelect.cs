using DaniDojo.Managers;
using DaniDojo.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace DaniDojo.Patches
{
    internal class DaniDojoDaniCourseSelect
    {
        public class DaniDojoSelectManager : MonoBehaviour
        {

            static DaniSeries currentSeries;
            static DaniCourse currentCourse;
            DaniCourse currentCourseLevel;

            private GameObject currentCourseObject;
            private GameObject previousCourseObject;
            float courseMoveTime = 0.1f;

            //public static bool isInDan = false; // Only set to true for testing, set to false for the real thing
            //public static int currentDanSongIndex;

            public DonCommon donCommon;
            public PlayerName playerName;

            private GameObject TopCourseParent;
            private GameObject CenterCourseParent;
            private GameObject LeftCourseParent;

            public void Start()
            {
                Plugin.Log.LogInfo("DaniDojoDaniCourseSelect Start");

                var previousCourse = DaniPlayManager.GetCurrentCourse();
                if (previousCourse != null)
                {
                    currentSeries = previousCourse.Parent;
                    currentCourse = previousCourse;
                }
                else
                {

                    currentSeries = CourseDataManager.AllSeriesData.Find((x) => x.IsActive);
                    if (currentSeries == null)
                    {
                        currentSeries = CourseDataManager.AllSeriesData[0];
                    }

                    currentCourse = SaveDataManager.GetDefaultCourse(currentSeries);
                }


                CenterCourseParent = new GameObject("CourseParent");
                LeftCourseParent = new GameObject("LeftCourseParent");
                TopCourseParent = new GameObject("TopCourseParent");

                StartCoroutine(InitializeScene());

                // This doesn't work how I planned
                // I thought having multiple CriPlayers would allow for multiple sounds to be played at the same time
                // However, the last sound will always be played
                // I thought the issue may have been from making the cue names all the same ("song_trance", which happens to be angel dream)
                // However, the same issue was popping up, even with separate cue names.
                // The important part is that the BGM plays properly. Just need to cut the audio so that it loops properly
                //DaniSoundManager.PlaySound("intro.bin", false);

                DaniSoundManager.SetupBgm("odai_primal_loop.bin", true);
                DaniSoundManager.PlayBgm();

                // My attempt at getting animations to work. IDK what I'm doing.
                //var outDonAnimation = donCommon.gameObject.GetComponent<OutDonAnimation>();
                //donCommon.GetDonClothes();
                //var clothes = donCommon.Clothes;
                //outDonAnimation.LoadAsyncAll(clothes.Core, clothes.Head, clothes.Body, clothes.Kigurumi, clothes.Puchi);
                //outDonAnimation.AnimationStart(OutDonAnimation.DonAnimation.Idoling, 0.6666666f);

                //donCommon.AnimationStart(0, 0, "Idle", 0.6666667f);

                //GameObject donObject = new GameObject("DonCommon");
                //GameObject playerObject = new GameObject("PlayerName");

                //donCommon = donObject.AddComponent<DonCommon>();
                //playerName = playerObject.AddComponent<PlayerName>();
            }

            IEnumerator InitializeScene()
            {
                DaniDojoAssets.SelectAssets.InitializeSceneAssets(this.gameObject);

                CenterCourseParent.transform.SetParent(this.transform);
                LeftCourseParent.transform.SetParent(this.transform);
                TopCourseParent.transform.SetParent(this.transform);


                DaniDojoAssets.SelectAssets.CreateSeriesAssets(currentSeries, TopCourseParent);
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Center);


                donCommon = GameObject.Instantiate(DaniDojoSongSelect.donCommonObject).GetComponent<DonCommon>();
                playerName = GameObject.Instantiate(DaniDojoSongSelect.playerNameObject).GetComponent<PlayerName>();

                donCommon.transform.SetParent(this.transform);
                playerName.transform.SetParent(this.transform);

                donCommon.transform.position = new Vector3(260, 340, 0);
                playerName.transform.position = new Vector3(260, 140, 0);
                yield return null;
            }

            public void Update()
            {
                GetInput();
            }

            float inputBuffer = 0.25f;
            float currentBuffer = 0;
            public void GetInput()
            {
                if (currentBuffer != 0)
                {
                    currentBuffer -= Time.deltaTime;
                    currentBuffer = Math.Max(currentBuffer, 0);
                }
                if (!DaniPlayManager.CheckIsInDan())
                {
                    ControllerManager.Dir dir = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionButton(ControllerManager.ControllerPlayerNo.Player1, ControllerManager.Prio.None, false);
                    if (dir == ControllerManager.Dir.None)
                    {
                        dir = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionMouseScrollWheel();
                    }
                    if (dir == ControllerManager.Dir.Left && currentBuffer == 0)
                    {
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                        PrevCourse();
                        currentBuffer = inputBuffer;
                    }
                    else if (dir == ControllerManager.Dir.Right && currentBuffer == 0)
                    {
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                        NextCourse();
                        currentBuffer = inputBuffer;
                    }
                    else if (dir == ControllerManager.Dir.Up && currentBuffer == 0)
                    {
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                        PrevSeries();
                        currentBuffer = inputBuffer;
                    }
                    else if (dir == ControllerManager.Dir.Down && currentBuffer == 0)
                    {
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                        NextSeries();
                        currentBuffer = inputBuffer;
                    }
                    else if (TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetOkDown(ControllerManager.ControllerPlayerNo.Player1) && currentBuffer == 0)
                    {
                        DaniPlayManager.StartDanPlay(currentCourse);
                        var songData = DaniPlayManager.GetSongData();

                        DaniDojoTempEnso.BeginSong(songData.SongId, songData.Level);
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);
                    }
                    else if (TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetCancelDown(ControllerManager.ControllerPlayerNo.Player1) && currentBuffer == 0)
                    {
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.ChangeScene("SongSelect", false);
                    }
                    else if (dir == ControllerManager.Dir.None)
                    {
                        currentBuffer = 0;
                    }
                }
            }

            private void NextCourse()
            {
                ReturnTopCourse(currentCourse);

                currentCourse = CourseDataManager.GetNextUnlockedCourse(currentSeries, currentCourse);

                previousCourseObject = currentCourseObject;
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Left);

                StartCoroutine(MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(-1920, 0, 0), courseMoveTime, true));
                StartCoroutine(MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));

                SelectTopCourse(currentCourse);
            }

            private void PrevCourse()
            {
                ReturnTopCourse(currentCourse);

                currentCourse = CourseDataManager.GetPreviousUnlockedCourse(currentSeries, currentCourse);

                previousCourseObject = currentCourseObject;
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Right);

                StartCoroutine(MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(1920, 0, 0), courseMoveTime, true));
                StartCoroutine(MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));

                SelectTopCourse(currentCourse);
            }

            private void NextSeries()
            {
                //ReturnTopCourse(currentCourse);
                var courseIndex = CourseDataManager.GetCourseIndex(currentSeries, currentCourse);
                var previousCourse = currentCourse;
                currentSeries = CourseDataManager.GetNextSeries(currentSeries);
                currentCourse = currentSeries.Courses.Find((x) => x.Id == previousCourse.Id);

                if (currentCourse == null)
                {
                    courseIndex = Math.Min(courseIndex, currentSeries.Courses.Count - 1);
                    currentCourse = currentSeries.Courses[courseIndex];
                }
                else
                {
                    courseIndex = currentSeries.Courses.FindIndex((x) => x == currentCourse);
                }

                if (SaveDataManager.IsCourseLocked(currentSeries, currentCourse))
                {
                    currentCourse = CourseDataManager.GetPreviousUnlockedCourse(currentSeries, currentCourse);
                }

                previousCourseObject = currentCourseObject;
                DaniDojoAssets.SelectAssets.CreateSeriesAssets(currentSeries, TopCourseParent);
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Up);

                StartCoroutine(MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(0, 1080, 0), courseMoveTime, true));
                StartCoroutine(MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));
                SelectTopCourse(currentCourse);
            }

            private void PrevSeries()
            {
                //ReturnTopCourse(currentCourse);
                var courseIndex = CourseDataManager.GetCourseIndex(currentSeries, currentCourse);

                var previousCourse = currentCourse;
                currentSeries = CourseDataManager.GetPreviousSeries(currentSeries);

                currentCourse = currentSeries.Courses.Find((x) => x.Id == previousCourse.Id);
                if (currentCourse == null)
                {
                    courseIndex = Math.Min(courseIndex, currentSeries.Courses.Count - 1);
                    currentCourse = currentSeries.Courses[courseIndex];
                }
                else
                {
                    courseIndex = currentSeries.Courses.FindIndex((x) => x == currentCourse);
                }

                if (SaveDataManager.IsCourseLocked(currentSeries, currentCourse))
                {
                    currentCourse = CourseDataManager.GetPreviousUnlockedCourse(currentSeries, currentCourse);
                }

                previousCourseObject = currentCourseObject;
                DaniDojoAssets.SelectAssets.CreateSeriesAssets(currentSeries, TopCourseParent);
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Down);

                StartCoroutine(MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(0, -1080, 0), courseMoveTime, true));
                StartCoroutine(MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));
                SelectTopCourse(currentCourse);
            }

            private void SelectTopCourse(DaniCourse course)
            {
                GameObject currentCourse = GameObject.Find(course.Title);
                if (currentCourse != null)
                {
                    var curPosition = currentCourse.transform.position;
                    curPosition.y -= 40;
                    currentCourse.transform.position = curPosition;
                }
            }

            private void ReturnTopCourse(DaniCourse course)
            {
                GameObject currentCourse = GameObject.Find(course.Title);
                if (currentCourse != null)
                {
                    var curPosition = currentCourse.transform.position;
                    curPosition.y = 884;
                    currentCourse.transform.position = curPosition;
                }
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

        static public void ChangeSceneDaniDojo()
        {
            if (Plugin.Assets != null)
            {
                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.ChangeRelayScene("DaniDojo", true);

                Plugin.Instance.StartCoroutine(AddCourseSelectManager());
            }
            else
            {
                var daniDojoScene = SceneManager.CreateScene("DaniDojo");

                var currentScene = SceneManager.GetActiveScene();
                SceneManager.UnloadSceneAsync(currentScene);
                SceneManager.SetActiveScene(daniDojoScene);

                var CourseSelectManager = new GameObject("CourseSelectManager");
                CourseSelectManager.AddComponent<DaniDojoDaniCourseSelect.DaniDojoSelectManager>();
            }
        }

        private static IEnumerator AddCourseSelectManager()
        {
            while (!TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.IsSceneChanged || TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.CurrentSceneName != "DaniDojo")
            {
                yield return null;
            }
            var CourseSelectManager = new GameObject("CourseSelectManager");
            CourseSelectManager.AddComponent<DaniDojoDaniCourseSelect.DaniDojoSelectManager>();
        }


    }
}
