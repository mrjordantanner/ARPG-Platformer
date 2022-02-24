using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class InventorySlotGUI : ItemSlotGUI, ISelectHandler, ISubmitHandler
    {
        /// <summary>
        /// Lives on a UI Button object inside a RectTransform inventory panel
        /// When selected, stats are compared
        /// When submitted upon, item is equipped
        /// </summary>
        
        // User navigates to and highlights item
        public void OnSelect(BaseEventData baseEventData)
        {
            //InventoryUI.Instance.DisplayItemStats(itemInSlot);

            //InventoryUI.Instance.CompareStatsWithEquipped(itemInSlot);

        }

        // User presses "submit" button while item is selected
        public void OnSubmit(BaseEventData baseEventData)
        {
            //if (itemInSlot )

            //Equipment.Instance.EquipItem()
            //Inventory.Instance.EquipItem(itemInSlot);

        }

    }
}
