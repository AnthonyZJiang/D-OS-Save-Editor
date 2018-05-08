using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for AddBoostDialog.xaml
    /// </summary>
    public partial class AddBoostDialog
    {
        public string BoostText { get; set; }

        public AddBoostDialog(string predictedKeyword)
        {
            InitializeComponent();

            foreach (var s in DataTable.GenerationBoosts)
            {
                BoostListBox.Items.Add(new ListBoxItem
                {
                    Content = s
                });
            }

            if (DataTable.IsOnlineBoostsGenerated)
            {
                foreach (var s in DataTable.GenerationBoostsAddOnline)
                {
                    BoostListBox.Items.Add(new ListBoxItem
                    {
                        Content = s
                    });
                }

                foreach (var s in DataTable.UnlistedStatsBoosts)
                {
                    BoostListBox.Items.Add(new ListBoxItem
                    {
                        Content = s
                    });
                }
            }
            else
            {
                foreach (var s in DataTable.UserGenerationBoosts)
                {
                    if (DataTable.GenerationBoosts.Contains(s)) continue;
                    BoostListBox.Items.Add(new ListBoxItem
                    {
                        Content = s
                    });
                }
            }

            BoostTextBox.Text = predictedKeyword;
        }

        private void BoostTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (ListBoxItem i in BoostListBox.Items)
            {
                var listBoxText = ((string)i.Content).ToLower();
                var searchTerms = BoostTextBox.Text.ToLower().Split(' ');
                var visiblily = Visibility.Visible;
                if (searchTerms.Any(s => !listBoxText.Contains(s)))
                {
                    visiblily = Visibility.Collapsed;
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
