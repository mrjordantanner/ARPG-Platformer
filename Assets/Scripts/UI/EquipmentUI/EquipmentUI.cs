using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Assets.Scripts
{
    public class EquipmentUI : MonoBehaviour
    {

        #region Singleton
        public static EquipmentUI Instance;

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
        #endregion

        // UI Panels in Unity Editor
        public RectTransform weaponPanel, helmPanel, mailPanel, cloakPanel, bracersPanel, bootsPanel;
        public List<RectTransform> allEquipmentPanels = new List<RectTransform>();

        public ItemSlotGUI _weaponSlotGUI;

        public Dictionary<ArmorType, ItemSlotGUI> _armorSlotGUIS = new Dictionary<ArmorType, ItemSlotGUI>();

        AttributeLabelGUI[] attributeLabelGUIs;
        StatComparisonGUI[] statComparisonGUIs;


        private void Start()
        {
            attributeLabelGUIs = FindObjectsOfType<AttributeLabelGUI>();
            statComparisonGUIs = FindObjectsOfType<StatComparisonGUI>();

            _armorSlotGUIS.Add(ArmorType.Helm, new ItemSlotGUI());

            allEquipmentPanels.Add(weaponPanel);
            allEquipmentPanels.Add(helmPanel);
            allEquipmentPanels.Add(mailPanel);
            allEquipmentPanels.Add(cloakPanel);
            allEquipmentPanels.Add(bracersPanel);
            allEquipmentPanels.Add(bootsPanel);

        }

        public void Refresh()
        {
            //ClearEquipmentUI();


            //foreach (var panel in allEquipmentPanels)
            //{


            //}

            //foreach (var gui in _armorSlotGUIS)
            //{
            //     _armorSlotGUIS[gui.Key]

            //}

            //for (int i = 0; i < _equipmentSlotGUIS.Count; i++)
            //{
            //    GameObject newButton = Instantiate(inventoryButtonPrefab);

            //    newButton.transform.SetParent(equipmentPanel);

            //    if (equipmentSlots[i].itemInSlot != null)
            //    {
            //        // assigns a reference to the Item this Button represents        }
            //        newButton.GetComponent<ItemSlotGUI>().itemInSlot = equipmentSlots[i].itemInSlot;
            //        newButton.GetComponentInChildren<TextMeshProUGUI>().text = equipmentSlots[i].itemInSlot.Name;
            //    }
            //    else
            //    {
            //        newButton.GetComponent<ItemSlotGUI>().itemInSlot = null;
            //        newButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
            //    }

            //}
        }

        public void ClearEquipmentUI()
        {

            foreach (var panel in allEquipmentPanels)
            {
                Button[] allEquipmentButtons = panel.GetComponentsInChildren<Button>();
                foreach (var b in allEquipmentButtons)
                    Destroy(b.gameObject);
            }


        }


        // Gets stats from currently highlighted item and updates the GUI to display them
        public void DisplayItemStats(IEquippable item)
        {
            ClearItemStats();

            if (item != null && item.attributes.Count > 0)
            {
                for (int i = 0; i < item.attributes.Count - 1; i++)
                {

                    attributeLabelGUIs[i].attributeLabel.text = item.attributes[i].StatType.ToString();
                    attributeLabelGUIs[i].attributeValue.text = item.attributes[i].Value.ToString();
                }
            }
            else
            {
                Debug.LogError("Unable to display item stats. Either item is null or has no attributes.");
            }
        }

        // Shows what changes to stats will occur if highlighted inventory item is equipped
        public void CompareStatsWithEquipped(IEquippable inventoryItem, IEquippable equippedItem)
        {
            // get all StatMods from inventoryItem- aka inventoryItem.itemAttributes
            // get all StatMods from equippedItem of same Type, if any
            // find what StatMod.Types both items have in common, if any
            // compare each StatMod.Value of same StatMod.Type
            // display the difference in the StatsGUI, changing color based on increase/decrease/no change

            ClearStatComparisons();

            // iterate through all StatMods on highlighted inventory item
            foreach (var inventoryStatMod in inventoryItem.attributes)
            {
                // No item equipped, all stat changes are improvements
                if (equippedItem == null)
                {
                    var matchingStatModType = inventoryStatMod.StatType;
                    var matchingStatModDifference = inventoryStatMod.Value;

                    foreach (var statComparisonGUI in statComparisonGUIs)
                        statComparisonGUI.SetText(matchingStatModType, matchingStatModDifference);
                }
                else
                    // iterate through all StatMods on equipped item
                    foreach (var equippedStatMod in equippedItem.attributes)
                    {
                        // if StatMod on equipped Item matches StatMod on highlighted inventory item, 
                        if (equippedStatMod.StatType == inventoryStatMod.StatType)
                        {
                            var matchingStatModType = equippedStatMod.StatType;
                            var matchingStatModDifference = inventoryStatMod.Value - equippedStatMod.Value;

                            // TODO:  Make specific references to GUI value labels for certain calculated values like Est Damage, Wpn Damage Bonus, Mag Damage Bonus, etc

                            // Determine whether it's an increase, decrease, or no change, and 
                            // Format and set the text
                            foreach (var statComparisonGUI in statComparisonGUIs)
                                statComparisonGUI.SetText(matchingStatModType, matchingStatModDifference);

                        }
                    }
            }
        }


        public void ClearStatComparisons()
        {
            foreach (var statComparisonGUI in statComparisonGUIs)
                statComparisonGUI.statComparisonValue.text = " ";
        }

        public void ClearItemStats()
        {
            foreach (var label in attributeLabelGUIs)
            {
                label.attributeLabel.text = "";
                label.attributeValue.text = "";

            }

        }




    }
}
