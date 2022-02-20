using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
using TMPro;

namespace Assets.Scripts
{
    public class Inventory : MonoBehaviour
    {
        /// <summary>
        /// Player inventory controller that contains all collectible items that are not
        /// stackable collectibles like crafting materials, etc.
        /// </summary>

        // TODO:  Make separate inventory list for each Item.Type, 
        // then have them sorted by Item.Quality within that list

        public static Inventory Instance;

        public RectTransform inventoryPanel;
        public RectTransform equipmentPanel;
        public GameObject inventoryButtonPrefab;

        [HideInInspector]
        public ItemSlotGUI[] equipmentSlots;

        public ItemSlotGUI equipSlot_weapon, equipSlot_helm, equipSlot_mail, equipSlot_cloak, equipSlot_bracers, equipSlot_boots;
        public ItemSlotGUI[] invSlots_weapon, invSlots_helm, invSlots_mail, invSlots_cloak, invSLots_bracers, invSlots_boots;

        [Header("Inventory Items")]
        public List<Item> inventoryItems = new List<Item>();
        public Item[] weapons = new Item[12];
        public Item[] helms = new Item[6];
        public Item[] mail = new Item[6];
        public Item[] cloaks = new Item[6];
        public Item[] bracers = new Item[6];
        public Item[] boots = new Item[6];

        AttributeLabelGUI[] attributeLabelGUIS;
        StatComparisonGUI[] statComparisonGUIS;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

        }

        void Start()
        {
            attributeLabelGUIS = FindObjectsOfType<AttributeLabelGUI>();
            statComparisonGUIS = FindObjectsOfType<StatComparisonGUI>();

            CreateEquipmentSlots();
        }

        void CreateEquipmentSlots()
        {
            equipmentSlots = new ItemSlotGUI[] {
                equipSlot_weapon,
                equipSlot_helm,
                equipSlot_mail,
                equipSlot_cloak,
                equipSlot_bracers,
                equipSlot_boots
            };

        }

