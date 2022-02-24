using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts
{
    public class InventoryUI : MonoBehaviour
    {
        #region Singleton
        public static InventoryUI Instance;

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

        public static void Refresh()
        {
            ClearInventoryUI();

            //for (int i = 0; i < inventoryItems.Count; i++)
            //{
            //    GameObject newButton = Instantiate(inventoryButtonPrefab);
            //    newButton.transform.SetParent(inventoryPanel);

            //    newButton.GetComponentInChildren<TextMeshProUGUI>().text = inventoryItems[i].itemName;
            //    var itemSlotGUI = newButton.GetComponent<ItemSlotGUI>();

            //    // on the GUI, assigns a reference to the Item this GUI represents       
            //    itemSlotGUI.itemInSlot = inventoryItems[i];

            //    // on the item, assigns reference to the ItemSlotGUI
            //    // inventoryItems[i].itemSlotGUI = itemSlotGUI;

            //    itemSlotGUI.icon = inventoryItems[i].icon;
            //}
        }



        public static void ClearInventoryUI()
        {
            //Button[] allInventoryButtons = inventoryPanel.GetComponentsInChildren<Button>();
            //foreach (var b in allInventoryButtons)
            //    Destroy(b.gameObject);

        }
    }
}
