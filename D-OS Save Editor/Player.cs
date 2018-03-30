using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D_OS_Save_Editor
{
    public class Player : ICloneable
    {
        //<node id="Character">
        #region ...
        //<attribute id="MaxVitalityPatchCheck" value="522" type="4" />
        public int MaxVitalityPatchCheck { get; set; }
        //<attribute id="Vitality" value="522" type="4" />
        public int Vitality { get; set; }
        //<attribute id="Inventory" value="335610004" type="5" />
        public string InventoryId { get; set; }
        #endregion

        #region .../children/node id=Stats
        //<attribute id="Experience" value="163375" type="4" />
        public int Experience { get; set; }
        //<attribute id="Reputation" value="2" type="4" />
        public int Reputation { get; set; }
        #endregion

        #region .../children/node id=MovementMachine
        //<attribute id="IsLeaderNPC" value="False" type="19" />
        //if value is True, this term is voided
        public bool IsLeaderNPC { get; set; } = true;
        #endregion

        #region .../children/node id=SkillManager/children
        //<node id="Skills">
        //<attribute id = "MapKey" value="Shout_MeleePowerStance" type="22" />
        //<attribute id = "IsLearned" value="True" type="19" />
        //<attribute id = "ActiveCooldown" value="0" type="6" />
        //</node>
        public Dictionary<string, bool> Skills { get; set; } = new Dictionary<string, bool>();
        #endregion

        #region .../children/node id=PlayerData/children/node id=PlayerUpgrade
        //<attribute id = "AttributePoints" value="0" type="4" />
        public int AttributePoints { get; set; }
        //<attribute id = "AbilityPoints" value="0" type="4" />
        public int AbilityPoints { get; set; }
        //<attribute id = "TalentPoints" value="0" type="4" />
        public int TalentPoints { get; set; }
        #endregion

        #region .../children/node id=PlayerData/children/node id=PlayerUpgrade/children
        //<node id = "Attributes" >
        //    < attribute id="Object" value="10" type="4" />
        //</node>
        public Dictionary<int, int> Attributes { get; set; } = new Dictionary<int, int>();
        //<node id = "Abilities" >
        //    < attribute id="Object" value="3" type="4" />
        //</node>
        public Dictionary<int, int> Abilities { get; set; } = new Dictionary<int, int>();
        //<node id = "Talents" >
        //    < attribute id="Object" value="1048584" type="5" />
        //</node>
        public Dictionary<int, int> Talents { get; set; } = new Dictionary<int, int>();
        //<node id = "Traits" >
        //    < attribute id="Object" value="0" type="2" />
        //</node>
        public Dictionary<int, int> Traits { get; set; } = new Dictionary<int, int>();
        #endregion

        #region .../children/node id=PlayerData/children/node id=PlayerCustomData
        //<attribute id = "Name" value="" type="29" />
        public string Name { get; set; }
        //<attribute id="Icon" value="" type="22" />
        public string Icon { get; set; }
        //<attribute id="ClassType" value="" type="22" />
        public string ClassType { get; set; }
        //<attribute id = "IsMale" value="False" type="19" />
        public bool IsMale { get; set; }
        #endregion

        public object Clone()
        {
            var player = this.MemberwiseClone() as Player;
            player.Skills = new Dictionary<string, bool>(player.Skills);
            player.Attributes = new Dictionary<int, int>(player.Attributes);
            player.Abilities = new Dictionary<int, int>(player.Abilities);
            player.Talents = new Dictionary<int, int>(player.Attributes);
            player.Traits = new Dictionary<int, int>(player.Traits);
            return player;
        }
    }
}
