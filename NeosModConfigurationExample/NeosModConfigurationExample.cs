using NeosModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using BaseX;

namespace NeosModConfigurationExample
{
    public class NeosModConfigurationExample : NeosMod
    {
        public override string Name => "NeosModConfigurationExample";
        public override string Author => "runtime";
        public override string Version => "1.4.0";
        public override string Link => "https://github.com/zkxs/NeosModConfigurationExample";

        [AutoRegisterConfigKey]
        private readonly ModConfigurationKey<bool> KEY_ENABLE = new ModConfigurationKey<bool>("enabled", "Enables the NeosModConfigurationExample mod", () => true);

        [AutoRegisterConfigKey]
        private readonly ModConfigurationKey<int> KEY_COUNT = new ModConfigurationKey<int>("count", "Example counter", internalAccessOnly: true);

        [AutoRegisterConfigKey]
        private readonly ModConfigurationKey<color> KEY_TEST_COLOR = new ModConfigurationKey<color>("test_color", "serialization test color");

        [AutoRegisterConfigKey]
        private readonly ModConfigurationKey<float3> KEY_TEST_FLOAT3 = new ModConfigurationKey<float3>("test_float3", "serialization test float3");

        [AutoRegisterConfigKey]
        private readonly ModConfigurationKey<Alignment> KEY_TEST_ENUM = new ModConfigurationKey<Alignment>("test_enum", "serialization test enum");

        [AutoRegisterConfigKey]
        private readonly ModConfigurationKey<CustomClass> KEY_TEST_CUSTOM_CLASS = new ModConfigurationKey<CustomClass>("test_custom_class", "serialization test custom class");

        // this override lets us change optional settings in our configuration definition
        public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder)
        {
            builder
                .Version(new Version(1, 0, 0)) // manually set config version (default is 1.0.0)
                .AutoSave(false); // don't autosave on Neos shutdown (default is true)
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

            // test serializing some Froox types
            config.Set(KEY_TEST_COLOR, new color(0.5f, 0.5f, 0.5f));
            config.Set(KEY_TEST_FLOAT3, new float3(1.0f, 2.0f, 3.0f));
            config.Set(KEY_TEST_ENUM, Alignment.BottomCenter);
            config.Set(KEY_TEST_CUSTOM_CLASS, new CustomClass("foo", "bar", "baz"));

            // It's good practice to save after you modify configuration values. This writes the in-memory changes to disk.
            config.Save();

            // list all configs
            EnumerateConfigs();
        }

        private void UpdateCount()
        {
            ModConfiguration config = GetConfiguration();
            int countValue = 0;
            if (config.TryGetValue(KEY_COUNT, out countValue))
            {
                int oldValue = countValue++;
                Msg($"Incrementing count from {oldValue} to {countValue}");
            }
            else
            {
                Msg($"Initializing count to {countValue}");
            }

            // This sets the value in memory. It's immediately available to anyone reading this config.
            config.Set(KEY_COUNT, countValue);
        }

        private void EnumerateConfigs()
        {
            List<NeosModBase> mods = new List<NeosModBase>(ModLoader.Mods());
            List<NeosModBase> configuredMods = mods
                .Where(m => m.GetConfiguration() != null) // mods that do not define a configuration have a null GetConfiguration() result.
                .ToList();
            Debug($"{mods.Count} mods are loaded, {configuredMods.Count} of them have configurations");

            foreach (NeosModBase mod in configuredMods)
            {
                ModConfiguration config = mod.GetConfiguration();
                foreach (ModConfigurationKey key in config.ConfigurationItemDefinitions)
                {
                    if (!key.InternalAccessOnly) // As we are an external mod enumerating configs, we should ignore internal-only configuration items
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
