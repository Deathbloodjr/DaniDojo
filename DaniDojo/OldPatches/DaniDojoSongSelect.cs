using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaniDojo.Patches
{
    internal class DaniDojoSongSelect
    {

        public static GameObject donCommonObject;
        public static GameObject playerNameObject;

        #region SongSelectMenu
        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        private static void ModeSelectMenu_Start_Postfix(ModeSelectMenu __instance)
        {
            var tmpItems = __instance.listItems;
            __instance.listItems = new ModeSelectMenu.ListItem[tmpItems.Length + 1];
            for (int i = 0; i < tmpItems.Length; i++)
            {
                __instance.listItems[i] = tmpItems[i];
            }

            __instance.listItems[tmpItems.Length] = tmpItems[0];

            var list1 = GameObject.Find("List1");
            var list7 = GameObject.Instantiate(list1);
            list7.name = "List7";
            list7.transform.SetParent(list1.transform.parent);
            var position = list7.transform.position;
            position.y = 100;
            list7.transform.position = position;

            var tmpGui = list7.GetComponentInChildren<TextMeshProUGUI>();
            tmpGui.text = "Dan-i Dojo";


        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.SelectButton))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool ModeSelectMenu_SelectButton_Prefix(ModeSelectMenu __instance, int buttonIndex, bool playAnim = true)
        {
            if (__instance.CurrentState != ModeSelectMenu.State.ModeSelecting)
            {
                return true;
            }
            if (buttonIndex == 6)
            {
                __instance.selectedItem = buttonIndex;
                __instance.UpdateButtonDisplay(playAnim);
                __instance.SetText(__instance.helpText, "mode_select_desc_dani_dojo", DataConst.DescriptionFontMaterialType.Plane);
                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.DecideItem))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static void ModeSelectMenu_DecideItem_Prefix(ModeSelectMenu __instance)
        {
            if (__instance.CurrentState != ModeSelectMenu.State.ModeSelecting)
            {
                return;
            }
            string source = __instance.GetSceneName(__instance.sourceScene);
            if (__instance.selectedItem == 6)
            {
                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);
                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.PlayData.ModeSelectLastSceneName = source;

                DaniDojoDaniCourseSelect.ChangeSceneDaniDojo(GameObject.Find("don_select_song"), GameObject.Find("PlayerName"));

            }
        }


        private static void CreateAndLoadDaniDojoScene(GameObject don = null, GameObject playerName = null)
        {
            var daniDojoScene = SceneManager.CreateScene("DaniDojo");

            var currentScene = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(currentScene);
            SceneManager.SetActiveScene(daniDojoScene);

            var CourseSelectManager = new GameObject("CourseSelectManager");
            var selectManager = CourseSelectManager.AddComponent<DaniDojoDaniCourseSelect.DaniDojoSelectManager>();

            //selectManager.donCommon = GameObject.Instantiate(don);
            //selectManager.playerName = GameObject.Instantiate(playerName);

            //GameObject.DontDestroyOnLoad(selectManager.donCommon);
            //GameObject.DontDestroyOnLoad(selectManager.playerName);
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.SetText))]
        [HarmonyPatch(new Type[] { typeof(TMP_Text), typeof(string), typeof(DataConst.DefaultFontMaterialType) },
            new ArgumentType[] { ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Normal })]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool ModeSelectMenu_SetText_Prefix(ModeSelectMenu __instance, in TMP_Text tmpText, in string key, DataConst.DefaultFontMaterialType fontMaterialType)
        {
            if (key == "mode_select_desc_dani_dojo")
            {
                Plugin.Log.LogInfo("Key found (Default)");
                WordDataManager.WordListKeysInfo wordListInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr.GetWordListInfo(key);
                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
                tmpText.fontSharedMaterial = fontTMPMgr.GetDefaultFontMaterial(wordListInfo.FontType, fontMaterialType);
                tmpText.font = fontTMPMgr.GetDefaultFontAsset(wordListInfo.FontType);
                tmpText.text = "This is the Dan-i Dojo button!";
                if (tmpText.enabled)
                {
                    tmpText.enabled = false;
                    tmpText.enabled = true;
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.SetText))]
        [HarmonyPatch(new Type[] { typeof(TMP_Text), typeof(string), typeof(DataConst.DescriptionFontMaterialType) },
            new ArgumentType[] { ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Normal })]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool ModeSelectMenu_SetText_Prefix(ModeSelectMenu __instance, in TMP_Text tmpText, in string key, DataConst.DescriptionFontMaterialType fontMaterialType)
        {
            if (key == "mode_select_desc_dani_dojo")
            {
                Plugin.Log.LogInfo("Key found (Description)");
                WordDataManager.WordListKeysInfo wordListInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr.GetWordListInfo("mode_select_desc_rank_match");
                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
                tmpText.fontSharedMaterial = fontTMPMgr.GetDescriptionFontMaterial(wordListInfo.FontType, fontMaterialType);
                tmpText.font = fontTMPMgr.GetDescriptionFontAsset(wordListInfo.FontType);
                tmpText.text = "This is the Dan-i Dojo button!";
                if (tmpText.enabled)
                {
                    tmpText.enabled = false;
                    tmpText.enabled = true;
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PlayerName))]
        [HarmonyPatch(nameof(PlayerName.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool PlayerName_Start_Prefix(PlayerName __instance)
        {
            //Plugin.Log.LogInfo("PlayerName Start");
            // TODO: Add Dan Rank to the left side of the name

            return true;
        }

        #endregion
    }
}
