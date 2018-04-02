using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class InventoryTab : UserControl
    {
        private Player _player;
        private Brush DefaultTextBoxBorderBrush { get; }
        private Brush[] _itemRarityColor =
            {Brushes.Black, Brushes.ForestGreen, Brushes.DodgerBlue, Brushes.BlueViolet, Brushes.DeepPink, Brushes.Gold, Brushes.DimGray};

        public Player Player
        {
            get => _player;

            set
            {
                _player = value;
                UpdateForm();
            }
        }

        public InventoryTab()
        {
            InitializeComponent();
            DefaultTextBoxBorderBrush = AmountTextBox.BorderBrush;

            RarityComboBox.ItemsSource = Enum.GetValues(typeof(Item.ItemRarityType)).Cast<Item.ItemRarityType>();
        }

        public void UpdateForm()
        {
            ItemsListBox.Items.Clear();
            foreach (var i in Player.Items)
                ItemsListBox.Items.Add(new ListBoxItem
                {
                    Content = i.StatsName,
                    Tag = i.ItemSort,
                    Foreground = _itemRarityColor[(int)i.ItemRarity]
                });

            // check filter
            foreach (var i in ShowWrapPanel.Children)
            {
                if (i is CheckBox)
                    CheckboxEventSetter_OnClick(i, new RoutedEventArgs());
            }

            // clear all text boxes
            foreach(var i in ValueWrapPanel.Children)
            {
                if (i is TextBox t)
                    t.Text = "";
            }
        }

        private void TextBoxEventSetter_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox s)) return;

            var text = s.Text;
            var valid = int.TryParse(text, out int _);
            s.BorderBrush = !valid ? Brushes.Red : DefaultTextBoxBorderBrush;
        }

        private void TextBoxEventSetter_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(sender is TextBox s)) return;

            var text = s.Text.Insert(s.SelectionStart, e.Text);
            e.Handled = !int.TryParse(text, out int _);
        }

        private void ItemsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // clear list boxes
            BoostsListBox.Items.Clear();
            PermBoostsListBox.Items.Clear();

            var lb = sender as ListBox;
            if (lb.SelectedIndex < 0)
                return;
            var item = Player.Items[lb.SelectedIndex];


            var allowedChanges = item.GetAllowedChangeType();
            #region enable disable controls

            if (allowedChanges.Contains(nameof(item.Vitality)))
            {
                VitalityTextBox.IsEnabled = true;
                MaxVitalityPatchCheckTextBox.IsEnabled = true;
            }
            else
            {
                VitalityTextBox.IsEnabled = false;
                MaxVitalityPatchCheckTextBox.IsEnabled = false;
            }

            RarityComboBox.IsEnabled = allowedChanges.Contains(nameof(item.ItemRarity));
            AmountTextBox.IsEnabled = allowedChanges.Contains(nameof(item.Amount));
            LockLevelTextBox.IsEnabled = allowedChanges.Contains(nameof(item.LockLevel));
            BoostsListBox.IsEnabled = allowedChanges.Contains(nameof(item.Generation));

            if (allowedChanges.Contains(nameof(item.Stats)))
            {
                DurabilityTextBox.IsEnabled = true;
                MaxDurabilityPatchCheckTextBox.IsEnabled = false;
                DurabilityCounterTextBox.IsEnabled = true;
                RepairDurabilityPenaltyTextBox.IsEnabled = true;
                LevelTextBox.IsEnabled = true;
                PermBoostsListBox.IsEnabled = true;
            }
            else
            {
                DurabilityTextBox.IsEnabled = false;
                MaxDurabilityPatchCheckTextBox.IsEnabled = false;
                DurabilityCounterTextBox.IsEnabled = false;
                RepairDurabilityPenaltyTextBox.IsEnabled = false;
                LevelTextBox.IsEnabled = false;
                PermBoostsListBox.IsEnabled = false;
            }
            #endregion

#if DEBUG && LOG_ITEMXML
            Console.WriteLine(item.Xml);
