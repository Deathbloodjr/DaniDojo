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
using DaniDojo.Assets;

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

            public static GameObject donCommon;
            public static GameObject playerName;

            //public DonCommon donCommon;
            //public PlayerName playerName;

            private GameObject TopCourseParent;
            private GameObject CenterCourseParent;
            private GameObject LeftCourseParent;

            bool isLoaded = false;

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


                TopCourseParent = new GameObject("TopCourseParent");
                CenterCourseParent = new GameObject("CourseParent");
                LeftCourseParent = new GameObject("LeftCourseParent");

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
                // Issue in this function:
                // I need Don-chan to be above the course assets, but below the play result icon
                // However, course assets and play result icon are children of the same parent, so I can't split up that layer
                // I also can't place Don-chan as a child of that parent, as it moves left and right

                // Basic scene assets
                DaniDojoAssets.SelectAssets.InitializeSceneAssets(this.gameObject);

                // Parent Initialization
                TopCourseParent.transform.SetParent(this.transform);
                CenterCourseParent.transform.SetParent(this.transform);
                LeftCourseParent.transform.SetParent(this.transform);

                // Don Initialization
                donCommon = GameObject.Instantiate(DaniDojoSongSelect.donCommonObject);
                playerName = GameObject.Instantiate(DaniDojoSongSelect.playerNameObject);

                donCommon.transform.SetParent(this.transform);
                playerName.transform.SetParent(this.transform);

                donCommon.transform.position = new Vector3(260, 340, 0);
                playerName.transform.position = new Vector3(260, 140, 0);

                // Scene data
                DaniDojoAssets.SelectAssets.CreateSeriesAssets(currentSeries, TopCourseParent);
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Center);

                isLoaded = true;

                TaikoSingletonMonoBehaviour<InputGuide>.Instance.DisableGuide();

                yield return null;
            }

            public void Update()
            {
                TaikoSingletonMonoBehaviour<InputGuide>.Instance.DisableGuide();
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
                if (!DaniPlayManager.CheckIsInDan() && isLoaded)
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

                        DaniSoundManager.StopBgm();

                        DaniDojoTempEnso.BeginDan(currentCourse);
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);
                    }
                    else if (dir == ControllerManager.Dir.None)
                    {
                        currentBuffer = 0;
                    }
                }
                if (TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetCancelDown(ControllerManager.ControllerPlayerNo.Player1) && currentBuffer == 0)
                {
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.ChangeScene("SongSelect", false);
                    DaniSoundManager.StopBgm();
                }
            }

            private void NextCourse()
            {
                ReturnTopCourse(currentCourse);

                currentCourse = CourseDataManager.GetNextUnlockedCourse(currentSeries, currentCourse);

                previousCourseObject = currentCourseObject;
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Left);

                StartCoroutine(AssetUtility.MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(-1920, 0, 0), courseMoveTime, true));
                StartCoroutine(AssetUtility.MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));

                SelectTopCourse(currentCourse);
            }

            private void PrevCourse()
            {
                ReturnTopCourse(currentCourse);

                currentCourse = CourseDataManager.GetPreviousUnlockedCourse(currentSeries, currentCourse);

                previousCourseObject = currentCourseObject;
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Right);

                StartCoroutine(AssetUtility.MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(1920, 0, 0), courseMoveTime, true));
                StartCoroutine(AssetUtility.MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));

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

                StartCoroutine(AssetUtility.MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(0, 1080, 0), courseMoveTime, true));
                StartCoroutine(AssetUtility.MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));
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

                StartCoroutine(AssetUtility.MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(0, -1080, 0), courseMoveTime, true));
                StartCoroutine(AssetUtility.MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));
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

        }

        static public void ChangeSceneDaniDojo(GameObject don = null, GameObject playerName = null)
        {


            if (Plugin.Assets != null)
            {
                if (don != null)
                {
                    DaniDojoSongSelect.donCommonObject = GameObject.Instantiate(don);
                    GameObject.DontDestroyOnLoad(DaniDojoSongSelect.donCommonObject);
                }
                if (playerName != null)
                {
                    DaniDojoSongSelect.playerNameObject = GameObject.Instantiate(playerName);
                    GameObject.DontDestroyOnLoad(DaniDojoSongSelect.playerNameObject);
                }

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
