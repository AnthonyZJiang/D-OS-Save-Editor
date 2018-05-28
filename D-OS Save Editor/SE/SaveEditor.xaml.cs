using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for SaveEditor.xaml
    /// </summary>
    public partial class SaveEditor
    {
        private Savegame Savegame { get; set; }
        private Player[] EditingPlayers { get; set; }


        public SaveEditor(string jsonFile)
        {
            InitializeComponent();

            Savegame = Savegame.GetSavegameFromJson(jsonFile);
            // make a copy of players
            try
            {
                EditingPlayers = Savegame.Players.Select(a => a?.DeepClone()).ToArray();
            }
            catch (Exception ex)
            {
                var er = new ErrorReporting($"Fail to clone players.\n\n{ex}", null);
                er.ShowDialog();
                throw;
            }

            foreach (var p in Savegame.Players)
            {
                PlayerSelectionComboBox.Items.Add(p.Name);
            }

            PlayerSelectionComboBox.SelectedIndex = 0;
        }

        public SaveEditor(Savegame savegame)
        {
            InitializeComponent();
            Savegame = savegame;

            // make a copy of players
            try
            {
                EditingPlayers = Savegame.Players.Select(a => a?.DeepClone()).ToArray();
            }
            catch (Exception ex)
            {
                var er = new ErrorReporting($"Fail to clone players.\n\n{ex}", null);
                er.ShowDialog();
                throw;
            }

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
            TraitsTab.Player = EditingPlayers[id];
            TalentTab.Player = EditingPlayers[id];

            if (EditingPlayers[id].Name == "Henchman")
            {
                //TraitsTab.IsEnabled = false;
            }
        }

        private void PlayerSelectionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowContent(PlayerSelectionComboBox.SelectedIndex);
        }

        private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;
            try
            {
                Cursor = Cursors.Wait;
                StatsTab.SaveEdits();
                AbilitiesTab.SaveEdits();
                TraitsTab.SaveEdits();
                TalentTab.SaveEdits();

                // progress indicator
                var progressIndicator = new ProgressIndicator("Saving", false) { Owner = Application.Current.MainWindow };
                var progress = new Progress<string>();
                progress.ProgressChanged += (o, s) =>
                {
                    progressIndicator.ProgressText = s;
                };
                progressIndicator.Show();

                // apply changes
                Savegame.Players = EditingPlayers;
                await Savegame.WriteEditsToLsxAsync(progress);
                // pack up files
                await Savegame.PackSavegameAsync(progress);
                
                progressIndicator.ProgressText = "Successful.";
                progressIndicator.CanCancel = true;
                progressIndicator.CancelButtonText = "Close";

                DialogResult = true;
            }
            catch (Exception ex)
            {
                SaveButton.IsEnabled = true;
                var er = new ErrorReporting($"Failed to save changes.\n\n{ex}", null);
                er.ShowDialog();
            }
            finally
            {
                Cursor = Cursors.Arrow;
                SaveButton.IsEnabled = true;
            }
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditingPlayers = Savegame.Players.Select(a => a.DeepClone()).ToArray();
            StatsTab.UpdateForm();
            AbilitiesTab.UpdateForm();
            InventoryTab.UpdateForm();
        }

        private void SaveEditor_OnClosed(object sender, EventArgs e)
        {
            Savegame = null;
            EditingPlayers = null;
        }

        private void DebugButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Tag)
            {
                case "AllPlayer":
                    Savegame.DumpSavegame();
                    break;
                case "AllInv":
                    Savegame.DumpAllInventory();
                    break;
                case "AllMod":
                    Savegame.DumpAllModifiers();
                    break;
                case "AllPerBoost":
                    Savegame.DumpAllPermanentBoosts();
                    break;
                case "AllSkills":
                    Savegame.DumpAllSkills();
                    break;
                case "AllTalents":
                    Savegame.DumpAllTalents();
                    break;
            }

            MessageBox.Show("A dump file has been created. Thank you!");
        }

        
        private void SavePlayer_OnClick(object sender, RoutedEventArgs e)
        {
            SavePlayer.IsEnabled = false;
            try
            {
                StatsTab.SaveEdits();
                AbilitiesTab.SaveEdits();
                TraitsTab.SaveEdits();
                TalentTab.SaveEdits();

                MessageBox.Show(this, "Changes have been applied to the selected character.", "Successful");
            }
            catch (Exception ex)
            {
                SavePlayer.IsEnabled = true;
                var er = new ErrorReporting($"Failed to save changes.\n\n{ex}", null);
                er.ShowDialog();
            }
            finally
            {
                SavePlayer.IsEnabled = true;
            }
        }

        private void DismissButton_OnClick(object sender, RoutedEventArgs e)
        {
            SubmitPanel.Visibility = Visibility.Collapsed;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RoutedEventArgs e)
        {
        }

        private void SaveEditor_OnClosing(object sender, CancelEventArgs e)
        {
            //if ()
            //{
            //    var result = MessageBox.Show(this, "You have unsaved changes. Do you want to close the window now?",
            //        "Unsaved changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            //}
        }

        private void BugReportButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(
                "https://docs.google.com/forms/d/e/1FAIpQLSeUeKYdV8InQslbvCvA1rmffJ5t1ieond4W6hpUHkHTH7I7dg/viewform");
        }
    }
}
