using NeosModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeosModConfigurationExample
{
    public class NeosModConfigurationExample : NeosMod
    {
        public override string Name => "NeosModConfigurationExample";
        public override string Author => "runtime";
        public override string Version => "1.1.0";
        public override string Link => "https://github.com/zkxs/NeosModConfigurationExample";

        private readonly ModConfigurationKey<bool> KEY_ENABLE = new ModConfigurationKey<bool>("enabled", "Enables the NeosModConfigurationExample mod", () => true);
        private readonly ModConfigurationKey<int> KEY_COUNT = new ModConfigurationKey<int>("count", "Example counter", internalAccessOnly: true);

        public override ModConfigurationDefinition GetConfigurationDefinition()
        {
            List<ModConfigurationKey> keys = new List<ModConfigurationKey>();
            keys.Add(KEY_ENABLE);
            keys.Add(KEY_COUNT);
            return DefineConfiguration(new Version(1, 0, 0), keys);
        }

        public override void OnEngineInit()
        {
            // disable the mod if the enabled config has been set to false
            ModConfiguration config = GetConfiguration();
            if (!config.GetValue(KEY_ENABLE)) // this is safe as the config has a default value
            {
                Debug("Mod disabled, returning early.");
                return;
            }

            // register event handler
            ModConfiguration.OnAnyConfigurationChanged += OnConfigurationChanged;

            // update our counter config
            UpdateCount();

            // list all configs
            EnumerateConfigs();
        }

        private void UpdateCount()
        {
            ModConfiguration config = GetConfiguration();
            int countValue = default(int);
            if (config.TryGetValue(KEY_COUNT, out countValue))
            {
                int oldValue = countValue++;
                Msg($"Incrementing count from {oldValue} to {countValue}");
            }
            else
            {
                Msg($"Initializing count to {countValue}");
            }

            config.Set(KEY_COUNT, countValue);
            config.Save();
        }

        private void EnumerateConfigs()
        {
            List<NeosModBase> mods = new List<NeosModBase>(ModLoader.Mods());
            List<NeosModBase> configuredMods = mods
                .Where(m => m.GetConfiguration() != null)
                .ToList();
            Debug($"{mods.Count} mods are loaded, {configuredMods.Count} of them have configurations");

            foreach (NeosModBase mod in configuredMods)
            {
                ModConfiguration config = mod.GetConfiguration();
                foreach (ModConfigurationKey key in config.ConfigurationItemDefinitions)
                {
                    if (!key.InternalAccessOnly)
                    {

                        if (config.TryGetValue(key, out object value))
                        {
                            Msg($"{mod.Name} has configuration \"{key.Name}\" with type \"{key.ValueType()}\" and value \"{value}\"");
                        }
                        else
                        {
                            Msg($"{mod.Name} has configuration \"{key.Name}\" with type \"{key.ValueType()}\" and no value");
                        }
                    }
                }
            }
        }

        private void OnConfigurationChanged(ConfigurationChangedEvent @event)
        {
            Debug($"ConfigurationChangedEvent fired for mod \"{@event.Config.Owner.Name}\" Config \"{@event.Key.Name}\"");
        }
    }
}
