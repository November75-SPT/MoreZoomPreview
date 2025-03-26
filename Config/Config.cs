using BepInEx.Configuration;

namespace MoreZoomPreview.Config
{
    internal static class ConfigManager
    {
        public static ConfigEntry<float> MaxZoomIn;
        public static ConfigEntry<float> SensitivityMultiplier;

        public static void Init(ConfigFile configFile)
        {
            MaxZoomIn = configFile.Bind(
                "Section", 
                "Max Zoom In", 
                -0.1f, 
                new ConfigDescription(
                    "Description", 
                    new AcceptableValueRange<float>(-1f, -0.01f)));
            SensitivityMultiplier = configFile.Bind(
                "Section", 
                "Sensitivity Multiplier", 
                1f, 
                new ConfigDescription(
                    "Description", 
                    new AcceptableValueRange<float>(0.001f, 2f)));
        }
    }
}