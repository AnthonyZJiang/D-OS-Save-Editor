using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D_OS_Save_Editor
{
    public class Player
    {
        private string _maxVitalityPatchCheck;

        private string _vitality;

        private string _inventoryId;

        private string _experience;

        private string _reputation;

        private string _attributePoints;

        private string _abilityPoints;

        private string _talentPoints;

        private string _gold;
        //<node id="Character">
        #region ...

        //<attribute id="MaxVitalityPatchCheck" value="522" type="4" />
        public string MaxVitalityPatchCheck
        {
            get => _maxVitalityPatchCheck;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("MaxVitalityPatchCheck", value);
                _maxVitalityPatchCheck = value;
            }
        }

        //<attribute id="Vitality" value="522" type="4" />
        public string Vitality
        {
            get => _vitality;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("Vitality", value);
                _vitality = value; }
        }

        //<attribute id="Inventory" value="335610004" type="5" />
        public string InventoryId
        {
            get => _inventoryId;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("InventoryId", value);
                _inventoryId = value; }
        }

        #endregion

        #region .../children/node id=Stats

        //<attribute id="Experience" value="163375" type="4" />
        public string Experience
        {
            get => _experience;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("Experience", value);
                _experience = value; }
        }

        //<attribute id="Reputation" value="2" type="4" />
        public string Reputation
        {
            get => _reputation;
            set
            {
                if (!XmlUtilities.IsInt(value))
                    throw new XmlValidationException("Reputation", value);
                _reputation = value; }
        }

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
        public string AttributePoints
        {
            get => _attributePoints;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("AttributePoints", value);
                _attributePoints = value; }
        }

        //<attribute id = "AbilityPoints" value="0" type="4" />
        public string AbilityPoints
        {
            get => _abilityPoints;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("AbilityPoints", value);
                _abilityPoints = value; }
        }

        //<attribute id = "TalentPoints" value="0" type="4" />
        public string TalentPoints
        {
            get => _talentPoints;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("TalentPoints", value);
                _talentPoints = value; }
        }

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
        public string IsMale { get; set; }
        #endregion

        #region items
        public Item[] Items { get; set; }

        public string Gold
        {
            get => _gold;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("Gold", value);
                _gold = value;
            }
        }

        public bool[] SlotsOccupation = new bool[16384];

        public Dictionary<string, ItemChange> ItemChanges { get; set; } = new Dictionary<string, ItemChange>();
        #endregion

        public Player DeepClone()
        {
            var player = this.MemberwiseClone() as Player;
            player.Skills = new Dictionary<string, bool>(player.Skills);
            player.Attributes = new Dictionary<int, int>(player.Attributes);
            player.Abilities = new Dictionary<int, int>(player.Abilities);
            player.Talents = new Dictionary<int, int>(player.Attributes);
            player.Traits = new Dictionary<int, int>(player.Traits);
            
            //player.Items = new Item[Items.Length];
            //for (var i = 0; i < Items.Length; i++)
            //{
            //    player.Items[i] = Items[i].DeepClone();
            //}

            player.Items = Items.Select(a => a.DeepClone()).ToArray();
            return player;
        }
    }
}
