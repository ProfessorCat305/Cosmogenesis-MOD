using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectOrbitalRing.Patches.Logic.MathematicalRateEngine
{
    internal class SiloAndEjector
    {
        /// <summary>
        /// 火箭发射器所需火箭修正，注意如果更改了巨构类型，而发射器内还存有不相符的火箭，该火箭将直接消失（为了防止用廉价火箭白嫖高价火箭）
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SiloComponent), "InternalUpdate")]
        public static void SiloUpdatePatch(ref SiloComponent __instance)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;
            if (gmProtoId != 2312) return; //只修改原始火箭发射器

            if (starIndex < 0 || starIndex > 999)
            {
                //Debug.LogWarning("SiloInternalUpdate Patch Error because starIndex out of range.");
                return;
            }

            int bulletIdExpected = 1503;

            if (GameMain.galaxy.stars[starIndex].type == EStarType.BlackHole) {
                if (ProjectOrbitalRing.MoreMegaStructureCompatibility) {
                    try {
                        // 使用反射动态获取类型
                        var mmType = Type.GetType("MoreMegaStructure.MoreMegaStructure, MoreMegaStructure");
                        var starMegaType = mmType?.GetField("StarMegaStructureType")?.GetValue(null) as int[];

                        if (starMegaType?[starIndex] != 0) {
                            if (__instance.bulletId == 6228 || __instance.bulletId == 6504 || __instance.bulletId == 6502) {
                                __instance.bulletCount = 0;
                                __instance.bulletInc = 0;
                                __instance.bulletId = 1503;
                            }
                            return;
                        }
                    } catch (Exception ex) {
                        // ignored
                    }
                }
                if (!GameMain.history.TechUnlocked(1952)) {
                    bulletIdExpected = 6228; // 默认无法发射火箭
                } else if (!GameMain.history.TechUnlocked(1960)) {
                    bulletIdExpected = 6502; // 进入二阶，发射深蓝之井火箭
                } else {
                    bulletIdExpected = 6504; // 进入三阶，发射深蓝之井火箭
                }

                if (__instance.bulletId != bulletIdExpected) {
                    __instance.bulletCount = 0;
                    __instance.bulletInc = 0;
                    __instance.bulletId = bulletIdExpected;
                }
            }
        }

        /// <summary>
        /// 弹射器所需发射物修正，类似上面的发射井
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static void EjectorUpdatePatch(ref EjectorComponent __instance)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;
            if (gmProtoId != 2311 && gmProtoId != 6513) return; //只修改原始弹射器

            if (starIndex < 0 || starIndex > 999)
            {
                return;
            }
            int bulletIdExpected = 1501;
            if (gmProtoId == 6513) {
                if (ProjectOrbitalRing.MoreMegaStructureCompatibility) {
                    try {
                        // 使用反射动态获取类型
                        var mmType = Type.GetType("MoreMegaStructure.MoreMegaStructure, MoreMegaStructure");
                        var starMegaType = mmType?.GetField("StarMegaStructureType")?.GetValue(null) as int[];

                        if (starMegaType?[starIndex] == 2) {
                            if (__instance.bulletId == 1501) {
                                __instance.bulletCount = 0;
                                __instance.bulletInc = 0;
                                __instance.bulletId = 6006;
                            }
                            return;
                        }

                    } catch (Exception ex) {
                        // ignored
                    }
                }
            }
            if (GameMain.galaxy.stars[starIndex].type == EStarType.BlackHole) {
                if (ProjectOrbitalRing.MoreMegaStructureCompatibility) {
                    try {
                        // 使用反射动态获取类型
                        var mmType = Type.GetType("MoreMegaStructure.MoreMegaStructure, MoreMegaStructure");
                        var starMegaType = mmType?.GetField("StarMegaStructureType")?.GetValue(null) as int[];

                        if (starMegaType?[starIndex] != 0) {
                            int targetBulletId = (starMegaType?[starIndex] == 2) ? 6006 : 1501;
                            if (__instance.bulletId != targetBulletId) {
                                __instance.bulletCount = 0;
                                __instance.bulletInc = 0;
                                __instance.bulletId = targetBulletId;

                            }
                            return;
                        }

                    } catch (Exception ex) {
                        // ignored
                    }
                }
                if (!GameMain.history.TechUnlocked(1802)) {
                    bulletIdExpected = 6228; // 默认无法发射弹射物
                } else if (!GameMain.history.TechUnlocked(1952)) {
                    bulletIdExpected = 9480; // 进入一阶，发射引力发生装置
                } else if (!GameMain.history.TechUnlocked(1960)) {
                    bulletIdExpected = 1803; // 进入二阶，发射反物质燃料棒
                } else {
                    bulletIdExpected = 9482; // 进入三阶，发射引力钻头
                }

                if (GameMain.history.TechUnlocked(1952) && !GameMain.history.TechUnlocked(1960) && __instance.bulletCount == 0) {
                    __instance.needs[0] = ((__instance.bulletCount >= 20) ? 0 : 1803);
                    __instance.needs[1] = ((__instance.bulletCount >= 20) ? 0 : 1804);
                }

                if (GameMain.history.TechUnlocked(1952) && !GameMain.history.TechUnlocked(1960) && __instance.bulletId == 1804) {
                    bulletIdExpected = 1804;
                }
                if (__instance.bulletId != bulletIdExpected) {
                    __instance.bulletCount = 0;
                    __instance.bulletInc = 0;
                    __instance.bulletId = bulletIdExpected;
                }
            }
        }

        // 疑似用不到，看上去只有矿机和电厂Component会用到这个InsertInto
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.InsertInto), new[]
        {
            typeof(int), typeof(int), typeof(int), typeof(byte),
            typeof(byte), typeof(byte),
        }, new[]
        {
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal,
            ArgumentType.Normal, ArgumentType.Out,
        })]
        [HarmonyPrefix]
        public static void PlanetFactory_Ejector_InsertInto_Patch(PlanetFactory __instance, int entityId, int offset, int itemId, byte itemCount, byte itemInc)
        {
            int beltId = __instance.entityPool[entityId].beltId;
            if (beltId <= 0) {
                int[] array = __instance.entityNeeds[entityId];
                int ejectorId = __instance.entityPool[entityId].ejectorId;
                if (ejectorId > 0) {
                    if (array == null) {
                        return;
                    }
                    EjectorComponent ejector = __instance.factorySystem.ejectorPool[ejectorId];
                    if (ejector.bulletId == 1803 && ejector.bulletCount == 0 && itemId == 1804) {
                        ejector.needs[0] = 1804;
                        __instance.entityNeeds[entityId][0] = 1804;
                        ejector.bulletId = 1804;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.InsertInto), new[]
        {
            typeof(uint), typeof(int), typeof(int), typeof(byte),
            typeof(byte), typeof(byte),
        }, new[]
        {
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal,
            ArgumentType.Normal, ArgumentType.Out,
        })]
        [HarmonyPrefix]
        public static void PlanetFactory_Ejector_InsertInto2_Patch(ref PlanetFactory __instance, int ioTargetTypedId, int offset, int itemId, byte itemCount, byte itemInc)
        {
            int num = (int)(ioTargetTypedId & 16777215U);
            EFactoryIOTargetType efactoryIOTargetType = (EFactoryIOTargetType)(ioTargetTypedId & 4278190080U);
            if (efactoryIOTargetType <= EFactoryIOTargetType.Silo) {
                if (efactoryIOTargetType == EFactoryIOTargetType.Ejector) {
                    ref EjectorComponent ejector = ref __instance.factorySystem.ejectorPool[num];
                    if (ejector.id <= 0) {
                        return;
                    }
                    int entityId3 = ejector.entityId;
                    int[] array4 = __instance.entityNeeds[entityId3];
                    if (array4 == null) {
                        return;
                    }
                    if (ejector.bulletId == 1803 && ejector.bulletCount == 0 && itemId == 1804) {
                        ejector.needs[0] = 1804;
                        __instance.entityNeeds[entityId3][0] = 1804;
                        ejector.bulletId = 1804;
                    } else if (ejector.bulletId == 1804 && ejector.bulletCount == 0 && itemId == 1803) {
                        ejector.needs[0] = 1803;
                        __instance.entityNeeds[entityId3][0] = 1803;
                        ejector.bulletId = 1803;
                    }
                }
            }
        }
    }
}
