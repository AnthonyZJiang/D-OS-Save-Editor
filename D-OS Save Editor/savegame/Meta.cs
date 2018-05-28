using System;
using System.Collections.Generic;
using System.Linq;

namespace D_OS_Save_Editor
{
    public class Meta
    {
        private string _year;
        private string _month;
        private string _day;
        private string _hours;
        private string _minutes;
        private string _seconds;
        private string _milliseconds;
        private string _level;
        private int _difficulty = -1;

        private readonly string[] _difficultyNames = {"Story", "Classic", "Tactician", "Honour"};
        private readonly string[] _savegameTypeName = { "Manual", "QuickSave", "AutoSave", "Honour Save" };


        //<node id="MetaData">

        //<attribute id = "Level" value="Cyseal" type="22" />
        /// <summary>
        /// Game map/level
        /// </summary>
        public string Level
        {
            get => _level;
            set => _level = value;
        }

        /// <summary>
        /// Difficulty Game
        /// </summary>
        public int Difficulty
        {
            get => _difficulty;
            set => _difficulty = value;
        }

        public string DifficultyString => Difficulty < 0 ? "" : _difficultyNames[Difficulty];

        /// <summary>
        /// Full game version
        /// </summary>
        public List<string> GameVersions { get; set; }

        public string LastGameVersion
        {
            get
            {
                if (GameVersions == null || GameVersions.Count == 0)
                    return "";
                return GameVersions[GameVersions.Count - 1]; 
            }
        }

        public bool IsOutdatedVersion => LastGameVersion != DataTable.SupportedGameVersion;

        public bool IsVersionWarning => !Properties.Settings.Default.AcceptMods.Split('|').Contains(LastGameVersion);

        /// <summary>
        /// List of Mod's Name load in game
        /// </summary>
        public List<string> ModNames { get; set; }

        public int ModCount => ModNames.Count - 1;

        public string ModCountDisplayText => ModCount >= 0 ? ModCount.ToString() : "" ;

        public string ModDisplayText => ModNames == null ? "" : string.Join(" | ", ModNames);

        public bool IsModed => ModCount > 0;

        public List<string> UnaccpectedMods
        {
            get
            {
                if (ModCount <= 0) return new List<string>();

                var acceptedmods = Properties.Settings.Default.AcceptMods.Split('|');
                return ModNames.Where(mod => !acceptedmods.Contains(mod)).ToList();
            }
        }

        public bool IsModWarning => UnaccpectedMods.Count > 0;

        public string Year
        {
            get => _year ?? "";
            set => _year = value;
        }

        public string Month
        {
            get => _month ?? "";
            set => _month = value;
        }

        public string Day
        {
            get => _day ?? "";
            set => _day = value;
        }

        public string Hours
        {
            get => _hours ?? "";
            set => _hours = value;
        }

        public string Minutes
        {
            get => _minutes ?? "";
            set => _minutes = value;
        }

        public string Seconds
        {
            get => _seconds ?? "";
            set => _seconds = value;
        }

        public string Milliseconds
        {
            get => _milliseconds ?? "";
            set => _milliseconds = value;
        }

        /// <summary>
        /// Time of savegame created
        /// </summary>
        public string SavegameTimeString
        {
            get
            {
                if (!int.TryParse(Seconds, out _))
                    return "";
                return Hours.PadLeft(2, '0') + ':' + Minutes.PadLeft(2, '0') + ':' + Seconds.PadLeft(2, '0');
            }
        }

        /// <summary>
        /// Date of savegame created
        /// </summary>
        public string SavegameDateString
        {
            get
            {
                if (!int.TryParse(Year, out var year))
                    return "";
                return Convert.ToString(1900 + year) + '/' + Month.PadLeft(2, '0') + '/' + Day.PadLeft(2, '0');
            }
        }

        /// <summary>
        /// Datetime of savegame created
        /// </summary>
        public string SavegameDatetimeString
        {
            get
            {
                if (!int.TryParse(Year, out var year))
                    return "";
                return Convert.ToString(1900 + year) + '/' + Month.PadLeft(2, '0') + '/' + Day.PadLeft(2, '0') + ' ' + Hours.PadLeft(2, '0') + ':' + Minutes.PadLeft(2, '0') + ':' + Seconds.PadLeft(2, '0');
            }
        }

        //<attribute id="SaveGameType" value="2" type="27" />
        /// <summary>
        /// The type of SaveGame
        /// </summary>
        public int SavegameType { get; set; }

        public string SaveGameTypeString => _savegameTypeName[SavegameType];

        public string Seed { get; set; }

        public Meta()
        {
            GameVersions = new List<string>();
            ModNames = new List<string>();
        }
    }
}
