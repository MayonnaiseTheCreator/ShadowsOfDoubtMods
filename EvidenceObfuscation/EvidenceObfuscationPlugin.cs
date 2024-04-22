﻿using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceObfuscation
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
#if MONO
    public class EvidenceObfuscationPlugin : BaseUnityPlugin
#elif IL2CPP
    public class EvidenceObfuscationPlugin : BasePlugin
#endif
    {
        public static ManualLogSource PluginLogger;

        public static ConfigEntry<bool> ModifyCityDirectory;
        public static ConfigEntry<bool> ShowAddressInCitizenCard;

#if MONO
        private void Awake()
        {
            PluginLogger = Logger;
#elif IL2CPP
        public override void Load()
        {
            PluginLogger = Log;
#endif

            ModifyCityDirectory = Config.Bind("City Directory", "Should the directory be modified to be phone numbers instead of addresses?", true);
            ShowAddressInCitizenCard = Config.Bind("City Directory", "Should the directory entry for citizens also include their address?", false);

            PluginLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            var harmony = new Harmony($"{MyPluginInfo.PLUGIN_GUID}");
            harmony.PatchAll();
            PluginLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is patched!");
        }
    }
}
