using EFT;
using EFT.UI.Matchmaker;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SPTVRAMCleaner.Patches
{
    internal class VRAMCleaner : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(EFT.UI.PreloaderUI), nameof(EFT.UI.PreloaderUI.ShowRaidStartInfo));
        }

        [PatchPostfix]
        static void Postfix(EFT.UI.PreloaderUI __instance)
        {

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Resources.UnloadUnusedAssets();

            Plugin.MyLog.LogWarning("Unused Assets have been unloaded - VRAM Usage cleaned");
        }
    }
}