        public void UpdateInventoryUI()
        {
            ClearInventoryUI();

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                GameObject newButton = Instantiate(inventoryButtonPrefab);
                newButton.transform.SetParent(inventoryPanel);

                newButton.GetComponentInChildren<TextMeshProUGUI>().text = inventoryItems[i].itemName;
                var itemSlotGUI = newButton.GetComponent<ItemSlotGUI>();

                // on the GUI, assigns a reference to the Item this GUI represents       
                itemSlotGUI.itemInSlot = inventoryItems[i];          

                // on the item, assigns reference to the ItemSlotGUI
                inventoryItems[i].itemSlotGUI = itemSlotGUI;     

                itemSlotGUI.icon = inventoryItems[i].icon;
            }
        }

        // 
        public void RefreshItemSlots(ItemSlotGUI[] slots)
        {
            // ClearUI();



            foreach(var slot in slots)
            {


                // Is slot occupied?

                // Iterate through each slot and compare against same index in the matching index in the inventory array
                // assign the item and item icon to the slot and make sure it's interactable and enabled
                // This will either refresh existing items or add new icons to the slots







            }



        }

        public void ClearInventoryUI()
        {
            Button[] allInventoryButtons = inventoryPanel.GetComponentsInChildren<Button>();
            foreach (var b in allInventoryButtons)
                Destroy(b.gameObject);

        }

        public void UpdateEquipmentUI()
        {
            ClearEquipmentUI();

            for (int i = 0; i < equipmentSlots.Length; i++)
            {
                GameObject newButton = Instantiate(inventoryButtonPrefab);
                newButton.transform.SetParent(equipmentPanel);
                if (equipmentSlots[i].itemInSlot != null)
                {
                    // assigns a reference to the Item this Button represents        }
                    newButton.GetComponent<ItemSlotGUI>().itemInSlot = equipmentSlots[i].itemInSlot; 
                    newButton.GetComponentInChildren<TextMeshProUGUI>().text = equipmentSlots[i].itemInSlot.itemName;
                }
                else
                {
                    newButton.GetComponent<ItemSlotGUI>().itemInSlot = null;
                    newButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }

            }
        }

        public void ClearEquipmentUI()
        {

            Button[] allEquipmentButtons = equipmentPanel.GetComponentsInChildren<Button>();
            foreach (var b in allEquipmentButtons)
                Destroy(b.gameObject);
        }

        public void EquipItem(Item item)
        {
            inventoryItems.Remove(item);

            if (item.type == Item.Type.Weapon)
            {
                if (equipSlot_weapon.itemInSlot != null)
                    UnequipItem(equipSlot_weapon.itemInSlot);

                equipSlot_weapon.itemInSlot = item;
                return;
            }

            switch (((Item.Armor)item).type)
            {
                case Item.Armor.Type.Helm:
                    if (equipSlot_helm.itemInSlot != null)
                        UnequipItem((Item.Armor)equipSlot_helm.itemInSlot);
                    equipSlot_helm.itemInSlot = item;
                    break;

                case Item.Armor.Type.Mail:
                    if (equipSlot_mail.itemInSlot != null)
                        UnequipItem((Item.Armor)equipSlot_mail.itemInSlot);
                    equipSlot_mail.itemInSlot = item;
                    break;

                case Item.Armor.Type.Cloak:
                    if (equipSlot_cloak.itemInSlot != null)
                        UnequipItem((Item.Armor)equipSlot_cloak.itemInSlot);
                    equipSlot_cloak.itemInSlot = item;
                    break;

                case Item.Armor.Type.Bracers:
                    if (equipSlot_bracers.itemInSlot != null)
                        UnequipItem((Item.Armor)equipSlot_bracers.itemInSlot);
                    equipSlot_bracers.itemInSlot = item;
                    break;

                case Item.Armor.Type.Boots:
                    if (equipSlot_boots.itemInSlot != null)
                        UnequipItem((Item.Armor)equipSlot_boots.itemInSlot);
                    equipSlot_boots.itemInSlot = item;
                    break;
            }

            Stats.Instance.AddStatsFromItem(item);
            item.equipped = true;
            UpdateInventoryUI();
            UpdateEquipmentUI();
            print("Equipped item: " + item.itemName + item.ID);

        }

        public void UnequipItem(Item item)
        {
            if (item == null) return;

            if (item.type == Item.Type.Weapon)
            {
                equipSlot_weapon.itemInSlot = null;
                Stats.Instance.RemoveStatModifiersFromSource(item, StatModifier.Source.EquippedWeapon);
            }
            else
            {
                switch (((Item.Armor)item).type)
                {
                    case Item.Armor.Type.Helm:
                        equipSlot_helm.itemInSlot = null;
                        Stats.Instance.RemoveStatModifiersFromSource(item, StatModifier.Source.EquippedHelm);
                        break;

                    case Item.Armor.Type.Mail:
                        equipSlot_mail.itemInSlot = null;
                        Stats.Instance.RemoveStatModifiersFromSource(item, StatModifier.Source.EquippedMail);
                        break;

                    case Item.Armor.Type.Cloak:
                        equipSlot_cloak.itemInSlot = null;
                        Stats.Instance.RemoveStatModifiersFromSource(item, StatModifier.Source.EquippedCloak);
                        break;

                    case Item.Armor.Type.Bracers:
                        equipSlot_bracers.itemInSlot = null;
                        Stats.Instance.RemoveStatModifiersFromSource(item, StatModifier.Source.EquippedBracers);
                        break;

                    case Item.Armor.Type.Boots:
                        equipSlot_boots.itemInSlot = null;
                        Stats.Instance.RemoveStatModifiersFromSource(item, StatModifier.Source.EquippedBoots);
                        break;

                }

                item.equipped = false;
                inventoryItems.Add(item);
                UpdateInventoryUI();
                UpdateEquipmentUI();
                ClearStatComparisons();
                print("Unequipped item: " + item.itemName + item.ID);

            }

        }

        public void UnequipAllItems()
        {
            foreach (EquipmentSlot slot in equipmentSlots)
            {
                UnequipItem(slot.itemInSlot);
            }

            UpdateInventoryUI();
            ClearEquipmentUI();
            ClearStatComparisons();
        }

        public void ClearItemStats()
        {
            foreach (var label in attributeLabelGUIS)
            {
                label.attributeLabel.text = "";
                label.attributeValue.text = "";

            }

        }


        // Gets stats from currently highlighted item and updates the GUI to display them
        public void DisplayItemStats(Item item)
        {
            ClearItemStats();

            if (item != null && item.itemAttributes.Count > 0)
            {
                for (int i = 0; i < item.itemAttributes.Count - 1; i++)
                {

                    attributeLabelGUIS[i].attributeLabel.text = item.itemAttributes[i].StatType.ToString();
                    attributeLabelGUIS[i].attributeValue.text = item.itemAttributes[i].Value.ToString();
                }
            }
            else
            {
                Debug.LogError("Unable to display item stats. Either item is null or has no itemAttributes.");
            }
        }

        // Shows what changes to stats will occur if highlighted inventory item is equipped
        public void CompareItemStats(Item inventoryItem, Item equippedItem)
        {
            // get all StatMods from inventoryItem- aka inventoryItem.itemAttributes
            // get all StatMods from equippedItem of same Type, if any
            // find what StatMod.Types both items have in common, if any
            // compare each StatMod.Value of same StatMod.Type
            // display the difference in the StatsGUI, changing color based on increase/decrease/no change

            ClearStatComparisons();

            // iterate through all StatMods on highlighted inventory item
            foreach (var inventoryStatMod in inventoryItem.itemAttributes)
            {
                // No item equipped, all stat changes are improvements
                if (equippedItem == null)
                {
                    var matchingStatModType = inventoryStatMod.StatType;
                    var matchingStatModDifference = inventoryStatMod.Value;

                    foreach (var statComparisonGUI in statComparisonGUIS)
                        statComparisonGUI.SetText(matchingStatModType, matchingStatModDifference);
                }
                else
                    // iterate through all StatMods on equipped item
                    foreach (var equippedStatMod in equippedItem.itemAttributes)
                    {
                        // if StatMod on equipped Item matches StatMod on highlighted inventory item, 
                        if (equippedStatMod.StatType == inventoryStatMod.StatType)
                        {
                            var matchingStatModType = equippedStatMod.StatType;
                            var matchingStatModDifference = inventoryStatMod.Value - equippedStatMod.Value;

                            // TODO:  Make specific references to GUI value labels for certain calculated values like Est Damage, Wpn Damage Bonus, Mag Damage Bonus, etc

                            // Determine whether it's an increase, decrease, or no change, and 
                            // Format and set the text
                            foreach (var statComparisonGUI in statComparisonGUIS)
                                statComparisonGUI.SetText(matchingStatModType, matchingStatModDifference);

                        }
                    }
            }
        }


        public void ClearStatComparisons()
        {
            foreach (var statComparisonGUI in statComparisonGUIS)
                statComparisonGUI.statComparisonValue.text = " ";
        }

        public void Clear()
        {
            inventoryItems.Clear();
            UpdateInventoryUI();
            ClearConsole();
        }

        public void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

    }



}