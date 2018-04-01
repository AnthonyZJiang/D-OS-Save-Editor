﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using LSLib.Granny;
using LSLib.LS.Story;

namespace D_OS_Save_Editor
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class LsxParser
    {
        public const string StatsGold = "Small_Gold";

        public static Player[] ParsePlayer(XmlDocument doc)
        {
            // find player data
            var playerData = doc.DocumentElement.SelectNodes("//*[@id='PlayerData']");

            if (playerData == null)
                throw new XmlException("Unable to find any player data in the savegame.");

            var players = new Player[playerData.Count];
            for (var i = 0; i < playerData.Count; i++)
            {
                players[i] = new Player
                {
                    MaxVitalityPatchCheck = playerData[i].ParentNode.ParentNode
                        .SelectSingleNode("attribute [@id='MaxVitalityPatchCheck']")?.Attributes[1].Value,
                    Vitality = playerData[i].ParentNode.ParentNode
                        .SelectSingleNode("attribute [@id='Vitality']")?.Attributes[1].Value,
                    InventoryId = playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='Inventory']")
                        ?.Attributes[1].Value,

                    Experience = playerData[i].ParentNode
                        .SelectSingleNode("node//attribute [@id='Experience']")?.Attributes[1].Value,
                    Reputation = playerData[i].ParentNode
                        .SelectSingleNode("node//attribute [@id='Reputation']")?.Attributes[1].Value,

                    AttributePoints =
                        playerData[i].SelectSingleNode("children//attribute [@id='AttributePoints']")
                            ?.Attributes[1].Value,
                    AbilityPoints =
                        playerData[i].SelectSingleNode("children//attribute [@id='AbilityPoints']")
                            ?.Attributes[1].Value,
                    TalentPoints = playerData[i].SelectSingleNode("children//attribute [@id='TalentPoints']")
                        ?.Attributes[1].Value,

                    Name = playerData[i].SelectSingleNode("children//attribute [@id='Name']")?.Attributes[1].Value,
                    Icon = playerData[i].SelectSingleNode("children//attribute [@id='Icon']")?.Attributes[1].Value,
                    ClassType = playerData[i].SelectSingleNode("children//attribute [@id='ClassType']")?.Attributes[1]
                        .Value
                };

                var nodes = playerData[i].ParentNode.SelectNodes("node//node [@id='Skills']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    players[i].Skills.Add(nodes[j].FirstChild.Attributes[1].Value,
                        nodes[j].ChildNodes[1].Attributes[1].Value == "True");
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Attributes']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    players[i].Attributes.Add(j, int.Parse(nodes[j].FirstChild.Attributes[1].Value));
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Abilities']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    players[i].Abilities.Add(j, int.Parse(nodes[j].FirstChild.Attributes[1].Value));
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Talents']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    players[i].Talents.Add(j, int.Parse(nodes[j].FirstChild.Attributes[1].Value));
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Traits']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    players[i].Traits.Add(j, int.Parse(nodes[j].FirstChild.Attributes[1].Value));
                }

                if (players[i].Name == "")
                    players[i].Name = "Henchman";

                // now we have the inventory id, we can get items
                var inventoryData =
                    doc.DocumentElement.SelectNodes($"//attribute [@id='Parent'] [@value='{players[i].InventoryId}']");
                if (inventoryData == null)
                    continue;

                players[i].Items = new Item[inventoryData.Count];
                for (var j = 0; j < inventoryData.Count; j++)
                {
                    var item = ParseItem(inventoryData[j].ParentNode);
                    players[i].Items[j] = item;
                    players[i].SlotsOccupation[int.Parse(item.Slot)] = true;
                    if (item.StatsName == StatsGold)
                        players[i].Gold = item.Amount;
                }
            }

            return players;
        }

        public static Item ParseItem(XmlNode node)
        {
            var item = new Item
            {
#if DEBUG
                Xml = XmlUtilities.BeautifyXml(node),
#endif
                Flags = node.SelectSingleNode("attribute [@id='Flags']").Attributes[1].Value,
                IsKey = node.SelectSingleNode("attribute [@id='IsKey']").Attributes[1].Value,
                StatsName = node.SelectSingleNode("attribute [@id='Stats']").Attributes[1].Value,
                Parent = node.SelectSingleNode("attribute [@id='Parent']").Attributes[1].Value,
                Slot = node.SelectSingleNode("attribute [@id='Slot']").Attributes[1].Value,
                Amount = node.SelectSingleNode("attribute [@id='Amount']").Attributes[1].Value,
                LockLevel = node.SelectSingleNode("attribute [@id='LockLevel']").Attributes[1].Value,
                Vitality = node.SelectSingleNode("attribute [@id='Vitality']").Attributes[1].Value,
                ItemType = node.SelectSingleNode("attribute [@id='ItemType']").Attributes[1].Value,
                MaxVitalityPatchCheck =
                    node.SelectSingleNode("attribute [@id='MaxVitalityPatchCheck']").Attributes[1].Value,
                MaxDurabilityPatchCheck =
                    node.SelectSingleNode("attribute [@id='MaxDurabilityPatchCheck']")?.Attributes[1].Value,
            };

            // sort item
            if (item.IsKey == "True")
                item.ItemSort = ItemSortType.Key;
            else if (Item.GoldNames.Contains(item.StatsName.ToLower()))
                item.ItemSort = ItemSortType.Gold;
            else
            {
                var nameParts = item.StatsName.ToLower().Split('_');

                if (nameParts[0] == "wpn" &&
                    Item.ArrowNames.Contains(nameParts[1]))
                    item.ItemSort = ItemSortType.Arrow;
                else
                    switch (nameParts[0])
                    {
                        case "item":
                            item.ItemSort = ItemSortType.Item;
                            break;
                        case "potion":
                            item.ItemSort = ItemSortType.Potion;
                            break;
                        case "arm":
                            item.ItemSort = ItemSortType.Armor;
                            break;
                        case "wpn":
                            item.ItemSort = ItemSortType.Weapon;
                            break;
                        case "skillbook":
                            item.ItemSort = ItemSortType.Skillbook;
                            break;
                        case "scroll":
                            item.ItemSort = ItemSortType.Scroll;
                            break;
                        case "grn":
                            item.ItemSort = ItemSortType.Granade;
                            break;
                        case "food":
                            item.ItemSort = ItemSortType.Food;
                            break;
                        case "fur":
                            item.ItemSort = ItemSortType.Furniture;
                            break;
                        case "loot":
                            item.ItemSort = ItemSortType.Loot;
                            break;
                        case "quest":
                            item.ItemSort = ItemSortType.Quest;
                            break;
                        case "tool":
                            item.ItemSort = ItemSortType.Tool;
                            break;
                        case "unique":
                            item.ItemSort = ItemSortType.Unique;
                            break;
                        case "book":
                            item.ItemSort = ItemSortType.Book;
                            break;
                        default:
                            item.ItemSort = ItemSortType.Other;
                            break;
                    }
            }

            // check if has generation
            var genNode = node.SelectSingleNode("children/node [@id='Generation']");
            if (genNode != null)
            {
                item.Generation = new Item.GenerationNode
                {
                    Base = genNode.SelectSingleNode("attribute [@id='Base']").Attributes[1].Value,
                    ItemType = genNode.SelectSingleNode("attribute [@id='ItemType']").Attributes[1].Value,
                    Level = genNode.SelectSingleNode("attribute [@id='Level']").Attributes[1].Value,
                    Random = genNode.SelectSingleNode("attribute [@id='Random']").Attributes[1].Value
                };
                var genBoostNodes = genNode.SelectNodes("children//attribute [@id='Object']");
                item.Generation.Boosts = new List<string>();
                foreach (XmlNode n in genBoostNodes)
                {
                    item.Generation.Boosts.Add(n.Attributes[1].Value);
                }
            }

            // check if has stats
            var statsNode = node.SelectSingleNode("children/node [@id='Stats']");
            if (statsNode == null)
                return item;

            item.Stats = new Item.StatsNode
            {
                Durability = statsNode.SelectSingleNode("attribute [@id='Durability']").Attributes[1].Value,
                DurabilityCounter =
                    statsNode.SelectSingleNode("attribute [@id='DurabilityCounter']").Attributes[1].Value,
                RepairDurabilityPenalty = statsNode.SelectSingleNode("attribute [@id='RepairDurabilityPenalty']")
                    .Attributes[1].Value,
                Level = statsNode.SelectSingleNode("attribute [@id='Level']").Attributes[1].Value,
                Charges = statsNode.SelectSingleNode("attribute [@id='Charges']").Attributes[1].Value
            };
            var statsBoostNodes = statsNode.SelectNodes("children/node [@id='PermanentBoost']/attribute");
            item.Stats.PermanentBoost = new Dictionary<string, string>();
            foreach (XmlNode n in statsBoostNodes)
            {
                item.Stats.PermanentBoost.Add(n.Attributes[0].Value, n.Attributes[1].Value);
            }

            return item;
        }

        public static XmlDocument WritePlayer(XmlDocument doc, Player[] players)
        {
            // find player data
            var playerData = doc.DocumentElement.SelectNodes("//*[@id='PlayerData']");

            if (playerData == null)
                throw new XmlException("Unable to find any player data in the savegame.");

            for (var i = 0; i < playerData.Count; i++)
            {
                playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='MaxVitalityPatchCheck']")
                    .Attributes[1].Value = players[i].MaxVitalityPatchCheck;
                playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='Vitality']").Attributes[1].Value =
                    players[i].Vitality;
                playerData[i].ParentNode.ParentNode.SelectSingleNode("attribute [@id='Inventory']").Attributes[1].Value
                    = players[i].InventoryId;
                playerData[i].ParentNode.SelectSingleNode("node//attribute [@id='Experience']").Attributes[1].Value =
                    players[i].Experience;
                playerData[i].ParentNode.SelectSingleNode("node//attribute [@id='Reputation']").Attributes[1].Value =
                    players[i].Reputation;

                playerData[i].SelectSingleNode("children//attribute [@id='AttributePoints']").Attributes[1].Value =
                    players[i].AttributePoints;
                playerData[i].SelectSingleNode("children//attribute [@id='AbilityPoints']").Attributes[1].Value =
                    players[i].AbilityPoints;
                playerData[i].SelectSingleNode("children//attribute [@id='TalentPoints']").Attributes[1].Value =
                    players[i].TalentPoints;


                //var nodes = playerData[i].ParentNode.SelectNodes("node//node [@id='Skills']");
                //for (var j = 0; j < nodes?.Count; j++)
                //{
                //    Players[i].Skills.Add(nodes[j].FirstChild.Attributes[1].Value, skills[j].ChildNodes[1].Attributes[1].Value == "True");
                //}

                var nodes = playerData[i].SelectNodes("children//node [@id='Attributes']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    nodes[j].FirstChild.Attributes[1].Value = players[i].Attributes[j].ToString();
                }

                nodes = playerData[i].SelectNodes("children//node [@id='Abilities']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    nodes[j].FirstChild.Attributes[1].Value = players[i].Abilities[j].ToString();
                }

                //nodes = playerData[i].SelectNodes("children//node [@id='Talents']");
                //for (var j = 0; j < nodes?.Count; j++)
                //{
                //    nodes[j].FirstChild.Attributes[1].Value = Players[i].Talents[j].ToString();
                //}

                nodes = playerData[i].SelectNodes("children//node [@id='Traits']");
                for (var j = 0; j < nodes?.Count; j++)
                {
                    nodes[j].FirstChild.Attributes[1].Value = players[i].Traits[j].ToString();
                }

                // write item changes
                doc = WriteItemChanges(doc, players[i]);
            }

            return doc;
        }

        public static XmlDocument WriteItemChanges(XmlDocument doc, Player player)
        {
            // get all items belong to this player
            // find item data
            var inventoryData = doc.DocumentElement.SelectNodes($"//attribute [@id='Parent'] [@value='{player.InventoryId}']");
            
            foreach (var ic in player.ItemChanges)
            {
                if (ic.Value.ChangeType == ChangeType.Add)
                {

                }
                else if (ic.Value.ChangeType == ChangeType.Delete)
                {

                }
                else if (ic.Value.ChangeType == ChangeType.Modify)
                {
                    var itemNode = inventoryData[ic.Value.ItemIndex].ParentNode;

                    itemNode.SelectSingleNode("attribute [@id='Amount']").Attributes[1].Value = ic.Value.Item.Amount;
                    itemNode.SelectSingleNode("attribute [@id='LockLevel']").Attributes[1].Value = ic.Value.Item.LockLevel;
                    itemNode.SelectSingleNode("attribute [@id='Vitality']").Attributes[1].Value = ic.Value.Item.Vitality;
                    itemNode.SelectSingleNode("attribute [@id='MaxVitalityPatchCheck']").Attributes[1].Value = ic.Value.Item.MaxVitalityPatchCheck;

                    var node = itemNode.SelectSingleNode("attribute [@id='MaxDurabilityPatchCheck']");
                    if (node != null)
                        node.Attributes[1].Value = ic.Value.Item.MaxDurabilityPatchCheck;

                    // check if has generation
                    var genNode = itemNode.SelectSingleNode("children/node [@id='Generation']");
                    if (genNode != null && ic.Value.Item.Generation != null)
                    {
                        var childrenNode = genNode.SelectSingleNode("children");
                        if (childrenNode == null)
                        {
                            childrenNode = doc.CreateNode("children", "", "");
                            genNode.AppendChild(childrenNode);
                        }
                        // wipe all existing boost
                        childrenNode.RemoveAll();
                        // then add each defined boosts
                        foreach (var boostName in ic.Value.Item.Generation.Boosts)
                        {
                            // 2x faster than the slower method
                            //// attribute node
                            //var boostAttNode = doc.CreateElement("attribute");
                            //var boostId = doc.CreateAttribute("id");
                            //boostId.Value = "Object";
                            //var boostValue = doc.CreateAttribute("value");
                            //boostValue.Value = boostName;
                            //var boostType = doc.CreateAttribute("type");
                            //boostType.Value = "22";
                            //boostAttNode.Attributes.SetNamedItem(boostId);
                            //boostAttNode.Attributes.SetNamedItem(boostValue);
                            //boostAttNode.Attributes.SetNamedItem(boostType);

                            //// boost node
                            //var boostNode = doc.CreateElement("node");
                            //var boostNodeId = doc.CreateAttribute("id");
                            //boostNodeId.Value = "Boost";
                            //boostNode.Attributes.SetNamedItem(boostNodeId);
                            //boostNode.AppendChild(boostAttNode);

                            //// add to children node
                            //childrenNode.AppendChild(boostNode);

                            // slower method but easy to read
                            var boost = doc.CreateDocumentFragment();
                            boost.InnerXml = $"<node id=\"Boost\"><attribute id=\"Object\" value=\"{boostName}\" type=\"22\" /></node>";
                            childrenNode.AppendChild(boost);
                        }
                    }

                    // check if has stats
                    var statsNode = itemNode.SelectSingleNode("children/node [@id='Stats']");
                    if (statsNode == null || ic.Value.Item.Stats == null)
                        continue;

                    statsNode.SelectSingleNode("attribute [@id='Durability']").Attributes[1].Value = ic.Value.Item.Stats.Durability;
                    statsNode.SelectSingleNode("attribute [@id='DurabilityCounter']").Attributes[1].Value = ic.Value.Item.Stats.DurabilityCounter;
                    statsNode.SelectSingleNode("attribute [@id='RepairDurabilityPenalty']").Attributes[1].Value = ic.Value.Item.Stats.RepairDurabilityPenalty;
                    statsNode.SelectSingleNode("attribute [@id='Level']").Attributes[1].Value = ic.Value.Item.Stats.Level;
                }
            }

            return doc;
        }
    }


}