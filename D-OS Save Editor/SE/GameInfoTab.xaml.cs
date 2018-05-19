

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Logique d'interaction pour GameInfoTab.xaml
    /// </summary>
    public partial class GameInfoTab
    {
        private string[] difficultyName = { "Story", "Classic", "Tactician", "Honour" };
        private string[] gameSavetypeName = { "Manual","QuickSave","AutoSave","Honour Save" };

        private Meta _meta;

        public Meta Meta
        {
            get => _meta; set
            {
                _meta = value;
                UpdateForm();
            }
        }

        public GameInfoTab()
        {
            InitializeComponent();
        }

        public void UpdateForm()
        {
            DifficultyTextBlock.Text = difficultyName[Meta.Difficulty];
            LevelTextBlock.Text = Meta.Level;
            SaveTimeTextBlock.Text = Meta.SaveTime;
            SeedTextBlock.Text = Meta.Seed;
            SaveGameTypeTextBlock.Text = gameSavetypeName[Meta.SaveGameType];
            foreach (string i in Meta.GameVersion)
            {
                if (GameVersionListBox.Items.Count == 0)
                    GameVersionListBox.Items.Add(new ListBoxItem { Content = i });
            }
            foreach (string i in Meta.ModsName)
            {
                if (ModsListBox.Items.Count == 0)
                    ModsListBox.Items.Add(new ListBoxItem { Content = i });
            }
        }
    }


}
