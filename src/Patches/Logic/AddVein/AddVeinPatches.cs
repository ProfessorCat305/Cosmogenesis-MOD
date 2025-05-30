﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CommonAPI;
using GalacticScale;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using ProjectGenesis.Utils;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.UI.GridLayoutGroup;
using static UnityEngine.UI.Image;
using static UnityEngine.UI.InputField;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable Unity.UnknownResource

namespace ProjectGenesis.Patches.Logic.AddVein
{
    public static partial class AddVeinPatches
    {
        internal const sbyte VeinTypeCount = 20;

        private static readonly Color32[] VeinColors =
        {
            new Color(0.538f, 0.538f, 0.538f), // Default
            new Color(0.288f, 0.587f, 0.858f), // 铁 1
            new Color(1.000f, 0.490f, 0.307f), // 铜
            new Color(0.214f, 0.745f, 0.531f), // 硅
            new Color(1.000f, 1.000f, 1.000f), // 钛
            new Color(0.483f, 0.461f, 0.444f), // 石
            new Color(0.113f, 0.130f, 0.140f), // 煤
            new Color(0.000f, 0.000f, 0.000f), // 油
            new Color(1.000f, 1.000f, 1.000f), // 可燃冰 8
            new Color(0.489f, 0.601f, 0.745f), // 金伯利 9
            new Color(0.066f, 0.290f, 0.160f), // 莫桑石 10
            new Color(0.538f, 0.613f, 0.078f), // 有机 11
            new Color(0.575f, 0.270f, 0.830f), // 光栅 12
            new Color(0.571f, 0.708f, 0.647f), // 刺笋 13
            new Color(0.349f, 0.222f, 0.247f), // 单极 14
            new Color(0.113f, 0.130f, 0.140f), // 石墨 15
            new Color(0.538f, 0.538f, 0.538f), // 深层岩浆 16
            new Color(0.685f, 0.792f, 0.000f), // 放射 17
            new Color(0.965f, 0.867f, 0.352f), // 黄铁 18
            new Color(0.000f, 0.000f, 0.000f), // 冰 19
        };

        internal static void ModifyVeinData()
        {
            AddVeinProtos(
                NewVein(15, "石墨矿脉", "I石墨矿", "Assets/texpack/钨矿脉", ProtoID.I石墨矿, 34, 1, 60),
                NewVein(16, "深层岩浆", "I深层岩浆", "Icons/Vein/oil-vein", ProtoID.I深层岩浆, 0, 6, 60),
                NewVein(17, "铀矿脉", "I铀矿", "Assets/texpack/放射晶体矿脉_新新", ProtoID.I放射性矿物, 35, 2, 90),
                NewVein(18, "黄铁矿脉", "I黄铁矿", "Assets/texpack/硫矿脉_新", ProtoID.I黄铁矿, 36, 1, 90),
                NewVein(19, "地下冰层", "I地下冰层", "Icons/Vein/oil-vein", ProtoID.I水, 0, 6, 60));
            return;

            VeinProto NewVein(int id, string name, string description, string iconPath, int miningItem, int miningEffect, int modelIndex,
                int miningTime) =>
                new VeinProto
                {
                    ID = id,
                    Name = name,
                    Description = description,
                    IconPath = iconPath,
                    MiningItem = miningItem,
                    MiningEffect = miningEffect,
                    ModelIndex = modelIndex,
                    MiningTime = miningTime,
                    CircleRadius = 1,
                    MinerBaseModelIndex = 58,
                    MinerCircleModelIndex = 59,
                    MiningAudio = 122,
                    ModelCount = 1,
                };
        }

