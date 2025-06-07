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
            return AccessTools.Method(typeof(Player), nameof(Player.VisualPass));
        }
        
        private static bool _hasCountdownFinished = false;
        private static float _countdownFinishedTime = -1f;
        private const float DelaySeconds = 3f;

        [PatchPostfix]
        static void Postfix(Player __instance)
        {
            if (!__instance.IsYourPlayer)
                return;

            if (_hasCountdownFinished && Time.realtimeSinceStartup >= _countdownFinishedTime + DelaySeconds)
            {
                _hasCountdownFinished = false;
                _countdownFinishedTime = -1f;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                Resources.UnloadUnusedAssets();

                Plugin.MyLog.LogWarning("Unused Assets have been unloaded - VRAM Usage cleaned");
            }
        }
        public static void SetCountdownFinished()
        {
            _hasCountdownFinished = true;
            _countdownFinishedTime = Time.realtimeSinceStartup;
        }
    }

    internal class VRAMCleanerInit : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchmakerFinalCountdown), nameof(MatchmakerFinalCountdown.Update));
        }

        private static readonly AccessTools.FieldRef<MatchmakerFinalCountdown, DateTime> _dateTimeRef =
    AccessTools.FieldRefAccess<MatchmakerFinalCountdown, DateTime>("dateTime_0");

        [PatchPostfix]
        private static void Postfix(MatchmakerFinalCountdown __instance)
        {
            TimeSpan timeSpan = _dateTimeRef(__instance) - EFTDateTimeClass.Now;
            if (timeSpan.TotalSeconds <= 0.0)
            {
                VRAMCleaner.SetCountdownFinished();
            }
        }
    }
}
