﻿using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace DDSLoader
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class DDSLoaderPlugin : BasePlugin
    {
        public static ManualLogSource Logger;

        public static List<DirectoryInfo> modsToLoadFrom = new List<DirectoryInfo>();

        public override void Load()
        {
            Logger = Log;

            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            var harmony = new Harmony($"{PluginInfo.PLUGIN_GUID}");
            harmony.PatchAll();
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is patched!");

            // Load all folders named DDSContent (includes subfolders), unless they have a disable file
            modsToLoadFrom = Directory.GetDirectories(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), ".."), "DDSContent", SearchOption.AllDirectories)
                .Select(dirPath => new DirectoryInfo(dirPath))
                .Where(dir => !File.Exists(Path.Combine(dir.Parent.FullName, "disabled.txt")))
                .ToList();
        }
    }
}
