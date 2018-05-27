using System;
using System.Collections.Generic;

namespace D_OS_Save_Editor
{
    public class Meta
    {
        public enum DifficultyEnum
        {
            Story = 0,
            Classic,
            Tactician,
            Honour
        }
        public enum SaveGameTypeEnum
        {
            Manual = 0,
            QuickSave,
            AutoSave,
            HonourSave
        }
        private string _level;
        private int _difficulty;
        private List<string> _gameVersion;
        private int _saveGameType;
        private string _seed;
        private string _saveTime;
        private List<string> _modsName;

        //<node id="MetaData">

        //<attribute id = "Level" value="Cyseal" type="22" />
        /// <summary>
        /// Game map/level
        /// </summary>
        public string Level { get => _level; set => _level = value; }
        /// <summary>
        /// Difficulty Game
        /// </summary>
        public int Difficulty { get => _difficulty; set => _difficulty = value; }
        /// <summary>
        /// Full game version
        /// </summary>
        public List<string> GameVersion { get => _gameVersion; set => _gameVersion = value; }
        /// <summary>
        /// Date of savegame created
        /// </summary>
        public string SaveTime { get => _saveTime; set => _saveTime = value; }
        /// <summary>
        /// List of Mod's Name load in game
        /// </summary>
        public List<string> ModsName { get => _modsName; set => _modsName = value; }
        //<attribute id="SaveGameType" value="2" type="27" />
        /// <summary>
        /// The type of SaveGame
        /// </summary>
        public int SaveGameType { get => _saveGameType; set => _saveGameType = value; }
        public string Seed { get => _seed; set => _seed = value; }

        public Meta()
        {
            GameVersion = new List<string>();
            ModsName = new List<string>();
        }

        public void SaveTimeRead(int year, string month, string day, string hours, string minutes, string secondes, int milliseconds)
        {
            SaveTime = Convert.ToString(1900 + year) + '/' + month.PadLeft(2, '0') + '/' + day.PadLeft(2, '0') + ' ' + hours.PadLeft(2, '0') + ':' + minutes.PadLeft(2, '0') + ':' + secondes.PadLeft(2, '0');
        }
    }
}
