using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LSLib.LS;
using LSLib.LS.Enums;

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

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void ParseLsx()
        {
            // load xml
            var doc = new XmlDocument();
            doc.Load(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");

            // find player data
            var playerData = doc.DocumentElement.SelectNodes("//*[@id='PlayerData']");

            if (playerData == null)
                throw new XmlException($"Unable to find any player data in the savegame {SavegameName}.");

            Players = new Player[playerData.Count];
            for (var i = 0; i < playerData.Count; i++)
            {
                Players[i] = new Player
                {
                    MaxVitalityPatchCheck = int.Parse(playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='MaxVitalityPatchCheck']")?.Attributes[1].Value),
                    Vitality = int.Parse(playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='Vitality']")?.Attributes[1].Value),
                    InventoryId = playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='Inventory']")?.Attributes[1].Value,

                    Experience = int.Parse(playerData[i].ParentNode.SelectSingleNode("node//attribute [@id='Experience']")?.Attributes[1].Value),
                    Reputation = int.Parse(playerData[i].ParentNode.SelectSingleNode("node//attribute [@id='Reputation']")?.Attributes[1].Value),
                    
                    AttributePoints = int.Parse(playerData[i].SelectSingleNode("children//attribute [@id='AttributePoints']")?.Attributes[1].Value),
                    AbilityPoints = int.Parse(playerData[i].SelectSingleNode("children//attribute [@id='AbilityPoints']")?.Attributes[1].Value),                 
                    TalentPoints = int.Parse(playerData[i].SelectSingleNode("children//attribute [@id='TalentPoints']")?.Attributes[1].Value),

                    Name = playerData[i].SelectSingleNode("children//attribute [@id='Name']")?.Attributes[1].Value,                    
                    Icon = playerData[i].SelectSingleNode("children//attribute [@id='Icon']")?.Attributes[1].Value,                    
                    ClassType = playerData[i].SelectSingleNode("children//attribute [@id='ClassType']")?.Attributes[1].Value
                };


                var isLeaderNPC = playerData[i].ParentNode.SelectSingleNode("node//attribute [@id='IsLeaderNPC']")
                    ?.Attributes[1].Value;
                if (isLeaderNPC == null)
                    Players[i].IsLeaderNPC = true;
                else
                {
                    Players[i].IsLeaderNPC = isLeaderNPC == "True";
                }

                var nodes = playerData[i].ParentNode.SelectNodes("node//node [@id='Skills']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    Players[i].Skills.Add(nodes[j].FirstChild.Attributes[1].Value, nodes[j].ChildNodes[1].Attributes[1].Value == "True");
                }
                
                nodes = playerData[i].SelectNodes("children//node [@id='Attributes']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    Players[i].Attributes.Add(j, int.Parse(nodes[j].FirstChild.Attributes[1].Value));
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Abilities']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    Players[i].Abilities.Add(j, int.Parse(nodes[j].FirstChild.Attributes[1].Value));
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Talents']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    Players[i].Talents.Add(j, int.Parse(nodes[j].FirstChild.Attributes[1].Value));
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Traits']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    Players[i].Traits.Add(j, int.Parse(nodes[j].FirstChild.Attributes[1].Value));
                }

                if (Players[i].Name == "")
                    Players[i].Name = "Henchman";

            }
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void WriteEditsToLsx()
        {
            // load xml
            var doc = new XmlDocument();
            doc.Load(UnpackDirectory + Path.DirectorySeparatorChar + "globals.lsx");

            // find player data
            var playerData = doc.DocumentElement.SelectNodes("//*[@id='PlayerData']");

            if (playerData == null)
                throw new XmlException($"Unable to find any player data in the savegame {SavegameName}.");

            for (var i = 0; i < playerData.Count; i++)
            {
                playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='MaxVitalityPatchCheck']").Attributes[1].Value = Players[i].MaxVitalityPatchCheck.ToString();
                playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='Vitality']").Attributes[1].Value = Players[i].Vitality.ToString();
                playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='Inventory']").Attributes[1].Value = Players[i].InventoryId;
                playerData[i].ParentNode.SelectSingleNode("node//attribute [@id='Experience']").Attributes[1].Value = Players[i].Experience.ToString();
                playerData[i].ParentNode.SelectSingleNode("node//attribute [@id='Reputation']").Attributes[1].Value = Players[i].Reputation.ToString();

                playerData[i].SelectSingleNode("children//attribute [@id='AttributePoints']").Attributes[1].Value= Players[i].AttributePoints.ToString();
                playerData[i].SelectSingleNode("children//attribute [@id='AbilityPoints']").Attributes[1].Value= Players[i].AbilityPoints.ToString();
                playerData[i].SelectSingleNode("children//attribute [@id='TalentPoints']").Attributes[1].Value= Players[i].TalentPoints.ToString();


                //var nodes = playerData[i].ParentNode.SelectNodes("node//node [@id='Skills']");
                //for (var j = 0; j < nodes?.Count; j++)
                //{
                //    Players[i].Skills.Add(nodes[j].FirstChild.Attributes[1].Value, skills[j].ChildNodes[1].Attributes[1].Value == "True");
                //}

                var nodes = playerData[i].SelectNodes("children//node [@id='Attributes']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    nodes[j].FirstChild.Attributes[1].Value = Players[i].Attributes[j].ToString();
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Abilities']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    nodes[j].FirstChild.Attributes[1].Value = Players[i].Abilities[j].ToString();
                }

                //nodes = playerData[i].SelectNodes("children//node [@id='Talents']");
                //for (var j = 0; j < nodes?.Count; j++)
                //{
                //    nodes[j].FirstChild.Attributes[1].Value = Players[i].Talents[j].ToString();
                //}

                nodes = playerData[i].SelectNodes("children//node [@id='Traits']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    nodes[j].FirstChild.Attributes[1].Value = Players[i].Traits[j].ToString();
                }
            }

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
    }
}
