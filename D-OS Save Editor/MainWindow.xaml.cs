//#define AVOIDUNPACK

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
        private enum BackupStatus { None, Current, Old, NoChecksum, NoImage }

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
                MessageBox.Show(this, "Savegame directory invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // get game version
            var gameVer = GetGameVersion(dir);
            if (gameVer == null)
            {
                MessageBox.Show(this, "Unidentified game version. Please check if you have entered a correct savegames path.",
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
#if (!DEBUG || !AVOIDUNPACK)
                savegame.UnpackSavegame();
#endif
                savegame.ParseLsx();
            }
            catch (NotAPackageException)
            {
                MessageBox.Show(this, $"The specified package ({savegame.SavegameFullFile}) is not a savegame file.",
                    "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Internal error!\n\n{ex}", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private BackupStatus IsBackedUp(string saveGameName)
        {
            var backupfile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                             saveGameName + ".bak";
            var imagefile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                            saveGameName + ".png";
            var checksumfile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                               saveGameName + ".cks";

            // backup file not exist
            if (!File.Exists(backupfile)) return BackupStatus.None;

            // checksum not exist
            if (!File.Exists(checksumfile))
            {
                return BackupStatus.NoChecksum;
            }

            // image not exist
            if (!File.Exists(imagefile))
            {
                return BackupStatus.NoImage;
            }

            // compare checksum
            string cks;
            using (var sr = new StreamReader(checksumfile))
            {
                cks = sr.ReadToEnd().Trim();
            }

            return cks != CalculateChecksumFromPng(imagefile) ? BackupStatus.Old : BackupStatus.Current;
        }

        private void BackupSavegame(string saveGameName)
        {
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
            var isNoImage = false;
            if (!File.Exists(imagefile))
            {
                isNoImage = true;
                Properties.Resources.NoImage.Save(imagefile);
            }
            using (var sw = new StreamWriter(checksumfile))
            {
                sw.WriteLine(CalculateChecksumFromPng(imagefile));
            }

            MessageBox.Show(this, isNoImage? "Backup successful!\n\n However, no savegame snapshot is found, a blank image is used for computing checksum." : "Backup successful!", "Successful");
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
            BackupButton.IsEnabled = true;
            RestoreButton.IsEnabled = true;
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

            // check backup
            switch (IsBackedUp(saveGameName))
            {
                case BackupStatus.None:
                    var dlgResult = MessageBox.Show(this, "The savegame is not backed up. Do you want to make a backup first?",
                        "No backup found.", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlgResult == MessageBoxResult.Yes)
                        BackupSavegame(saveGameName);
                    break;
                case BackupStatus.Current:
                    break;
                case BackupStatus.Old:
                    dlgResult = MessageBox.Show(this,
                        "The backup seems to be old because it failed checksum validation. Do you want to make a new backup?",
                        "Old backup found", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlgResult == MessageBoxResult.Yes)
                        BackupSavegame(saveGameName);
                    break;
                case BackupStatus.NoChecksum:
                    dlgResult = MessageBox.Show(this,
                        "The backup may be old because it does not have a checksum file. Do you want to make a new backup?",
                        "No checksum file", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlgResult == MessageBoxResult.No) return;
                    else break;
                case BackupStatus.NoImage:
                    dlgResult = MessageBox.Show(this,
                        "The backup may be old because it does not have a checksum file. Do you want to make a new backup?",
                        "No image file", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlgResult == MessageBoxResult.No) return;
                    else break;
            }

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
            BackupSavegame(saveGameName);
        }

        private void RestoreButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SavegameListBox.SelectedItem == null) return;

            var saveGameName = ((TextBlock)SavegameListBox.SelectedItem).Uid;

            var savegamefile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                               saveGameName + ".lsv";
            var backupfile = DirectoryTextBox.Text + DirectorySeparatorChar + saveGameName + DirectorySeparatorChar +
                             saveGameName + ".bak";

            switch (IsBackedUp(saveGameName))
            {
                case BackupStatus.None:
                    MessageBox.Show("No backup found.");
                    return;
                case BackupStatus.Current:
                    break;
                case BackupStatus.Old:
                    var dlgResult = MessageBox.Show(this,
                        "The backup failed checksum validation. Do you still want to restore the savegame? The backup could be an old savegame.",
                        "Checksum failed", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlgResult == MessageBoxResult.No) return;
                    else break;
                case BackupStatus.NoChecksum:
                    dlgResult = MessageBox.Show(this,
                        "No checksum file was found. Do you still want to restore the savegame? The backup could be an old savegame.",
                        "No checksum file", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlgResult == MessageBoxResult.No) return;
                    else break;
                case BackupStatus.NoImage:
                    dlgResult = MessageBox.Show(this,
                        "No image file was found. Do you still want to restore the savegame? The backup could be an old savegame.",
                        "No image file", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlgResult == MessageBoxResult.No) return;
                    else break;
            }

            File.Copy(backupfile, savegamefile, true);
            MessageBox.Show(this, "Restore successful!", "Successful");
        }
        #endregion ui events
    }
}
