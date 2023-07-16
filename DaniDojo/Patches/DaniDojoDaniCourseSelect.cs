﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Patches
{
    internal class DaniDojoDaniCourseSelect
    {
        public class DaniDojoSelectManager : MonoBehaviour
        {
            public static DaniSeriesData currentSeries;
            public static DaniData currentCourse;
            public DaniCourse currentCourseLevel;

            private GameObject currentCourseObject;
            private GameObject previousCourseObject;
            float courseMoveTime = 0.1f;

            public static bool isInDan = false; // Only set to true for testing, set to false for the real thing
            public static int currentDanSongIndex;

            public DonCommon donCommon;
            public PlayerName playerName;

            private GameObject TopCourseParent;
            private GameObject CenterCourseParent;
            private GameObject LeftCourseParent;

            public void Start()
            {
                Plugin.Log.LogInfo("DaniDojoDaniCourseSelect Start");

                currentSeries = Plugin.AllDaniData.Find((x) => x.isActiveDan);
                // Default to 1st dan

                currentCourse = currentSeries.courseData[5];
                // Check to see if there's any higher clears, and set the current dan to the one after the highest cleared

                for (int j = 0; j < currentSeries.courseData.Count; j++)
                {
                    for (int i = 0; i < Plugin.AllDaniScores.Count; i++)
                    {
                        var course = currentSeries.courseData[j];
                        if (course.hash == Plugin.AllDaniScores[i].hash)
                        {
                            if (course.danId >= currentCourse.danId && Plugin.AllDaniScores[i].danResult >= DanResult.RedClear)
                            {
                                if (currentSeries.courseData.Count > j + 1)
                                {
                                    currentCourse = currentSeries.courseData[j + 1];
                                }
                                else
                                {
                                    currentCourse = course;
                                }
                            }
                        }
                    }
                }

                currentCourseLevel = currentCourse.course;

                CenterCourseParent = new GameObject("CourseParent");
                LeftCourseParent = new GameObject("LeftCourseParent");
                TopCourseParent = new GameObject("TopCourseParent");

                StartCoroutine(InitializeScene());



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

                isInDan = false;
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
                if (!isInDan)
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
                        isInDan = true;
                        currentDanSongIndex = 0;

                        DaniDojoTempEnso.result = new DaniDojoCurrentPlay(currentCourse);

                        DaniDojoTempEnso.BeginSong(currentCourse.songs[0].songId, currentCourse.songs[0].level);
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
                var index = currentSeries.courseData.FindIndex((x) => x == currentCourse);

                do
                {
                    index++;
                    if (index >= currentSeries.courseData.Count)
                    {
                        index = 0;
                    }
                } while (!CheckIfCourseUnlocked(currentSeries, currentSeries.courseData[index]));

                currentCourse = currentSeries.courseData[index];
                previousCourseObject = currentCourseObject;
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Left);

                StartCoroutine(MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(-1920, 0, 0), courseMoveTime, true));
                StartCoroutine(MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));

                SelectTopCourse(currentCourse);
                currentCourseLevel = currentCourse.course;

                Plugin.Log.LogInfo("locked: " + currentCourse.locked);
            }

            private void PrevCourse()
            {
                ReturnTopCourse(currentCourse);
                var index = currentSeries.courseData.FindIndex((x) => x == currentCourse);
                index--;
                if (index < 0)
                {
                    index = currentSeries.courseData.Count - 1;
                }

                while (!CheckIfCourseUnlocked(currentSeries, currentSeries.courseData[index]))
                {
                    index--;
                    if (index < 0)
                    {
                        index = currentSeries.courseData.Count - 1;
                    }
                }

                currentCourse = currentSeries.courseData[index];
                previousCourseObject = currentCourseObject;
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Right);

                StartCoroutine(MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(1920, 0, 0), courseMoveTime, true));
                StartCoroutine(MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));
                SelectTopCourse(currentCourse);
                currentCourseLevel = currentCourse.course;

                Plugin.Log.LogInfo("locked: " + currentCourse.locked);
            }

            private void NextSeries()
            {
                //ReturnTopCourse(currentCourse);
                var courseIndex = currentSeries.courseData.FindIndex((x) => x == currentCourse);
                var seriesIndex = Plugin.AllDaniData.FindIndex((x) => x == currentSeries);
                seriesIndex++;
                if (seriesIndex >= Plugin.AllDaniData.Count)
                {
                    seriesIndex = 0;
                }
                currentSeries = Plugin.AllDaniData[seriesIndex];
                currentCourse = currentSeries.courseData.Find((x) => x.course == currentCourseLevel);

                if (currentCourse == null)
                {
                    courseIndex = Math.Min(courseIndex, currentSeries.courseData.Count - 1);
                    currentCourse = currentSeries.courseData[courseIndex];
                }
                else
                {
                    courseIndex = currentSeries.courseData.FindIndex((x) => x == currentCourse);
                }

                while (!CheckIfCourseUnlocked(currentSeries, currentSeries.courseData[courseIndex]))
                {
                    // I question this logic, as I didn't think it through at all
                    courseIndex--;
                    if (courseIndex < 0)
                    {
                        courseIndex = currentSeries.courseData.Count;
                    }
                }

                currentCourse = currentSeries.courseData[courseIndex];

                previousCourseObject = currentCourseObject;
                DaniDojoAssets.SelectAssets.CreateSeriesAssets(currentSeries, TopCourseParent);
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Up);

                StartCoroutine(MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(0, 1080, 0), courseMoveTime, true));
                StartCoroutine(MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));
                SelectTopCourse(currentCourse);

                Plugin.Log.LogInfo("locked: " + currentCourse.locked);
            }

            private void PrevSeries()
            {
                //ReturnTopCourse(currentCourse);
                var courseIndex = currentSeries.courseData.FindIndex((x) => x == currentCourse);
                var seriesIndex = Plugin.AllDaniData.FindIndex((x) => x == currentSeries);
                seriesIndex--;
                if (seriesIndex < 0)
                {
                    seriesIndex = Plugin.AllDaniData.Count - 1;
                }

                currentSeries = Plugin.AllDaniData[seriesIndex];
                currentCourse = currentSeries.courseData.Find((x) => x.course == currentCourseLevel);

                if (currentCourse == null)
                {
                    courseIndex = Math.Min(courseIndex, currentSeries.courseData.Count - 1);
                    currentCourse = currentSeries.courseData[courseIndex];
                }
                else
                {
                    courseIndex = currentSeries.courseData.FindIndex((x) => x == currentCourse);
                }

                while (!CheckIfCourseUnlocked(currentSeries, currentSeries.courseData[courseIndex]))
                {
                    // I question this logic, as I didn't think it through at all
                    courseIndex--;
                    if (courseIndex < 0)
                    {
                        courseIndex = currentSeries.courseData.Count;
                    }
                }

                currentCourse = currentSeries.courseData[courseIndex];

                previousCourseObject = currentCourseObject;
                DaniDojoAssets.SelectAssets.CreateSeriesAssets(currentSeries, TopCourseParent);
                currentCourseObject = DaniDojoAssets.SelectAssets.CreateCourseAssets(currentCourse, CenterCourseParent, DaniDojoAssets.SelectAssets.CourseCreateDir.Down);

                StartCoroutine(MoveOverSeconds(previousCourseObject, previousCourseObject.transform.position + new Vector3(0, -1080, 0), courseMoveTime, true));
                StartCoroutine(MoveOverSeconds(currentCourseObject, new Vector2(342, 26), courseMoveTime));
                SelectTopCourse(currentCourse);

                Plugin.Log.LogInfo("locked: " + currentCourse.locked);
            }

            private void SelectTopCourse(DaniData course)
            {
                GameObject currentCourse = GameObject.Find(course.title);
                var curPosition = currentCourse.transform.position;
                curPosition.y -= 40;
                currentCourse.transform.position = curPosition;
            }

            private void ReturnTopCourse(DaniData course)
            {
                GameObject currentCourse = GameObject.Find(course.title);
                var curPosition = currentCourse.transform.position;
                curPosition.y = 884;
                currentCourse.transform.position = curPosition;
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

            private bool CheckIfCourseUnlocked(DaniSeriesData series, DaniData course)
            {
                Plugin.Log.LogInfo("seriesName: " + series.seriesTitle);
                Plugin.Log.LogInfo("courseName: " + course.title);
                if (!course.locked)
                {
                    return true;
                }

                // course is in fact, locked

                // Get the index of the current course
                var currentIndex = series.courseData.FindIndex((x) => x == course);
                if (currentIndex == 0)
                {
                    // If it is the first course in the list, it cannot be locked
                    // I feel like this is a safe rule to have hardcoded
                    return true;
                }

                // Get the courseData for the previous course
                var previousCourse = series.courseData[currentIndex - 1];

                // Get the high score for the previous course to see if it was cleared

                var highScore = Plugin.AllDaniScores.Find((x) => x.hash == previousCourse.hash);
                if (highScore == null)
                {
                    return false;
                }

                // Check to see if the prevous course was cleared
                // If the danResult is RedClear or greater (which would just be GoldClear), then this course is unlocked
                return highScore.danResult >= DanResult.RedClear;
            }
        }

    }
}
