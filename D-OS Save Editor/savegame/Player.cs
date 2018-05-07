using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace D_OS_Save_Editor
{
    public class Player
    {
        private string _maxVitalityPatchCheck;

        private string _vitality;

        private string _experience;

        private string _reputation;

        private string _attributePoints;

        private string _abilityPoints;

        private string _talentPoints;

        private string _gold;
        //<node id="Character">
        #region ...

        //<attribute id="MaxVitalityPatchCheck" value="522" type="4" />
        /// <summary>
        /// Max vitality/HP
        /// </summary>
        public string MaxVitalityPatchCheck
        {
            get => _maxVitalityPatchCheck;
            set
            {
                if (!XmlUtilities.IsUint(value))
                    throw new XmlValidationException("MaxVitalityPatchCheck", value);
                _maxVitalityPatchCheck = value;
            }
        }

        //<attribute id="Vitality" value="522" type="4" />
        /// <summary>
        /// Current vitality/HP
        /// </summary>
        public string Vitality
        {
            get => _vitality;
            set
            {
                if (!XmlUtilities.IsUint(value))
                    throw new XmlValidationException("Vitality", value);
                _vitality = value; }
        }

        //<attribute id="Inventory" value="335610004" type="5" />
        /// <summary>
        /// The id of the inventory associated with the player. All items in the player's inventory will have a "Parent" attribute with this property as its value.
        /// </summary>
        public string InventoryId { get; set; }

        #endregion

        #region .../children/node id=Stats

        //<attribute id="Experience" value="163375" type="4" />
        /// <summary>
        /// Experience gained by the player.
        /// </summary>
        public string Experience
        {
            get => _experience;
            set
            {
                if (!XmlUtilities.IsUint(value))
                    throw new XmlValidationException("Experience", value);
                _experience = value; }
        }

        //<attribute id="Reputation" value="2" type="4" />
        /// <summary>
        /// Reputation of the player
        /// </summary>
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
        /// <summary>
        /// Skills acquired by the player. Key is the skill name, Value indicates if the skill is acquired or not. (false may indicate the skill was forgotten)
        /// </summary>
        public Dictionary<string, bool> Skills { get; set; } = new Dictionary<string, bool>();
        #endregion

        #region .../children/node id=PlayerData/children/node id=PlayerUpgrade

        //<attribute id = "AttributePoints" value="0" type="4" />
        /// <summary>
        /// Points left to be assigned to attributes (str, dex, int etc.)
        /// </summary>
        public string AttributePoints
        {
            get => _attributePoints;
            set
            {
                if (!XmlUtilities.IsUint(value))
                    throw new XmlValidationException("AttributePoints", value);
                _attributePoints = value; }
        }

        //<attribute id = "AbilityPoints" value="0" type="4" />
        /// <summary>
        /// Points left to be assigned to abilities
        /// </summary>
        public string AbilityPoints
        {
            get => _abilityPoints;
            set
            {
                if (!XmlUtilities.IsUint(value))
                    throw new XmlValidationException("AbilityPoints", value);
                _abilityPoints = value; }
        }

        //<attribute id = "TalentPoints" value="0" type="4" />
        /// <summary>
        /// Points left to acquire talents
        /// </summary>
        public string TalentPoints
        {
            get => _talentPoints;
            set
            {
                if (!XmlUtilities.IsUint(value))
                    throw new XmlValidationException("TalentPoints", value);
                _talentPoints = value; }
        }

        #endregion

        #region .../children/node id=PlayerData/children/node id=PlayerUpgrade/children
        //<node id = "Attributes" >
        //    < attribute id="Object" value="10" type="4" />
        //</node>
        /// <summary>
        /// Acquired attribute points (str, dex, int...) of the player. Keys are attribute ids which can be found in ConversionTable class, and Values are points that have been assigned to the corresponding attributes.
        /// </summary>
        public Dictionary<int, int> Attributes { get; set; } = new Dictionary<int, int>();
        //<node id = "Abilities" >
        //    < attribute id="Object" value="3" type="4" />
        //</node>
        /// <summary>
        /// Acquired ability points of the player. Keys are ability ids which can be found in ConversionTable class, and Values are points that have been assigned to the corresponding abilities.
        /// </summary>
        public Dictionary<int, int> Abilities { get; set; } = new Dictionary<int, int>();
        //<node id = "Talents" >
        //    < attribute id="Object" value="1048584" type="5" />
        //</node>
        /// <summary>
        /// Acquired talents of the player.
        /// </summary>
        public List<uint> Talents { get; set; } = new List<uint>();
        //<node id = "Traits" >
        //    < attribute id="Object" value="0" type="2" />
        //</node>
        /// <summary>
        /// Trait points of the player. Keys are trait ids which can be found in ConversionTable class, and Values are points of the corresponding talents.
        /// </summary>
        public Dictionary<int, int> Traits { get; set; } = new Dictionary<int, int>();
        #endregion

        #region .../children/node id=PlayerData/children/node id=PlayerCustomData
        //<attribute id = "Name" value="" type="29" />
        /// <summary>
        /// Name of the player, which is only available for created players at the beginning of the game. Henchmen have empty names.
        /// </summary>
        public string Name { get; set; }
        //<attribute id="Icon" value="" type="22" />
        /// <summary>
        /// Icon id of the player
        /// </summary>
        public string Icon { get; set; }
        //<attribute id="ClassType" value="" type="22" />
        /// <summary>
        /// Class type of the player
        /// </summary>
        public string ClassType { get; set; }
        //<attribute id = "IsMale" value="False" type="19" />
        /// <summary>
        /// Gender of the player, true for it being a male.
        /// </summary>
        public string IsMale { get; set; }
        #endregion

        #region items
        /// <summary>
        /// Items in the player's inventory
        /// </summary>
        public Item[] Items { get; set; }

        /// <summary>
        /// NOT IN USE. Amount of gold.
        /// </summary>
        public string Gold
        {
            get => _gold;
            set
            {
                //if (!XmlUtilities.IsUnint(value))
                //    throw new XmlValidationException("Gold", value);
                _gold = value;
            }
        }

        /// <summary>
        /// Indicates whether the slot that has been occupied in the player's inventory.
        /// </summary>
        public bool[] SlotsOccupation = new bool[16384];

        /// <summary>
        /// Any change in item (modification, adding, deleting) should be kept here. And this record will be used for writing any change in item to the savegame file; the original item will not be used.
        /// </summary>
        public Dictionary<string, ItemChange> ItemChanges { get; set; } = new Dictionary<string, ItemChange>();
        #endregion

        /// <summary>
        /// Make a deep copy of the object
        /// </summary>
        /// <returns></returns>
        public Player DeepClone()
        {
            var player = MemberwiseClone() as Player;
            player.Skills = new Dictionary<string, bool>(player.Skills);
            player.Attributes = new Dictionary<int, int>(player.Attributes);
            player.Abilities = new Dictionary<int, int>(player.Abilities);
            player.Talents = new List<uint>(player.Talents);
            player.Traits = new Dictionary<int, int>(player.Traits);
            player.Items = Items.Select(a => a.DeepClone()).ToArray();
            return player;
        }
    }

    public class PlayerParserException: Exception
    {
        public PlayerParserException() { }

        public PlayerParserException(Exception inner, XmlNode playerNode) : 
            base($"Player XML:\n\n{XmlUtilities.BeautifyXml(playerNode)}\n\n", inner)
        { }
    }
}
