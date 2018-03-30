using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for AbilitiesTab.xaml
    /// </summary>
    public partial class AbilitiesTab : UserControl
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
            ManAtArmsTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.ManAtArms].ToString();
            ExpertMarksmanTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.ExpertMarksman].ToString();
            ScoundrelTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Scoundrel].ToString();
            SingleHandedTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.SingleHanded].ToString();
            TwoHandedTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.TwoHanded].ToString();
            BowTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Bow].ToString();
            CrossbowTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Crossbow].ToString();
            ShieldSpecialistTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.ShieldSpecialist].ToString();
            ArmorSpecialistTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.ArmorSpecialist].ToString();
            WitchcraftTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Witchcraft].ToString();
            TelekinesisTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Telekinesis].ToString();
            WillpowerTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Willpower].ToString();
            PyrokineticTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Pyrokinetic].ToString();
            HydrosophistTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Hydrosophist].ToString();
            AerotheurgeTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Aerotheurge].ToString();
            GeomancerTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Geomancer].ToString();
            BlacksmithingTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Blacksmithing].ToString();
            SneakingTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Sneaking].ToString();
            PickpocketingTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Pickpocketing].ToString();
            LockpickingTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Lockpicking].ToString();
            LoremasterTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Loremaster].ToString();
            CraftingTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Crafting].ToString();
            BarteringTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Bartering].ToString();
            CharismaTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Charisma].ToString();
            LeadershipTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Leadership].ToString();
            LuckyCharmTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.LuckyCharm].ToString();
            BodybuildingTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Bodybuilding].ToString();
            DuelWeldingTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.DuelWelding].ToString();
            WandTextBox.Text = Player.Abilities[(int)ConversionTable.Abilities.Wand].ToString();
        }

        public void SaveEdits()
        {
            Player.Abilities[(int)ConversionTable.Abilities.ManAtArms] = int.Parse(ManAtArmsTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.ExpertMarksman] = int.Parse(ExpertMarksmanTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Scoundrel] = int.Parse(ScoundrelTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.SingleHanded] = int.Parse(SingleHandedTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.TwoHanded] = int.Parse(TwoHandedTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Bow] = int.Parse(BowTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Crossbow] = int.Parse(CrossbowTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.ShieldSpecialist] = int.Parse(ShieldSpecialistTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.ArmorSpecialist] = int.Parse(ArmorSpecialistTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Witchcraft] = int.Parse(WitchcraftTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Telekinesis] = int.Parse(TelekinesisTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Willpower] = int.Parse(WillpowerTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Pyrokinetic] = int.Parse(PyrokineticTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Hydrosophist] = int.Parse(HydrosophistTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Aerotheurge] = int.Parse(AerotheurgeTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Geomancer] = int.Parse(GeomancerTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Blacksmithing] = int.Parse(BlacksmithingTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Sneaking] = int.Parse(SneakingTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Pickpocketing] = int.Parse(PickpocketingTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Lockpicking] = int.Parse(LockpickingTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Loremaster] = int.Parse(LoremasterTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Crafting] = int.Parse(CraftingTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Bartering] = int.Parse(BarteringTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Charisma] = int.Parse(CharismaTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Leadership] = int.Parse(LeadershipTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.LuckyCharm] = int.Parse(LuckyCharmTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Bodybuilding] = int.Parse(BodybuildingTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.DuelWelding] = int.Parse(DuelWeldingTextBox.Text);
            Player.Abilities[(int)ConversionTable.Abilities.Wand] = int.Parse(WandTextBox.Text);
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
