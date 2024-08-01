using DaniDojo.Assets;
using HarmonyLib;
using SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Hooks
{
    internal class SongSelectHooks
    {
        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void SongSelectManager_Start_Postfix(SongSelectManager __instance)
        {
            SongSelectAssets.PopulateActiveSongIds();
        }

        [HarmonyPatch(typeof(SongSelectKanban))]
        [HarmonyPatch("UpdateDisplay")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UpdateDisplay_Postfix(SongSelectKanban __instance, in SongSelectManager.Song song)
        {
            SongSelectAssets.UpdateSongSelectDaniDojoIcons(__instance, song);
        }
    }
}
