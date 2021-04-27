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
using IL;

namespace JotunnModStub
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibilty(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class JotunnModStub : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.windlass";
        public const string PluginName = "Windlass";
        public const string PluginVersion = "0.0.1";
        public static new Jotunn.Logger Logger;
        private AssetBundle embeddedResourceBundle;
        public CustomStatusEffect WindlassEffect;

        private void Awake()
        {
            LoadAssets();
            AddStatusEffects();
            CreateThing();
        }

        
        private void Update()
        {
  
        }
         
        private void LoadAssets()
        {
            Jotunn.Logger.LogInfo($"Embedded resources: {string.Join(",", Assembly.GetExecutingAssembly().GetManifestResourceNames())}");
            embeddedResourceBundle = AssetUtils.LoadAssetBundleFromResources("bow", Assembly.GetExecutingAssembly());
        }

        private void CreateThing()
        {
            var bow_prefab = embeddedResourceBundle.LoadAsset<GameObject>("Windlass");
            var bow = new CustomItem(bow_prefab, fixReference: true,
                new ItemConfig
                {
                    Amount = 1,
                    CraftingStation = "forge",
                    MinStationLevel = 2,
                    RepairStation = "forge",
                    Requirements = new[]
                    {
                        new RequirementConfig { Item = "Obsidian", Amount = 1},
                        new RequirementConfig { Item = "DragonTear", Amount = 1, AmountPerLevel = 4},
                        new RequirementConfig { Item = "Bronze", Amount = 3, AmountPerLevel = 10}
                    }
                }); 
            var itemDrop = bow.ItemDrop;
            itemDrop.m_itemData.m_shared.m_equipStatusEffect = WindlassEffect.StatusEffect;
            
            ItemManager.Instance.AddItem(bow);
            
        }

        private void AddStatusEffects()
        {
            // Create a new status effect. The base class "StatusEffect" does not do very much except displaying messages
            // A Status Effect is normally a subclass of StatusEffects which has methods for further coding of the effects (e.g. SE_Stats).
            StatusEffect effect = ScriptableObject.CreateInstance<StatusEffect>();
            
            effect.name = "WindlassEffect";
            effect.m_name = "Windlass Spirit";
            effect.m_icon = AssetUtils.LoadSpriteFromFile("Windlass/Assets/Windlass.png");
            effect.m_startMessageType = MessageHud.MessageType.TopLeft;
            effect.m_startMessage = "The wind begins to howl";
            effect.m_stopMessageType = MessageHud.MessageType.TopLeft;
            effect.m_stopMessage = "The wind calms down";
            var multiplier = 5f;
            effect.ModifyHealthRegen(ref multiplier);
            var drainer = -10f;
            effect.ModifyRunStaminaDrain(1, drain: ref drainer);
            effect.ModifyRaiseSkill(Skills.SkillType.Bows, ref multiplier);
            
            WindlassEffect = new CustomStatusEffect(effect, fixReference: false);  // We dont need to fix refs here, because no mocks were used
            ItemManager.Instance.AddStatusEffect(WindlassEffect);

         
           /* void fermenterthing()
            {
                Fermenter.ItemConversion balls = ScriptableObject.CreateInstance<Fermenter.ItemConversion>();
                balls.m_from;
                balls.m_to;
                balls.m_producedItems;



                //public bool IsItemAllowed(ItemDrop.ItemData item);
                //public bool IsItemAllowed(string itemName);

            }*/
          
        }

       
    }
}