        private static void AddVeinProtos(params VeinProto[] protos)
        {
            VeinProtoSet veins = LDB.veins;

            int dataArrayLength = veins.dataArray.Length;

            Array.Resize(ref veins.dataArray, dataArrayLength + protos.Length);

            for (var index = 0; index < dataArrayLength; ++index)
            {
                if (veins.dataArray[index].Name == "分形硅矿")
                {
                    veins.dataArray[index].Name = "莫桑石矿";
                    veins.dataArray[index].Description = "I莫桑石";
                }
                Debug.LogFormat("veins id {0} {1}, MinerBaseModelIndex{2} MiningEffect{3}", index, veins.dataArray[index].Name, veins.dataArray[index].MinerBaseModelIndex, veins.dataArray[index].MiningEffect);
                Debug.LogFormat("MinerCircleModelIndex {0} ModelIndex{1} MiningAudio {2} ModelCount{3}", veins.dataArray[index].MinerCircleModelIndex, veins.dataArray[index].ModelIndex, veins.dataArray[index].MiningAudio, veins.dataArray[index].ModelCount);
                Debug.LogFormat("CircleRadius {0} MiningItem {1} MiningTime {2}", veins.dataArray[index].CircleRadius, veins.dataArray[index].MiningItem, veins.dataArray[index].MiningTime);
                Debug.LogFormat("IconPath {0}", veins.dataArray[index].IconPath);
            }

            for (var index = 0; index < protos.Length; ++index)
            {
                if (protos[index].ID == 16 || protos[index].ID == 19)
                {
                    protos[index].MinerBaseModelIndex = 0;
                    protos[index].MinerCircleModelIndex = 0;
                    protos[index].CircleRadius = 1.5f;
                }
                veins.dataArray[dataArrayLength + index] = protos[index];
            }

            veins.OnAfterDeserialize();
        }

