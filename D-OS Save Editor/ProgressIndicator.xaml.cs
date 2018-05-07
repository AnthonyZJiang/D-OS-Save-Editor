using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for ProgressIndicator.xaml
    /// </summary>
    public partial class ProgressIndicator:INotifyPropertyChanged
    {
        private string _progressText = "Busy...";
        private bool _canCancel;
        private string _cancelButtonText = "Cancel";

        public string ProgressText
        {
            get => _progressText;
            set
            {
                _progressText = value; 
                OnPropertyChanged();
            }
        }

        public bool CanCancel
        {
            get => _canCancel;
            set
            {
                _canCancel = value;
                OnPropertyChanged();
            }
        }

        public string CancelButtonText
        {
            get => _cancelButtonText;
            set
            {
                _cancelButtonText = value;
                OnPropertyChanged();
            }
        }

        public ProgressIndicator()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ProgressIndicator(string title, bool canCancel)
        {
            InitializeComponent();
            DataContext = this;

            Title = title;
            CanCancel = canCancel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
