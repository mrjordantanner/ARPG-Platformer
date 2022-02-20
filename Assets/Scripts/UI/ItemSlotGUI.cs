using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class ItemSlotGUI : MonoBehaviour, ISelectHandler
    {
        [HideInInspector]
        public Selectable selectable;
        Button button;

        public Item itemInSlot;
        public Icon icon;

        public enum Type { Weapon, Helm, Mail, Cloak, Bracers, Boots }
        public Type type;

        public enum Style { inventory, equip }
        public Style style;

        private void Start()
        {
            button = GetComponent<Button>();
            selectable = button.GetComponent<Selectable>();
            //onButtonPress.AddListener(InventoryItemSelected);
            button.interactable = false;

        }

        public void OnSelect(BaseEventData baseEventData)
        {
            if (itemInSlot == null) return;



            Inventory.Instance.DisplayItemStats(itemInSlot);








            // If an inventory Weapon slot is selected, compare stats with the currently equipped weapon etc
            Item equippedItem = null;

            if (itemInSlot.type == Item.Type.Weapon)
            {
                equippedItem = Inventory.Instance.equipSlot_weapon.itemInSlot;
            }
            else
            {
                switch (((Item.Armor)itemInSlot).type)
                {
                    case Item.Armor.Type.Helm:
                        equippedItem = Inventory.Instance.equipSlot_helm.itemInSlot;
                        break;

                    case Item.Armor.Type.Mail:
                        equippedItem = Inventory.Instance.equipSlot_mail.itemInSlot;
                        break;

                    case Item.Armor.Type.Cloak:
                        equippedItem = Inventory.Instance.equipSlot_cloak.itemInSlot;
                        break;

                    case Item.Armor.Type.Bracers:
                        equippedItem = Inventory.Instance.equipSlot_bracers.itemInSlot;
                        break;

                    case Item.Armor.Type.Boots:
                        equippedItem = Inventory.Instance.equipSlot_boots.itemInSlot;
                        break;

                }

            }

            Inventory.Instance.CompareItemStats(itemInSlot, equippedItem);

        }

        public void InventoryItemHighlighted()
        {
            // TODO: On Button highlight, display stats from that item in a separate Item Stats window, 
            // show differences from what's equipped

            // Get stats from Item
            // Based on what the Item Type is, compare to stats from equipped Item in that Item Slot, if anything is equipped there

        }


        public void InventoryItemSelected()
        {
            // When user presses "Submit" button on an inventory item button
            // Is item currently equipped?  If so, unequip it.
            // If not, unequip currently equipped item of that type and equip new item

            foreach (var eSlot in Inventory.Instance.equipmentSlots)
            {
                if (eSlot.itemInSlot == itemInSlot)
                {
                    Inventory.Instance.UnequipItem(itemInSlot);
                    return;
                }
            }

            Inventory.Instance.EquipItem(itemInSlot);
            // itemInSlot.inventorySlotGUI.selectable.Select();
        }


    }
}