using DaniDojo.Assets;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Hooks
{
    internal class SongCourseSelectHook
    {


        [HarmonyPatch(typeof(CourseSelect))]
        [HarmonyPatch(nameof(CourseSelect.SetInfo))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void CourseSelect_SetInfo_Postfix(CourseSelect __instance, MusicDataInterface.MusicInfoAccesser song)
        {
            SongSelectAssets.SetCurrentSong(song);
        }

        [HarmonyPatch(typeof(CourseSelect))]
        [HarmonyPatch(nameof(CourseSelect.UpdateDiffCourseAnim))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void CourseSelect_UpdateDiffCourseAnim_Postfix(CourseSelect __instance)
        {
            SongSelectAssets.AddDaniDojoIconToCourseSelect();
        }

        
    }
}
