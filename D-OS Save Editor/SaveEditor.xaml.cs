using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for SaveEditor.xaml
    /// </summary>
    public partial class SaveEditor : Window
    {
        private Savegame Savegame { get; }
        private Player[] EditingPlayers { get; set; }

        public SaveEditor(Savegame savegame)
        {
            InitializeComponent();

            Savegame = savegame;
            // make a copy of players
            EditingPlayers = Savegame.Players.Select(a => a?.DeepClone()).ToArray();

            foreach (var p in Savegame.Players)
            {
                PlayerSelectionComboBox.Items.Add(p.Name);
            }

            PlayerSelectionComboBox.SelectedIndex = 0;
        }

        private void ShowContent(int id)
        {
            StatsTab.Player = EditingPlayers[id];
            AbilitiesTab.Player = EditingPlayers[id];
            InventoryTab.Player = EditingPlayers[id];
        }

        private void PlayerSelectionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowContent(PlayerSelectionComboBox.SelectedIndex);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;
            try
            {
                Cursor = Cursors.Wait;
                StatsTab.SaveEdits();
                AbilitiesTab.SaveEdits();

                // apply changes
                Savegame.Players = EditingPlayers;
                Savegame.WriteEditsToLsx();
                // pack up files
                Savegame.PackSavegame();

                MessageBox.Show(this, "Successfuly saved Savegame file.");
                this.Close();
            }
            catch (Exception ex)
            {
                SaveButton.IsEnabled = true;
                MessageBox.Show(this, $"Failed to save changes.\n\n{ex}", "Failed", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                Cursor = Cursors.Arrow;
                SaveButton.IsEnabled = true;
            }
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditingPlayers = Savegame.Players.Select(a => (Player)a.DeepClone()).ToArray();
            StatsTab.UpdateForm();
            AbilitiesTab.UpdateForm();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
