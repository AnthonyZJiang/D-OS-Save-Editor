using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// Interaction logic for TalentTab.xaml
    /// </summary>
    public partial class TalentTab
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
            var checkedTalents = new bool[DataTable.TalentIsHidden.Length];
            foreach (CheckBox ckb in TalentGroup0.Children)
            {
                checkedTalents[(int)ckb.Tag] = ckb.IsChecked == true;
            }
            foreach (CheckBox ckb in TalentGroup1.Children)
            {
                checkedTalents[(int)ckb.Tag] = ckb.IsChecked == true;
            }

            var bytes = new byte[12];
            new BitArray(checkedTalents).CopyTo(bytes, 0);

            Player.Talents[0] = BitConverter.ToUInt32(bytes, 0);
            Player.Talents[1] = BitConverter.ToUInt32(bytes, 4);
            Player.Talents[2] = BitConverter.ToUInt32(bytes, 8);
        }

        private void UpdateForm()
        {
            TalentGroup0.Children.Clear();
            TalentGroup1.Children.Clear();

            var playerTalentIds = GetPlayerTalentIds();
            var talentArray = DataTable.GetTalentArray();
            // order by IsHidden property
            talentArray = talentArray.OrderBy(talent => talent.Name).ToArray();

            void AddToPanel(Panel panel, Talent talent)
            {
                //object toolTipContent;
                //if (talent.IsHidden)
                //{
                //    toolTipContent = new StackPanel();
                //    ((StackPanel)toolTipContent).Children.Add(new Image());
                //    ((StackPanel)toolTipContent).Children.Add(new TextBlock { Text = talent.Effect });
                //}
                //else
                //{
                //    toolTipContent = talent.Effect;
                //}

                panel.Children.Add(
                    new CheckBox
                    {
                        Content = talent.Name,
                        Tag = talent.Index,
                        ToolTip = talent.Effect,
                        IsChecked = playerTalentIds.Contains(talent.Index)
                    });
            }

            foreach (var talent in talentArray)
            {
                AddToPanel(talent.IsHidden ? TalentGroup1 : TalentGroup0, talent);
            }
        }

        private int[] GetPlayerTalentIds()
        {
            var talentIds = new List<int>();
            var bits = new BitArray(BitConverter.GetBytes(Player.Talents[0]));
            for (var i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    talentIds.Add(i);
            }
            bits = new BitArray(BitConverter.GetBytes(Player.Talents[1]));
            for (var i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    talentIds.Add(i+32);
            }
            bits = new BitArray(BitConverter.GetBytes(Player.Talents[2]));
            for (var i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    talentIds.Add(i + 64);
            }
           
            return talentIds.ToArray();
        }
    }
}
