// JotunnModStub
// a Valheim mod skeleton using JötunnLib
// 
// File:    JotunnModStub.cs
// Project: JotunnModStub

using BepInEx;
using UnityEngine;
using BepInEx.Configuration;
using Jotunn.Utils;
using System.Reflection;
using Jotunn.Entities;
using Jotunn.Configs;
using Jotunn.Managers;

namespace Animals
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class Animals : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.masterchef";
        public const string PluginName = "MasterChef";
        public const string PluginVersion = "1.0.4";
        public static new Jotunn.Logger Logger;
        private AssetBundle embeddedResourceBundle;
 

        private void Awake()
        {
            LoadAssets();
            addfab();
        }



        private void LoadAssets()
        {
            Jotunn.Logger.LogInfo($"Embedded resources: {string.Join(",", Assembly.GetExecutingAssembly().GetManifestResourceNames())}");
            embeddedResourceBundle = AssetUtils.LoadAssetBundleFromResources("testfish", Assembly.GetExecutingAssembly());
        }

      
        private void addfab()
        {
            var fab = embeddedResourceBundle.LoadAsset<GameObject>("Marlin");
            var fab2 = embeddedResourceBundle.LoadAsset<GameObject>("Marlin1");
            var fab3 = embeddedResourceBundle.LoadAsset<GameObject>("Yak");
            PrefabManager.Instance.AddPrefab(fab);
            PrefabManager.Instance.AddPrefab(fab2);
            PrefabManager.Instance.AddPrefab(fab3);
        }
    }
} 