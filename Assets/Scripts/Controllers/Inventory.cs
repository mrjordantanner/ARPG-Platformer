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
        /// Player inventory controller that contains player's weapons and armor
        /// May eventually contain stackable collectibles like crafting materials, etc.
        /// Weapons and Armor that are currently equipped are controlled by Equipment.cs
        /// </summary>

        #region Singleton
        public static Inventory Instance;

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

        int maxWeaponQuantity = 12;
        int maxArmorQuantity = 6;

        public IEquippable[] inventoryWeaponSlots;
        public Dictionary<ArmorType, IEquippable[]> inventoryArmorSlots = new Dictionary<ArmorType, IEquippable[]>();

        void Start()
        {
            inventoryWeaponSlots = new IEquippable[maxWeaponQuantity];

            inventoryArmorSlots.Add(ArmorType.Helm, new IEquippable[maxArmorQuantity]);
            inventoryArmorSlots.Add(ArmorType.Mail, new IEquippable[maxArmorQuantity]);
            inventoryArmorSlots.Add(ArmorType.Cloak, new IEquippable[maxArmorQuantity]);
            inventoryArmorSlots.Add(ArmorType.Bracers, new IEquippable[maxArmorQuantity]);
            inventoryArmorSlots.Add(ArmorType.Boots, new IEquippable[maxArmorQuantity]);
        }

        public bool CanAddItem(IEquippable item)
        {
            if (item == (Weapon)item)
            {
                Weapon newWeapon = (Weapon)item;
                return inventoryWeaponSlots.Length < maxArmorQuantity;
            }
            else
            {
                var newArmor = (Armor)item;
                return inventoryArmorSlots[newArmor.type].Length < maxArmorQuantity;
            }

        }

        public void Add(IEquippable item)
        {
            // Add Weapon
            if (item == (Weapon)item)
            {
                Weapon newWeapon = (Weapon)item;

                if (CanAddItem(newWeapon))
                {
                    inventoryWeaponSlots[inventoryWeaponSlots.Length] = item;

                    InventoryUI.Refresh();
                }
                else
                {
                    print("Not enough inventory space!");
                }
            }

            // Add Armor
            else
            {
                var newArmor = (Armor)item;

                if (CanAddItem(newArmor))
                {
                    var armorSlots = inventoryArmorSlots[newArmor.type];
                    
                    armorSlots[armorSlots.Length] = item;

                    InventoryUI.Refresh();
                }
                else
                {
                    print("Not enough inventory space!");
                }

            }
        }

        public void Remove(IEquippable item)
        {
            


        }

        public void ClearInventory()
        {
            foreach (KeyValuePair<ArmorType, IEquippable[]> slot in inventoryArmorSlots)
            {
                foreach(IEquippable item in inventoryArmorSlots[slot.Key])
                {
                    Remove(item);
                }
            }

            InventoryUI.Refresh();
            Utilities.ClearConsole();
        }



    }



}