using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using D_OS_Save_Editor.Annotations;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for TraitCouple.xaml
    /// </summary>
    public partial class TraitCouple : UserControl
    {
        private Brush DefaultTextBoxBorderBrush { get; }

        public TraitCouple(Trait leftTrait, Trait rightTrait)
        {
            InitializeComponent();
            DefaultTextBoxBorderBrush = LeftTextBox.BorderBrush;

            DataContext = new TraitCoupleViewModel(leftTrait, rightTrait);

        }

        public TraitCouple GetDataContext()
        {
            return (TraitCouple)DataContext;
        }
    }

    public class Trait:INotifyPropertyChanged
    {
        private string _value;
        public string Name { get; }
        public string Effect { get; }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;

                if (!int.TryParse(value, out var _))
                    return;
                OnPropertyChanged(nameof(Value));
            }
        }

        public Trait(string name, string value, string effect)
        {
            Name = name;
            Value = value;
            Effect = effect;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// View model
    /// </summary>
    public class TraitCoupleViewModel : INotifyPropertyChanged
    {
        private int _sliderValue;
        public Trait LeftTrait { get; }
        public Trait RightTrait { get; }

        public int SliderValue { 
            get => _sliderValue;
            private set
            {
                _sliderValue = value;
                OnPropertyChanged(nameof(SliderValue));
            }
        }

        public TraitCoupleViewModel(Trait leftTrait, Trait rightTrait)
        {
            LeftTrait = leftTrait;
            RightTrait = rightTrait;
            LeftTrait.PropertyChanged += TraitOnPropertyChanged;
            RightTrait.PropertyChanged += TraitOnPropertyChanged;

            UpdateSliderValue();
        }

        private void TraitOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LeftTrait.Value))
            {
                UpdateSliderValue();
            }
        }

        private void UpdateSliderValue()
        {
            var lVal = int.Parse(LeftTrait.Value);
            var rVal = int.Parse(RightTrait.Value);
            if (lVal > rVal)
            {
                SliderValue = -1;
            }
            else if (lVal == rVal)
            {
                SliderValue = 0;
            }
            else
            {
                SliderValue = 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
