using System.Windows.Controls;

namespace D_OS_Save_Editor
{
    public class Talent
    {
        public int Index { get; }
        public string Name { get; }
        public string Effect { get; }
        public bool IsHidden { get; }

        public Talent(int index, string name, string effect, bool isHidden)
        {
            Index = index;
            Name = name;
            Effect = effect;
            IsHidden = isHidden;
        }
    }
}
