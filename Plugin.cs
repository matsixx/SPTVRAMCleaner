using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPTVRAMCleaner.Patches;
using HarmonyLib;
using SPT.Reflection;
using System.Reflection;

namespace SPTVRAMCleaner
{
    [BepInPlugin("com.matsix.sptvramcleaner", "SPTVRAMCleaner-matsix", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {

        public static ManualLogSource MyLog;

        private void Awake()
        {
            MyLog = Logger;
            Logger.LogInfo("VRAMCleaner loaded!");

            new VRAMCleaner().Enable();
            new VRAMCleanerReset().Enable();
            //new MethodTrack().Enable();
        }
    }
}