        internal static void SetMinerMk2Color()
        {
            Texture texture = Resources.Load<Texture>("Assets/texpack/矿机渲染索引");
            int veinColorTex = Shader.PropertyToID("_VeinColorTex");

            ref PrefabDesc prefabDesc = ref LDB.models.Select(256).prefabDesc;
            prefabDesc.materials[0].SetTexture(veinColorTex, texture);
            ref Material[] prefabDescLODMaterial = ref prefabDesc.lodMaterials[0];
            prefabDescLODMaterial[0].SetTexture(veinColorTex, texture);
            prefabDescLODMaterial[1].SetTexture(veinColorTex, texture);
            prefabDescLODMaterial[2].SetTexture(veinColorTex, texture);

            prefabDesc = ref LDB.models.Select(59).prefabDesc;
            prefabDescLODMaterial = ref prefabDesc.lodMaterials[0];
            prefabDescLODMaterial[1].SetTexture(veinColorTex, texture);
            prefabDescLODMaterial[2].SetTexture(veinColorTex, texture);
            prefabDescLODMaterial[3].SetTexture(veinColorTex, texture);
        }
        [HarmonyPatch(typeof(UISandboxMenu), nameof(UISandboxMenu.StaticLoad))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UISandboxMenu_StaticLoad_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(LDB), nameof(LDB.veins))), new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldlen));

            matcher.Advance(1).SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)14))
               .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop)).SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
               .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop));

            return matcher.InstructionEnumeration();
        }


        [HarmonyPatch(typeof(UISandboxMenu), nameof(UISandboxMenu.StaticLoad))]
        [HarmonyPostfix]
        public static void UISandboxMenu_StaticLoad_Postfix(ref VeinProto[,] ___veinProtos)
        {
            ___veinProtos[1, 7] = LDB.veins.Select(15);
            ___veinProtos[1, 8] = LDB.veins.Select(16);
        }


        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_GenerateVeins_ResizeVeinList_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)15));
            matcher.SetOperandAndAdvance(20);

            return matcher.InstructionEnumeration();
        }

        /*
        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_GenerateVeins_RemoveHeightLimit_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PlanetData), nameof(PlanetData.radius))),
                new CodeMatch
                {
                    opcodes = new List<OpCode>
                    {
                        OpCodes.Blt, OpCodes.Blt_S,
                    },
                });

            CodeMatcher matcher2 = matcher.Clone();
            matcher2.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_0), new CodeMatch(OpCodes.Stloc_S));
            Label label = matcher2.Labels.First();

            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Pop));
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Pop));
            matcher.SetAndAdvance(OpCodes.Br, label);

            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PlanetData), nameof(PlanetData.waterItemId))));
            matcher.Advance(1).SetOpcodeAndAdvance(OpCodes.Br);

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_GenerateVeins_RemoveVeinPositionBias_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop)).SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
               .SetOpcodeAndAdvance(OpCodes.Br_S);

            return matcher.InstructionEnumeration();
        }


        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_InitialVeins_Transpiler(IEnumerable<CodeInstruction> instructions,
            MethodBase original)
        {
            var matcher = new CodeMatcher(instructions);

            Type type = original.DeclaringType;

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Ldc_I4_2),
                new CodeMatch(OpCodes.Stfld, AccessTools.Field(type, "veinVectorCount")));

            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AddVeinPatches), nameof(InitBirthVeinVectors))));

            matcher.Advance(1).SetOpcodeAndAdvance(OpCodes.Ldc_I4_5);

            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_2));

            matcher.SetOpcodeAndAdvance(OpCodes.Ldc_I4_0);

            return matcher.InstructionEnumeration();
        }

        public static void InitBirthVeinVectors(PlanetAlgorithm algorithm)
        {
            //algorithm.veinVectorTypes[2] = EVeinType.Aluminum;
            //algorithm.veinVectors[2] = algorithm.planet.birthResourcePoint2;
            algorithm.veinVectorTypes[3] = EVeinType.Coal;
            algorithm.veinVectors[3] = algorithm.planet.birthResourcePoint3;
            algorithm.veinVectorTypes[4] = EVeinType.Stone;
            algorithm.veinVectors[4] = algorithm.planet.birthResourcePoint4;
        }


        [HarmonyPatch(typeof(PlanetData), nameof(PlanetData.GenBirthPoints), typeof(PlanetRawData), typeof(int))]
        [HarmonyPostfix]
        public static void PlanetData_GenBirthPoints_Postfix(PlanetData __instance, PlanetRawData rawData, int _birthSeed)
        {
            var dotNet35Random = new DotNet35Random(_birthSeed);
            Pose pose = __instance.PredictPose(85.0);
            Vector3 vector3 = Maths.QInvRotateLF(pose.rotation, __instance.star.uPosition - (VectorLF3)pose.position * 40000.0);
            vector3.Normalize();
            Vector3 x_direction = Vector3.Cross(vector3, Vector3.up).normalized;
            Vector3 y_direction = Vector3.Cross(x_direction, vector3).normalized;
            var num1 = 0;
            const int num2 = 512;

            for (; num1 < num2; ++num1)
            {
                float num3 = (float)(dotNet35Random.NextDouble() * 2.0 - 1.0) * 0.5f;
                float num4 = (float)(dotNet35Random.NextDouble() * 2.0 - 1.0) * 0.5f;
                Vector3 random = vector3 + num3 * x_direction + num4 * y_direction;
                random.Normalize();
                __instance.birthPoint = random * (float)(__instance.realRadius + 0.20000000298023224 + 1.4500000476837158);
                var tmpVector3 = Vector3.Cross(random, Vector3.up);
                x_direction = tmpVector3.normalized;
                tmpVector3 = Vector3.Cross(x_direction, random);
                y_direction = tmpVector3.normalized;

                for (var index = 0; index < 10; ++index)
                {
                    Vector2 rotate_0 = new Vector2((float)(dotNet35Random.NextDouble() * 2.0 - 1.0),
                        (float)(dotNet35Random.NextDouble() * 2.0 - 1.0)).normalized * 0.1f;
                    Vector2 rotate_1 = Rotate(rotate_0, 120);
                    Modify(dotNet35Random, ref rotate_1);
                    Vector2 rotate_2 = Rotate(rotate_0, 240);
                    Modify(dotNet35Random, ref rotate_2);
                    Vector2 rotate_3 = Rotate(rotate_0, 60);
                    Modify(dotNet35Random, ref rotate_3);

                    tmpVector3 = random + rotate_0.x * x_direction + rotate_0.y * y_direction;
                    __instance.birthResourcePoint0 = tmpVector3.normalized;

                    tmpVector3 = random + rotate_1.x * x_direction + rotate_1.y * y_direction;
                    __instance.birthResourcePoint1 = tmpVector3.normalized;

                    tmpVector3 = random + rotate_2.x * x_direction + rotate_2.y * y_direction;
                    __instance.birthResourcePoint2 = tmpVector3.normalized;

                    tmpVector3 = random + rotate_0.x * -2 * x_direction + rotate_0.y * -2 * y_direction;
                    __instance.birthResourcePoint3 = tmpVector3.normalized;

                    tmpVector3 = random + rotate_3.x * 2 * x_direction + rotate_3.y * 2 * y_direction;
                    __instance.birthResourcePoint4 = tmpVector3.normalized;

                    if (QueryHeightsNear(rawData, x_direction, y_direction, __instance.realRadius, random, __instance.birthResourcePoint0,
                        __instance.birthResourcePoint1, __instance.birthResourcePoint2, __instance.birthResourcePoint3,
                        __instance.birthResourcePoint4))
                        return;
                }
            }
        }

        */
        internal static Vector2 Rotate(Vector2 v, float angle)
        {
            float delta = angle * Mathf.PI / 180;

            return new Vector2(v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta), v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta));
        }
        private static void Modify(DotNet35Random dotNet35Random, ref Vector2 vector)
        {
            vector.x += (float)(dotNet35Random.NextDouble() * 2.0 - 1.0) * 0.006f;
            vector.y += (float)(dotNet35Random.NextDouble() * 2.0 - 1.0) * 0.006f;
        }

        private static bool QueryHeights(PlanetRawData rawData, float radius, params Vector3[] points)
        {
            return points.All(point => rawData.QueryHeight(point) > radius);
        }

        private static bool QueryHeightsNear(PlanetRawData rawData, Vector3 x_vector, Vector3 y_vector, float radius,
            params Vector3[] points)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Vector3 point in points)
            {
                Vector3 pos1 = point + x_vector * 0.03f;
                Vector3 pos2 = point - x_vector * 0.03f;
                Vector3 pos3 = point + y_vector * 0.03f;
                Vector3 pos4 = point - y_vector * 0.03f;

                if (!QueryHeights(rawData, radius, point, pos1, pos2, pos3, pos4)) return false;
            }

            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
        public static void CheckBuildConditionsPatch(BuildTool_Click __instance, ref bool __result)
        {
            for (int i = 0; i < __instance.buildPreviews.Count; i++)
            {
                BuildPreview buildPreview = __instance.buildPreviews[i];
                Vector3 vector2 = buildPreview.lpos;
                Quaternion quaternion = buildPreview.lrot;
                Pose lPose = new Pose(buildPreview.lpos, buildPreview.lrot);
                Vector3 forward = lPose.forward;
                Vector3 up = lPose.up;
                if (buildPreview.desc.veinMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    PrebuildData prebuildData = default(PrebuildData);
                    int paramCount = 0;
                    if (buildPreview.desc.isVeinCollector)
                    {
                        Vector3 center = vector2.normalized * __instance.controller.cmd.test.magnitude + forward * -10f;
                        int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center, 18f, ref BuildTool._tmp_ids);
                        prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                        VeinData[] veinPool = __instance.factory.veinPool;
                        EVeinType eVeinType = EVeinType.None;
                        for (int j = 0; j < veinsInAreaNonAlloc; j++)
                        {
                            if (BuildTool._tmp_ids[j] != 0 && veinPool[BuildTool._tmp_ids[j]].id == BuildTool._tmp_ids[j])
                            {
                                if (veinPool[BuildTool._tmp_ids[j]].type == EVeinType.Oil || veinPool[BuildTool._tmp_ids[j]].type == EVeinType.Ice || veinPool[BuildTool._tmp_ids[j]].type == EVeinType.DeepMagma || !MinerComponent.IsTargetVeinInRange(veinPool[BuildTool._tmp_ids[j]].pos, lPose, buildPreview.desc))
                                {
                                    continue;
                                }

                                if (eVeinType != veinPool[BuildTool._tmp_ids[j]].type)
                                {
                                    if (eVeinType == EVeinType.None)
                                    {
                                        eVeinType = veinPool[BuildTool._tmp_ids[j]].type;
                                    }
                                    else
                                    {
                                        buildPreview.condition = EBuildCondition.NeedSingleResource;
                                    }
                                }

                                prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[j];
                            }
                            else
                            {
                                Assert.CannotBeReached();
                            }
                        }
                    }
                    else
                    {
                        Vector3 center2 = vector2.normalized * __instance.controller.cmd.test.magnitude + forward * -1.2f;
                        int veinsInAreaNonAlloc2 = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center2, 12f, ref BuildTool._tmp_ids);
                        prebuildData.InitParametersArray(veinsInAreaNonAlloc2);
                        VeinData[] veinPool2 = __instance.factory.veinPool;
                        EVeinType eVeinType2 = EVeinType.None;
                        for (int k = 0; k < veinsInAreaNonAlloc2; k++)
                        {
                            if (BuildTool._tmp_ids[k] != 0 && veinPool2[BuildTool._tmp_ids[k]].id == BuildTool._tmp_ids[k])
                            {
                                if (veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.Oil || veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.Ice || veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.DeepMagma || !MinerComponent.IsTargetVeinInRange(veinPool2[BuildTool._tmp_ids[k]].pos, lPose, buildPreview.desc))
                                {
                                    continue;
                                }

                                if (eVeinType2 != veinPool2[BuildTool._tmp_ids[k]].type)
                                {
                                    if (eVeinType2 == EVeinType.None)
                                    {
                                        eVeinType2 = veinPool2[BuildTool._tmp_ids[k]].type;
                                    }
                                    else
                                    {
                                        buildPreview.condition = EBuildCondition.NeedResource;
                                    }
                                }

                                prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[k];
                            }
                            else
                            {
                                Assert.CannotBeReached();
                            }
                        }
                    }

                    prebuildData.paramCount = paramCount;
                    prebuildData.ArrangeParametersArray();
                    if (buildPreview.desc.isVeinCollector)
                    {
                        if (buildPreview.paramCount == 0)
                        {
                            buildPreview.parameters = new int[2048];
                            buildPreview.paramCount = 2048;
                        }

                        if (prebuildData.paramCount > 0)
                        {
                            Array.Resize(ref buildPreview.parameters, buildPreview.paramCount + prebuildData.paramCount);
                            Array.Copy(prebuildData.parameters, 0, buildPreview.parameters, buildPreview.paramCount, prebuildData.paramCount);
                            buildPreview.paramCount += prebuildData.paramCount;
                        }
                    }
                    else
                    {
                        buildPreview.parameters = prebuildData.parameters;
                        buildPreview.paramCount = prebuildData.paramCount;
                    }

                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    if (prebuildData.paramCount == 0)
                    {
                        buildPreview.condition = EBuildCondition.NeedResource;
                        continue;
                    }
                }
                else if (buildPreview.desc.oilMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    Vector3 vector4 = vector2;
                    Vector3 vector5 = -up;
                    int veinsInAreaNonAlloc3 = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector4, 10f, ref BuildTool._tmp_ids);
                    PrebuildData prebuildData2 = default(PrebuildData);
                    prebuildData2.isDestroyed = false;
                    prebuildData2.InitParametersArray(veinsInAreaNonAlloc3);
                    VeinData[] veinPool3 = __instance.factory.veinPool;
                    int num2 = 0;
                    float num3 = 100f;
                    Vector3 pos = vector4;
                    for (int l = 0; l < veinsInAreaNonAlloc3; l++)
                    {
                        if (BuildTool._tmp_ids[l] != 0 && veinPool3[BuildTool._tmp_ids[l]].id == BuildTool._tmp_ids[l] && (veinPool3[BuildTool._tmp_ids[l]].type == EVeinType.Oil || veinPool3[BuildTool._tmp_ids[l]].type == EVeinType.Ice || veinPool3[BuildTool._tmp_ids[l]].type == EVeinType.DeepMagma))
                        {
                            Vector3 pos2 = veinPool3[BuildTool._tmp_ids[l]].pos;
                            Vector3 vector6 = pos2 - vector4;
                            float num4 = Vector3.Dot(vector5, vector6);
                            float sqrMagnitude = (vector6 - vector5 * num4).sqrMagnitude;
                            if (sqrMagnitude < num3)
                            {
                                num3 = sqrMagnitude;
                                num2 = BuildTool._tmp_ids[l];
                                pos = pos2;
                                buildPreview.condition = EBuildCondition.Ok;
                            }
                        }
                    }

                    if (num2 == 0)
                    {
                        buildPreview.condition = EBuildCondition.NeedResource;
                        continue;
                    }

                    prebuildData2.parameters[0] = num2;
                    prebuildData2.paramCount = 1;
                    prebuildData2.ArrangeParametersArray();
                    buildPreview.parameters = prebuildData2.parameters;
                    buildPreview.paramCount = prebuildData2.paramCount;
                    Vector3 vector7 = __instance.factory.planet.aux.Snap(pos, onTerrain: true);
                    vector2 = (lPose.position = (buildPreview.lpos2 = (buildPreview.lpos = vector7)));
                    quaternion = (lPose.rotation = (buildPreview.lrot2 = (buildPreview.lrot = Maths.SphericalRotation(vector7, __instance.yaw))));
                    forward = lPose.forward;
                    up = lPose.up;
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                }
            }

            __result = true;
            for (int num86 = 0; num86 < __instance.buildPreviews.Count; num86++)
            {
                BuildPreview buildPreview4 = __instance.buildPreviews[num86];
                if (buildPreview4.condition != 0 && buildPreview4.condition != EBuildCondition.NeedConn)
                {
                    __result = false;
                    __instance.actionBuild.model.cursorState = -1;
                    __instance.actionBuild.model.cursorText = buildPreview4.conditionText;
                    break;
                }
            }
            if (__result)
            {
                __instance.actionBuild.model.cursorState = 0;
                __instance.actionBuild.model.cursorText = BuildPreview.GetConditionText(EBuildCondition.Ok);
            }
        }


        public static void AddBirthGalaxyRareVein(PlanetAlgorithm algorithm, ref int[] array, ref float[] array2, ref float[] array3)
        {
            //star id是1，star index是0
            if (algorithm.planet.star.id == 1)
            {
                if (algorithm.planet.index == 3)
                {
                    array[8] = 7;
                    array2[8] = 0.4f;
                    array3[8] = 0.8f;

                    array[17] = 4;
                    array2[17] = 0.2f;
                    array3[17] = 0.5f;

                    array[18] = 3;
                    array2[18] = 0.2f;
                    array3[18] = 0.5f;

                    array[19] = 12;
                    array2[19] = 1.0f;
                    array3[19] = 1.0f;
                }
                if (algorithm.planet.index == 0)
                {
                    array[17] = 4;
                    array2[17] = 0.3f;
                    array3[17] = 0.7f;
                }
            }
        }

        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        //[HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins))]  PlanetAlgorithm7的OIL特判只有7处，其他都是8处
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm_GenerateVeins_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            //matcher.MatchForward(false, new CodeMatch(OpCodes.Stelem_R4), new CodeMatch(OpCodes.Ldc_I4_0));
            //matcher.Advance(1).InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0),
            //    new CodeInstruction(OpCodes.Ldloca_S, 11),
            //    new CodeInstruction(OpCodes.Ldloca_S, 12),
            //    new CodeInstruction(OpCodes.Ldloca_S, 13),
            //   new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AddVeinPatches), nameof(AddBirthGalaxyRareVein))));



            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            object index = matcher.Operand;
            object jumpTo = matcher.Advance(2).Operand;
            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo));


            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ceq));
            matcher
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, jumpTo));


            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;
            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo));

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ceq));
            matcher
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, jumpTo));

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ceq));
            matcher
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, jumpTo));


            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;
            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo));


            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(3).Operand;
            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo));

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(3).Operand;

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ceq));
            matcher
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, jumpTo));

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlanetAlgorithm7_GenerateVeins_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            object index = matcher.Operand;
            object jumpTo = matcher.Advance(2).Operand;
            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo));

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;
            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo));

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ceq));
            matcher
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, jumpTo));

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ceq));
            matcher
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index)).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, jumpTo));


            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(2).Operand;
            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo));


            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(3).Operand;
            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19)).InsertAndAdvance(new CodeInstruction(OpCodes.Beq_S, jumpTo));

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))), new CodeMatch(OpCodes.Ldc_I4_7));
            index = matcher.Operand;
            jumpTo = matcher.Advance(3).Operand;

            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ceq));
            matcher
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)16))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, index))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VeinData), nameof(VeinData.type))))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)19))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Ceq)).InsertAndAdvance(new CodeInstruction(OpCodes.Or))
               .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, jumpTo));

            return matcher.InstructionEnumeration();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIVeinDetailNode), "_OnUpdate")]
        public static void _OnUpdatePatch(ref UIVeinDetailNode __instance)
        {
            VeinGroup veinGroup = __instance.inspectFactory.veinGroups[__instance.veinGroupIndex];
            if (veinGroup.count == 0 || veinGroup.type == EVeinType.None)
            {
                __instance._Close();
                return;
            }

            if (__instance.counter % 4 == 0 && __instance.showingAmount != veinGroup.amount)
            {
                __instance.showingAmount = veinGroup.amount;
                if (veinGroup.type == EVeinType.DeepMagma || veinGroup.type == EVeinType.Ice)
                {
                    __instance.infoText.text = veinGroup.count + "空格个".Translate() + __instance.veinProto.name + "产量".Translate() + ((float)veinGroup.amount * VeinData.oilSpeedMultiplier).ToString("0.0000") + "/s";
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIVeinDetailNode), "Refresh")]
        public static void RefreshPatch(ref UIVeinDetailNode __instance)
        {
            if (__instance.inspectFactory == null)
            {
                return;
            }

            if (__instance.veinGroupIndex >= __instance.inspectFactory.veinGroups.Length)
            {
                __instance._Close();
                return;
            }

            VeinGroup veinGroup = __instance.inspectFactory.veinGroups[__instance.veinGroupIndex];
            if (veinGroup.count == 0 || veinGroup.type == EVeinType.None)
            {
                __instance._Close();
                return;
            }

            __instance.veinProto = LDB.veins.Select((int)veinGroup.type);
            if (__instance.veinProto != null)
            {
                __instance.veinIcon.sprite = __instance.veinProto.iconSprite;
                __instance.showingAmount = veinGroup.amount;
                if (veinGroup.type == EVeinType.DeepMagma || veinGroup.type == EVeinType.Ice)
                {
                    __instance.infoText.text = veinGroup.count + "空格个".Translate() + __instance.veinProto.name + "产量".Translate() + ((float)veinGroup.amount * VeinData.oilSpeedMultiplier).ToString("0.0000") + "/s";
                    return;
                }
            }
            else
            {
                __instance.veinIcon.sprite = null;
                __instance.showingAmount = veinGroup.amount;
                if (__instance.menuButton != null)
                {
                    __instance.menuButton.gameObject.SetActive(value: false);
                }

                if (veinGroup.type == EVeinType.DeepMagma || veinGroup.type == EVeinType.Ice)
                {
                    __instance.infoText.text = veinGroup.count + "空格个".Translate() + " ? " + "产量".Translate() + ((float)veinGroup.amount * VeinData.oilSpeedMultiplier).ToString("0.0000") + "/s";
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIResAmountEntry), "SetInfo")]
        public static void SetInfoPatch(ref UIResAmountEntry __instance, string label, ref string strBuilderFormat)
        {
            if (label.Equals("深层岩浆") || label.Equals("水"))
            {
                strBuilderFormat = "         /s";
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIPlanetDetail), "RefreshDynamicProperties")]
        public static bool RefreshDynamicPropertiesPatch(ref UIPlanetDetail __instance)
        {
            if (__instance.veinAmounts == null)
            {
                __instance.veinAmounts = new long[64];
            }

            if (__instance.veinCounts == null)
            {
                __instance.veinCounts = new int[64];
            }

            Array.Clear(__instance.veinAmounts, 0, __instance.veinAmounts.Length);
            Array.Clear(__instance.veinCounts, 0, __instance.veinCounts.Length);
            bool isInfiniteResource = GameMain.data.gameDesc.isInfiniteResource;
            if (__instance.planet == null)
            {
                return true;
            }

            if (!__instance._scanned && __instance.planet.scanned)
            {
                __instance.OnPlanetDataSet();
                return true;
            }

            __instance._scanned = __instance.planet.scanned;
            int num = ((__instance.planet == GameMain.localPlanet) ? 1 : ((__instance.planet.star == GameMain.localStar) ? 2 : (((GameMain.mainPlayer.uPosition - __instance.planet.uPosition).magnitude < 14400000.0) ? 3 : 4)));
            bool flag = GameMain.history.universeObserveLevel >= num;
            if (__instance.planet.factory != null && GameMain.history.universeObserveLevel >= 1)
            {
                flag = true;
            }

            if (__instance._scanned && flag)
            {
                __instance.planet.SummarizeVeinAmountsByFilter(ref __instance.veinAmounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
                __instance.planet.SummarizeVeinCountsByFilter(ref __instance.veinCounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
            }

            foreach (UIResAmountEntry entry in __instance.entries)
            {
                if (entry.refId <= 0)
                {
                    continue;
                }

                if (flag)
                {
                    if (entry.refId == 7 || entry.refId == (int)EVeinType.DeepMagma || entry.refId == (int)EVeinType.Ice)
                    {
                        double num2 = (double)__instance.veinAmounts[entry.refId] * (double)VeinData.oilSpeedMultiplier;
                        if (__instance.uiGame.veinAmountDisplayFilter == 1)
                        {
                            num2 *= (double)GameMain.history.miningSpeedScale;
                        }

                        StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 8, (float)num2);
                        entry.DisplayStringBuilder();
                    }
                    else
                    {
                        long num3 = __instance.veinAmounts[entry.refId];
                        int num4 = __instance.veinCounts[entry.refId];
                        if (isInfiniteResource)
                        {
                            StringBuilderUtility.WriteCommaULong(entry.sb, 0, 16, (ulong)num4);
                        }
                        else if (num3 < 1000000000)
                        {
                            StringBuilderUtility.WriteCommaULong(entry.sb, 0, 16, (ulong)num3);
                        }
                        else
                        {
                            StringBuilderUtility.WriteKMG(entry.sb, 15, num3);
                        }

                        entry.DisplayStringBuilder();
                    }

                    entry.SetObserved(_observed: true);
                }
                else
                {
                    entry.valueString = "未知".Translate();
                    if (entry.refId > 7)
                    {
                        entry.overrideLabel = "未知珍奇信号".Translate();
                    }

                    if (entry.refId > 7)
                    {
                        entry.SetObserved(_observed: false);
                    }
                    else
                    {
                        entry.SetObserved(_observed: true);
                    }
                }
            }

            if (__instance.tipEntry != null)
            {
                if (!flag)
                {
                    __instance.tipEntry.valueString = "宇宙探索等级".Translate() + num;
                }
                else
                {
                    __instance.tipEntry.valueString = "";
                }

                __instance.SetResCount(flag ? (__instance.entries.Count - 1) : __instance.entries.Count);
            }
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarDetail), "RefreshDynamicProperties")]
        public static bool UIStarDetailRefreshDynamicPropertiesPatch(ref UIStarDetail __instance)
        {
            bool isInfiniteResource = GameMain.data.gameDesc.isInfiniteResource;
            if (__instance.veinAmounts == null)
            {
                __instance.veinAmounts = new long[64];
            }

            if (__instance.veinCounts == null)
            {
                __instance.veinCounts = new int[64];
            }

            Array.Clear(__instance.veinAmounts, 0, __instance.veinAmounts.Length);
            Array.Clear(__instance.veinCounts, 0, __instance.veinCounts.Length);
            if (__instance.star == null)
            {
                return true;
            }

            if (!__instance.calculated && __instance.star.scanned)
            {
                __instance.OnStarDataSet();
                return false;
            }

            __instance.calculated = __instance.star.scanned;
            bool num = __instance.observed;
            double magnitude = (__instance.star.uPosition - GameMain.mainPlayer.uPosition).magnitude;
            int num2 = ((__instance.star == GameMain.localStar) ? 2 : ((magnitude < 14400000.0) ? 3 : 4));
            __instance.observed = GameMain.history.universeObserveLevel >= num2;
            if (num != __instance.observed)
            {
                __instance.OnStarDataSet();
                return true;
            }

            __instance.loadingTextGo.SetActive(__instance.observed && !__instance.calculated);
            if (__instance.calculated && __instance.observed)
            {
                __instance.star.CalcVeinAmounts(ref __instance.veinAmounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
                __instance.star.CalcVeinCounts(ref __instance.veinCounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
            }

            foreach (UIResAmountEntry entry in __instance.entries)
            {
                if (entry.refId <= 0)
                {
                    continue;
                }

                if (__instance.observed)
                {
                    long num3 = __instance.veinAmounts[entry.refId];
                    long value = __instance.veinCounts[entry.refId];
                    if (entry.refId == 7 || entry.refId == (int)EVeinType.DeepMagma || entry.refId == (int)EVeinType.Ice)
                    {
                        double num4 = (double)num3 * (double)VeinData.oilSpeedMultiplier;
                        if (__instance.uiGame.veinAmountDisplayFilter == 1)
                        {
                            num4 *= (double)GameMain.history.miningSpeedScale;
                        }

                        StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 8, (float)num4);
                        entry.DisplayStringBuilder();
                    }
                    else
                    {
                        if (isInfiniteResource)
                        {
                            StringBuilderUtility.WriteCommaULong(entry.sb, 0, 16, (ulong)value);
                        }
                        else if (num3 < 1000000000)
                        {
                            StringBuilderUtility.WriteCommaULong(entry.sb, 0, 16, (ulong)num3);
                        }
                        else
                        {
                            StringBuilderUtility.WriteKMG(entry.sb, 15, num3);
                        }

                        entry.DisplayStringBuilder();
                    }

                    entry.SetObserved(_observed: true);
                }
                else
                {
                    entry.valueString = "未知".Translate();
                    if (entry.refId > 7)
                    {
                        entry.overrideLabel = "未知珍奇信号".Translate();
                    }

                    if (entry.refId > 7)
                    {
                        entry.SetObserved(_observed: false);
                    }
                    else
                    {
                        entry.SetObserved(_observed: true);
                    }
                }
            }

            if (__instance.tipEntry != null)
            {
                if (!__instance.observed)
                {
                    __instance.tipEntry.valueString = "宇宙探索等级".Translate() + num2;
                }
                else
                {
                    __instance.tipEntry.valueString = "";
                }

                __instance.SetResCount(__instance.observed ? (__instance.entries.Count - 1) : __instance.entries.Count);
            }
            return false;
        }
    }
}
