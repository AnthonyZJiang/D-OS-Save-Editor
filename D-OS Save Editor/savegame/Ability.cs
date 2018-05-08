using System.Windows.Controls;

namespace D_OS_Save_Editor
{
    public class Ability
    {
        public int Index { get; }
        public string DevName { get; }
        public string Name { get; }
        public string Effect { get; }

        public Ability(int index, string devName, string name, string effect)
        {
            Index = index;
            Name = name;
            DevName = devName;
            Effect = effect;
        }
    }
}
