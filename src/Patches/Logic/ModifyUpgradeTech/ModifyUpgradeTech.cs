﻿using CommonAPI.Systems;
using HarmonyLib;
using ProjectGenesis.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using WinAPI;
using static GalacticScale.PatchOnUIGalaxySelect;
using static ProjectGenesis.Patches.Logic.ModifyUpgradeTech.AddUpgradeTech;

namespace ProjectGenesis.Patches.Logic.ModifyUpgradeTech
{
    internal static class ModifyUpgradeTech
    {
        private static readonly int[] Items5 =
                                      {
                                          6001, 6002, 6003, 6004,
                                          6005,
                                      },
                                      Items4 =
                                      {
                                          6001, 6002, 6003, 6004,
                                      },
                                      Items3 = { 6001, 6002, 6003, },
                                      Items2 = { 6001, 6002, };

        private static readonly int[] unlockHandcraftRecipes =
        {
            21, 26, 28, 29, 34, 36, 38, 39, 41, 42, 43, 44, 47, 51, 52, 53,
            54, 57, 70, 71, 72, 73, 80, 81, 99, 100, 101, 105, 109, 115, 116, 119,
            124, 128, 132, 135, 140, 141, 142, 143, 145, 146, 153, 154, 155, 156, 157, 159,
            402, 403, 408, 416, 418, 424, 425, 519, 523, 802, 709, 710, 716, 751, 752, 754, 771,
            772, 783, 785, 789, 793, 794, 795,
        };

        private static readonly int[] unlockWreckFallingItemIdLevel1 =
        {
            1108, 1109, 1112, 1202, 1301, 5206,
        };
        private static readonly float[] unlockWreckFallingItemDropCountLevel1 =
        {
            1.8f, 1.6f, 1.0f, 2.0f, 2.0f, 2.5f
        };
        private static readonly int[] unlockWreckFallingItemIdLevel2 =
        {
            1103, 1105, 1111, 1131, 1203, 1401,
        };
        private static readonly float[] unlockWreckFallingItemDropCountLevel2 =
        {
            1.56f, 1.8f, 1.4f, 2.0f, 1.2f, 1.3f
        };

        private static readonly int[] unlockWreckFallingItemIdLevel3 =
        {
            1106, 1115, 1123, 1204, 1404, 1407,
        };
        private static readonly float[] unlockWreckFallingItemDropCountLevel3 =
        {
            2.2f, 1.0f, 1.0f, 0.8f, 1.4f, 1.0f
        };
        private static readonly int[] unlockWreckFallingItemIdLevel4 =
        {
            1107, 1113, 1119, 1124, 1205, 1405, 5201
        };
        private static readonly float[] unlockWreckFallingItemDropCountLevel4 =
        {
            1.1f, 1.2f, 1.4f, 0.8f, 0.5f, 0.6f, 2.5f
        };
        private static readonly int[] unlockWreckFallingItemIdLevel5 =
        {
            1118, 1126, 1303, 5203, 6277, 7804,
        };
        private static readonly float[] unlockWreckFallingItemDropCountLevel5 =
        {
            0.8f, 0.55f, 0.75f, 1.5f, 1.2f, 0.7f,
        };
        private static readonly int[] unlockWreckFallingItemIdLevel6 =
        {
            1125, 1127, 1206, 1402, 5202, 6201,
        };
        private static readonly float[] unlockWreckFallingItemDropCountLevel6 =
        {
            0.4f, 0.4f, 0.4f, 0.25f, 1.5f, 0.6f,
        };
        private static readonly int[] unlockWreckFallingItemIdLevel7 =
        {
            1014, 1210, 1304, 1406, 5204, 6243,
        };
        private static readonly float[] unlockWreckFallingItemDropCountLevel7 =
        {
            2f, 0.3f, 0.4f, 0.3f, 1.4f, 0.4f,
        };
        private static readonly int[] unlockWreckFallingItemIdLevel8 =
        {
            1016, 1209, 5205, 6271, 7805,
        };
        private static readonly float[] unlockWreckFallingItemDropCountLevel8 =
        {
            2f, 0.4f, 1.5f, 0.3f, 0.3f,
        };

        private static int WreckFallingLevel = 0;

        private static int UAVHPAndfiringRateUpgradeLevel = 0;

        private static bool isUnlockRecipesHandcraft = false;
        
        private static bool isUnlockCrackingRay = false;

        private static int vanillaTechSpeed = 1;
        private static int synapticLatheTechSpeed = 1;

        private static bool isItemGetHashUnlock = false;

        private static int[] WhichFleetUpgradeChoose = { 0, 0, 0 };
        private static bool isCraftUnitAttackRangeUpgrade = false;


