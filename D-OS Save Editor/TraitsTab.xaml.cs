using System.Windows.Controls;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for TraitsTab.xaml
    /// </summary>
    public partial class TraitsTab : UserControl
    {
        private Player _player;
        public Player Player
        {
            get => _player;

            set
            {
                _player = value;
                UpdateForm();
            }
        }

        private TraitCouple[] _traitCouples;

        public TraitsTab()
        {
            InitializeComponent();
        }

        public void UpdateForm()
        {
            _traitCouples = new TraitCouple[ConversionTable.TraitNames.Length / 2];
            StackPanel.Children.Clear();

            for (var i = 0; i < ConversionTable.TraitNames.Length / 2; i++)
            {
                _traitCouples[i] = new TraitCouple(
                    new Trait(ConversionTable.TraitNames[2 * i], Player.Traits[2 * i].ToString(),
                        ConversionTable.TraitEffects[2 * i]),
                    new Trait(ConversionTable.TraitNames[2 * i + 1], Player.Traits[2 * i + 1].ToString(),
                        ConversionTable.TraitEffects[2 * i + 1]));

                StackPanel.Children.Add(_traitCouples[i]);
            }
        }

        public void SaveEdits()
        {
            for (var i = 0; i < ConversionTable.TraitNames.Length / 2; i++)
            {
                var tc = _traitCouples[i].DataContext as TraitCoupleViewModel;
                Player.Traits[2 * i] = int.Parse(tc.LeftTrait.Value);
                Player.Traits[2 * i + 1] = int.Parse(tc.RightTrait.Value);
            }
        }
    }
}
