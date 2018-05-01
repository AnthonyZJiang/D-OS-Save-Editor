using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for ErrorReporting.xaml
    /// </summary>
    public partial class ErrorReporting
    {
        public ErrorReporting()
        {
            InitializeComponent();
            DataContext = new ErrorModel();
        }

        public ErrorReporting(string message, object data)
        {
            InitializeComponent();
            DataContext = new ErrorModel(message, data);
        }
    }

    public class ErrorModel
    {
        public string ErrorMessage { get; set; }
        public object ErrorData { get; set; }

        public ErrorModel()
        {
        }

        public ErrorModel(string message, object data)
        {
            ErrorMessage = message;
            ErrorData = data;
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