        internal static void ModifyUpgradeTeches()
        {
            TechProto tech = LDB.techs.Select(ProtoID.T批量建造1);
            tech.HashNeeded = 1200;
            tech.UnlockValues = new[] { 450.0, };

            tech = LDB.techs.Select(ProtoID.T批量建造2);
            tech.UnlockValues = new[] { 900.0, };

            tech = LDB.techs.Select(ProtoID.T批量建造3);
            tech.UnlockValues = new[] { 1800.0, };

            tech = LDB.techs.Select(ProtoID.T能量回路4);
            tech.Items = Items4;
            tech.ItemPoints = Enumerable.Repeat(12, 4).ToArray();

            tech = LDB.techs.Select(ProtoID.T驱动引擎4);
            tech.Items = Items4;
            tech.ItemPoints = Enumerable.Repeat(10, 4).ToArray();

            tech = LDB.techs.Select(ProtoID.T驱动引擎5);
            tech.Items = Items5;
            tech.ItemPoints = Enumerable.Repeat(10, 5).ToArray();

            tech = LDB.techs.Select(ProtoID.T垂直建造3);
            tech.Items = Items3;
            tech.ItemPoints = new[] { 20, 20, 10, };

            tech = LDB.techs.Select(ProtoID.T垂直建造6);
            tech.Items = Items5;
            tech.ItemPoints = Enumerable.Repeat(6, 5).ToArray();

            tech = LDB.techs.Select(ProtoID.T集装分拣6);
            tech.Items = Items5;
            tech.ItemPoints = Enumerable.Repeat(6, 5).ToArray();
            /*
            for (int i = 2501; i <= 2506; i++)
            {
                TechProto techProto = LDB.techs.Select(i);
                Debug.LogFormat("scppppppppppppperppppppppp");
                Debug.LogFormat("techProto.ID {0} techProto.Name {1} techProto.IconPath {2}", techProto.ID, techProto.Name, techProto.IconPath);
                Debug.LogFormat("techProto.Position[0] {0} techProto.Position[1] {1} ", techProto.Position[0], techProto.Position[1]);
                //Debug.LogFormat("techProto.PreTechsImplicit[0] {0} techProto.PreTechs {1}", techProto.PreTechsImplicit[0], techProto.PreTechs[0]);
                if (techProto.PreTechs != null)
                {
                    for (int j = 0; j < techProto.PreTechs.Length; j++)
                    {
                        Debug.LogFormat("techProto.PreTechs {0} j = {1}", techProto.PreTechs[j], j);
                    }
                }
                if (techProto.PreTechsImplicit != null)
                {
                    for (int j = 0; j < techProto.PreTechsImplicit.Length; j++)
                    {
                        Debug.LogFormat("techProto.PreTechsImplicit {0} j = {1}", techProto.PreTechsImplicit[j], j);
                    }
                }
                if (techProto.UnlockValues != null)
                {
                    for (int j = 0; j < techProto.UnlockValues.Length; j++)
                    {
                        Debug.LogFormat("techProto.UnlockValues {0} j = {1}", techProto.UnlockValues[j], j);
                    }
                }
                if (techProto.UnlockFunctions != null)
                {
                    for (int j = 0; j < techProto.UnlockFunctions.Length; j++)
                    {
                        Debug.LogFormat("techProto.UnlockFunctions {0} j = {1}", techProto.UnlockFunctions[j], j);
                    }
                }
            }
            */
            ModifyAllUpgradeTechs();


            ModifyCoreUpgradeTechs();
            ModifyMoveUpgradeTechs();
            ModifyPackageUpgradeTechs();
            ModifyBuilderNumberUpgradeTechs();
            ModifyReBuildUpgradeTechs();
            ModifyCombustionPowerUpgradeTechs();
            ModifyBuilderSpeedUpgradeTechs();
            ModifyFlySpeedUpgradeTechs();
            ModifySolarSailingLifeUpgradeTechs();
            ModifySolarSailingAdsorbSpeedUpgradeTechs();
            ModifyRayEfficiencyUpgradeTechs();
            ModifyWhiteGrabUpgradeTechs();
            ModifySpacecraftSpeedUpgradeTechs();
            ModifySpacecraftExpansionUpgradeTechs();
            ModifyMinerUpgradeTechs();
            ModifyDamageUpgradeTechs();
            ModifyWreckageRecoveryUpgradeTechs();
            ModifyUAVHPAndfiringRateUpgradeTechs();
            ModifyGroundFormationExpansionUpgradeTechs();
            ModifySpaceFormationExpansionUpgradeTechs();
            ModifyPlanetFieldUpgradeTechs();
            ModifyFleetUpgradeTechs();

            AddUpgradeTechs();


            /*
            foreach (TechProto techProto in LDB.techs.dataArray)
            {
                if (techProto.ID == 3701 || techProto.ID == 3706)
                {
                    Debug.LogFormat("scppppppppppppperppppppppp");
                    Debug.LogFormat("techProto.ID {0} techProto.Name {1} techProto.IconPath {2}", techProto.ID, techProto.Name, techProto.IconPath);
                    if (techProto.UnlockValues != null)
                    {
                        for (int j = 0; j < techProto.UnlockValues.Length; j++)
                        {
                            Debug.LogFormat("techProto.UnlockValues {0} j = {1}", techProto.UnlockValues[j], j);
                        }
                    }
                }
            }
            */

            tech = LDB.techs.Select(4102);
            tech.Items = Items2;
            tech.ItemPoints = Enumerable.Repeat(10, 2).ToArray();

            tech = LDB.techs.Select(4103);
            tech.Items = new[] { 6003, };
            tech.ItemPoints = new[] { tech.ItemPoints[0], };

            tech = LDB.techs.Select(ProtoID.T宇宙探索4);
            tech.Items = new[] { 6003, 6278 };
            tech.ItemPoints = new[] { tech.ItemPoints[0], tech.ItemPoints[1], };

            

            for (int i = 3701; i <= 3706; i++)
            {
                TechProto techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new[] { 1d, 1d };
            }

            /*
            for (int i = ProtoID.T宇宙探索1; i <= ProtoID.T宇宙探索4; i++)
            {
                TechProto techProto = LDB.techs.Select(i);
                techProto.Items = new[] { 6001, };
                techProto.ItemPoints = new[] { techProto.ItemPoints[0], };
                techProto.PreTechsImplicit = Array.Empty<int>();
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (TechProto techProto in LDB.techs.dataArray)
            {
                if (techProto.ID < 2000) continue;

                Debug.LogFormat("techProto.Desc {0} techProto.Items {1} techProto.ItemPoints {2}", techProto.Desc, techProto.Items, techProto.ItemPoints);
                Debug.LogFormat("techProto.HashNeeded {0} techProto.UnlockFunctions {1} techProto.UnlockValues {2}", techProto.HashNeeded, techProto.UnlockFunctions, techProto.UnlockValues);
                Debug.LogFormat("techProto.Level {0} techProto.MaxLevel {1} techProto.LevelCoef1 {2} techProto.LevelCoef2 {3}", techProto.Level, techProto.MaxLevel, techProto.LevelCoef1, techProto.LevelCoef2);
                Debug.LogFormat("techProto.PreTechsMax {0}", techProto.PreTechsMax);

                int[] items = techProto.Items;

                if (items.SequenceEqual(Items5))
                {
                    techProto.Items = new[] { 6280, };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[4], };
                    continue;
                }

                if (items.SequenceEqual(Items4))
                {
                    if (techProto.ID % 100 > 2)
                    {
                        techProto.Items = new[] { 6278, 6279, };
                        techProto.ItemPoints = new[] { techProto.ItemPoints[1], techProto.ItemPoints[3], };
                    }
                    else
                    {
                        techProto.Items = new[] { 6278, 6003, 6004, };
                        techProto.ItemPoints = new[] { techProto.ItemPoints[1], techProto.ItemPoints[2], techProto.ItemPoints[3], };
                    }

                    continue;
                }

                if (items.SequenceEqual(Items3))
                {
                    techProto.Items = new[] { 6278, 6003, };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[1], techProto.ItemPoints[2], };
                    continue;
                }

                // ReSharper disable once InvertIf
                if (items.SequenceEqual(Items2) && techProto.ID % 100 > 2)
                {
                    techProto.Items = new[] { 6278, };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[0], };
                    continue;
                }
            }*/
        }

