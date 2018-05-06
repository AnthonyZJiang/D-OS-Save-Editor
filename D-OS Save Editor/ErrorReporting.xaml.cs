using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using D_OS_Save_Editor.Annotations;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for ErrorReporting.xaml
    /// </summary>
    public partial class ErrorReporting : INotifyPropertyChanged
    {
        private string _errorMessage;
        private object _errorData;
        private string _message = "An error has occurred.";

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public object ErrorData
        {
            get => _errorData;
            set
            {
                _errorData = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (value == _message) return;
                _message = value;
                OnPropertyChanged();
            }
        }

        public ErrorReporting()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ErrorReporting(string message, string exceptionMessage, object data)
        {
            InitializeComponent();

            Message = message;
            ErrorMessage = exceptionMessage;
            _errorData = data;
            DataContext = this;
        }

        public ErrorReporting(string exceptionMessage, object data)
        {
            InitializeComponent();

            _errorMessage = exceptionMessage;
            _errorData = data;
            DataContext = this;
        }

        private void ReportButtonClicked(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"{ErrorMessage}\n\n{MainWindow.Version}");
            Process.Start(
                @"https://docs.google.com/forms/d/e/1FAIpQLSeUeKYdV8InQslbvCvA1rmffJ5t1ieond4W6hpUHkHTH7I7dg/viewform?usp=pp_url&entry.1687355392=Error+report");
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class SystemIconConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var icon = (Icon)typeof(SystemIcons).GetProperty(parameter.ToString(), BindingFlags.Public | BindingFlags.Static).GetValue(null, null);
            var bs = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return bs;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
