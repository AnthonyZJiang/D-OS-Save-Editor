using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LSLib.LS;
using LSLib.LS.Enums;
using Microsoft.Win32;
using static System.IO.Path;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _defaultProfileDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{DirectorySeparatorChar}Larian Studios{DirectorySeparatorChar}Divinity Original Sin Enhanced Edition{DirectorySeparatorChar}PlayerProfiles";

        public MainWindow()
        {
            InitializeComponent();

            // set default savegame directory
            DirectoryTextBox.Text = GetMostRecentProfile();

            LoadSavegamesPath(DirectoryTextBox.Text);
        }

        #region private methods
        private string GetMostRecentProfile()
        {
            var dirs = Directory.GetDirectories(_defaultProfileDir);
            var idx = 0;
            var saveTime = DateTime.MinValue;
            for (var i = 0; i < dirs.Length; i++)
            {
                var saveDir = dirs[i] + DirectorySeparatorChar + "Savegames_patch";
                if (!Directory.Exists(saveDir)) continue;
                var saveDirs = Directory.GetDirectories(saveDir);
                foreach (var d in saveDirs)
                {
                    var lastWriteTime = Directory.GetLastWriteTime(d);
                    if (lastWriteTime <= saveTime) continue;
                    saveTime = lastWriteTime;
                    idx = i;
                }
            }

            return dirs[idx] + DirectorySeparatorChar + "Savegames_patch";
        }

        private void LoadSavegamesPath(string dir)
        {
            // clear ui
            GameEditionTextBlock.Text = "";
            SavegameListBox.Items.Clear();

            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Savegame directory invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // get game version
            var gameVer = GetGameVersion(dir);
            if (gameVer == null)
            {
                MessageBox.Show("Unidentified game version. Please check if you have entered a correct savegames path.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GameEditionTextBlock.Text = Regex.Replace(gameVer.ToString(), "([DOS2]|E{2})", " $1").Trim();
            GameEditionTextBlock.Tag = gameVer;

            var dirs = Directory.GetDirectories(dir);
            // find save time and save names
            var fnames = new List<string>();
            var dates = new List<DateTime>();
            foreach (var d in dirs)
            {
                // check if the folder contains a save by looking for the lsv file of the same fname
                var fname = d.Split(DirectorySeparatorChar).Last();
                if (!File.Exists(d + DirectorySeparatorChar + fname + ".lsv"))
                    continue;

                fnames.Add(fname);
                dates.Add(Directory.GetLastWriteTime(d));
            }

            // order by time
            var idx = Enumerable.Range(0, dates.Count).ToArray();
            Array.Sort<int>(idx, (a, b) => dates[b].CompareTo(dates[a]));

            // add to list box
            for (int i = 0; i < dates.Count; i++)
            {
                SavegameListBox.Items.Add(new TextBlock()
                {
                    Text = String.Join(" ", fnames[idx[i]].Split('_')),
                    Uid = fnames[idx[i]],
                    Tag = dates[idx[i]].ToString("dd/MM/yyyy|HH:mm:ss")
                });
            }
        }

        private static Game? GetGameVersion(string path)
        {
            if (path.Contains("Divinity Original Sin Enhanced Edition")) return Game.DivinityOriginalSinEE;
            if (path.Contains("Divinity Original Sin 2")) return Game.DivinityOriginalSin2;
            if (path.Contains("Divinity Original Sin")) return Game.DivinityOriginalSin;

            return null;
        }

        private bool UnpackSave(Savegame savegame)
        {
            try
            {
                Cursor = Cursors.Wait;
                savegame.UnpackSavegame();
                savegame.ParseLsx();
            }
            catch (NotAPackageException)
            {
                MessageBox.Show($"The specified package ({savegame.SavegameFullFile}) is not a savegame file.",
                    "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Internal error!\n\n{ex}", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                Cursor = Cursors.Arrow;
                LoadButton.IsEnabled = true;
            }
            return true;
        }

        private string CalculateChecksumFromPng(string imagefile)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(imagefile))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        #endregion private methods

        #region ui events
        private void DirectoryTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            LoadSavegamesPath(DirectoryTextBox.Text);

        }

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var initialDir = Directory.Exists(DirectoryTextBox.Text)
                ? DirectoryTextBox.Text
                : $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{DirectorySeparatorChar}Larian Studios";
            if (!Directory.Exists(initialDir))
            {
                initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = initialDir,
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DirectoryTextBox.Text = dialog.FileName;
            }
        }

        private void SavegameListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadButton.IsEnabled = true;
            SavegameTimeTextBox.Text = ((TextBlock)e.AddedItems[e.AddedItems.Count - 1]).Tag.ToString().Split('|')[1];
            SavegameDateTextBox.Text = ((TextBlock)e.AddedItems[e.AddedItems.Count - 1]).Tag.ToString().Split('|')[0];
            var saveGameName = ((TextBlock) e.AddedItems[e.AddedItems.Count-1]).Uid;
            var imageDir = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar + saveGameName + ".png";
            if (File.Exists(imageDir))
            {
                SavegameImage.Source = new BitmapImage(new Uri(imageDir));
            }
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoadButton.IsEnabled = false;

            if (SavegameListBox.SelectedItem == null) return;

            var unpackDir = GetTempPath() + "DOSSE" + DirectorySeparatorChar + "unpackaged";
            var saveGameName = ((TextBlock)SavegameListBox.SelectedItem).Uid;
            var savegame = new Savegame(
                DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar + saveGameName + ".lsv", 
                unpackDir,
                (Game)GameEditionTextBlock.Tag);

            // unpack
            if (!UnpackSave(savegame)) return;

            var se = new SaveEditor(savegame);
            se.ShowDialog();
        }

        private void BackupButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SavegameListBox.SelectedItem == null) return;

            var saveGameName = ((TextBlock)SavegameListBox.SelectedItem).Uid;
            var savegamefile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                               saveGameName + ".lsv";
            var backupfile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                             saveGameName + ".bak";
            var imagefile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                            saveGameName + ".png";
            var checksumfile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                               saveGameName + ".cks";

            File.Copy(savegamefile, backupfile, true);

            // generate checksum 
            if (!File.Exists(imagefile))
            {
                MessageBox.Show("Savegame has been backed up! However, no image file was found.", "Successful");
                return;
            }
            using (var sw = new StreamWriter(checksumfile))
            {
                sw.WriteLine(CalculateChecksumFromPng(imagefile));
            }

            MessageBox.Show("Backup successful!", "Successful");
        }
        #endregion ui events


        private void RestoreButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SavegameListBox.SelectedItem == null) return;

            var saveGameName = ((TextBlock)SavegameListBox.SelectedItem).Uid;
            var savegamefile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                               saveGameName + ".lsv";
            var backupfile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                             saveGameName + ".bak";
            var imagefile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                            saveGameName + ".png";
            var checksumfile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                               saveGameName + ".cks";

            if (!File.Exists(backupfile))
            {
                MessageBox.Show("No backup found.");
                return;
            }

            if (!File.Exists(checksumfile))
            {
                var dlgResult = MessageBox.Show(
                    "No checksum file was found. Do you still want to restore the savegame? The backup could be an old savegame.",
                    "No checksum file", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (dlgResult == MessageBoxResult.No) return;
            }
            else if (!File.Exists(imagefile))
            {
                var dlgResult = MessageBox.Show(
                    "No image file was found. Do you still want to restore the savegame? The backup could be an old savegame.",
                    "No image file", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (dlgResult == MessageBoxResult.No) return;
            }
            else
            {
                //load check sum
                string cks;
                using (var sr = new StreamReader(checksumfile))
                {
                    cks = sr.ReadToEnd().Trim();
                }

                if (cks != CalculateChecksumFromPng(imagefile))
                {
                    var dlgResult = MessageBox.Show(
                        "The backup failed checksum validation. Do you still want to restore the savegame? The backup could be an old savegame.",
                        "Checksum failed", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlgResult == MessageBoxResult.No) return;
                }
            }

            File.Copy(backupfile, savegamefile, true);
            MessageBox.Show("Restore successful!","Successful");
        }
    }
}
