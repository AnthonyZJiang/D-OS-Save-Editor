using System.Windows;
using System.Windows.Controls;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for AddBoostDialog.xaml
    /// </summary>
    public partial class AddBoostDialog : Window
    {
        public string BoostText { get; set; }

        public AddBoostDialog()
        {
            InitializeComponent();

            foreach (var s in ConversionTable.BoostTexts)
            {
                BoostListBox.Items.Add(new ListBoxItem
                {
                    Content = s
                });
            }
        }

        private void BoostTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (ListBoxItem i in BoostListBox.Items)
            {
                var listBoxText = ((string)i.Content).ToLower();
                var searchTerms = BoostTextBox.Text.ToLower().Split(' ');
                var visiblily = Visibility.Visible;
                foreach (var s in searchTerms)
                {
                    if (listBoxText.Contains(s)) continue;

                    visiblily = Visibility.Collapsed;
                    break;
                }
                i.Visibility = visiblily;
            }
        }

        private void BoostListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BoostTextBox.Text = (string)((ListBoxItem) e.AddedItems[0]).Content;
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            BoostText = BoostTextBox.Text;
            DialogResult = true;
        }
    }
}
