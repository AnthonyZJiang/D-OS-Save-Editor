using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace D_OS_Save_Editor
{
    /// <summary>
    /// The type of change made to the item
    /// </summary>
    public enum ChangeType { Modify, Add, Delete}
    /// <summary>
    /// Item category or type
    /// </summary>
    public enum ItemSortType { Item = 0, Potion, Armor, Weapon, Gold, Skillbook, Scroll, Granade, Food, Furniture, Loot, Quest, Tool, Unique, Book, Other, Key, Arrow }
    public class Item
    {
        /// <summary>
        /// Rarity of the item
        /// </summary>
        public enum ItemRarityType { Common = 0, Uncommon, Rare, Epic, Legendary, Divine, Unique}
        
        private string _flags;
        private string _isKey;
        private string _parent;
        private string _slot;
        private string _amount;
        private string _lockLevel;
        private string _vitality;
        private string _maxVitalityPatchCheck;
        private string _maxDurabilityPatchCheck;
        private string _itemType;
        private string _isGenerated;

        /// <summary>
        /// The origianl beautified xml content of the item, only available in debug mode.
        /// </summary>
        public string Xml { get; set; }

        /// <summary>
        /// Item category
        /// </summary>
        public ItemSortType ItemSort { get; set; }

        /// <summary>
        /// Item rarity
        /// </summary>
        public ItemRarityType ItemRarity { get; set; }

        /// <summary>
        /// Xml node name: Flag of the item, not sure what this does.
        /// </summary>
        public string Flags
        {
            get => _flags;
            set
            {
                if (!XmlUtilities.IsLong(value))
                    throw new XmlValidationException("Flag", value);
                _flags = value;
            }
        }

        /// <summary>
        /// Xml node name: Indicates if the item is a key or not.
        /// </summary>
        public string IsKey
        {
            get => _isKey;
            set
            {
                if (!XmlUtilities.IsBool(value))
                    throw new XmlValidationException("IsKey", value);
                _isKey = value;
            }
        }

        /// <summary>
        /// Xml node name: Name of the item (Xml attribute id: Stats)
        /// </summary>
        public string StatsName { get; set; }

        /// <summary>
        /// Xml node name: Inventory id of the item
        /// </summary>
        public string Parent
        {
            get => _parent;
            set
            {
                if (!XmlUtilities.IsLong(value))
                    throw new XmlValidationException("Parenet", value);
                _parent = value;
            }
        }

        /// <summary>
        /// Xml node name: Inventory slot that the item occupies
        /// </summary>
        public string Slot
        {
            get => _slot;
            set
            {
                if (!XmlUtilities.IsUint(value))
                    throw new XmlValidationException("Slot", value);
                _slot = value;
            }
        }

        /// <summary>
        /// Xml node name: Quantity of the itme
        /// </summary>
        public string Amount
        {
            get => _amount;
            set
            {
                if (!XmlUtilities.IsUint(value))
                    throw new XmlValidationException("Amount", value);
                _amount = value;
            }
        }

        /// <summary>
        /// Xml node name: Indicates whether the item is generated or not. This property must be set to True to enable anything specified in the Generation node.
        /// </summary>
        public string IsGenerated
        {
            get => _isGenerated;
            set
            {
                if (!XmlUtilities.IsBool(value))
                    throw new XmlValidationException("IsKey", value);
                _isGenerated = value;
            }
        }

        /// <summary>
        /// Xml node name: Lock level of the tiem. only makes sense to a chest; for all other items, it is a constant 1; for unlocked chests -1
        /// </summary>
        public string LockLevel
        {
            get => _lockLevel;
            set
            {
                if (!XmlUtilities.IsInt(value))
                    throw new XmlValidationException("LockLevel", value);
                _lockLevel = value;
            }
        }

        /// <summary>
        /// Xml node name: Current hp of an item, makes sense to items such as chests; for items with no hp, it is a constant -1
        /// </summary>
        public string Vitality
        {
            get => _vitality;
            set
            {
                if (!XmlUtilities.IsInt(value))
                    throw new XmlValidationException("Vitality", value);
                _vitality = value;
            }
        }

        /// <summary>
        /// Xml node name: Max hp of an item, makes sense to items such as chests; for items with no hp, it is a constant -1
        /// </summary>
        public string MaxVitalityPatchCheck
        {
            get => _maxVitalityPatchCheck;
            set
            {
                if (!XmlUtilities.IsInt(value))
                    throw new XmlValidationException("MaxVitalityPatchCheck", value);
                _maxVitalityPatchCheck = value;
            }
        }

        /// <summary>
        /// Xml node name: Rarity of the item.
        /// </summary>
        public string ItemType
        {
            get => _itemType;
            set
            {
                if (!Enum.TryParse(
                    value[0].ToString().ToUpper() + value.Substring(1, value.Length - 1).ToLower(), out ItemRarityType ir))
                    throw new XmlValidationException("ItemType", value);

                _itemType = value;
                ItemRarity = ir;
            }
        }

        /// <summary>
        /// Xml node name: max durability of an item, makes sense to a piece of equipment
        /// </summary>
        public string MaxDurabilityPatchCheck
        {
            get => _maxDurabilityPatchCheck;
            set
            {
                if (value != null && !XmlUtilities.IsInt(value))
                    throw new XmlValidationException("MaxDurabilityPatchCheck", value);
                _maxDurabilityPatchCheck = value;
            }
        }

        /// <summary>
        /// Xml node name: the children of this node contains generated item stats and modifiers
        /// </summary>
        public GenerationNode Generation { get; set; }

        /// <summary>
        /// Xml node name: stats of the item
        /// </summary>
        public StatsNode Stats { get; set; }


        public class GenerationNode
        {
            private string _random;

            /// <summary>
            /// Xml node name: Item base
            /// </summary>
            public string Base { get; set; }

            /// <summary>
            /// Xml node name: Random seed, seems like can be safely set to any arbitary value
            /// </summary>
            public string Random
            {
                get => _random;
                set
                {
                    if (!XmlUtilities.IsLong(value))
                        throw new XmlValidationException("Random", value);
                    _random = value;
                }
            }

            /// <summary>
            /// Xml node name: Modifiers generated on the item
            /// </summary>
            public List<string> Boosts { get; set; }

            public GenerationNode() { }

            public GenerationNode(string baseStats, string random)
            {
                Base = baseStats;
                Random = random;
            }

            /// <summary>
            /// Create a deep copy of the object
            /// </summary>
            /// <returns></returns>
            public GenerationNode DeepClone()
            {
                if (!(MemberwiseClone() is GenerationNode stats)) return null;

                stats.Boosts = Boosts.ToList();
                return stats;

            }
        }

        public class StatsNode
        {
            private string _durability;
            private string _durabilityCounter;
            private string _repairDurabilityPenalty;
            private string _level;
            private string _charges;

            /// <summary>
            /// Xml node name: Current durability of the item
            /// </summary>
            public string Durability
            {
                get => _durability;
                set
                {
                    if (!XmlUtilities.IsInt(value))
                        throw new XmlValidationException("Durability", value);
                    _durability = value;
                }
            }

            /// <summary>
            /// Xml node name: Not sure what this does. Supposedly, this indicates how many times left for this item to be usable after repare
            /// </summary>
            public string DurabilityCounter
            {
                get => _durabilityCounter;
                set
                {
                    if (!XmlUtilities.IsInt(value))
                        throw new XmlValidationException("DurabilityCounter", value);
                    _durabilityCounter = value;
                }
            }

            /// <summary>
            /// Xml node name: Penalty to the item max durability caused by repair
            /// </summary>
            public string RepairDurabilityPenalty
            {
                get => _repairDurabilityPenalty;
                set
                {
                    if (!XmlUtilities.IsLong(value))
                        throw new XmlValidationException("RepairDurabilityPenalty", value);
                    _repairDurabilityPenalty = value;
                }
            }

            /// <summary>
            /// Xml node name: Item level.
            /// </summary>
            public string Level
            {
                get => _level;
                set
                {
                    if (!XmlUtilities.IsUint(value))
                        throw new XmlValidationException("Level", value);
                    _level = value;
                }
            }

            /// <summary>
            /// Xml node name: Not sure what this does. A signed integer, can be negative, can be very large, can be very small.
            /// </summary>
            public string Charges
            {
                get => _charges;
                set
                {
                    if (!XmlUtilities.IsLong(value))
                        throw new XmlValidationException("Charges", value);
                    _charges = value;
                }
            }

            /// <summary>
            /// Xml node name: Not sure what this does. Most items have a boost called HasReflect.
            /// </summary>
            public Dictionary<string, string> PermanentBoost { get; set; }

            /// <summary>
            /// Create a deep copy of the object
            /// </summary>
            /// <returns></returns>
            public StatsNode DeepClone()
            {
                if (!(MemberwiseClone() is StatsNode stats)) return null;

                stats.PermanentBoost = new Dictionary<string, string>(PermanentBoost);
                return stats;

            }
        }

        /// <summary>
        /// Create a deep copy of the object
        /// </summary>
        /// <returns></returns>
        public Item DeepClone()
        {
            if (!(MemberwiseClone() is Item item)) return null;

            item.Generation = Generation?.DeepClone();
            item.Stats = Stats?.DeepClone();
            return item;

        }

        /// <summary>
        /// Generate an item with some fixed property set
        /// </summary>
        public class GenerateItem
        {
            public XmlDocument GenerateEquipment(string flags, string statsName, string parent, string itemType, string maxDurabilityPatchCheck, GenerationNode generation, StatsNode stats)
            {
                var doc = new XmlDocument();

                // common attributes
                var baseNode = 
                    $@"<node id=""Item"">
<attribute id=""Translate"" value=""0 0 0"" type=""12"" />
<attribute id=""Flags"" value=""{flags}"" type=""5"" />
<attribute id=""Level"" value="""" type=""22"" />
<attribute id=""Rotate"" value="" 1.00  0.00  0.00 &#xD;&#xA; 0.00  1.00  0.00 &#xD;&#xA; 0.00  0.00  1.00 &#xD;&#xA;"" type=""15"" />
<attribute id=""Scale"" value=""1"" type=""6"" />
<attribute id=""Global"" value=""True"" type=""19"" />
<attribute id=""Velocity"" value=""0 0 0"" type=""12"" />
<attribute id=""GoldValueOverwrite"" value=""-1"" type=""4"" />
<attribute id=""UnsoldGenerated"" value=""False"" type=""19"" />
<attribute id=""IsKey"" value=""False"" type=""19"" />
<attribute id=""TreasureGenerated"" value=""False"" type=""19"" />
<attribute id=""CurrentTemplate"" value=""50ebbcff-e6d7-4a02-94ba-0978adcdb024"" type=""22"" />
<attribute id=""CurrentTemplateType"" value=""0"" type=""1"" />
<attribute id=""OriginalTemplate"" value=""50ebbcff-e6d7-4a02-94ba-0978adcdb024"" type=""22"" />
<attribute id=""OriginalTemplateType"" value=""0"" type=""1"" />
<attribute id=""Stats"" value=""{statsName}"" type=""22"" />
<attribute id=""IsGenerated"" value=""True"" type=""19"" />
<attribute id=""Inventory"" value=""0"" type=""5"" />
<attribute id=""Parent"" value=""{parent}"" type=""5"" />
<attribute id=""Slot"" value=""43"" type=""3"" />
<attribute id=""Amount"" value=""1"" type=""4"" />
<attribute id=""Key"" value="""" type=""22"" />
<attribute id=""LockLevel"" value=""1"" type=""4"" />
<attribute id=""SurfaceCheckTimer"" value=""0"" type=""6"" />
<attribute id=""Vitality"" value=""-1"" type=""4"" />
<attribute id=""LifeTime"" value=""0"" type=""6"" />
<attribute id=""owner"" value=""67174634"" type=""5"" />
<attribute id=""ItemType"" value=""{itemType}"" type=""22"" />
<attribute id=""MaxVitalityPatchCheck"" value=""-1"" type=""4"" />
<attribute id=""MaxDurabilityPatchCheck"" value=""{maxDurabilityPatchCheck}"" type=""4"" />
</node>";
                doc.LoadXml(baseNode);


                return doc;
            }
        }

        /// <summary>
        /// Get the names of the properties that can be safely and meaningfully changed.
        /// </summary>
        /// <returns>The names of the properties</returns>
        public string GetAllowedChangeType()
        {
            var s = "";
            if (Vitality != "-1")
            {
                s += nameof(Vitality);
            }

            if (ItemSort == ItemSortType.Armor || ItemSort == ItemSortType.Weapon)
            {
                s += nameof(ItemRarity);
                s += nameof(Generation);
            }

            if (ItemSort == ItemSortType.Potion ||
                ItemSort == ItemSortType.Gold ||
                ItemSort == ItemSortType.Granade ||
                ItemSort == ItemSortType.Scroll ||
                ItemSort == ItemSortType.Food)
            {
                s += nameof(Amount);
            }

            if (ItemSort == ItemSortType.Furniture)
            {
                s += nameof(LockLevel);
            }

            // generation
            if (Generation != null)
            {
                s += nameof(Generation);
            }

            // stats
            if (Stats != null)
            {
                s += nameof(Stats);
            }

            return s;
        }
    }

    /// <summary>
    /// Changes made to an item
    /// </summary>
    public class ItemChange
    {
        public Item Item { get; }
        public ChangeType ChangeType { get;}
        public int ItemIndex { get; set; }

        public ItemChange(Item item, ChangeType changeType, int itemIndex)
        {
            Item = item.DeepClone();
            ChangeType = changeType;
            ItemIndex = itemIndex;
        }
    }

    public class ItemParserException : Exception
    {
        public ItemParserException() { }

        public ItemParserException(Exception inner, XmlNode node) : 
            base($"Item XML:\n\n{XmlUtilities.BeautifyXml(node)}\n\n", inner)
        { }
    }
}