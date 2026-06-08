using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using Rust;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("CoreLoot", "Corevalence", "2.0.0")]
    [Description("Pure vanilla loot multiplier")]
    class CoreLoot : RustPlugin
    {
        private PluginConfig config;
        private readonly HashSet<NetworkableId> processed = new HashSet<NetworkableId>();

        class CrateData
        {
            public bool Enabled = true;
            public double Multiplier = 2.0;
        }

        class PluginConfig
        {
            [JsonProperty("Global Multiplier")]
            public double GlobalMultiplier = 2.0;

            public Dictionary<string, double> ItemListMultipliers = new Dictionary<string, double>();
            public Dictionary<string, CrateData> Containers = new Dictionary<string, CrateData>();
            public List<string> Blacklist = new List<string>();
        }

        protected override void LoadDefaultConfig() => config = new PluginConfig();
        
        protected override void LoadConfig()
        {
            base.LoadConfig();
            config = Config.ReadObject<PluginConfig>() ?? new PluginConfig();
        }
        
        protected override void SaveConfig() => Config.WriteObject(config, true);

        void OnServerInitialized()
        {
            PopulateDefaults();
            timer.Once(8f, RefreshAllLootContainers);
            Puts($"CoreLoot v2.0.0 loaded | Global Multiplier: x{config.GlobalMultiplier}");
        }

        private void PopulateDefaults()
        {
            if (config.Containers.Count == 0)
            {
                Puts("Populating default container list...");
                var prefabs = UnityEngine.Object.FindObjectsOfType<LootContainer>()
                    .Select(c => c.ShortPrefabName)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .OrderBy(s => s);

                foreach (var p in prefabs)
                {
                    config.Containers[p] = new CrateData 
                    { 
                        Multiplier = config.GlobalMultiplier 
                    };
                }
            }
            SaveConfig();
        }

        object OnLootSpawn(LootContainer container) => ProcessContainer(container);

        void OnEntitySpawned(BaseNetworkable entity)
        {
            if (entity is LootContainer container)
                NextTick(() => ProcessContainer(container));
        }

        private object ProcessContainer(LootContainer container)
        {
            if (container == null || container.inventory == null || !container.enabled)
                return null;

            NetworkableId netId = container.net.ID;
            if (processed.Contains(netId)) return null;

            string prefab = container.ShortPrefabName;
            if (string.IsNullOrEmpty(prefab) || !config.Containers.TryGetValue(prefab, out var data) || !data.Enabled)
                return null;

            processed.Add(netId);

            NextTick(() => ApplyMultiplier(container, data));

            return null;
        }

        private void ApplyMultiplier(LootContainer container, CrateData data)
        {
            if (container?.inventory == null) return;

            try
            {
                var items = container.inventory.itemList.ToList();

                foreach (var item in items)
                {
                    if (item == null || config.Blacklist.Contains(item.info.shortname)) continue;

                    double mult = GetMultiplier(item.info.shortname, data);
                    if (mult <= 1.0) continue;

                    int newAmount = Mathf.Clamp((int)(item.amount * mult + 0.5f), 1, item.info.stackable);

                    if (newAmount != item.amount)
                    {
                        item.amount = newAmount;
                        item.MarkDirty();
                    }
                }
            }
            catch (Exception ex)
            {
                PrintError($"Error applying multiplier to {container.ShortPrefabName}: {ex.Message}");
            }
        }

        private double GetMultiplier(string shortname, CrateData data)
        {
            if (config.ItemListMultipliers.TryGetValue(shortname, out double m))
                return m;
            return data.Multiplier;
        }

        public void RefreshAllLootContainers()
        {
            processed.Clear();
            int count = 0;

            foreach (var container in UnityEngine.Object.FindObjectsOfType<LootContainer>())
            {
                if (container?.inventory != null)
                {
                    ProcessContainer(container);
                    count++;
                }
            }
            Puts($"CoreLoot: Refreshed loot in {count} containers.");
        }

        [ChatCommand("cl.refresh")]
        void CmdRefresh(BasePlayer player)
        {
            if (!player.IsAdmin) return;
            RefreshAllLootContainers();
            player.ChatMessage("CoreLoot: All loot containers refreshed!");
        }

        void Unload() => processed.Clear();
    }
}