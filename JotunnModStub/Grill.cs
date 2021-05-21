using BepInEx;
using UnityEngine;
using BepInEx.Configuration;
using Jotunn.Utils;
using System.Reflection;
using Jotunn.Entities;
using Jotunn.Configs;
using Jotunn.Managers;
using System;

namespace Grills
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class Grills : BaseUnityPlugin
    {
        public const string PluginGUID = "com.zarboz.grills";
        public const string PluginName = "RockRKittenGrill";
        public const string PluginVersion = "1.0.0";
        public AssetBundle assetBundle;
        private void Awake()
        {
            AssetLoad();
            LoadItem();
            
        }

        private void AssetLoad()
        {
            assetBundle = AssetUtils.LoadAssetBundleFromResources("grill", typeof(Grills).Assembly);


        }
       
        private void LoadItem()
        {
            //piece_grill

            var grillfab = assetBundle.LoadAsset<GameObject>("piece_grill");
            var grill = new CustomPiece(grillfab,
                new PieceConfig
                {
                    CraftingStation = "forge",
                    AllowedInDungeons = false,
                    Enabled = true,
                    PieceTable = "_HammerPieceTable",
                    Requirements = new[]
                    {
                        new RequirementConfig
                        {
                            Item = "Wood",
                            Amount = 1,
                            Recover = false,
                        }
                    }
                });
            PieceManager.Instance.AddPiece(grill);
        }

    }
}