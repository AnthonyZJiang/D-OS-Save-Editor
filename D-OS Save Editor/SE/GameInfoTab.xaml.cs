

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
        private string[] gameSavetypeName = { "Manual", "QuickSave", "AutoSave", "Honour Save" };

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
            if (GameVersionListBox.Items.Count == 0)
                foreach (string i in Meta.GameVersion)
                {

                    GameVersionListBox.Items.Add(new ListBoxItem { Content = i });
                }
            if (ModsListBox.Items.Count == 0)
                foreach (string i in Meta.ModsName)
                {

                    ModsListBox.Items.Add(new ListBoxItem { Content = i });
                }
        }
    }


}
