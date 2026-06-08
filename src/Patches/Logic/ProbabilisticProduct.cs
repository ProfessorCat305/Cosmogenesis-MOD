using HarmonyLib;
using ProjectOrbitalRing.Patches.Logic.OrbitalRing;
using ProjectOrbitalRing.Utils;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static ProjectOrbitalRing.ProjectOrbitalRing;

namespace ProjectOrbitalRing.Patches.Logic
{
    internal class ProbabilisticProduct
    {
        public static void ProbabilisticProductProcess(int recipeId, ref int[] produced, int[] productRegister)
        {
            DotNet35Random DotNet35Random = new DotNet35Random();
            double random = DotNet35Random.NextDouble();
            int[] products = RecipeProto.recipeExecuteData[recipeId].products;
            switch (recipeId) {
                // 气云超精炼 已废弃
                //case 510:
                //    if (random > 0.00001 && random <= 0.90001) {
                //        produced[2]--;
                //        productRegister[products[2]]--;
                //    }
                //    if (random > 0.05001 && random <= 0.9501) {
                //        produced[3]--;
                //        productRegister[products[3]]--;
                //    }
                //    break;
                case 517:
                    if (random > 0.00001 && random <= 0.25001) {
                        produced[0]--;
                        productRegister[products[0]]--;
                    } else if (random > 0.25001 && random <= 0.50001) {
                        produced[0] -= 2;
                        productRegister[products[0]] -= 2;
                    } else if (random > 0.50001 && random <= 0.75001) {
                        produced[0] -= 3;
                        productRegister[products[0]] -= 3;
                    }
                    break;
                case 774:
                    if (random > 0.00001 && random <= 0.40001) {
                        produced[0]--;
                        productRegister[products[0]]--;
                    } else if (random > 0.40001 && random <= 0.50001) {
                        produced[0] -= 2;
                        productRegister[products[0]] -= 2;
                    }
                    break;
                case 784:
                    if (random > 0.00001 && random <= 0.50001) {
                        produced[0]--;
                        productRegister[products[0]]--;
                    } else if (random > 0.50001 && random <= 0.55001) {
                        produced[0] -= 2;
                        productRegister[products[0]] -= 2;
                    }
                    break;
                case 705:
                    if (random > 0.00001 && random <= 0.50001) {
                        produced[0]--;
                        productRegister[products[0]]--;
                    }
                    break;
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(AssemblerComponent), nameof(AssemblerComponent.InternalUpdate))]
        public static IEnumerable<CodeInstruction> AssemblerComponent_InternalUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            // 插入概率性产物的逻辑
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(AssemblerComponent), nameof(AssemblerComponent.recipeExecuteData))),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(RecipeExecuteData), nameof(RecipeExecuteData.productCounts)))
                );
            matcher.Advance(1).InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(AssemblerComponent), nameof(AssemblerComponent.recipeId))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(AssemblerComponent), nameof(AssemblerComponent.produced))),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ProbabilisticProduct), nameof(ProbabilisticProductProcess)))
            );

            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(AssemblerComponent), nameof(AssemblerComponent.extraSpeed)))
                );

            matcher.Advance(-2).InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(AssemblerComponent), nameof(AssemblerComponent.recipeId))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(AssemblerComponent), nameof(AssemblerComponent.produced))),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ProbabilisticProduct), nameof(ProbabilisticProductProcess)))
            );

            //matcher.LogInstructionEnumeration();
            return matcher.InstructionEnumeration();
        }

    }
}
