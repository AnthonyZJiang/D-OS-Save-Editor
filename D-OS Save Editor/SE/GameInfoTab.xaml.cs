using System.Windows.Controls;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Logique d'interaction pour GameInfoTab.xaml
    /// </summary>
    public partial class GameInfoTab
    {
        private readonly string[] _difficultyName = { "Story", "Classic", "Tactician", "Honour" };
        private readonly string[] _gameSavetypeName = { "Manual", "QuickSave", "AutoSave", "Honour Save" };

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
            DifficultyTextBlock.Text = _difficultyName[Meta.Difficulty];
            LevelTextBlock.Text = Meta.Level;
            SaveTimeTextBlock.Text = Meta.SavegameTimeString;
            SeedTextBlock.Text = Meta.Seed;
            SaveGameTypeTextBlock.Text = _gameSavetypeName[Meta.SavegameType];
            if (GameVersionListBox.Items.Count == 0)
                foreach (var i in Meta.GameVersions)
                {

                    GameVersionListBox.Items.Add(new ListBoxItem { Content = i });
                }

            if (ModsListBox.Items.Count == 0)
                foreach (var i in Meta.ModNames)
                {

                    ModsListBox.Items.Add(new ListBoxItem { Content = i });
                }
        }
    }


}
