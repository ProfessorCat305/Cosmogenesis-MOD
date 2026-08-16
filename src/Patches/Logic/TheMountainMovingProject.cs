using ProjectOrbitalRing.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectOrbitalRing.Patches.Logic
{
    public static class TheMountainMovingMappings
    {
        public static readonly int[] OreIdList = new int[]
        {
            1001,1002,1003,1004,1005,1006,1011,1012,1013,1014,1015,1016,1117,6207,7706,6222
        };
        // 第一步：定义基础的正向映射（编码阶段定死的核心数据）
        private static readonly Dictionary<int, int> _forwardMap = new Dictionary<int, int>()
        {
            {1001, ProtoID.I乾坤铁},
            {1002, 7809},
            {1003, 7813},
            {1004, 7815},
            {1005, 7812},
            {1006, 7810},
            {1011, 7820},
            {1012, 7811},
            {1013, 7814},
            {1014, 7818},
            {1015, 7821},
            {1016, 7819},
            {1117, 7822},
            {6207, 7808},
            {7706, 7816},
            {6222, 7817},
        };

        // 第二步：基于正向映射自动构建反向映射（避免手动维护两份数据，减少出错）
        private static readonly ReadOnlyDictionary<int, int> getQianKunMap;
        private static readonly ReadOnlyDictionary<int, int> getOreMap;

        // 静态构造函数：初始化只读字典（仅执行一次）
        static TheMountainMovingMappings()
        {
            // 正向只读字典（对外不可修改）
            getQianKunMap = new ReadOnlyDictionary<int, int>(_forwardMap);

            // 构建反向字典并包装为只读
            var reverseMap = new Dictionary<int, int>();
            foreach (var pair in _forwardMap) {
                // 确保B组值唯一（一一对应，避免重复）
                if (reverseMap.ContainsKey(pair.Value)) {
                    throw new InvalidOperationException($"B组值 {pair.Value} 重复，不符合一一对应规则");
                }
                reverseMap.Add(pair.Value, pair.Key);
            }
            getOreMap = new ReadOnlyDictionary<int, int>(reverseMap);
        }

        public static int GetQianKunItemId(int oreId)
        {
            if (getQianKunMap.TryGetValue(oreId, out int QianKunItemId)) {
                return QianKunItemId;
            }
            return oreId;
        }

        public static int GetOreId(int qianKunItemId)
        {
            if (getOreMap.TryGetValue(qianKunItemId, out int oreId)) {
                return oreId;
            }
            return qianKunItemId;
        }
    }
    internal class TheMountainMovingProject
    {
        public static void MineAllVein(ref PlayerAction_Mine __instance, PlanetFactory factory, int miningTick, double energyGet, double energyChange)
        {
            if (__instance.player.package.GetItemCount(ProtoID.I乾坤) <= 0) {
                return;
            }
            bool isInfiniteResource = GameMain.data.gameDesc.isInfiniteResource;
            VeinData veinData = factory.GetVeinData(__instance.miningId);
            __instance.miningProtoId = (int)veinData.type;
            VeinProto veinProto = LDB.veins.Select((int)veinData.type);
            if (veinProto != null) {
                __instance.miningTick += miningTick;
                __instance.player.mecha.coreEnergy -= energyGet;
                __instance.player.mecha.MarkEnergyChange(5, 0.0 - energyChange);
                __instance.percent = Mathf.Clamp01((float)((double)__instance.miningTick / (double)(veinProto.MiningTime * 10000)));
                while (__instance.miningTick >= veinProto.MiningTime * 10000) {
                    __instance.miningTick -= veinProto.MiningTime * 10000;
                    if (factory.veinPool[__instance.miningId].amount > 0) {
                        int mineCount = 1;
                        int addToPackageItem = veinProto.MiningItem;
                        int itemId = ProtoID.I乾坤;
                        int count = 1;
                        int mountainMoveItem = TheMountainMovingMappings.GetQianKunItemId(veinProto.MiningItem);
                        if (mountainMoveItem != veinProto.MiningItem) {
                            __instance.player.TakeItemFromPlayer(ref itemId, ref count, out _, true, null);
                            if (itemId == ProtoID.I乾坤 && count == 1) {
                                mineCount = (int)(factory.veinPool[__instance.miningId].amount / GameMain.history.miningCostRate);
                                addToPackageItem = mountainMoveItem;
                            }
                        } else {
                            return;
                        }
                        if (mineCount <= 0) {
                            mineCount = 999999999;
                        }
                        int num13 = __instance.player.TryAddItemToPackage(addToPackageItem, mineCount, 0, throwTrash: true);
                        __instance.AddProductionStat(veinProto.MiningItem, mineCount, factory);
                        __instance.miningFlag |= 1 << (int)veinData.type;
                        __instance.veinMiningFlag |= 1 << (int)veinData.type;
                        if (num13 > 0) {
                            UIItemup.Up(addToPackageItem, num13);
                            UIRealtimeTip.PopupItemGet(addToPackageItem, num13, veinData.pos + veinData.pos.normalized * 2f, 0);
                        }

                        if (!isInfiniteResource) {
                            factory.veinPool[__instance.miningId].amount -= mineCount;
                            factory.veinGroups[veinData.groupIndex].amount -= mineCount;
                        }

                        VFEffectEmitter.Emit(veinProto.MiningEffect, veinData.pos, Maths.SphericalRotation(veinData.pos, 0f));
                        VFAudio.Create(veinProto.MiningAudio, null, veinData.pos, play: true, 0, -1, -1L);
                        int amount = factory.veinPool[__instance.miningId].amount;
                        factory.veinAnimPool[__instance.miningId].time = ((amount >= 20000) ? 0f : (1f - (float)amount * 5E-05f));
                        __instance.controller.gameData.history.AddFeatureValue(2200000 + veinProto.MiningItem, 1);
                    }

                    if (factory.veinPool[__instance.miningId].amount <= 0) {
                        int groupIndex = veinData.groupIndex;
                        VFEffectEmitter.Emit(veinProto.MiningEffect, veinData.pos, Maths.SphericalRotation(veinData.pos, 0f));
                        VFAudio.Create(veinProto.MiningAudio, null, veinData.pos, play: true, 0, -1, -1L);
                        GameMain.data.warningSystem.Broadcast(EBroadcastVocal.MineralDepleted, factory, (int)veinData.type, veinData.pos);
                        GameMain.data.statistics.charts.NotifyOnVeinExhausted(factory.planet.astroId, veinData.groupIndex, veinData.pos);
                        factory.RemoveVeinWithComponents(veinData.id);
                        factory.RecalculateVeinGroup(groupIndex);
                        __instance.miningTick = 0;
                        __instance.miningType = EObjectType.None;
                        __instance.miningId = 0;
                        if (GameMain.gameScenario != null) {
                            GameMain.gameScenario.NotifyOnVeinExhausted();
                        }

                        break;
                    }
                }
            } else {
                __instance.miningTick = 0;
                __instance.percent = 0f;
                factory.RemoveVeinWithComponents(veinData.id);
                factory.RecalculateAllVeinGroups();
            }
        }
    }
}
