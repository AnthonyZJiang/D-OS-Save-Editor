using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for AbilitiesTab.xaml
    /// </summary>
    public partial class AbilitiesTab
    {
        private Player _player;
        private Brush DefaultTextBoxBorderBrush { get; }

        public Player Player
        {
            get => _player;

            set
            {
                _player = value;
                UpdateForm();
            }
        }

        public AbilitiesTab()
        {
            InitializeComponent();
            DefaultTextBoxBorderBrush = ManAtArmsTextBox.BorderBrush;
        }

        public void UpdateForm()
        {

            var Cursor = 0;
            for (var i = 0; i < DataTable.AbilitiesNamesEffects.Length / 3; i++)
            {
                TextBlock txtBlock = FindName(DataTable.AbilitiesNamesEffects[Cursor] + "TextBlock") as TextBlock;
                TextBox txtBox = FindName(DataTable.AbilitiesNamesEffects[Cursor] + "TextBox") as TextBox;
                Cursor++;
                if (!DataTable.AbilitiesNamesEffects[Cursor].Contains("PlaceHolder"))
                {
                    txtBlock.Text = DataTable.AbilitiesNamesEffects[Cursor];
                    txtBlock.ToolTip = DataTable.AbilitiesNamesEffects[Cursor + 1];
                    txtBox.Text = Player.Abilities[Convert.ToInt32(Enum.Parse(typeof(DataTable.Abilities), DataTable.AbilitiesNamesEffects[Cursor - 1]))].ToString();
                }
                Cursor++;
                Cursor++;
            }
        }

        public void SaveEdits()
        {
            var Cursor = 0;
            for (var i = 0; i < DataTable.AbilitiesNamesEffects.Length / 3; i++)
            {
                TextBox txtBox = FindName(DataTable.AbilitiesNamesEffects[Cursor] + "TextBox") as TextBox;
                if (!DataTable.AbilitiesNamesEffects[Cursor].Contains("PlaceHolder"))
                {
                    Player.Abilities[Convert.ToInt32(Enum.Parse(typeof(DataTable.Abilities), DataTable.AbilitiesNamesEffects[Cursor]))] = int.Parse(txtBox.Text);
                }
                Cursor++;
                Cursor++;
                Cursor++;
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
    }
}
