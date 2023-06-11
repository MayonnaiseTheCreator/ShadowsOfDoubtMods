﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class Helpers
{
    public static void CopyFolder(DirectoryInfo source, DirectoryInfo target, bool overwrite, bool copy)
    {
        CopyFolder_Internal(source, target.CreateSubdirectory(source.Name), overwrite, copy);

        if (!copy) source.Delete(true);
    }

    private static void CopyFolder_Internal(DirectoryInfo source, DirectoryInfo target, bool overwrite, bool copy)
    {
        foreach (DirectoryInfo dir in source.GetDirectories())
        {
            CopyFolder_Internal(dir, target.CreateSubdirectory(dir.Name), overwrite, copy);
        }

        foreach (FileInfo file in source.GetFiles())
        {
            if(copy)
            {
                file.CopyTo(Path.Join(target.FullName, file.Name), overwrite);
            }
            else
            {
                file.MoveTo(Path.Join(target.FullName, file.Name), overwrite);
            }
        }
    }

    public static void RepointGUIDs(string pathIdMapPath, DirectoryInfo directory)
    {
        Dictionary<string, Remap> mapping = new Dictionary<string, Remap>();

        // Scripts
        foreach(var file in Directory.GetFiles(Path.Join(directory.FullName, "Scripts"), "*.cs.meta", SearchOption.AllDirectories))
        {
            var fo = new FileInfo(file);
            var guid = File.ReadAllLines(file)[1].Substring(6);
            mapping[guid] = new Remap()
            {
                type = Remap.RemapType.Both,
                id = FileIDUtil.Compute("", fo.Name.Split(".")[0]).ToString(),
                guid = "34dbb99afe9d0774ba685b3ff21205e7"
            };
        }

        // TMPPro
        // MonoBehaviour
        mapping["34dbb99afe9d0774ba685b3ff21205e7"] = new Remap()
        {
            type = Remap.RemapType.Both,
            id = "11500000",
            guid = "9541d86e2fd84c1d9990edf0852d74ab"
        };

        // Font Asset
        /* todo - need a proper script mapping system
        mapping["34dbb99afe9d0774ba685b3ff21205e7"] = new Remap()
        {
            type = Remap.RemapType.Both,
            id = "11500000",
            guid = "71c1514a6bd24e1e882cebbe1904ce04"
        };
        */

        // Material shaders
        mapping["2f4a68c7e72e2fe4a94462f14ffd2d2e"] = new Remap()
        {
            type = Remap.RemapType.PreserveId,
            guid = "6e4ae4064600d784cac1e41a9e6f2e59"
        };

        Remap.types = mapping;

        RepointGUID_Internal(directory);
    }

    private static void RepointGUID_Internal(DirectoryInfo directory)
    {
        var knownExtensions = new string[] { ".prefab", ".asset", ".mat" };

        foreach (DirectoryInfo dir in directory.GetDirectories())
            RepointGUID_Internal(dir);
        foreach (FileInfo file in directory.GetFiles())
        {
            if(knownExtensions.Contains(file.Extension))
            {
                string content = Regex.Replace(File.ReadAllText(file.FullName), "fileID: (.*), guid: (.{32})", match =>
                {
                    if (Remap.types.ContainsKey(match.Groups[2].Value))
                    {
                        var remapped = Remap.types[match.Groups[2].Value];

                        if (remapped.type == Remap.RemapType.Both)
                        {
                            return $"fileID: {remapped.id}, guid: {remapped.guid}";
                        }
                        else
                        {
                            return $"fileID: {match.Groups[1].Value}, guid: {remapped.guid}";
                        }
                    }

                    return match.Value;
                }, RegexOptions.Multiline | RegexOptions.IgnoreCase);

                File.WriteAllText(file.FullName, content);

                Console.WriteLine($"{file.Name} processed");
            }
        }
    }

    private class Remap
    {
        public static Dictionary<string, Remap> types;

        public enum RemapType
        {
            Both,
            PreserveId
        }

        public RemapType type;
        public string? guid;
        public string? id;
    }
}