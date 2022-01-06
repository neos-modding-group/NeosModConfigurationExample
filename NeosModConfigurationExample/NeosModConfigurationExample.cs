using NeosModLoader;
using System;
using System.Collections.Generic;

namespace NeosModConfigurationExample
{
    public class NeosModConfigurationExample : NeosMod
    {
        public override string Name => "NeosModConfigurationExample";
        public override string Author => "runtime";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/zkxs/NeosModConfigurationExample";

        private readonly ModConfigurationKey<int> KEY_COUNT = new ModConfigurationKey<int>("count", "Example counter", internalAccessOnly: true);

        public override ModConfigurationDefinition GetConfigurationDefinition()
        {
            List<ModConfigurationKey> keys = new List<ModConfigurationKey>();
            keys.Add(KEY_COUNT);
            return DefineConfiguration(new Version(1, 0, 0), keys);
        }

        public override void OnEngineInit()
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
    }
}
