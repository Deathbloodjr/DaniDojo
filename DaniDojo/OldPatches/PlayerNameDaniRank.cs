using DaniDojo.Data;
using DaniDojo.Managers;
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
            if (!Plugin.Instance.ConfigNamePlateDanRankEnabled.Value)
            {
                return;
            }

            var highScore = SaveDataManager.GetHighestActiveClear();
            if (highScore == null)
            {
                return;
            }

            if (highScore.RankCombo.Rank == DaniRank.None)
            {
                return;
            }


            var rect = __instance.gameObject.transform.FindChild("TextName").GetComponent<RectTransform>();


            GameObject danRankObject = new GameObject("DanRank");
            danRankObject.transform.SetParent(__instance.gameObject.transform);

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

            string bgImage = string.Empty;
            string courseImage = string.Empty;
            switch (highScore.RankCombo.Combo)
            {
                case DaniCombo.Silver:
                    bgImage = "ClearBg.png";
                    break;
                case DaniCombo.Gold:
                    bgImage = "FcBg.png";
                    break;
                case DaniCombo.Rainbow:
                    bgImage = "DfcBg.png";
                    break;
            }

            var rank = highScore.RankCombo.Rank;
   
            switch (highScore.Course.CourseLevel)
            {
                case DaniCourseLevel.kyuu5: courseImage = (rank == DaniRank.RedClear ? "kyuu5.png" : "goldKyuu5.png"); break;
                case DaniCourseLevel.kyuu4: courseImage = (rank == DaniRank.RedClear ? "kyuu4.png" : "goldKyuu4.png"); break;
                case DaniCourseLevel.kyuu3: courseImage = (rank == DaniRank.RedClear ? "kyuu3.png" : "goldKyuu3.png"); break;
                case DaniCourseLevel.kyuu2: courseImage = (rank == DaniRank.RedClear ? "kyuu2.png" : "goldKyuu2.png"); break;
                case DaniCourseLevel.kyuu1: courseImage = (rank == DaniRank.RedClear ? "kyuu1.png" : "goldKyuu1.png"); break;
                case DaniCourseLevel.dan1: courseImage = (rank == DaniRank.RedClear ? "dan1.png" : "goldDan1.png"); break;
                case DaniCourseLevel.dan2: courseImage = (rank == DaniRank.RedClear ? "dan2.png" : "goldDan2.png"); break;
                case DaniCourseLevel.dan3: courseImage = (rank == DaniRank.RedClear ? "dan3.png" : "goldDan3.png"); break;
                case DaniCourseLevel.dan4: courseImage = (rank == DaniRank.RedClear ? "dan4.png" : "goldDan4.png"); break;
                case DaniCourseLevel.dan5: courseImage = (rank == DaniRank.RedClear ? "dan5.png" : "goldDan5.png"); break;
                case DaniCourseLevel.dan6: courseImage = (rank == DaniRank.RedClear ? "dan6.png" : "goldDan6.png"); break;
                case DaniCourseLevel.dan7: courseImage = (rank == DaniRank.RedClear ? "dan7.png" : "goldDan7.png"); break;
                case DaniCourseLevel.dan8: courseImage = (rank == DaniRank.RedClear ? "dan8.png" : "goldDan8.png"); break;
                case DaniCourseLevel.dan9: courseImage = (rank == DaniRank.RedClear ? "dan9.png" : "goldDan9.png"); break;
                case DaniCourseLevel.dan10: courseImage = (rank == DaniRank.RedClear ? "dan10.png" : "goldDan10.png"); break;
                case DaniCourseLevel.kuroto: courseImage = (rank == DaniRank.RedClear ? "kuroto.png" : "goldKuroto.png"); break;
                case DaniCourseLevel.meijin: courseImage = (rank == DaniRank.RedClear ? "meijin.png" : "goldMeijin.png"); break;
                case DaniCourseLevel.chojin: courseImage = (rank == DaniRank.RedClear ? "chojin.png" : "goldChojin.png"); break;
                case DaniCourseLevel.tatsujin: courseImage = (rank == DaniRank.RedClear ? "tatsujin.png" : "goldTatsujin.png"); break;
                default: return;
            }

            DaniDojoAssetUtility.CreateImage("DanRankBg", Path.Combine(BaseImageFilePath, "NamePlate", bgImage), bgRect, danRankObject.transform);
            DaniDojoAssetUtility.CreateImage("DanRankName", Path.Combine(BaseImageFilePath, "NamePlate", courseImage), rankRect, danRankObject.transform);
        }

        //private static (DaniCourse course, DanResult danResult, DanComboResult comboResult) GetHighestRank()
        //{
        //    var activeSeries = CourseDataManager.AllSeriesData.Find((x) => x.IsActive);

        //    (DaniCourse course, DanResult danResult, DanComboResult comboResult) item = (activeSeries.Courses[0].course, DanResult.NotClear, DanComboResult.Clear);

        //    for (int i = 0; i < activeSeries.Courses.Count; i++)
        //    {
        //        var score = Plugin.AllDaniScores.Find((x) => x.hash == activeSeries.Courses[i].Hash);
        //        if (score != null)
        //        {
        //            if (score.danResult >= DanResult.RedClear)
        //            {
        //                item = (activeSeries.Courses[i].course, score.danResult, score.comboResult);
        //            }
        //        }
        //    }

        //    Plugin.Log.LogInfo("GetHighestRank: DaniCourse: " + item.course);
        //    Plugin.Log.LogInfo("GetHighestRank: DanResult: " + item.danResult);
        //    Plugin.Log.LogInfo("GetHighestRank: DanComboResult: " + item.comboResult);
        //    return item;
        //}
    }
}
