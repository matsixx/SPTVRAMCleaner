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
        public static bool cleanerRan = false;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(EFT.UI.PreloaderUI), nameof(EFT.UI.PreloaderUI.ShowRaidStartInfo));
        }

        [PatchPostfix]
        static void Postfix(EFT.UI.PreloaderUI __instance)
        {
            if (cleanerRan == false)
            {
                cleanerRan = true;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Resources.UnloadUnusedAssets();
                Plugin.MyLog.LogInfo("Unused Assets have been unloaded - VRAM Usage cleaned");
            }
        }
    }

    internal class VRAMCleanerReset : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchmakerFinalCountdown), "Show", new[] { typeof(Profile), typeof(DateTime) });
        }

        [PatchPrefix]
        static void Prefix(MatchmakerFinalCountdown __instance)
        {
            Plugin.MyLog.LogInfo("VRAMCleanerReset - Resetting cleanerRan to false");
            VRAMCleaner.cleanerRan = false;
        }
    }
}
