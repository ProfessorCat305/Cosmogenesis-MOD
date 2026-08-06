using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using ProjectOrbitalRing.Utils;

// ReSharper disable InconsistentNaming

namespace ProjectOrbitalRing.Compatibility
{
    internal static class Auxilaryfunction
    {
        internal const string GUID = "cn.blacksnipe.dsp.Auxilaryfunction";

        private static readonly Harmony HarmonyPatch = new Harmony("ProjectOrbitalRing.Compatibility." + GUID);

        internal static void Awake()
        {
            if (!Chainloader.PluginInfos.TryGetValue(GUID, out PluginInfo pluginInfo)) return;

            Assembly assembly = pluginInfo.Instance.GetType().Assembly;

            Type type = assembly.GetType("Auxilaryfunction.Patch.PlayerOperationPatch");

            HarmonyPatch.Patch(AccessTools.Method(type, "FlyAwayPlanet"), new HarmonyMethod(typeof(Auxilaryfunction), nameof(FlyAwayPlanet_Prefix)));
        }

        public static bool FlyAwayPlanet_Prefix()
        {
            PlayerController controller = GameMain.mainPlayer.controller;
            controller.input0.z = 1;
            controller.input1.y += 1;
            if (controller.actionFly.currentAltitude > 49 && controller.horzSpeed < 12.5) {
                controller.input0.y += 1;
            }
            return false;
        }
    }
}
