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
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Major)]
    internal class Windlass : BaseUnityPlugin
    {
        public const string PluginGUID = "com.zarboz.windlass";
        public const string PluginName = "Windlass";
        public const string PluginVersion = "0.1.2";
        public static new Jotunn.Logger Logger;
        private AssetBundle embeddedResourceBundle;
        public static CustomStatusEffect SE_windlass;
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
        private ConfigEntry<int> attackforce;
        private ConfigEntry<bool> arrowsenabled;
        private ConfigEntry<float> SEStamdrain;
        private ConfigEntry<float> SEdam;
        private ConfigEntry<float> SEStamregen;
        private ConfigEntry<float> SEHealthregen;
        private ConfigEntry<float> SEstealth;
        private float Skillbuff = 0.010f;

        public CustomStatusEffect SE_Windlass { get; private set; }

        private void Awake()
        {
            Configurator();
            LoadAssets();
            SE_statthing();
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
            itemDrop.m_itemData.m_shared.m_attackForce = attackforce.Value;
            itemDrop.m_itemData.m_shared.m_equipStatusEffect = SE_windlass.StatusEffect;
            ItemManager.Instance.AddItem(bow);
            
        }

        private void SE_statthing()
        {
            SE_Stats SE = ScriptableObject.CreateInstance<SE_Stats>();
            SE.name = "WindlassEffect";
            SE.m_name = "Windlass Spirit";
            SE.m_icon = AssetUtils.LoadSpriteFromFile("Windlass/Assets/Windlass.png");
            SE.m_tooltip = "Increased Bow Skills gain when Windlass is equipped";
            SE.m_runStaminaDrainModifier = -SEStamdrain.Value;
            SE.m_modifyAttackSkill = Skills.SkillType.Bows;
            SE.m_damageModifier = SEdam.Value;
            SE.m_staminaRegenMultiplier = SEStamregen.Value;
            SE.m_healthRegenMultiplier = SEHealthregen.Value;
            SE.m_noiseModifier = SEstealth.Value;
            SE.ModifyRaiseSkill(Skills.SkillType.Bows, ref Skillbuff);

            SE_windlass = new CustomStatusEffect(SE, fixReference: true);
            ItemManager.Instance.AddStatusEffect(SE_windlass);
        }
        private void AddArrows()
        {
            var arrowsfab = embeddedResourceBundle.LoadAsset<GameObject>("ArrowTest");
            var arrows_proj = embeddedResourceBundle.LoadAsset<GameObject>("bow_projectile_test");
            var arrows = new CustomItem(arrowsfab, fixReference: true,
                new ItemConfig
                {
                    Amount = 5,
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
            damage = Config.Bind("Windlass Damage", "Overall Damage", 250, new ConfigDescription("Overall Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            blunt = Config.Bind("Windlass Damage", "Blunt Damge", 50, new ConfigDescription("Blunt Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            slashval = Config.Bind("Windlass Damage", "Slash Damage", 500, new ConfigDescription("Slash Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            pierce = Config.Bind("Windlass Damage", "Pierce Damge", 400, new ConfigDescription("Pierce Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            chop = Config.Bind("Windlass Damage", "Chop Damage", 950, new ConfigDescription("Chop Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            pickaxe = Config.Bind("Windlass Damage", "PickAxe Damage", 1500, new ConfigDescription("Pickaxe Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            fire = Config.Bind("Windlass Damage", "Fire Damage", 0, new ConfigDescription("Fire Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            frost = Config.Bind("Windlass Damage", "Frost Damage", 1500, new ConfigDescription("Frost Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            lightning = Config.Bind("Windlass Damage", "Lightning Damage", 1500, new ConfigDescription("Lightning Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            poison = Config.Bind("Windlass Damage", "Poison Damage", 500, new ConfigDescription("Poison Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            spirit = Config.Bind("Windlass Damage", "Spirit Damage", 1500, new ConfigDescription("Spirit Damage", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            
            damageper = Config.Bind("Per Level", "Overall Damage Per Level", 50, new ConfigDescription("Overall Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            bluntper = Config.Bind("Per Level", "Blunt Damage Per Level", 50, new ConfigDescription("Blunt Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            slashper = Config.Bind("Per Level", "Slash Damage Per Level", 50, new ConfigDescription("Slash Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            pierceper = Config.Bind("Per Level", "Pierce Damage Per Level", 50, new ConfigDescription("Pierce Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            chopper = Config.Bind("Per Level", "Chop Damage Per Level", 50, new ConfigDescription("Chop Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            pickaxeper = Config.Bind("Per Level", "PickAxe Damage Per Level", 50, new ConfigDescription("PickAxe Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            fireper = Config.Bind("Per Level", "Fire Damage Per Level", 50, new ConfigDescription("Fire Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            frostper = Config.Bind("Per Level", "Frost Damage Per Level", 50, new ConfigDescription("Frost Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            lightningper = Config.Bind("Per Level", "Lightning Damage Per Level", 50, new ConfigDescription("Lightning Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            poisonper = Config.Bind("Per Level", "Poison Damage Per Level", 50, new ConfigDescription("Poison Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            spiritper = Config.Bind("Per Level", "Spirit Damage Per Level", 50, new ConfigDescription("Spirit Damage per level", new AcceptableValueRange<int>(0, 2500), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            
            tier = Config.Bind("Windlass", "Tool Tier", 5, new ConfigDescription("Tool Tier", new AcceptableValueRange<int>(0, 10), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            attackforce = Config.Bind("Windlass", "Attack Force", 90, new ConfigDescription("Attack Force", new AcceptableValueRange<int>(0, 100), null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            arrowsenabled = Config.Bind("Windlass", "Arrows Enabled", true, new ConfigDescription("Enable Special Arrows", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));


            SEStamdrain = Config.Bind("SE", "StaminaDrin", -0.15f, new ConfigDescription("Stamina Drain", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            SEdam = Config.Bind("SE", "DamageMult", 1.5f, new ConfigDescription("Damage Multiplier", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            SEStamregen = Config.Bind("SE", "StaminaRegen", 1.2f, new ConfigDescription("Stamina Regen",null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            SEHealthregen = Config.Bind("SE", "HealthRegen", 1.2f, new ConfigDescription("Healh Regen",null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            SEstealth = Config.Bind("SE", "Stealth", 1.5f, new ConfigDescription("Stealth buff", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
            

            

        }
    }
}