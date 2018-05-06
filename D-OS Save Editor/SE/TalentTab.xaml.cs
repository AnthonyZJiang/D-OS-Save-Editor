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
    /// Interaction logic for TalentTab.xaml
    /// </summary>
    public partial class TalentTab : UserControl
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

        public TalentTab()
        {
            InitializeComponent();
        }

        public void SaveEdits()
        {
            Player.Talents[0] = CalculateTalentValue(TalentGroup0);
            Player.Talents[1] = CalculateTalentValue(TalentGroup1);
            Player.Talents[2] = CalculateTalentValue(TalentGroup2);
        }

        private uint CalculateTalentValue(Panel group)
        {
            uint value = 0;
            for (var i = 0; i < group.Children.Count; i++)
            {
                var element = (CheckBox)group.Children[i];
                if (element.IsChecked != null && (bool)element.IsChecked)
                {
                    value += (uint)Math.Pow(2, i);
                }
            }

            return value;
        }

        public void UpdateForm()
        {
            UpdateTalents((uint)Player.Talents[0], TalentGroup0);
            UpdateTalents((uint)Player.Talents[1], TalentGroup1);
            UpdateTalents((uint)Player.Talents[2], TalentGroup2);
        }

        private void UpdateTalents(uint talentValue, Panel group)
        {
            for (var i = 0; i < group.Children.Count; i++)
            {
                var hasTalent = (talentValue & (uint) Math.Pow(2, i)) != 0;
                ((CheckBox) group.Children[i]).IsChecked = hasTalent;
            }
        }
    }
}
