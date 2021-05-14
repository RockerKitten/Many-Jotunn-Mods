// JotunnModStub
// a Valheim mod skeleton using JötunnLib
// 
// File:    JotunnModStub.cs
// Project: JotunnModStub

using BepInEx;
using UnityEngine;
using Jotunn.Utils;
using System.Reflection;
using Jotunn.Entities;
using Jotunn.Configs;
using Jotunn.Managers;
using BepInEx.Configuration;

namespace JotunnModStub
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class JotunnModStub : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.windlass";
        public const string PluginName = "Windlass";
        public const string PluginVersion = "0.1.1";
        public static new Jotunn.Logger Logger;
        private AssetBundle embeddedResourceBundle;
        public CustomStatusEffect WindlassEffect;
        public float m_damageBonus = 0.05f;
        public float m_activationHealth = 0f;
        public float m_damageIncrement = 0f;
        private ConfigEntry<int> damage;
        private ConfigEntry<int> damageper;
        private ConfigEntry<int> blunt;
        private ConfigEntry<int> bluntper;
        private ConfigEntry<int> slashval;
        private ConfigEntry<int> slashper;
        private ConfigEntry<int> pierce;
        private ConfigEntry<int> pierceper;
        private ConfigEntry<int> chop;
        private ConfigEntry<int> chopper;
        private ConfigEntry<int> pickaxe;
        private ConfigEntry<int> pickaxeper;
        private ConfigEntry<int> fire;
        private ConfigEntry<int> fireper;
        private ConfigEntry<int> frost;
        private ConfigEntry<int> frostper;
        private ConfigEntry<int> lightning;
        private ConfigEntry<int> lightningper;
        private ConfigEntry<int> poison;
        private ConfigEntry<int> poisonper;
        private ConfigEntry<int> spirit;
        private ConfigEntry<int> spiritper;
        private ConfigEntry<int> tier;
        private ConfigEntry<bool> arrowsenabled;

        private void Awake()
        {
            Configurator();
            LoadAssets();
            AddStatusEffects(15f, 0.05f);
            CreateThing();
            AddArrows();
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
                        new RequirementConfig { Item = "Obsidian", Amount = 15},
                        new RequirementConfig { Item = "DragonTear", Amount = 1, AmountPerLevel = 4},
                        new RequirementConfig { Item = "Bronze", Amount = 3, AmountPerLevel = 10},
                        new RequirementConfig { Item = "BowDraugrFang", Amount = 1, AmountPerLevel = 0}
                    }
                }); 
            var itemDrop = bow.ItemDrop;
            itemDrop.m_itemData.m_shared.m_damages.m_damage = damage.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_blunt = blunt.Value;
            itemDrop.m_itemData.m_shared.m_toolTier = tier.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_slash = slashval.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_pierce = pierce.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_chop = chop.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_pickaxe = pickaxe.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_fire = fire.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_frost = frost.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_lightning = lightning.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_poison = poison.Value;
            itemDrop.m_itemData.m_shared.m_damages.m_spirit = spirit.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_damage = damageper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_blunt = bluntper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_slash = slashper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_pierce = pierceper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_chop = chopper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_pickaxe = pickaxeper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_fire = fireper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_frost = frostper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_lightning = lightningper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_poison = poisonper.Value;
            itemDrop.m_itemData.m_shared.m_damagesPerLevel.m_spirit = spiritper.Value;

            itemDrop.m_itemData.m_shared.m_equipStatusEffect = WindlassEffect.StatusEffect;
            
            ItemManager.Instance.AddItem(bow);
            
        }

        private void AddStatusEffects(float damage,float bonus)
        {
            StatusEffect effect = ScriptableObject.CreateInstance<StatusEffect>();
            
            effect.name = "WindlassEffect";
            effect.m_name = "Windlass Spirit";
            effect.m_icon = AssetUtils.LoadSpriteFromFile("Windlass/Assets/Windlass.png");
            effect.m_startMessageType = MessageHud.MessageType.TopLeft;
            effect.m_startMessage = "The wind begins to howl";
            effect.m_stopMessageType = MessageHud.MessageType.TopLeft;
            effect.m_tooltip = "Gain Attack Power When Equipped";        
            m_damageBonus = damage;
            m_damageIncrement = m_damageBonus / 10;
            effect.m_tooltip = "Gain +" + m_damageIncrement * 100 + "% damage while equipped, up to " + m_damageBonus * 100 + "" +
            $"\nEvery 5 seconds, when an attack would have killed you, survive at 1 hp.";
            m_damageBonus = bonus;
            effect.m_tooltip = "Ranger Weapon Increased by " + bonus * 10 + "%";
            effect.ModifySpeed(ref damage);
            effect.ModifyRaiseSkill(Skills.SkillType.Bows, ref bonus);
            WindlassEffect = new CustomStatusEffect(effect, fixReference: true);
            ItemManager.Instance.AddStatusEffect(WindlassEffect);
        }
     
        private void AddArrows()
        {
            var arrowsfab = embeddedResourceBundle.LoadAsset<GameObject>("ArrowTest");
            var arrows_proj = embeddedResourceBundle.LoadAsset<GameObject>("bow_projectile_test");
            var arrows = new CustomItem(arrowsfab, fixReference: true,
                new ItemConfig
                {
                    Amount = 1,
                    CraftingStation = "forge",
                    MinStationLevel = 4,
                    Enabled = arrowsenabled.Value,
                    RepairStation = "forge",
                    Requirements = new[]
                    {
                        new RequirementConfig { Item = "DragonTear", Amount = 1},
                        new RequirementConfig { Item = "FineWood", Amount = 15},
                        new RequirementConfig { Item = "Feathers", Amount = 10}
                    }
                });
            PrefabManager.Instance.AddPrefab(arrows_proj);
            ItemManager.Instance.AddItem(arrows);
        }
       
        private void Configurator()
        {
            Config.SaveOnConfigSet = true;
            damage = Config.Bind("Windlass", "Overall Damage", 50, new ConfigDescription("Overall Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            blunt = Config.Bind("Windlass", "Blunt Damge", 50, new ConfigDescription("Blunt Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            slashval = Config.Bind("Windlass", "Slash Damage", 50, new ConfigDescription("Slash Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            pierce = Config.Bind("Windlass", "Pierce Damge", 50, new ConfigDescription("Pierce Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            chop = Config.Bind("Windlass", "Chop Damage", 50, new ConfigDescription("Chop Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            pickaxe = Config.Bind("Windlass", "PickAxe Damage", 50, new ConfigDescription("Pickaxe Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            fire = Config.Bind("Windlass", "Fire Damage", 50, new ConfigDescription("Fire Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            frost = Config.Bind("Windlass", "Frost Damage", 50, new ConfigDescription("Frost Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            lightning = Config.Bind("Windlass", "Lightning Damage", 50, new ConfigDescription("Lightning Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            poison = Config.Bind("Windlass", "Poison Damage", 50, new ConfigDescription("Poison Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            spirit = Config.Bind("Windlass", "Spirit Damage", 50, new ConfigDescription("Spirit Damage", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            
            damageper = Config.Bind("Windlass", "Overall Damage Per Level", 50, new ConfigDescription("Overall Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            bluntper = Config.Bind("Windlass", "Blunt Damage Per Level", 50, new ConfigDescription("Blunt Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            slashper = Config.Bind("Windlass", "Slash Damage Per Level", 50, new ConfigDescription("Slash Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            pierceper = Config.Bind("Windlass", "Pierce Damage Per Level", 50, new ConfigDescription("Pierce Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            chopper = Config.Bind("Windlass", "Chop Damage Per Level", 50, new ConfigDescription("Chop Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            pickaxeper = Config.Bind("Windlass", "PickAxe Damage Per Level", 50, new ConfigDescription("PickAxe Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            fireper = Config.Bind("Windlass", "Fire Damage Per Level", 50, new ConfigDescription("Fire Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            frostper = Config.Bind("Windlass", "Frost Damage Per Level", 50, new ConfigDescription("Frost Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            lightningper = Config.Bind("Windlass", "Lightning Damage Per Level", 50, new ConfigDescription("Lightning Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            poisonper = Config.Bind("Windlass", "Poison Damage Per Level", 50, new ConfigDescription("Poison Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            spiritper = Config.Bind("Windlass", "Spirit Damage Per Level", 50, new ConfigDescription("Spirit Damage per level", new AcceptableValueRange<int>(5, 250), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            
            tier = Config.Bind("Windlass", "Tool Tier", 5, new ConfigDescription("Tool Tier", new AcceptableValueRange<int>(0, 10), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            arrowsenabled = Config.Bind("Windlass", "Arrows Enabled", true, new ConfigDescription("Enable Special Arrows", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
           
            

        }
    }
}