        internal static void ModifyAllUpgradeTechs()
        {
            foreach (TechProto techProto in LDB.techs.dataArray)
            {
                if (techProto.ID < 2000) continue;

                int[] items = techProto.Items;

                if (items.SequenceEqual(Items5))
                {
                    techProto.Items = new[] { 6279, 6004 };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[0], techProto.ItemPoints[0]};
                    continue;
                }

                if (items.SequenceEqual(Items4))
                {
                    techProto.Items = new int[] { 6003, 6278 };
                    techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                    continue;
                }

                if (items.SequenceEqual(Items3))
                {
                    techProto.Items = new[] { 6003, };
                    techProto.ItemPoints = new[] { techProto.ItemPoints[0] };
                    continue;
                }
            }
        }

        internal static void ModifyCoreUpgradeTechs()
        {
            TechProto techProto;
            double coreNenergy = 0;
            for (int i = 2101; i <= 2105; i++)
            {
                techProto = LDB.techs.Select(i);
                coreNenergy = techProto.UnlockValues[0];
                techProto.UnlockFunctions = new[] { 6, 82, 83, };
                techProto.UnlockValues = new double[] { coreNenergy, 4d, 1000d, };

                switch (i)
                {
                    case 2101:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.IsLabTech = true;
                        break;
                    case 2102:
                        break;
                    case 2103:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2104:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2105:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
            TechProto coreInfinItetechProto = LDB.techs.Select(2106);
            coreNenergy = coreInfinItetechProto.UnlockValues[0];
            coreInfinItetechProto.UnlockFunctions = new[] { 6, 83, };
            coreInfinItetechProto.UnlockValues = new double[] { coreNenergy, 200d, };
        }

        internal static void ModifyMoveUpgradeTechs()
        {
            TechProto techProto;
            double moveSpped = 0;
            for (int i = 2201; i <= 2208; i++)
            {
                techProto = LDB.techs.Select(i);
                moveSpped = techProto.UnlockValues[0];
                techProto.UnlockFunctions = new int[] { 3, 81 };
                techProto.UnlockValues = new double[] { moveSpped, moveSpped * 75000 };
                switch (i)
                {
                    case 2201:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.IsLabTech = true;
                        break;
                    case 2202:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2203:
                        break;
                    case 2204:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2205:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2206:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2207:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2208:
                        techProto.Items = new int[] { 6279, 6004, 6005 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void ModifyPackageUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 2301; i <= 2307; i++)
            {
                techProto = LDB.techs.Select(i);
                switch (i)
                {
                    case 2301:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.IsLabTech = true;
                        break;
                    case 2302:
                        break;
                    case 2303:
                        techProto.Items = new int[] { 6001, 6002 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2304:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2305:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2306:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2307:
                        techProto.Items = new int[] { 6279, 6004, 6005 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void ModifyBuilderNumberUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 2404; i <= 2406; i++)
            {
                techProto = LDB.techs.Select(i);
                switch (i)
                {
                    case 2404:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2305:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2306:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void ModifyReBuildUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 2953; i <= 2956; i++)
            {
                techProto = LDB.techs.Select(i);

                switch (i)
                {
                    case 2953:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        break;
                    case 2954:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2955:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2956:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void ModifyCombustionPowerUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 2501; i <= 2506; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new double[] { techProto.UnlockValues[0] * 2 };

                switch (i)
                {
                    case 2501:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0] };
                        techProto.IsLabTech = true;
                        break;
                    case 2502:
                    case 2503:
                        break;
                    case 2504:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
                        break;
                    case 2506:
                        break;
                    default:
                        break;
                }
            }
        }

        
        internal static void ModifyBuilderSpeedUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(2606);
            techProto.MaxLevel = 21;
        }

        internal static void ModifyFlySpeedUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(2901);
            techProto.Items = new int[] { 1407 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[1] / 2 };

            techProto = LDB.techs.Select(2902);
            techProto.Items = new int[] { 1405 };
            techProto.ItemPoints = new int[] { 2 };
            techProto.HashNeeded = 21600;
            techProto.IsLabTech = false;

            techProto = LDB.techs.Select(2903);
            techProto.Items = new int[] { 6277 };
            techProto.ItemPoints = new int[] { 2 };
            techProto.HashNeeded = 45000;
            techProto.IsLabTech = false;

            techProto = LDB.techs.Select(2904);
            techProto.Items = new int[] { 6227 };
            techProto.ItemPoints = new int[] { 1 };
            techProto.HashNeeded = 3600;
            techProto.IsLabTech = false;

            techProto = LDB.techs.Select(2905);
            techProto.Items = new int[] { 6227 };
            techProto.ItemPoints = new int[] { 1 };
            techProto.HashNeeded = 14400;
            techProto.IsLabTech = false;

            techProto = LDB.techs.Select(2906);
            techProto.Items = new int[] { 6227 };
            techProto.ItemPoints = new int[] { 1 };
            techProto.HashNeeded = -90000;
            techProto.LevelCoef1 = 18000;
            techProto.LevelCoef2 = 0;
            techProto.IsLabTech = false;
        }

        internal static void ModifySolarSailingLifeUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3106);
            techProto.Items = new int[] { 6279, 6004 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifySolarSailingAdsorbSpeedUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 4201; i <= 4206; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Items = new int[] { 6279, 6004, 6005 };
                techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
            }
        }

        internal static void ModifyRayEfficiencyUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3207);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifyWhiteGrabUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 3313; i <= 3314; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Items = new int[] { 6003, 6278 };
                techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
            }
            techProto = LDB.techs.Select(3315);
            techProto.Items = new int[] { 6279, 6004 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(3316);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifySpacecraftSpeedUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3404);
            techProto.UnlockFunctions = new int[] { 15, 34, 16 };
            techProto.UnlockValues = new double[] { 0.3, 0.15, 0.5 };

            techProto = LDB.techs.Select(3406);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifySpacecraftExpansionUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3508);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifyMinerUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(3604);
            techProto.Items = new int[] { 6279, 6004 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(3605);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(3606);
            techProto.MaxLevel = 11;
        }

