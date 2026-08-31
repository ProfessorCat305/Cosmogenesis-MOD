using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using static ProjectOrbitalRing.ProjectOrbitalRing;
using System;

namespace ProjectOrbitalRing.Patches.Logic
{
    public static class DrawLocalOutlineMeshNowPatches
    {
        [HarmonyPatch(typeof(PlayerControlGizmo), nameof(PlayerControlGizmo.DrawLocalOutlineMeshNow), new Type[]
        {
            typeof(PrefabDesc), typeof(int), typeof(Vector3), typeof(Quaternion),
            typeof(int), typeof(int), typeof(AnimData)
        }, new[]
        {
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal,
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref,
        })]
        //[HarmonyPatch(typeof(PlayerControlGizmo), nameof(PlayerControlGizmo.DrawLocalOutlineMeshNow))]
        [HarmonyPrefix]
        public static bool PlayerControlGizmo_DrawLocalOutlineMeshNow_Patch(PlayerControlGizmo __instance, PrefabDesc pdesc, int astroId, Vector3 pos, Quaternion rot, int modelId, int objId, ref AnimData anim)
        {
            Vector4 value = new Vector4(rot.x, rot.y, rot.z, rot.w);
            Vector3 zero = Vector3.zero;
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            bool flag = false;
            ObjectRenderer objectRenderer = __instance.gpuiManager.GetObjectRenderer(pdesc.modelIndex);
            if (objectRenderer != null && objectRenderer.lodBatches != null && objectRenderer.lodBatches.Length != 0) {
                BatchRenderer batchRenderer = objectRenderer.lodBatches[0];
                if (pdesc.lodMeshes != null && pdesc.lodMeshes.Length != 0 && pdesc.lodMeshes[0] != null) {
                    uint value2 = 0U;
                    LabRenderer labRenderer = objectRenderer as LabRenderer;
                    if (labRenderer != null) {
                        value2 = labRenderer.statePool[modelId];
                    }
                    Material[] materials = batchRenderer.materials;
                    int num = pdesc.castShadow;
                    if (num < 1) {
                        num = 1;
                    }
                    int num2 = 0;
                    while (num2 < materials.Length && num2 < num) {
                        if (materials[num2] == null) {
                            num2++;
                            continue;
                        }
                        materials[num2].SetInt("_Mono_Inst", 1);
                        materials[num2].SetInt("_Mono_AstroId", astroId);
                        materials[num2].SetVector("_Mono_Pos", pos);
                        materials[num2].SetVector("_Mono_Pos2", pos);
                        materials[num2].SetVector("_Mono_Rot", value);
                        materials[num2].SetVector("_Mono_Rot2", value);
                        materials[num2].SetVector("_Mono_Scl", one);
                        materials[num2].SetInt("_Mono_State", (int)value2);
                        materials[num2].SetInt("_Mono_ObjID", objId);
                        materials[num2].SetFloat("_Mono_Anim_Time", anim.time);
                        materials[num2].SetFloat("_Mono_Anim_LP", anim.prepare_length);
                        materials[num2].SetFloat("_Mono_Anim_LW", anim.working_length);
                        materials[num2].SetInt("_Mono_Anim_State", (int)anim.state);
                        materials[num2].SetFloat("_Mono_Anim_Power", anim.power);
                        int passCount = materials[num2].passCount;
                        for (int i = 0; i < passCount; i++) {
                            if (materials[num2].SetPass(i)) {
                                flag = true;
                                Graphics.DrawMeshNow(batchRenderer.mesh, zero, identity, num2);
                                break;
                            }
                        }
                        materials[num2].SetInt("_Mono_Inst", 0);
                        num2++;
                    }
                }
            }
            if (flag) {
                __instance._tmp_outline_local_pos += pos;
                __instance._tmp_outline_local_objcnt++;
            }
            return false;
        }
    }
}
