using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        public Meta Meta { get; set; }
        public string[] AllSkills { get; set; }
        public Game GameVersion { get; }

        public Savegame(string savegameFullFile, string unpackDirectory, Game gameVersion)
        {
            SavegameFullFile = savegameFullFile;
            UnpackDirectory = unpackDirectory;
            GameVersion = gameVersion;

            SavegameName = savegameFullFile.Split(Path.DirectorySeparatorChar).Last();
            SavegamePath = savegameFullFile.Substring(0, savegameFullFile.Length - SavegameName.Length - 1);
        }

        public async Task ParseLsxAsync(IProgress<string> progress)
        {
            // update progress
            progress.Report("Loading unpacked savegame.");
            await Task.Delay(1);
            // load xml
            var doc = new XmlDocument();
            var metaDoc = new XmlDocument();
            doc.Load(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");
            // update progress
            progress.Report("Analysing savegame.");
            await Task.Delay(1);
            // parse xlml
            Players = LsxParser.ParsePlayer(doc);
            // update progress
            progress.Report("Loading data.");
            await Task.Delay(1);
            DataTable.UserGenerationBoosts = LsxParser.GenerationBoostCollector.ToArray();
            DataTable.UserStatsBoosts = LsxParser.StatsBoostsCollector.ToArray();
        }

        public async Task WriteEditsToLsxAsync(IProgress<string> progress)
        {
            // update progress
            progress.Report("Loading package.");
            await Task.Delay(1);
            // load xml
            var doc = new XmlDocument();
            doc.Load(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");

            // update progress
            progress.Report("Applying changes.");
            await Task.Delay(1);
            doc = LsxParser.WritePlayer(doc, Players);

            // update progress
            progress.Report("Saving package.");
            await Task.Delay(1);
            doc.Save(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");
        }

        /// <summary>
        /// unpack savegame to UnpackDirectory (lsv->lsx)
        /// </summary>
        public async Task UnpackSavegameAsync(IProgress<string> progress, bool metaOnly = false)
        {
            // update progress
            progress.Report("Unpacking savegame.");
            await Task.Delay(1);
            // unpackage
            var packager = new Packager();
            packager.UncompressPackage(SavegameFullFile, UnpackDirectory);
            // update progress
            progress.Report("Uncompressing package.");
            await Task.Delay(1);
            var outputVersion = GameVersion == Game.DivinityOriginalSin2
                ? FileVersion.VerExtendedNodes
                : FileVersion.VerChunkedCompress;
            // uncompress and save global.lsf
            ResourceUtils.SaveResource(
                ResourceUtils.LoadResource(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsf"),
                UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx", ResourceFormat.LSX, outputVersion);
        }

        /// <summary>
        /// Pack up unpacked savegame (lsx->lsv)
        /// </summary>
        public async Task PackSavegameAsync(IProgress<string> progress)
        {
            // update progress
            progress.Report("Compressing package.");
            await Task.Delay(1);

            // compress .lsx
            var global = ResourceUtils.LoadResource(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");
            var outputVersion = GameVersion == Game.DivinityOriginalSin2
                ? FileVersion.VerExtendedNodes
                : FileVersion.VerChunkedCompress;
            
            ResourceUtils.SaveResource(global, UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsf",
                ResourceFormat.LSF, outputVersion);

            // delete .lsx
            File.Delete(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");
            if (File.Exists(UnpackDirectory + Path.DirectorySeparatorChar + "meta.lsx"))
                File.Delete(UnpackDirectory + Path.DirectorySeparatorChar + "meta.lsx");

            // update progress
            progress.Report("Packing savegame.");
            await Task.Delay(1);
            // packup to .lsv
            var packager = new Packager();
            packager.CreatePackage(SavegameFullFile, UnpackDirectory, PackageVersion.V13, CompressionMethod.LZ4, false);
        }

        /// <summary>
        /// unpack savegame to UnpackDirectory (lsv->lsx)
        /// </summary>
        public void GetMetaBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            
            // unpackage
            var packager = new Packager();
            packager.UncompressPackage(SavegameFullFile, UnpackDirectory);
            if (worker?.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
            var outputVersion = GameVersion == Game.DivinityOriginalSin2
                ? FileVersion.VerExtendedNodes
                : FileVersion.VerChunkedCompress;
            // uncompress and save meta.lsf
            ResourceUtils.SaveResource(
                ResourceUtils.LoadResource(UnpackDirectory + Path.DirectorySeparatorChar + "meta.lsf"),
                UnpackDirectory + Path.DirectorySeparatorChar + "meta.lsx", ResourceFormat.LSX, outputVersion);
            if (worker?.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
            var metaDoc = new XmlDocument();
            metaDoc.Load(UnpackDirectory + Path.DirectorySeparatorChar + "meta.lsx");
            e.Result = LsxParser.ParseMeta(metaDoc);
        }

        #region dump methods
        /// <summary>
        /// Serialize the Savegame object to a zipped json file. This json file can be deserialized back to the Savegame object using GetSavegameFromJson method.
        /// </summary>
        public void DumpSavegame()
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

        /// <summary>
        /// Serialize all players' items to a zipped json file.
        /// </summary>
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

        /// <summary>
        /// Serialize the supplied Item object to a zipped json file, with a message (an exception message could be useful)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="msg"></param>
        /// <returns>file name</returns>
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

        /// <summary>
        /// Writes all unique modifiers found in current savegame to a txt file.
        /// </summary>
        public void DumpAllModifiers()
        {
            var fileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}AllModifiers_{DateTime.Now:yyMMdd_HHmmss}";
            using (var file = new StreamWriter(fileName + ".txt"))
            {
                foreach (var boost in DataTable.UserGenerationBoosts)
                {
                    file.WriteLine(boost);
                }
            }

            using (var zip = ZipFile.Open(fileName + ".zip", ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(fileName + ".txt", "AllModifiers.txt");
            }
#if !DEBUG
            File.Delete(fileName + ".txt");
#endif
        }

        /// <summary>
        /// Writes all unique permanent boosts found in current savegame to a txt file.
        /// </summary>
        public void DumpAllPermanentBoosts()
        {
            var fileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}AlllPermanentBoosts_{DateTime.Now:yyMMdd_HHmmss}";
            using (var file = new StreamWriter(fileName + ".txt"))
            {
                foreach (var boost in DataTable.UserStatsBoosts)
                {
                    file.WriteLine(boost);
                }
            }

            using (var zip = ZipFile.Open(fileName + ".zip", ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(fileName + ".txt", "AlllPermanentBoosts.txt");
            }
#if !DEBUG
            File.Delete(fileName + ".txt");
#endif
        }

        /// <summary>
        /// Writes all unique skills found in current savegame to a txt file.
        /// </summary>
        public void DumpAllSkills()
        {
            var fileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}AllSkills_{DateTime.Now:yyMMdd_HHmmss}";
            var skillList = new List<string>();
            using (var file = new StreamWriter(fileName + ".txt"))
            {
                foreach (var player in Players)
                {
                    foreach (var skill in player.Skills.Keys)
                    {
                        if (skillList.Contains(skill)) continue;
                        file.WriteLine(skill);
                        skillList.Add(skill);
                    }
                }
            }

            using (var zip = ZipFile.Open(fileName + ".zip", ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(fileName + ".txt", "AllSkills.txt");
            }
#if !DEBUG
            File.Delete(fileName + ".txt");
#endif
        }

        /// <summary>
        /// Writes all unique talents found in current savegame to a txt file.
        /// </summary>
        public void DumpAllTalents()
        {
            var fileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}AllTalents_{DateTime.Now:yyMMdd_HHmmss}";
            using (var file = new StreamWriter(fileName + ".txt"))
            {
                foreach (var player in Players)
                {
                    foreach (var talent in player.Talents)
                    {
                        file.WriteLine($"{player.Name}\t{talent}");
                    }
                }
            }

            using (var zip = ZipFile.Open(fileName + ".zip", ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(fileName + ".txt", "AllTalents.txt");
            }
#if !DEBUG
            File.Delete(fileName + ".txt");
#endif
        }

        /// <summary>
        /// Deserialise the json file to its original Savegame object
        /// </summary>
        /// <param name="jsonFile">json file path + name</param>
        /// <returns>the Savegame object used to create the supplied json file</returns>
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

    #endregion
}