        internal static void ModifyDamageUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5004; i <= 5005; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new double[] { 0.2 };
            }
            techProto = LDB.techs.Select(5006);
            techProto.Items = new int[] { 5201 };
            techProto.IsLabTech = false;
            techProto.HashNeeded = techProto.HashNeeded / 10;
            techProto.LevelCoef1 = techProto.LevelCoef1 / 10;
            techProto.LevelCoef2 = techProto.LevelCoef2 / 10;
            techProto.Desc = "T动能武器伤害无限";
            techProto.RefreshTranslation();

            for (int i = 5104; i <= 5105; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new double[] { 0.2 };
            }
            techProto = LDB.techs.Select(5106);
            techProto.Items = new int[] { 5201 };
            techProto.IsLabTech = false;
            techProto.HashNeeded = techProto.HashNeeded / 10;
            techProto.LevelCoef1 = techProto.LevelCoef1 / 10;
            techProto.LevelCoef2 = techProto.LevelCoef2 / 10;
            techProto.Desc = "T能量武器伤害无限";
            techProto.RefreshTranslation();

            for (int i = 5204; i <= 5205; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.UnlockValues = new double[] { 0.2 };
            }
            techProto = LDB.techs.Select(5206);
            techProto.Items = new int[] { 5201 };
            techProto.IsLabTech = false;
            techProto.HashNeeded = techProto.HashNeeded / 10;
            techProto.LevelCoef1 = techProto.LevelCoef1 / 10;
            techProto.LevelCoef2 = techProto.LevelCoef2 / 10;
            techProto.Desc = "T爆破武器伤害无限";
            techProto.RefreshTranslation();

            techProto = LDB.techs.Select(5201);
            techProto.PreTechsImplicit = new[] { 1807, };
        }

        internal static void ModifyWreckageRecoveryUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5301; i <= 5305; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Name = "残骸回收分析";
                techProto.Desc = "T残骸回收分析";
                techProto.RefreshTranslation();
                techProto.IconPath = "Assets/texpack/回收科技";

                switch (i)
                {
                    case 5301:
                        techProto.Items = new int[] { 6001 };
                        techProto.ItemPoints = new int[] { 10 };
                        techProto.HashNeeded = 18000;
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 3 };
                        break;
                    case 5302:
                        techProto.Items = new int[] { 6001, 6002 };
                        techProto.ItemPoints = new int[] { 10, 10 };
                        techProto.HashNeeded = 36000;
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 6 };
                        break;
                    case 5303:
                        techProto.Items = new int[] { 6003 };
                        techProto.ItemPoints = new int[] { 10 };
                        techProto.HashNeeded = 54000;
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 9 };
                        break;
                    case 5304:
                        techProto.Items = new int[] { 6003, 6278 };
                        techProto.ItemPoints = new int[] { 10, 10 };
                        techProto.HashNeeded = 72000;
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 12 };
                        break;
                    case 5305:
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { 10, 8 };
                        techProto.HashNeeded = 108000;
                        techProto.LevelCoef1 = 0;
                        techProto.LevelCoef2 = 0;
                        techProto.UnlockFunctions = new int[] { 101 };
                        techProto.UnlockValues = new double[] { 15 };
                        break;
                    default:
                        break;
                }
            }
            techProto = LDB.techs.Select(5301);
            techProto.PreTechsImplicit = new[] { 1826, };

            techProto = LDB.techs.Select(5305);
            techProto.MaxLevel = 5;

        }

        internal static void ModifyFleetUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5401; i <= 5405; i++)
            {
                techProto = LDB.techs.Select(i);
                switch (i)
                {
                    case 5401:
                        techProto.Name = "太空舰队结构优化";
                        techProto.Desc = "T太空舰队结构优化";
                        techProto.IconPath = "Icons/Tech/5601";
                        techProto.RefreshTranslation();
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { 10, 10 };
                        techProto.HashNeeded = 216000;
                        techProto.UnlockFunctions = new int[] { 73, 72 };
                        techProto.UnlockValues = new double[] { 0.25, 0.3 };
                        techProto.PreTechsImplicit = new[] { 1822, };
                        techProto.IsHiddenTech = false;
                        break;
                    case 5402:
                        techProto.Name = "太空舰队结构优化";
                        techProto.Desc = "T太空舰队结构优化";
                        techProto.IconPath = "Icons/Tech/5602";
                        techProto.RefreshTranslation();
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { 10, 10 };
                        techProto.HashNeeded = 288000;
                        techProto.UnlockFunctions = new int[] { 73, 72 };
                        techProto.UnlockValues = new double[] { 0.3, 0.3 };
                        break;
                    case 5403:
                        techProto.Name = "太空舰队结构优化";
                        techProto.Desc = "T太空舰队结构优化";
                        techProto.IconPath = "Icons/Tech/5603";
                        techProto.RefreshTranslation();
                        techProto.Items = new int[] { 6279, 6004, 6005 };
                        techProto.ItemPoints = new int[] { 10, 10, 10 };
                        techProto.HashNeeded = 360000;
                        techProto.UnlockFunctions = new int[] { 73, 72 };
                        techProto.UnlockValues = new double[] { 0.35, 0.4 };
                        break;
                    case 5404:
                        techProto.Name = "太空舰队火力升级";
                        techProto.Desc = "T太空舰队火力升级";
                        techProto.IconPath = "Icons/Tech/5301";
                        techProto.RefreshTranslation();
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { 10, 10 };
                        techProto.HashNeeded = 216000;
                        techProto.Level = 1;
                        techProto.MaxLevel = 1;
                        techProto.UnlockFunctions = new int[] { 71, 72 };
                        techProto.UnlockValues = new double[] { 0.4, 0.3 };
                        techProto.PreTechs = new int[] {  };
                        techProto.PreTechsImplicit = new[] { 1822 }; //1822,
                        techProto.IsHiddenTech = false;
                        break;
                    case 5405:
                        techProto.Name = "太空舰队火力升级";
                        techProto.Desc = "T太空舰队火力升级";
                        techProto.IconPath = "Icons/Tech/5302";
                        techProto.RefreshTranslation();
                        techProto.Items = new int[] { 6279, 6004 };
                        techProto.ItemPoints = new int[] { 10, 10 };
                        techProto.HashNeeded = 288000;
                        techProto.UnlockFunctions = new int[] { 71, 72 };
                        techProto.UnlockValues = new double[] { 0.6, 0.3 };
                        techProto.Level = 2;
                        techProto.MaxLevel = 2;
                        break;
                }
            }
        }

        internal static void ModifyUAVHPAndfiringRateUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5601; i <= 5605; i++)
            {
                techProto = LDB.techs.Select(i);
                switch (i)
                {
                    case 5601:
                        techProto.Name = "机兵升级计划";
                        techProto.Desc = "T机兵升级计划";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69};
                        techProto.UnlockValues = new double[] { 0.1, 0.05};
                        break;
                    case 5602:
                        techProto.Name = "迭代升级";
                        techProto.Desc = "T迭代升级";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69 };
                        techProto.UnlockValues = new double[] { 0.1, 0.1 };
                        break;
                    case 5603:
                        techProto.Name = "迭代升级";
                        techProto.Desc = "T迭代升级";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69 };
                        techProto.UnlockValues = new double[] { 0.2, 0.1 };
                        break;
                    case 5604:
                        techProto.Name = "军械量产方案";
                        techProto.Desc = "T军械量产方案";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69 };
                        techProto.UnlockValues = new double[] { 0.3, 0.05 };
                        break;
                    case 5605:
                        techProto.Name = "迭代升级";
                        techProto.Desc = "T迭代升级";
                        techProto.RefreshTranslation();
                        techProto.UnlockFunctions = new int[] { 68, 69 };
                        techProto.UnlockValues = new double[] { 0.3, 0.2 };
                        break;
                }

            }
            techProto = LDB.techs.Select(5601);
            techProto.PreTechsImplicit = new[] { 1819, };
        }

        internal static void ModifyGroundFormationExpansionUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(5801);
            techProto.UnlockFunctions = new int[] { 77 };
            techProto.UnlockValues = new double[] { 1 };

            techProto = LDB.techs.Select(5803);
            techProto.UnlockFunctions = new int[] { 78 };
            techProto.UnlockValues = new double[] { 2 };

            techProto = LDB.techs.Select(5806);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        internal static void ModifySpaceFormationExpansionUpgradeTechs()
        {
            TechProto techProto;
            for (int i = 5901; i <= 5903; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Items = new int[] { 6279, 6004, };
                techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };
            }
            for (int i = 5904; i <= 5905; i++)
            {
                techProto = LDB.techs.Select(i);
                techProto.Items = new int[] { 6279, 6004, 6005 };
                techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
            }
        }

        internal static void ModifyPlanetFieldUpgradeTechs()
        {
            TechProto techProto = LDB.techs.Select(5702);
            techProto.Items = new int[] { 6003, 6278 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(5703);
            techProto.Items = new int[] { 6279, 6004, };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0] };

            techProto = LDB.techs.Select(5704);
            techProto.Items = new int[] { 6279, 6004, 6005 };
            techProto.ItemPoints = new int[] { techProto.ItemPoints[0], techProto.ItemPoints[0], techProto.ItemPoints[0] };
        }

        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.NotifyTechUnlock))]
        [HarmonyPrefix]
        public static void NotifyTechUnlockPatch(GameHistoryData __instance, int _techId)
        {
            //Debug.LogFormat("scpppppppeopppppppppp UnlockTech techId {0}", _techId);
            CombatDroneMotify(_techId);
            WreckFalling(_techId);
            CrackingRayTechAndItemModify(_techId);
            UnlockRecipesHandcraft(_techId);
            UpdateTechSpeed(_techId);
            ItemGetHash(_techId);
            CraftUnitAttackRangeUpgrade(_techId);
        }

        static void UAVHPAndfiringRateUpgrade(int level)
        {
            ItemProto itemProto;
            RecipeProto recipeProto;
            if (level == 1)
            {
                recipeProto = LDB.recipes.Select(147);
                recipeProto.Items[2] = 1303;
                recipeProto = LDB.recipes.Select(148);
                recipeProto.Items[3] = 1113;
                recipeProto = LDB.recipes.Select(149);
                recipeProto.Items[3] = 1113;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机A型";
                itemProto.Description = "I攻击无人机A型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机A型";
                itemProto.Description = "I精准无人机A型";
                itemProto.RefreshTranslation();
            }
            else if (level == 2)
            {
                recipeProto = LDB.recipes.Select(147);
                recipeProto.Items[0] = 1107;
                recipeProto.Items[3] = 1126;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机B型";
                itemProto.Description = "I攻击无人机B型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机B型";
                itemProto.Description = "I精准无人机B型";
                itemProto.RefreshTranslation();
            }
            else if (level == 3)
            {
                recipeProto = LDB.recipes.Select(147);
                recipeProto.Items[0] = 6225;
                recipeProto = LDB.recipes.Select(148);
                recipeProto.Items[2] = 1206;
                recipeProto = LDB.recipes.Select(149);
                recipeProto.Items[2] = 1206;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机C型";
                itemProto.Description = "I攻击无人机C型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机C型";
                itemProto.Description = "I精准无人机C型";
                itemProto.RefreshTranslation();
            }
            else if (level == 4)
            {
                recipeProto = LDB.recipes.Select(147);
                recipeProto.Type = ERecipeType.Assemble;
                recipeProto = LDB.recipes.Select(148);
                recipeProto.Type = ERecipeType.Assemble;
                recipeProto = LDB.recipes.Select(149);
                recipeProto.Type = ERecipeType.Assemble;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机D型";
                itemProto.Description = "I攻击无人机D型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机D型";
                itemProto.Description = "I精准无人机D型";
                itemProto.RefreshTranslation();
            }
            else if (level == 5)
            {
                recipeProto = LDB.recipes.Select(148);
                recipeProto.Items[3] = 1118;
                recipeProto.Items[2] = 6243;
                recipeProto = LDB.recipes.Select(149);
                recipeProto.Items[3] = 1118;
                recipeProto.Items[2] = 6243;

                itemProto = LDB.items.Select(5102);
                itemProto.Name = "攻击无人机E型";
                itemProto.Description = "I攻击无人机E型";
                itemProto.RefreshTranslation();

                itemProto = LDB.items.Select(5103);
                itemProto.Name = "精准无人机E型";
                itemProto.Description = "I精准无人机E型";
                itemProto.RefreshTranslation();
            }
        }

        static void CombatDroneMotify(int techId)
        {
            if (techId == 5601)
            {
                UAVHPAndfiringRateUpgrade(1);
                UAVHPAndfiringRateUpgradeLevel = 1;
            }
            else if (techId == 5602)
            {
                UAVHPAndfiringRateUpgrade(2);
                UAVHPAndfiringRateUpgradeLevel = 2;
            }
            else if (techId == 5603)
            {
                UAVHPAndfiringRateUpgrade(3);
                UAVHPAndfiringRateUpgradeLevel = 3;
            }
            else if (techId == 5604)
            {
                UAVHPAndfiringRateUpgrade(4);
                UAVHPAndfiringRateUpgradeLevel = 4;
            }
            else if (techId == 5605)
            {
                UAVHPAndfiringRateUpgrade(5);
                UAVHPAndfiringRateUpgradeLevel = 5;
            }
        }

        static void UnlockWreckFalling(int unlockLevel)
        {
            ItemProto itemProto;
            switch (unlockLevel)
            {
                case 1:
                    for (int i = 0; i < unlockWreckFallingItemIdLevel1.Length; i++)
                    {
                        itemProto = LDB.items.Select(unlockWreckFallingItemIdLevel1[i]);
                        itemProto.EnemyDropCount = unlockWreckFallingItemDropCountLevel1[i];
                    }
                    ItemProto.InitEnemyDropTables();
                    break;
                case 2:
                    for (int i = 0; i < unlockWreckFallingItemIdLevel2.Length; i++)
                    {
                        itemProto = LDB.items.Select(unlockWreckFallingItemIdLevel2[i]);
                        itemProto.EnemyDropCount = unlockWreckFallingItemDropCountLevel2[i];
                    }
                    ItemProto.InitEnemyDropTables();
                    break;
                case 3:
                    for (int i = 0; i < unlockWreckFallingItemIdLevel3.Length; i++)
                    {
                        itemProto = LDB.items.Select(unlockWreckFallingItemIdLevel3[i]);
                        itemProto.EnemyDropCount = unlockWreckFallingItemDropCountLevel3[i];
                    }
                    ItemProto.InitEnemyDropTables();
                    break;
                case 4:
                    for (int i = 0; i < unlockWreckFallingItemIdLevel4.Length; i++)
                    {
                        itemProto = LDB.items.Select(unlockWreckFallingItemIdLevel4[i]);
                        itemProto.EnemyDropCount = unlockWreckFallingItemDropCountLevel4[i];
                    }
                    ItemProto.InitEnemyDropTables();
                    break;
                case 5:
                    for (int i = 0; i < unlockWreckFallingItemIdLevel5.Length; i++)
                    {
                        itemProto = LDB.items.Select(unlockWreckFallingItemIdLevel5[i]);
                        itemProto.EnemyDropCount = unlockWreckFallingItemDropCountLevel5[i];
                    }
                    ItemProto.InitEnemyDropTables();
                    break;
                case 6:
                    for (int i = 0; i < unlockWreckFallingItemIdLevel6.Length; i++)
                    {
                        itemProto = LDB.items.Select(unlockWreckFallingItemIdLevel6[i]);
                        itemProto.EnemyDropCount = unlockWreckFallingItemDropCountLevel6[i];
                    }
                    ItemProto.InitEnemyDropTables();
                    break;
                case 7:
                    for (int i = 0; i < unlockWreckFallingItemIdLevel7.Length; i++)
                    {
                        itemProto = LDB.items.Select(unlockWreckFallingItemIdLevel7[i]);
                        itemProto.EnemyDropCount = unlockWreckFallingItemDropCountLevel7[i];
                    }
                    ItemProto.InitEnemyDropTables();
                    break;
                case 8:
                    for (int i = 0; i < unlockWreckFallingItemIdLevel8.Length; i++)
                    {
                        itemProto = LDB.items.Select(unlockWreckFallingItemIdLevel8[i]);
                        itemProto.EnemyDropCount = unlockWreckFallingItemDropCountLevel8[i];
                    }
                    ItemProto.InitEnemyDropTables();
                    break;
            }
        }

        static void WreckFalling(int techId)
        {
            if (techId == 5301)
            {
                // 解锁3级的黑雾掉落
                UnlockWreckFalling(1);
                WreckFallingLevel = 1;
            }
            else if (techId == 5302)
            {
                // 解锁6级的黑雾掉落
                UnlockWreckFalling(2);
                WreckFallingLevel = 2;
            } else if (techId == 5303)
            {
                // 解锁9级的黑雾掉落
                UnlockWreckFalling(3);
                WreckFallingLevel = 3;
            } else if (techId == 5304)
            {
                // 解锁12级的黑雾掉落
                UnlockWreckFalling(4);
                WreckFallingLevel = 4;
            } else if (techId == 5305)
            {
                // 解锁15级的黑雾掉落
                UnlockWreckFalling(5);
                WreckFallingLevel = 5;
            } else if (techId == 5306) {
                // 解锁18级的黑雾掉落
                UnlockWreckFalling(6);
                WreckFallingLevel = 6;
            } else if (techId == 5307)
            {
                // 解锁21级的黑雾掉落
                UnlockWreckFalling(7);
                WreckFallingLevel = 7;
            } else if (techId == 5308)
            {
                // 解锁24级的黑雾掉落
                UnlockWreckFalling(8);
                WreckFallingLevel = 8;
            }
        }

        static void UnlockCrackingRayTech()
        {
            ItemProto itemProto;
            TechProto techProto;
            itemProto = LDB.items.Select(6216);
            itemProto.Name = "裂解射线发生器";
            itemProto.Description = "I裂解射线发生器";
            itemProto.RefreshTranslation();

            techProto = LDB.techs.Select(1945);
            techProto.Name = "终末螺旋";
            techProto.Desc = "T终末螺旋";
            techProto.RefreshTranslation();
        }

        static void CrackingRayTechAndItemModify(int techId)
        {
            if (techId == 1945)
            {
                UnlockCrackingRayTech();
                isUnlockCrackingRay = true;
            }
        }

        static void UnlockRecipesHandcraft(int techId)
        {
            RecipeProto recipeProto;
            if (techId == 1945)
            {
                for (int i = 0; i < unlockHandcraftRecipes.Length; i++)
                {
                    recipeProto = LDB.recipes.Select(unlockHandcraftRecipes[i]);
                    recipeProto.Handcraft = true;
                }
                isUnlockRecipesHandcraft = true;
            }
        }

        static void UpdateTechSpeed(int techId)
        {
            TechProto techProto = LDB.techs.Select(techId);

            if (techProto.UnlockFunctions.Length > 0 && techProto.UnlockFunctions[0] == 22)
            {
                vanillaTechSpeed++;
            }
        }


        [HarmonyPatch(typeof(TechProto), nameof(TechProto.UnlockFunctionText))]
        [HarmonyPrefix]
        public static bool UnlockFunctionTextPatch(TechProto __instance, StringBuilder sb, ref string __result)
        {
            string text = "";
            if (__instance.UnlockFunctions.Length > 0)
            {
                if (__instance.UnlockFunctions[0] == 101)
                {
                    text = text + "黑雾".Translate() + __instance.UnlockValues[0] + "级残骸物品掉落".Translate();
                    __result = text;
                    return false;
                }
                if (__instance.UnlockFunctions.Length > 1) {
                    if (__instance.UnlockFunctions[0] == 7 && __instance.UnlockFunctions[1] == 102)
                    {
                        text = text + "+" + __instance.UnlockValues[0].ToString("0%") + "手动合成速度".Translate();
                        text += "\r\n";
                        text = text + "解锁手搓".Translate();
                        __result = text;
                        return false;
                    }
                    else if (__instance.UnlockFunctions[0] == 103 && __instance.UnlockFunctions[1] == 72)
                    {
                        text = text + "驱逐舰射程增加至".Translate() + __instance.UnlockValues[0] + "，护卫舰射程增加至".Translate() + __instance.UnlockValues[0] * 0.4;
                        text += "\r\n";
                        text += string.Format("太空战斗机攻速升级".Translate(), __instance.UnlockValues[1]);
                        __result = text;
                        return false;
                    }
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.TryAddItemToPackage))]
        [HarmonyPrefix]
        public static bool TryAddItemToPackagePatch(ref Player __instance, int itemId, int count, ref int __result)
        {
            if (itemId == 6254 && count > 0)
            {
                RecipeProto recipeProto;
                
                if (__instance.mecha.gameData.history.currentTech > 0)
                {
                    if (LDB.techs.Select(__instance.mecha.gameData.history.currentTech).IsLabTech == false)
                    {
                        recipeProto = LDB.recipes.Select(533);
                        if (recipeProto.ItemCounts[0] == 2)
                        {
                            synapticLatheTechSpeed = 1;
                            vanillaTechSpeed = __instance.mecha.gameData.history.techSpeed;
                        }
                        recipeProto.ItemCounts[0] = recipeProto.ItemCounts[0] * 2;
                        synapticLatheTechSpeed = synapticLatheTechSpeed * 2;
                        __instance.mecha.gameData.history.techSpeed = vanillaTechSpeed + synapticLatheTechSpeed;
                        __result = 0;
                        return false;
                    }
                    else
                    {
                        recipeProto = LDB.recipes.Select(533);
                        recipeProto.ItemCounts[0] = 2;
                        synapticLatheTechSpeed = 1;
                        __instance.mecha.gameData.history.techSpeed = vanillaTechSpeed;
                        __result = 0;
                        return false;
                    }
                } else
                {
                    recipeProto = LDB.recipes.Select(533);
                    if (recipeProto.ItemCounts[0] == 2)
                    {
                        synapticLatheTechSpeed = 1;
                        vanillaTechSpeed = __instance.mecha.gameData.history.techSpeed;
                    }
                    recipeProto.ItemCounts[0] = recipeProto.ItemCounts[0] * 2;
                    synapticLatheTechSpeed = synapticLatheTechSpeed * 2;
                    __instance.mecha.gameData.history.techSpeed = vanillaTechSpeed + synapticLatheTechSpeed;
                    __result = 0;
                    return false;
                }
            } else if (itemId == 6255 && count > 0)
            {
                __instance.mecha.gameData.history.AddTechHash(count * 1800000);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.RemoveTechInQueue))]
        [HarmonyPrefix]
        public static void RemoveTechInQueuePatch(GameHistoryData __instance, int index)
        {
            if (index == 0) { 
                RecipeProto recipeProto;
                recipeProto = LDB.recipes.Select(533);
                recipeProto.ItemCounts[0] = 2;
                synapticLatheTechSpeed = 1;
                __instance.techSpeed = vanillaTechSpeed;
            }
        }

        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.DequeueTech))]
        [HarmonyPrefix]
        public static void DequeueTechPatch(GameHistoryData __instance)
        {
            RecipeProto recipeProto;
            recipeProto = LDB.recipes.Select(533);
            recipeProto.ItemCounts[0] = 2;
            synapticLatheTechSpeed = 1;
            __instance.techSpeed = vanillaTechSpeed;
        }

        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.EnqueueTech))]
        [HarmonyPrefix]
        public static void EnqueueTechPatch(GameHistoryData __instance, int techId)
        {
            if (__instance.techQueue[0] == 0)
            {
                if (LDB.techs.Select(techId).IsLabTech == true)
                {
                    RecipeProto recipeProto;
                    recipeProto = LDB.recipes.Select(533);
                    recipeProto.ItemCounts[0] = 2;
                    synapticLatheTechSpeed = 1;
                    __instance.techSpeed = vanillaTechSpeed;
                }
            }
        }

        static void ItemGetHash(int techId)
        {
            if (techId == 1962)
            {
                ItemProto itemProto = LDB.items.Select(6234);
                itemProto.Name = "十七公斤重的文明";
                itemProto.Description = "I十七公斤重的文明";
                itemProto.RefreshTranslation();
                isItemGetHashUnlock = true;
            }
        }

        static void SetUnclockValuesIneffective(int techId)
        {
            TechProto techProto;
            techProto = LDB.techs.Select(techId);
            techProto.UnlockValues = new double[] { 0, 0 };
            techProto.RefreshTranslation();
        }

        static void CraftUnitAttackRangeUpgrade(int techId)
        {
            switch (techId)
            {
                case 5401:
                    if (WhichFleetUpgradeChoose[0] == 0)
                    {
                        SetUnclockValuesIneffective(5404);
                        WhichFleetUpgradeChoose[0] = 5404;
                    }
                    break;
                case 5402:
                    if (WhichFleetUpgradeChoose[1] == 0)
                    {
                        SetUnclockValuesIneffective(5405);
                        WhichFleetUpgradeChoose[1] = 5405;
                    }
                    break;
                case 5403:
                    if (WhichFleetUpgradeChoose[2] == 0)
                    {
                        SetUnclockValuesIneffective(5406);
                        WhichFleetUpgradeChoose[2] = 5406;
                    }
                    break;
                case 5404:
                    if (WhichFleetUpgradeChoose[0] == 0)
                    {
                        SetUnclockValuesIneffective(5401);
                        WhichFleetUpgradeChoose[0] = 5401;
                    }
                    break;
                case 5405:
                    if (WhichFleetUpgradeChoose[1] == 0)
                    {
                        SetUnclockValuesIneffective(5402);
                        WhichFleetUpgradeChoose[1] = 5402;
                    }
                    break;
                case 5406:
                    if (WhichFleetUpgradeChoose[2] == 0)
                    {
                        SetUnclockValuesIneffective(5403);
                        WhichFleetUpgradeChoose[2] = 5403;
                        ModelProto modelProto;
                        modelProto = LDB.models.Select(451); // 护卫
                        modelProto.prefabDesc.craftUnitAttackRange0 = 4000f;
                        modelProto.prefabDesc.craftUnitSensorRange = 4500f;
                        modelProto = LDB.models.Select(452); // 驱逐
                        modelProto.prefabDesc.craftUnitAttackRange0 = 10000f;
                        modelProto.prefabDesc.craftUnitAttackRange1 = 10000f;
                        modelProto.prefabDesc.craftUnitSensorRange = 12000f;
                    }
                    break;
                default:
                    break;
            }
        }

        internal static void Export(BinaryWriter w)
        {
            w.Write(WreckFallingLevel);
            w.Write(isUnlockCrackingRay);
            w.Write(isUnlockRecipesHandcraft);
            w.Write(UAVHPAndfiringRateUpgradeLevel);
            w.Write(vanillaTechSpeed);
            w.Write(synapticLatheTechSpeed);
            w.Write(isItemGetHashUnlock);
            for (int i = 0; i < 3; i++)
            {
                w.Write(WhichFleetUpgradeChoose[i]);
            }
        }

        internal static void Import(BinaryReader r)
        {
            try
            {
                WreckFallingLevel = r.ReadInt32();
                for (int i = 1; i <= WreckFallingLevel; i++)
                {
                    UnlockWreckFalling(i);
                }

                isUnlockCrackingRay = r.ReadBoolean();
                if (isUnlockCrackingRay)
                {
                    UnlockCrackingRayTech();
                }

                isUnlockRecipesHandcraft = r.ReadBoolean();
                if (isUnlockRecipesHandcraft)
                {
                    UnlockRecipesHandcraft(1945);
                }

                UAVHPAndfiringRateUpgradeLevel = r.ReadInt32();
                for (int i = 1; i <= UAVHPAndfiringRateUpgradeLevel; i++)
                {
                    UAVHPAndfiringRateUpgrade(i);
                }

                vanillaTechSpeed = r.ReadInt32();
                synapticLatheTechSpeed = r.ReadInt32();

                isItemGetHashUnlock = r.ReadBoolean();
                if (isUnlockRecipesHandcraft)
                {
                    ItemProto itemProto = LDB.items.Select(6234);
                    itemProto.Name = "十七公斤重的文明";
                    itemProto.Description = "I十七公斤重的文明";
                    itemProto.RefreshTranslation();
                }

                for (int i = 0; i < 3; i++)
                {
                    WhichFleetUpgradeChoose[i] = r.ReadInt32();
                    if (WhichFleetUpgradeChoose[i] != 0)
                    {
                        TechProto techProto = LDB.techs.Select(WhichFleetUpgradeChoose[i]);
                        techProto.UnlockValues = new double[] { 0, 0 };
                        if (i == 2 && WhichFleetUpgradeChoose[i] == 5403)
                        {
                            ModelProto modelProto;
                            modelProto = LDB.models.Select(451); // 护卫
                            modelProto.prefabDesc.craftUnitAttackRange0 = 4000f;
                            modelProto.prefabDesc.craftUnitSensorRange = 4500f;
                            modelProto = LDB.models.Select(452); // 驱逐
                            modelProto.prefabDesc.craftUnitAttackRange0 = 10000f;
                            modelProto.prefabDesc.craftUnitAttackRange1 = 10000f;
                            modelProto.prefabDesc.craftUnitSensorRange = 12000f;
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
                // ignored
            }
        }
    }
}