#endif
            // textbox contents
            AmountTextBox.Text = item.Amount;
            LockLevelTextBox.Text = item.LockLevel;
            VitalityTextBox.Text = item.Vitality;
            MaxVitalityPatchCheckTextBox.Text = item.MaxVitalityPatchCheck;
            DurabilityTextBox.Text = item.Stats?.Durability;
            DurabilityCounterTextBox.Text = item.Stats?.DurabilityCounter;
            MaxDurabilityPatchCheckTextBox.Text = item.MaxDurabilityPatchCheck;
            RepairDurabilityPenaltyTextBox.Text = item.Stats?.RepairDurabilityPenalty;
            LevelTextBox.Text = item.Stats?.Level;

            // combobox
            RarityComboBox.SelectedIndex = (int) item.ItemRarity;

            // generation
            if (item.Generation != null)
            {
                foreach (var m in item.Generation.Boosts)
                {
                    BoostsListBox.Items.Add(m);
                }
            }

            // stats
            if (item.Stats != null)
            {
                foreach (var m in item.Stats.PermanentBoost)
                {
                    PermBoostsListBox.Items.Add($"{m.Key} - {m.Value}");
                }
            }
        }
        
        private void CheckboxEventSetter_OnClick(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (!(ckb?.Tag is ItemSortType))
                return;

            if ((ItemSortType) ckb.Tag == ItemSortType.Other)
            {
                foreach (ListBoxItem i in ItemsListBox.Items)
                {
                    if ((ItemSortType) i.Tag == ItemSortType.Item || (ItemSortType) i.Tag == ItemSortType.Unique ||
                        (ItemSortType) i.Tag == ItemSortType.Other)
                        i.Visibility = (bool)ckb.IsChecked ? Visibility.Visible:Visibility.Collapsed;
                }
            }
            else
            {
                foreach (ListBoxItem i in ItemsListBox.Items)
                {
                    if ((ItemSortType)i.Tag == (ItemSortType)ckb.Tag)
                        i.Visibility = (bool)ckb.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void CheckAllButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var i in ShowWrapPanel.Children)
            {
                if (!(i is CheckBox box)) continue;
                box.IsChecked = true;
                CheckboxEventSetter_OnClick(i, new RoutedEventArgs());
            }
        }

        private void UncheckAllButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var i in ShowWrapPanel.Children)
            {
                if (!(i is CheckBox box)) continue;
                box.IsChecked = false;
                CheckboxEventSetter_OnClick(i, new RoutedEventArgs());
            }
        }

        private void ApplyChangesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ItemsListBox.SelectedIndex < 0)
                return;

            try
            {
                // apply changes to a copy of the item
                var item = Player.Items[ItemsListBox.SelectedIndex].DeepClone();
                var allowedChanges = item.GetAllowedChangeType();
                if (allowedChanges.Contains(nameof(item.Amount)))
                    item.Amount = AmountTextBox.Text;

                if (allowedChanges.Contains(nameof(item.LockLevel)))
                    item.LockLevel = LockLevelTextBox.Text;

                if (allowedChanges.Contains(nameof(item.Vitality)))
                {
                    item.Vitality = VitalityTextBox.Text;
                    item.MaxVitalityPatchCheck = MaxVitalityPatchCheckTextBox.Text;
                }
                
                if (allowedChanges.Contains(nameof(item.ItemRarity)))
                    item.ItemRarity = (Item.ItemRarityType) RarityComboBox.SelectedIndex;

                if (allowedChanges.Contains(nameof(item.Stats)))
                {
                    item.Stats.Durability = DurabilityTextBox.Text;
                    item.Stats.DurabilityCounter = DurabilityCounterTextBox.Text;
                    item.Stats.RepairDurabilityPenalty = RepairDurabilityPenaltyTextBox.Text;
                    item.Stats.Level = LevelTextBox.Text;
                }

                if (allowedChanges.Contains(nameof(item.Generation)))
                {
                    if (item.Generation == null)
                    {
                        //TODO if Item.Stats is null, an error will occur later on when writing xml because Geneartion.Level is taken from Stats.Level.
                        //Since Item.Stats == null is not likely to be possible, let's handle it later on if we have an report of this case.
                        if (item.Stats == null)
                        {
                            var dlgResult = MessageBox.Show(
                                "Sorry, it is not possible to add modifier to this item yet. No changes have been applied. If you would like to help, please click \"Yes\".", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                            if (dlgResult == MessageBoxResult.No) return;

                            var fn = Savegame.DumpItem(item, "Item.Stats null");
                            dlgResult = MessageBox.Show(
                                $"Thank you for your help. Now please click \"OK\" to go to Github and file an issue. \n\nYou only need to go through the following steps:\n\n1. navigate to the application root folder;\n2. locate the {fn} file;\n3. drag {fn} to the issue reporting window;\n4. submit.",
                                "Thank you",MessageBoxButton.OKCancel,MessageBoxImage.Information);
                            if (dlgResult == MessageBoxResult.Cancel) return;

                            System.Diagnostics.Process.Start("https://github.com/tmxkn1/D-OS-Save-Editor/issues/new");
                            return;
                        }
                        item.Generation = new Item.GenerationNode(item.StatsName, "0");
                    }

                    item.Generation.Boosts = new List<string>();
                    foreach (string s in BoostsListBox.Items)
                    {
                        item.Generation.Boosts.Add(s);
                    }
                }

                // add changes
                if (Player.ItemChanges.ContainsKey(item.Slot))
                {
                    Player.ItemChanges[item.Slot] = new ItemChange(item, Player.ItemChanges[item.Slot].ChangeType,
                        ItemsListBox.SelectedIndex);
                }
                else
                {
                    Player.ItemChanges.Add(item.Slot,
                        new ItemChange(item, ChangeType.Modify, ItemsListBox.SelectedIndex));
                }

                // apply changes to the original item
                Player.Items[ItemsListBox.SelectedIndex] = item;

                // change colour
                ((ListBoxItem) ItemsListBox.Items[ItemsListBox.SelectedIndex]).Foreground =
                    _itemRarityColor[(int) item.ItemRarity];

                MessageBox.Show("Changes have been applied.");
            }
            catch (XmlValidationException ex)
            {
                MessageBox.Show($"Invalid value entered: {ex.Name}: {ex.Value}. No change has been applied.\n\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Internal error. No change has been applied.\n\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BoostsContextMenu_Click(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Header)
            {
                case "Add":
                    var dlg = new AddBoostDialog();
                    dlg.ShowDialog();
                    if (dlg.DialogResult == true)
                        BoostsListBox.Items.Add(dlg.BoostText);
                    break;
                case "Copy text":
                    Clipboard.SetText((string)BoostsListBox.SelectedValue);
                    break;
                case "Delete":
                    BoostsListBox.Items.RemoveAt(BoostsListBox.SelectedIndex);
                    break;
            }
        }
    }
}
