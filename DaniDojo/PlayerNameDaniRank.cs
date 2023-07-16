using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Patches
{
    internal class PlayerNameDaniRank
    {
        static string BaseImageFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;


        [HarmonyPatch(typeof(PlayerName))]
        [HarmonyPatch(nameof(PlayerName.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void PlayerName_Start_Postfix(PlayerName __instance)
        {
            Plugin.Log.LogInfo("PlayerName Start Postfix");
            var rect = __instance.gameObject.transform.FindChild("TextName").GetComponent<RectTransform>();


            GameObject danRankObject = new GameObject("DanRank");
            danRankObject.transform.SetParent(__instance.gameObject.transform);

            Plugin.Log.LogInfo("CurrentSceneName: " + TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.CurrentSceneName);
            var newPos = rect.position;
            newPos.x -= 163;
            newPos.y -= 18;
            danRankObject.transform.position = newPos;
            //if (TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.CurrentSceneName == "Enso")
            //{
            //    danRankObject.transform.position = new Vector3(60, 160, 0);
            //}
            //else if (TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.CurrentSceneName == "SongSelect")
            //{
            //    danRankObject.transform.position = new Vector3(60, 160, 0);
            //}
            //else
            //{
            //    danRankObject.transform.position = new Vector3(0, 0, 0);
            //}

            Vector2 bgRect = new Vector2(0, 0);
            Vector2 rankRect = new Vector2(3, 0);

            var rank = GetHighestRank();
            if (rank.danResult != DanResult.NotClear)
            {


                string bgImage = string.Empty;
                string courseImage = string.Empty;
                #region Rank if/else
                switch (rank.comboResult)
                {
                    case DanComboResult.Clear:
                        bgImage = "ClearBg.png";
                        break;
                    case DanComboResult.FC:
                        bgImage = "FcBg.png";
                        break;
                    case DanComboResult.DFC:
                        bgImage = "DfcBg.png";
                        break;
                }
                switch (rank.course)
                {
                    case DaniCourse.kyuu5:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "kyuu5.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldKyuu5.png";
                        break;
                    case DaniCourse.kyuu4:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "kyuu4.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldKyuu4.png";
                        break;
                    case DaniCourse.kyuu3:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "kyuu3.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldKyuu3.png";
                        break;
                    case DaniCourse.kyuu2:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "kyuu2.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldKyuu2.png";
                        break;
                    case DaniCourse.kyuu1:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "kyuu1.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldKyuu1.png";
                        break;
                    case DaniCourse.dan1:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan1.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan1.png";
                        break;
                    case DaniCourse.dan2:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan2.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan2.png";
                        break;
                    case DaniCourse.dan3:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan3.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan3.png";
                        break;
                    case DaniCourse.dan4:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan4.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan4.png";
                        break;
                    case DaniCourse.dan5:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan5.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan5.png";
                        break;
                    case DaniCourse.dan6:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan6.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan6.png";
                        break;
                    case DaniCourse.dan7:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan7.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan7.png";
                        break;
                    case DaniCourse.dan8:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan8.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan8.png";
                        break;
                    case DaniCourse.dan9:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan9.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan9.png";
                        break;
                    case DaniCourse.dan10:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "dan10.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldDan10.png";
                        break;
                    case DaniCourse.kuroto:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "kuroto.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldKuroto.png";
                        break;
                    case DaniCourse.meijin:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "meijin.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldMeijin.png";
                        break;
                    case DaniCourse.chojin:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "chojin.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldChojin.png";
                        break;
                    case DaniCourse.tatsujin:
                        if (rank.danResult == DanResult.RedClear)
                            courseImage = "tatsujin.png";
                        else if (rank.danResult == DanResult.GoldClear)
                            courseImage = "goldTatsujin.png";
                        break;
                }
                #endregion

                DaniDojoAssetUtility.CreateImage("DanRankBg", Path.Combine(BaseImageFilePath, "NamePlate", bgImage), bgRect, danRankObject.transform);
                DaniDojoAssetUtility.CreateImage("DanRankName", Path.Combine(BaseImageFilePath, "NamePlate", courseImage), rankRect, danRankObject.transform);

                //__instance.uiTextName.text = "PeePeePooPoo";
            }
        }

        private static (DaniCourse course, DanResult danResult, DanComboResult comboResult) GetHighestRank()
        {
            var activeSeries = Plugin.AllDaniData.Find((x) => x.isActiveDan);

            (DaniCourse course, DanResult danResult, DanComboResult comboResult) item = (activeSeries.courseData[0].course, DanResult.NotClear, DanComboResult.Clear);

            for (int i = 0; i < activeSeries.courseData.Count; i++)
            {
                var score = Plugin.AllDaniScores.Find((x) => x.hash == activeSeries.courseData[i].hash);
                if (score != null)
                {
                    if (score.danResult >= DanResult.RedClear)
                    {
                        item = (activeSeries.courseData[i].course, score.danResult, score.comboResult);
                    }
                }
            }

            Plugin.Log.LogInfo("GetHighestRank: DaniCourse: " + item.course);
            Plugin.Log.LogInfo("GetHighestRank: DanResult: " + item.danResult);
            Plugin.Log.LogInfo("GetHighestRank: DanComboResult: " + item.comboResult);
            return item;
        }
    }
}
