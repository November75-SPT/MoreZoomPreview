using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
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
        }
    }
}
