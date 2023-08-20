using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DaniDojo.Patches;
using HarmonyLib;
using System.IO;
using System.Text.Json.Nodes;
using DaniDojo.Data;
using DaniDojo.Managers;

namespace DaniDojo.Patches
{
    internal class DaniDojoTempSelect
    {
        private static DaniDojoSelectionMenu _daniDojoSelectionMenu;
        private static string daniDojoMenuGameObjectName = "DaniDojoMenuObject";

        private static bool pressedF1 = false;

        public static bool isInDan = false; // Only set to true for testing, set to false for the real thing
        public static int currentDanSongIndex = -1;
        public static Data.DaniCourse currentDan;


        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.UpdateSongSelect))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongSelectManager_UpdateSongSelect_Prefix(SongSelectManager __instance)
        {

            bool currentlyPressingF1 = Input.GetKey(KeyCode.F4);
            if (currentlyPressingF1 && !pressedF1)
            {
                pressedF1 = true;
                GameObject search = GameObject.Find(daniDojoMenuGameObjectName);
                if (search != null)
                {
                    //Plugin.Log.LogInfo("practiceModeGameObjectName found");
                    //_practiceModeMenu = search.GetComponent<PracticeModeMenu>();
                    GameObject.Destroy(search);
                }
                else
                {
                    TaikoSingletonMonoBehaviour<ControllerManager>.Instance.usedType.type = ControllerManager.ControllerType.Keyboard;

                    //Plugin.Log.LogInfo("practiceModeGameObjectName not found");
                    //Plugin.Log.LogInfo("UpdateSongSelect startTime: " + startTime);
                    search = new GameObject(daniDojoMenuGameObjectName);
                    search.transform.parent = __instance.gameObject.transform;

                    _daniDojoSelectionMenu = search.AddComponent<DaniDojoSelectionMenu>();
                    _daniDojoSelectionMenu.SceneManager = __instance;
                    _daniDojoSelectionMenu.IsActive = true;
                }
            }
            else if (!currentlyPressingF1 && pressedF1)
            {
                pressedF1 = false;
            }
            else
            {
                return true;
            }

            return false;
        }
    }

    public class DaniDojoSelectionMenu : MonoBehaviour
    {
        private bool _internalIsActive = false;
        public bool IsActive
        {
            get => _internalIsActive;
            set
            {
                _internalIsActive = value;

                if (_internalIsActive)
                {
                    TaikoSingletonMonoBehaviour<ControllerManager>.Instance.SetForcedMouseVisibleOff(false);
                    TaikoSingletonMonoBehaviour<ControllerManager>.Instance.SetMouseVisible(true);
                }
            }
        }
        public SongSelectManager SceneManager { get; set; }
        public DaniSeries selectedSeries;

        private void Start()
        {
            
        }


        private void OnGUI()
        {
            if (selectedSeries != null)
            {
                int numButtonsPerRow = 3;

                int ButtonLeft = 1550;
                int ButtonWidth = 100;
                int ButtonTop = 300;
                int ButtonHeight = 30;

                int buttonRow = 0 / numButtonsPerRow;
                int buttonCol = 0 % numButtonsPerRow;

                if (GUI.Button(new Rect(ButtonLeft + (buttonCol * ButtonWidth), ButtonTop + (buttonRow * ButtonHeight), ButtonWidth, ButtonHeight), "Back"))
                {
                    selectedSeries = null;
                }
                for (int j = 0; j < selectedSeries.Courses.Count; j++)
                {
                    buttonRow = (j + 1) / numButtonsPerRow;
                    buttonCol = (j + 1) % numButtonsPerRow;
                    if (GUI.Button(new Rect(ButtonLeft + (buttonCol * ButtonWidth), ButtonTop + (buttonRow * ButtonHeight), ButtonWidth, ButtonHeight), selectedSeries.Courses[j].Title))
                    {
                        DaniDojoTempSelect.currentDan = selectedSeries.Courses[j];
                        DaniDojoTempSelect.isInDan = true;
                        DaniDojoTempSelect.currentDanSongIndex = 0;

                        DaniDojoTempEnso.result = new DaniDojoCurrentPlay(DaniDojoTempSelect.currentDan);

                        DaniDojoTempEnso.BeginSong(DaniDojoTempSelect.currentDan.Songs[0].SongId, DaniDojoTempSelect.currentDan.Songs[0].Level);
                    }
                }
            }
            else
            {
                int numButtonsPerRow = 1;

                int ButtonLeft = 1600;
                int ButtonWidth = 200;
                int ButtonTop = 300;
                int ButtonHeight = 30;

                for (int j = 0; j < CourseDataManager.AllSeriesData.Count; j++)
                {
                    int buttonRow = (j) / numButtonsPerRow;
                    int buttonCol = (j) % numButtonsPerRow;
                    if (GUI.Button(new Rect(ButtonLeft + (buttonCol * ButtonWidth), ButtonTop + (buttonRow * ButtonHeight), ButtonWidth, ButtonHeight), CourseDataManager.AllSeriesData[j].Title))
                    {
                        selectedSeries = CourseDataManager.AllSeriesData[j];
                    }
                }
            }

            //for (int i = 0; i < AllDaniData[0].courseData.Count; i++)
            //{
            //    int buttonRow = i / numButtonsPerRow;
            //    int buttonCol = i % numButtonsPerRow;
            //    if (GUI.Button(new Rect(ButtonLeft + (buttonCol * ButtonWidth), ButtonTop + (buttonRow * ButtonHeight), ButtonWidth, ButtonHeight), AllDaniData[0].courseData[i].title))
            //    {

            //    }
            //}

        }
    }
}
