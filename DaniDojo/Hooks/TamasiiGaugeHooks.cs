using DaniDojo.Managers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Hooks
{
    internal class TamasiiGaugeHooks
    {
        public static bool Initialize = true;

        [HarmonyPatch(typeof(TamasiiGaugePlayerSprite))]
        [HarmonyPatch(nameof(TamasiiGaugePlayerSprite.SetTamasiiGauge))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool TamasiiGaugePlayerSprite_SetTamasiiGauge_Prefix(TamasiiGaugePlayerSprite __instance, ref int value, ref float speed)
        {
            if (DaniPlayManager.CheckIsInDan())
            {
                // For the first song, we want it to initialize to 0 like normal
                if (Initialize)
                {
                    Initialize = false;
                    return true;
                }
                if (speed != 69)
                {
                    return false;
                }
                speed = 1;
                value /= 3;
            }
            return true;
        }
    }
}
