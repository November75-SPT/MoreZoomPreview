using System;
using System.Reflection;
using SPT.Reflection.Patching;
using EFT.UI.Map;
using HarmonyLib;
using EFT.UI.WeaponModding;
using UnityEngine;
using System.Collections.Generic;
using MoreZoomPreview.Config;

namespace MoreZoomPreview.Patches
{
    public class WeaponPreviewZoomPatch : ModulePatch
    {
        private static FieldInfo _nullable_0FieldInfo = AccessTools.Field(typeof(WeaponPreview), "nullable_0");
        protected override MethodBase GetTargetMethod()
        {        

            return AccessTools.Method(typeof(WeaponPreview), nameof(WeaponPreview.Zoom));
        }

        [PatchPrefix]
        public static bool PatchPrefix(WeaponPreview __instance, float zoom, ref float __state)
        {
            var nullable_0 = _nullable_0FieldInfo.GetValue(__instance) as Bounds?;
            if (__instance.WeaponPreviewCamera == null || nullable_0 == null) return false;

            if (!nullable_0.HasValue)
            {
                return false;
            }

            zoom *= ConfigManager.SensitivityMultiplier.Value;

            Transform cameraTransform = __instance.WeaponPreviewCamera.transform;
            cameraTransform.Translate(new Vector3(0f, 0f, zoom));

            // Remove visibility restriction
            // if (zoom > 0f && !GClass839.IsFullyVisibleByCamera(__instance.WeaponPreviewCamera, nullable_0.Value))
            // {
            //     do
            //     {
            //         cameraTransform.Translate(new Vector3(0f, 0f, -0.01f));
            //     }
            //     while (!GClass839.IsFullyVisibleByCamera(__instance.WeaponPreviewCamera, nullable_0.Value));
            // }

            Vector3 localPosition = cameraTransform.localPosition;            
            localPosition.z = Mathf.Clamp(localPosition.z, -2.7f, ConfigManager.MaxZoomIn.Value);   
            cameraTransform.localPosition = localPosition;
#if DEBUG
            Plugin.log.LogInfo($"Custom Zoom: input:{zoom} camera.z:{cameraTransform.localPosition.z}");
#endif

            return false; // Prevent original method from executing
        }
    }
}


/* 3.11 original method
    public void Zoom(float zoom)
    {
        if (!nullable_0.HasValue)
        {
            return;
        }

        Transform transform = WeaponPreviewCamera.transform;
        transform.Translate(new Vector3(0f, 0f, zoom));
        if (zoom > 0f && !GClass839.IsFullyVisibleByCamera(WeaponPreviewCamera, nullable_0.Value))
        {
            do
            {
                transform.Translate(new Vector3(0f, 0f, -0.01f));
            }
            while (!GClass839.IsFullyVisibleByCamera(WeaponPreviewCamera, nullable_0.Value));
        }

        Vector3 localPosition = transform.localPosition;
        localPosition.z = Mathf.Clamp(localPosition.z, -2.7f, -0.45f);
        transform.localPosition = localPosition;
    }
*/