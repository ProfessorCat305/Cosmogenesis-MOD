using HarmonyLib;
using ProjectOrbitalRing.Utils;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static ProjectOrbitalRing.ProjectOrbitalRing;

namespace ProjectOrbitalRing.Patches.Logic.BattleRelated
{
    internal class InfiniteReloadAmmo
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TurretComponent), "LoadAmmo")]
        public static void LoadAmmoPatch(ref TurretComponent __instance)
        {
            if (__instance.itemId == ProtoID.I零素矢)
            {
                if (__instance.itemCount >= 1)
                {
                    __instance.itemCount++;
                }
            }
        }

        [HarmonyPatch(typeof(Mecha), nameof(Mecha.LoadAmmo))]
        [HarmonyPrefix]
        public static bool LoadAmmoPatch(Mecha __instance, bool findAnother = true)
        {
            if (__instance.ammoSelectSlot <= 0) {
                return false;
            }
            StorageComponent.GRID[] grids = __instance.ammoStorage.grids;
            int num = __instance.ammoSelectSlot - 1;
            int size = __instance.ammoStorage.size;
            ItemProto itemProto = (__instance.ammoItemId == 0) ? null : LDB.items.Select(__instance.ammoItemId);
            if (grids[num].count == 0) {
                if (__instance.autoReplenishAmmo) {
                    __instance.AutoReplenishAmmo();
                }
                int num2 = (grids[num].count > 0) ? num : -1;
                if (itemProto != null && findAnother) {
                    if (num2 < 0) {
                        for (int i = num; i < num + size; i++) {
                            int num3 = i % size;
                            if (grids[num3].itemId == __instance.ammoItemId && grids[num3].count > 0) {

                                num2 = num3;
                                break;
                            }
                        }
                    }
                    if (num2 < 0) {
                        for (int j = num; j < num + size; j++) {
                            int num4 = j % size;
                            ItemProto itemProto2 = (grids[num4].itemId == 0) ? null : LDB.items.Select(grids[num4].itemId);
                            if (itemProto2 != null && itemProto2.AmmoType == itemProto.AmmoType && grids[num4].count > 0) {
                                num2 = num4;
                                break;
                            }
                        }
                    }
                }
                if (findAnother && num2 < 0) {
                    for (int k = num; k < num + size; k++) {
                        int num5 = k % size;
                        if (grids[num5].itemId > 0 && grids[num5].count > 0) {
                            num2 = num5;
                            break;
                        }
                    }
                }
                if (num2 >= 0) {
                    __instance.ammoSelectSlot = num2 + 1;
                    num = num2;
                }
            }
            if (grids[num].count == 0) {
                return false;
            }
            int num6 = 0;
            int num7 = 1;
            int num8;
            if (grids[num].itemId == ProtoID.I零素矢) {
                grids[num].count++;
            }
            __instance.ammoStorage.TakeItemFromGrid(num, ref num6, ref num7, out num8);
            if (num7 > 0) {
                __instance.AddConsumptionStat(num6, num7, __instance.player.nearestFactory);
                ItemProto itemProto3 = LDB.items.Select(num6);
                if (itemProto3 != null) {
                    __instance.ammoItemId = num6;
                    __instance.ammoInc = ((num8 > 10) ? 10 : num8);
                    int num9 = itemProto3.HpMax;
                    int num10 = (int)((double)num9 * ((double)Cargo.incTable[num8] * 0.001) + ((num9 < 12) ? 0.51 : 0.1));
                    __instance.ammoBulletCount = num9 + num10;
                    if (grids[num].count == 0 && __instance.autoReplenishAmmo) {
                        __instance.AutoReplenishAmmo();
                    }
                }
            }
            return false;
        }

        // 让机枪塔放零素矢时只放一个
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UITurretWindow), nameof(UITurretWindow.AmmoBtn_onClick))]
        public static IEnumerable<CodeInstruction> UITurretWindow_AmmoBtn_onClick_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)100));

            object num = matcher.Advance(4).Operand;

            matcher.Advance(1).InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_S, num),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(UITurretWindow), nameof(UITurretWindow.player))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Player), nameof(Player.inhandItemId))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InfiniteReloadAmmo), nameof(ChangeInputAmmoCount))),
                new CodeInstruction(OpCodes.Stloc_S, num)
            );
            //matcher.LogInstructionEnumeration();
            return matcher.InstructionEnumeration();
        }

        public static int ChangeInputAmmoCount(int num, int itemId)
        {
            if (itemId == ProtoID.I零素矢) {
                num = (int)(num - 99);
            }
            return num;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.EntityFastFillIn))]
        public static IEnumerable<CodeInstruction> PlanetFactory_EntityFastFillIn_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(TurretComponent), nameof(TurretComponent.itemId))));

            object itemId = matcher.Advance(1).Operand;
            object num = matcher.Advance(5).Operand;

            matcher.Advance(1).InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_S, num),
                new CodeInstruction(OpCodes.Ldloc_S, itemId),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InfiniteReloadAmmo), nameof(ChangeInputAmmoCount))),
                new CodeInstruction(OpCodes.Stloc_S, num)
            );
            //matcher.LogInstructionEnumeration();
            return matcher.InstructionEnumeration();
        }
    }
}
