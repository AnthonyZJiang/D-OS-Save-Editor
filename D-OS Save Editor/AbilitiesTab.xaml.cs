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
            ManAtArmsTextBox.Text = Player.Abilities[(int)DataTable.Abilities.ManAtArms].ToString();
            ExpertMarksmanTextBox.Text = Player.Abilities[(int)DataTable.Abilities.ExpertMarksman].ToString();
            ScoundrelTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Scoundrel].ToString();
            SingleHandedTextBox.Text = Player.Abilities[(int)DataTable.Abilities.SingleHanded].ToString();
            TwoHandedTextBox.Text = Player.Abilities[(int)DataTable.Abilities.TwoHanded].ToString();
            BowTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Bow].ToString();
            CrossbowTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Crossbow].ToString();
            ShieldSpecialistTextBox.Text = Player.Abilities[(int)DataTable.Abilities.ShieldSpecialist].ToString();
            ArmorSpecialistTextBox.Text = Player.Abilities[(int)DataTable.Abilities.ArmorSpecialist].ToString();
            WitchcraftTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Witchcraft].ToString();
            TelekinesisTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Telekinesis].ToString();
            WillpowerTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Willpower].ToString();
            PyrokineticTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Pyrokinetic].ToString();
            HydrosophistTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Hydrosophist].ToString();
            AerotheurgeTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Aerotheurge].ToString();
            GeomancerTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Geomancer].ToString();
            BlacksmithingTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Blacksmithing].ToString();
            SneakingTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Sneaking].ToString();
            PickpocketingTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Pickpocketing].ToString();
            LockpickingTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Lockpicking].ToString();
            LoremasterTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Loremaster].ToString();
            CraftingTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Crafting].ToString();
            BarteringTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Bartering].ToString();
            CharismaTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Charisma].ToString();
            LeadershipTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Leadership].ToString();
            LuckyCharmTextBox.Text = Player.Abilities[(int)DataTable.Abilities.LuckyCharm].ToString();
            BodybuildingTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Bodybuilding].ToString();
            DuelWeldingTextBox.Text = Player.Abilities[(int)DataTable.Abilities.DuelWelding].ToString();
            WandTextBox.Text = Player.Abilities[(int)DataTable.Abilities.Wand].ToString();
        }

        public void SaveEdits()
        {
            Player.Abilities[(int)DataTable.Abilities.ManAtArms] = int.Parse(ManAtArmsTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.ExpertMarksman] = int.Parse(ExpertMarksmanTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Scoundrel] = int.Parse(ScoundrelTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.SingleHanded] = int.Parse(SingleHandedTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.TwoHanded] = int.Parse(TwoHandedTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Bow] = int.Parse(BowTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Crossbow] = int.Parse(CrossbowTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.ShieldSpecialist] = int.Parse(ShieldSpecialistTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.ArmorSpecialist] = int.Parse(ArmorSpecialistTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Witchcraft] = int.Parse(WitchcraftTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Telekinesis] = int.Parse(TelekinesisTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Willpower] = int.Parse(WillpowerTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Pyrokinetic] = int.Parse(PyrokineticTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Hydrosophist] = int.Parse(HydrosophistTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Aerotheurge] = int.Parse(AerotheurgeTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Geomancer] = int.Parse(GeomancerTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Blacksmithing] = int.Parse(BlacksmithingTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Sneaking] = int.Parse(SneakingTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Pickpocketing] = int.Parse(PickpocketingTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Lockpicking] = int.Parse(LockpickingTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Loremaster] = int.Parse(LoremasterTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Crafting] = int.Parse(CraftingTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Bartering] = int.Parse(BarteringTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Charisma] = int.Parse(CharismaTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Leadership] = int.Parse(LeadershipTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.LuckyCharm] = int.Parse(LuckyCharmTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Bodybuilding] = int.Parse(BodybuildingTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.DuelWelding] = int.Parse(DuelWeldingTextBox.Text);
            Player.Abilities[(int)DataTable.Abilities.Wand] = int.Parse(WandTextBox.Text);
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
