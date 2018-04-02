using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace D_OS_Save_Editor
{
    public enum ChangeType { Modify, Add, Delete}
    public enum ItemSortType { Item = 0, Potion, Armor, Weapon, Gold, Skillbook, Scroll, Granade, Food, Furniture, Loot, Quest, Tool, Unique, Book, Other, Key, Arrow }
    public class Item
    {
        public enum ItemRarityType { Common = 0, Uncommon, Rare, Epic, Legendary, Divine, Unique}

        

        public static readonly string[] GoldNames = {"small_gold", "inbetween_gold", "trader_large_gold"};
        public static readonly string[] ArrowNames = {"arrow", "arrowhead", "arrowshaft"};

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

        public string Xml { get; set; }

        public ItemSortType ItemSort { get; set; }

        public ItemRarityType ItemRarity { get; set; }

        public string Flags
        {
            get => _flags;
            set
            {
                if (!XmlUtilities.IsInt(value))
                    throw new XmlValidationException("Flag", value);
                _flags = value;
            }
        }

        // True if it is a key
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

        // name of the item (Xml attribute id: Stats)
        public string StatsName { get; set; }

        // inventory key
        public string Parent
        {
            get => _parent;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("Parenet", value);
                _parent = value;
            }
        }

        // inventory slot
        public string Slot
        {
            get => _slot;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("Slot", value);
                _slot = value;
            }
        }

        // quantity
        public string Amount
        {
            get => _amount;
            set
            {
                if (!XmlUtilities.IsUnint(value))
                    throw new XmlValidationException("Amount", value);
                _amount = value;
            }
        }

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

        // only makes sense to a chest; for all other items, it is a constant 1; for unlocked chests -1
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

        // current hp of an item, makes sense to items such as chests; for items with no hp, it is a constant -1
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

        // the same as above but the max hp
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

        // common, rare etc.
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

        // max durability of an item, makes sense to a piece of equipment
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

        public GenerationNode Generation { get; set; }

        public StatsNode Stats { get; set; }


        public class GenerationNode
        {
            private string _random;
            public string Base { get; set; }
            
            public string Random
            {
                get => _random;
                set
                {
                    if (!XmlUtilities.IsUnint(value))
                        throw new XmlValidationException("Random", value);
                    _random = value;
                }
            }

            public List<string> Boosts { get; set; }

            public GenerationNode() { }

            public GenerationNode(string baseStats, string random)
            {
                Base = baseStats;
                Random = random;
            }

            public GenerationNode DeepClone()
            {
                var stats = this.MemberwiseClone() as GenerationNode;
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

            public string RepairDurabilityPenalty
            {
                get => _repairDurabilityPenalty;
                set
                {
                    if (!XmlUtilities.IsInt(value))
                        throw new XmlValidationException("RepairDurabilityPenalty", value);
                    _repairDurabilityPenalty = value;
                }
            }

            public string Level
            {
                get => _level;
                set
                {
                    if (!XmlUtilities.IsUnint(value))
                        throw new XmlValidationException("Level", value);
                    _level = value;
                }
            }

            public string Charges
            {
                get => _charges;
                set
                {
                    if (!XmlUtilities.IsInt(value))
                        throw new XmlValidationException("Charges", value);
                    _charges = value;
                }
            }

            public Dictionary<string, string> PermanentBoost { get; set; }

            public StatsNode DeepClone()
            {
                var stats = this.MemberwiseClone() as StatsNode;
                stats.PermanentBoost = new Dictionary<string,string>(PermanentBoost);
                return stats;
            }
        }

        public Item DeepClone()
        {
            var item = this.MemberwiseClone() as Item;
            item.Generation = Generation?.DeepClone();
            item.Stats = Stats?.DeepClone();
            return item;
        }

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
}