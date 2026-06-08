using HarmonyLib;
using ProjectOrbitalRing.Patches.Logic.OrbitalRing;
using ProjectOrbitalRing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOrbitalRing.Patches.Logic.BattleRelated
{
    internal class BanInitLevelSetting
    {
        [HarmonyPatch(typeof(UICombatSettingsDF), nameof(UICombatSettingsDF.OnInitLevelSliderChanged))]
        [HarmonyPrefix]
        public static bool OnInitLevelSliderChangedPrePatch(UICombatSettingsDF __instance)
        {
            // 禁止初始等级设置，强制为0
            __instance.initLevelSlider.value = 0;
            __instance.initLevelText.text = "锁定为0".TranslateFromJson();
            return false;
        }
    }
}
