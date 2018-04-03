using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Xml;
using LSLib.LS;
using LSLib.LS.Enums;
using Newtonsoft.Json;

namespace D_OS_Save_Editor
{
    public class Savegame
    {
        public string SavegameName { get; set; }
        public string SavegamePath { get; set; }
        public string SavegameFullFile { get; set; }
        public string UnpackDirectory { get; }
        public Player[] Players { get; set; }
        public Game GameVersion { get; }

        public Savegame(string savegameFullFile, string unpackDirectory, Game gameVersion)
        {
            SavegameFullFile = savegameFullFile;
            UnpackDirectory = unpackDirectory;
            GameVersion = gameVersion;

            SavegameName = savegameFullFile.Split(Path.DirectorySeparatorChar).Last();
            SavegamePath = savegameFullFile.Substring(0, savegameFullFile.Length - SavegameName.Length - 1);
        }

        public void ParseLsx()
        {
            // load xml
            var doc = new XmlDocument();
            doc.Load(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");

            Players = LsxParser.ParsePlayer(doc);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void WriteEditsToLsx()
        {
            // load xml
            var doc = new XmlDocument();
            doc.Load(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");

            doc = LsxParser.WritePlayer(doc, Players);

            doc.Save(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");
        }

        public void UnpackSavegame()
        {
            // unpackage
            var packager = new Packager();
            packager.UncompressPackage(SavegameFullFile, UnpackDirectory);
            // uncompress global.lsf
            var global = ResourceUtils.LoadResource(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsf");
            var outputVersion = GameVersion == Game.DivinityOriginalSin2
                ? FileVersion.VerExtendedNodes
                : FileVersion.VerChunkedCompress;
            ResourceUtils.SaveResource(global, UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx",
                ResourceFormat.LSX, outputVersion);
        }

        public void PackSavegame()
        {
            // compress .lsx
            var global = ResourceUtils.LoadResource(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");
            var outputVersion = GameVersion == Game.DivinityOriginalSin2
                ? FileVersion.VerExtendedNodes
                : FileVersion.VerChunkedCompress;
            ResourceUtils.SaveResource(global, UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsf",
                ResourceFormat.LSF, outputVersion);

            // delete .lsx
            File.Delete(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");

            // packup to .lsv
            var packager = new Packager();
            packager.CreatePackage(SavegameFullFile, UnpackDirectory, PackageVersion.V13, CompressionMethod.LZ4, false);
        }

        public void DumpSaveGame()
        {
            var fileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}SaveGame_{DateTime.Now:yyMMdd_HHmmss}";
            using (var file = File.CreateText(fileName + ".json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, this);
            }

            using (var zip = ZipFile.Open(fileName + ".zip", ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(fileName + ".json", "SaveGame.json");
            }
#if !DEBUG
            File.Delete(fileName + ".json");
#endif
        }

        public void DumpAllInventory()
        {
            var fileName =
                $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}AllInventory_{DateTime.Now:yyMMdd_HHmmss}";
            var fileNames = new string[Players.Length];
            for (var i = 0; i < Players.Length; i++)
            {
                fileNames[i] = $"{fileName}_{i}";
                using (var file = File.CreateText(fileNames[i] + ".json"))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, Players[i].Items);
                }
            }

            using (var zip = ZipFile.Open(fileName + ".zip", ZipArchiveMode.Create))
            {
                for (var i = 0; i < fileNames.Length; i++)
                {
                    zip.CreateEntryFromFile(fileNames[i] + ".json", $"AllInventory_{i}.json");
                }
            }
#if !DEBUG
            foreach (var s in fileNames)
                File.Delete(s + ".json");
#endif
        }

        public static string DumpItem(Item item, string msg)
        {
            var name = $"{item.StatsName}_{DateTime.Now:yyMMdd_HHmmss}";
            var fileName =
                $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{name}";
            using (var file = File.CreateText(fileName + ".json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, item);
                serializer.Serialize(file, msg);
            }

            using (var zip = ZipFile.Open(fileName + ".zip", ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(fileName + ".json", fileName + ".json");
            }
#if !DEBUG
            File.Delete(fileName + ".json");
#endif
            return name + ".zip";
        }

        public void DumpAllModifier()
        {
            var fileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}AllModifier_{DateTime.Now:yyMMdd_HHmmss}";
            var modList = new List<string>();
            using (var file = new StreamWriter(fileName + ".txt"))
            {
                foreach (var player in Players)
                {
                    foreach (var item in player.Items)
                    {
                        if (item.Generation == null) continue;
                        foreach (var boost in item.Generation.Boosts)
                        {
                            if (modList.Contains(item.ItemSort+boost)) continue;
                            file.WriteLine(item.ItemSort + " " + boost);
                            modList.Add(item.ItemSort+boost);
                        }
                    }
                }
            }

            using (var zip = ZipFile.Open(fileName + ".zip", ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(fileName + ".txt", "AllModifier.txt");
            }
#if !DEBUG
            File.Delete(fileName + ".json");
#endif
        }

        public static Savegame GetSavegameFromJson(string jsonFile)
        {
            Savegame sg;
            using (var sr = new StreamReader(jsonFile))
            {
                sg = JsonConvert.DeserializeObject<Savegame>(sr.ReadToEnd());
            }

            return sg;
        }
    }
}
