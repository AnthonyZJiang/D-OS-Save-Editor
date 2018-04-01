using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
    }
}
