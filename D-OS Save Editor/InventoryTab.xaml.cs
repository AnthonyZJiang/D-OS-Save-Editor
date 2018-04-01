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

            #region enable disable textboxes
            // generation and stats related controls are enabled/disabled later on
            if (item.Vitality == "-1")
            {
                VitalityTextBox.IsEnabled = false;
                MaxVitalityPatchCheckTextBox.IsEnabled = false;
            }
            else
            {
                VitalityTextBox.IsEnabled = true;
                MaxVitalityPatchCheckTextBox.IsEnabled = true;
            }

            if (item.ItemSort == ItemSortType.Potion ||
                item.ItemSort == ItemSortType.Gold ||
                item.ItemSort == ItemSortType.Granade ||
                item.ItemSort == ItemSortType.Scroll ||
                item.ItemSort == ItemSortType.Food)
            {
                AmountTextBox.IsEnabled = true;
            }
            else
            {
                AmountTextBox.IsEnabled = false;
            }

            LockLevelTextBox.IsEnabled = item.ItemSort == ItemSortType.Furniture;
            #endregion

#if DEBUG
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

            // generation
            if (item.Generation != null)
            {
                foreach (var m in item.Generation.Boosts)
                {
                    BoostsListBox.Items.Add(m);
                }

                BoostsListBox.IsEnabled = true;
            }
            else
            {
                BoostsListBox.IsEnabled = false;

            }

            // stats
            if (item.Stats != null)
            {
                foreach (var m in item.Stats.PermanentBoost)
                {
                    PermBoostsListBox.Items.Add($"{m.Key} - {m.Value}");
                }
                DurabilityTextBox.IsEnabled = true;
                MaxDurabilityPatchCheckTextBox.IsEnabled = true;
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

            var item = Player.Items[ItemsListBox.SelectedIndex];
            item.Amount = AmountTextBox.Text;
            item.LockLevel = LockLevelTextBox.Text;
            item.Vitality = VitalityTextBox.Text;
            item.MaxVitalityPatchCheck = MaxVitalityPatchCheckTextBox.Text;
            if (item.Stats != null)
            {
                item.Stats.Durability = DurabilityTextBox.Text;
                item.MaxDurabilityPatchCheck = MaxDurabilityPatchCheckTextBox.Text;
                item.Stats.DurabilityCounter = DurabilityCounterTextBox.Text;
                item.Stats.RepairDurabilityPenalty = RepairDurabilityPenaltyTextBox.Text;
                item.Stats.Level = LevelTextBox.Text;
            }

            if (Player.ItemChanges.ContainsKey(item.Slot))
            {
                Player.ItemChanges[item.Slot] = new ItemChange(item, Player.ItemChanges[item.Slot].ChangeType, ItemsListBox.SelectedIndex);
            }
            else
            {
                Player.ItemChanges.Add(item.Slot, new ItemChange(item, ChangeType.Modify, ItemsListBox.SelectedIndex));
            }

            MessageBox.Show("Changes have been applied.");
        }
    }
}
