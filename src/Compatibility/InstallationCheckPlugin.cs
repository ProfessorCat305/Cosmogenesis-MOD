using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ProjectOrbitalRing.Patches.UI;
using ProjectOrbitalRing.Utils;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace ProjectOrbitalRing.Compatibility
{
    /// <summary>
    ///     special thanks for https://github.com/kremnev8/DSP-Mods/blob/master/Mods/BlueprintTweaks/InstallationChecker.cs
    /// </summary>
    [BepInPlugin(MODGUID, MODNAME, ProjectOrbitalRing.VERSION)]
    [BepInDependency(BlueprintTweaks.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(Bottleneck.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(MoreMegaStructure.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(GalacticScale.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(PlanetwideMining.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(PlanetVeinUtilization.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(FastTravelEnabler.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(GigaStationsUpdated.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(LazyOutposting.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(WeaponPlus.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class InstallationCheckPlugin : BaseUnityPlugin
    {
        public const string MODGUID = "org.ProfessorCat305.OrbitalRing.InstallationCheck";
        public const string MODNAME = "OrbitalRing.InstallationCheck";
        public const string PreferBepinExVersion = "5.4.17";
        private static bool MessageShown, PreloaderInstalled, BepinExVersionMatch;

        internal static ManualLogSource logger;

        public void Awake()
        {
            logger = Logger;

            BepInEx.Logging.Logger.Listeners.Add(new HarmonyLogListener());

            var currentVersion = typeof(Paths).Assembly.GetName().Version.ToString(3);

            BepinExVersionMatch = currentVersion == PreferBepinExVersion;

            FieldInfo birthResourcePoint2 = AccessTools.DeclaredField(typeof(PlanetData), nameof(PlanetData.birthResourcePoint2));
            PreloaderInstalled = birthResourcePoint2 != null;

            new Harmony(MODGUID).Patch(AccessTools.Method(typeof(VFPreload), nameof(VFPreload.InvokeOnLoadWorkEnded)), null,
                new HarmonyMethod(typeof(InstallationCheckPlugin), nameof(OnMainMenuOpen)) { priority = Priority.Last, });

            AwakeCompatibilityPatchers();
        }

        public static void AwakeCompatibilityPatchers()
        {
            Auxilaryfunction.Awake();
            MoreMegaStructure.Awake();
            PlanetVeinUtilization.Awake();
            BlueprintTweaks.Awake();
            Bottleneck.Awake();
            PlanetwideMining.Awake();
            FastTravelEnabler.Awake();
            GigaStationsUpdated.Awake();
            LazyOutposting.Awake();
            WeaponPlus.Awake();

            try { GalacticScale.Awake(); }
            catch (FileNotFoundException)
            {
                // ignore
            }
        }

        public static void OnMainMenuOpen()
        {
            if (MessageShown) return;

            MessageShown = true;

            string msg = null;

            if (ProjectOrbitalRing.ShowMessageBoxEntry.Value) msg = "OrbitalRingLoadMessage";

            if (!ProjectOrbitalRing.LoadCompleted) msg = "ProjectOrbitalRingNotLoaded";

            if (!BepinExVersionMatch) msg = "BepinExVersionNotMatch";

            if (!PreloaderInstalled) msg = "PreloaderNotInstalled";

            if (string.IsNullOrEmpty(msg)) return;

            UIMessageBox.Show("OrbitalRingTitle".TranslateFromJson(), msg.TranslateFromJson(), "确定".TranslateFromJson(),
                "跳转交流群".TranslateFromJson(), "查看不兼容MOD".TranslateFromJson(), UIMessageBox.INFO, null, OpenBrowser, IncompatibleMods);
        }

        public static void OpenBrowser() => Application.OpenURL("星环链接".TranslateFromJson());

        public static void IncompatibleMods() => UIMessageBox.Show("查看不兼容MOD".TranslateFromJson(), "不兼容MOD".TranslateFromJson(), "确定".TranslateFromJson(),
               UIMessageBox.INFO);
    }
}
