using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using EFT.UI;
using HarmonyLib;
using MoreZoomPreview.Config;
using MoreZoomPreview.Patches;

namespace MoreZoomPreview
{
    [BepInPlugin("com.November75.MoreZoomPreview", "MoreZoomPreview", "1.0.0")]
    [BepInDependency("com.SPT.core", "3.11.0")]
    [BepInDependency("Tyfon.UIFixes", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource log;
        private void Awake()
        {
            log = Logger;
            ConfigManager.Init(Config);
            new WeaponPreviewZoomPatch().Enable();

            var harmony = new Harmony("com.November75.MoreZoomPreview");
            // UIFixes가 로드되었는지 확인 후 비활성화
             try
            {
                if (Loaded())
                {
                    DisableUIFixesZoomPatch(harmony);
                }
                else
                {
                    log.LogInfo("UIFixes not detected or version too old, skipping patch disable.");
                }
            }
            catch (Exception ex)
            {
                log.LogWarning($"Failed to disable UIFixes patch: {ex.Message}");
            }
        }
        private static bool? UIFixesLoaded = null;
        private static readonly Version RequiredVersion = new Version("1.0.0"); // UIFixes 최소 버전

        private static bool Loaded()
        {
            if (!UIFixesLoaded.HasValue)
            {
                log.LogInfo("Checking loaded plugins:");
                foreach (var plugin in Chainloader.PluginInfos)
                {
                    log.LogInfo($"Plugin: {plugin.Key}, Version: {plugin.Value.Metadata.Version}");
                }

                bool present = Chainloader.PluginInfos.TryGetValue("Tyfon.UIFixes", out PluginInfo pluginInfo);
                UIFixesLoaded = present && pluginInfo.Metadata.Version >= RequiredVersion;
                if (UIFixesLoaded.Value)
                {
                    log.LogInfo($"UIFixes detected, version: {pluginInfo.Metadata.Version}");
                }
                else
                {
                    log.LogWarning("UIFixes not found in Chainloader.PluginInfos.");
                }
            }
            return UIFixesLoaded.Value;
        }

        private static void DisableUIFixesZoomPatch(Harmony harmony)
        {
            var originalMethod = AccessTools.Method(typeof(EditBuildScreen), nameof(EditBuildScreen.Awake));
            if (originalMethod == null)
            {
                log.LogWarning("Could not find EditBuildScreen.Awake method.");
                return;
            }

            // UIFixes 패치 동적 로드
            var patchType = Type.GetType("UIFixes.WeaponZoomPatches+EditBuildScreenZoomPatch, Tyfon.UIFixes");
            if (patchType != null)
            {
                var prefixMethod = AccessTools.Method(patchType, "Prefix");
                if (prefixMethod != null)
                {
                    harmony.Unpatch(originalMethod, prefixMethod);
                    log.LogInfo("UIFixes EditBuildScreenZoomPatch disabled.");
                }
                else
                {
                    log.LogWarning("Could not find Prefix method in UIFixes.WeaponZoomPatches.EditBuildScreenZoomPatch.");
                }
            }
            else
            {
                log.LogWarning("UIFixes.WeaponZoomPatches+EditBuildScreenZoomPatch type not found.");
            }
        }
        // private static void DisableUIFixesZoomPatch(Harmony harmony)
        // {
        //     var originalMethod = AccessTools.Method(typeof(EditBuildScreen), nameof(EditBuildScreen.Awake));
        //     var prefixMethod = AccessTools.Method(typeof(UIFixes.WeaponZoomPatches.EditBuildScreenZoomPatch), "Prefix");
        //     harmony.Unpatch(originalMethod, prefixMethod);
        //     log.LogInfo("UIFixes EditBuildScreenZoomPatch disabled.");
        // }
    }
}
