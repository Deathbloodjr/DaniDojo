//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DaniDojo.Patches
//{
//    internal class testing
//    {
//        [HarmonyPatch(typeof(PlayerName))]
//        [HarmonyPatch(nameof(PlayerName.Start))]
//        [HarmonyPatch(MethodType.Normal)]
//        [HarmonyPrefix]
//        public static bool PlayerName_Start_Prefix(PlayerName __instance)
//        {
//            //Plugin.Log.LogInfo("PlayerName Start");
//            return true;
//        }
//    }
//}
