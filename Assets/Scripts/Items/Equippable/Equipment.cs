using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Equipment : MonoBehaviour
    {
        /// <summary>
        /// Equipment controller that tracks equipped items, handles equipping/unequipping
        /// </summary>

        #region Singleton
        public static Equipment Instance;

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

        public Dictionary<ArmorType, IEquippable> _equipmentSlots = new Dictionary<ArmorType, IEquippable>();
        public IEquippable _weaponSlot = null;

        private void Start()
        {
            CreateEquipmentSlots();
        }

        public void CreateEquipmentSlots()
        {
            _equipmentSlots.Add(ArmorType.Helm, null);
            _equipmentSlots.Add(ArmorType.Mail, null);
            _equipmentSlots.Add(ArmorType.Cloak, null);
            _equipmentSlots.Add(ArmorType.Bracers, null);
            _equipmentSlots.Add(ArmorType.Boots, null);

        }

        public void EquipWeapon(Weapon newWeapon)
        {
            UnequipWeapon();

            _weaponSlot = newWeapon;

            Stats.Instance.AddStatsFromItem(newWeapon);

            AttackController.Instance.UpdateEquippedWeapon();

            InventoryUI.Refresh();
            EquipmentUI.Instance.Refresh();

        }

        public void EquipArmor(Armor newArmor)
        {
            Inventory.Instance.Remove(newArmor);

            UnequipArmor(newArmor.type);

            _equipmentSlots[newArmor.type] = newArmor;

            Stats.Instance.AddStatsFromItem(newArmor);

            InventoryUI.Refresh();
            EquipmentUI.Instance.Refresh();

        }

        public void UnequipWeapon()
        {
            if (_weaponSlot == null) return;

            Stats.Instance.RemoveStatModifiersFromSource(_weaponSlot, StatModifierSource.EquippedWeapon);

            Inventory.Instance.Add(_weaponSlot);

            _weaponSlot = null;

            InventoryUI.Refresh();
            EquipmentUI.Instance.Refresh();

        }

        public void UnequipArmor(ArmorType armorType)
        {
            if (_equipmentSlots[armorType] == null) return;

             IEquippable armor = null;
                
                switch (armorType)
                {
                    case ArmorType.Helm:
                        armor = _equipmentSlots[ArmorType.Helm];
                        Stats.Instance.RemoveStatModifiersFromSource(armor, StatModifierSource.EquippedHelm);
                        _equipmentSlots[ArmorType.Helm] = null;
                        break;

                    case ArmorType.Mail:
                        armor = _equipmentSlots[ArmorType.Mail];
                        Stats.Instance.RemoveStatModifiersFromSource(armor, StatModifierSource.EquippedMail);
                        _equipmentSlots[ArmorType.Mail] = null;
                    break;

                    case ArmorType.Cloak:
                        armor = _equipmentSlots[ArmorType.Cloak];
                        Stats.Instance.RemoveStatModifiersFromSource(armor, StatModifierSource.EquippedCloak);
                        _equipmentSlots[ArmorType.Cloak] = null;
                    break;

                    case ArmorType.Bracers:
                        armor = _equipmentSlots[ArmorType.Bracers];
                        Stats.Instance.RemoveStatModifiersFromSource(armor, StatModifierSource.EquippedBracers);
                        _equipmentSlots[ArmorType.Bracers] = null;
                    break;

                    case ArmorType.Boots:
                        armor = _equipmentSlots[ArmorType.Boots];
                        Stats.Instance.RemoveStatModifiersFromSource(armor, StatModifierSource.EquippedBoots);
                        _equipmentSlots[ArmorType.Boots] = null;
                    break;

                }

                Inventory.Instance.Add(armor);

                InventoryUI.Refresh();

                EquipmentUI.Instance.Refresh();
                EquipmentUI.Instance.ClearStatComparisons();

        }

        public void UnequipAllItems()
        {
            UnequipWeapon();

            foreach (KeyValuePair<ArmorType, IEquippable> slot in _equipmentSlots)
            {
                UnequipArmor(slot.Key);
            }

            EquipmentUI.Instance.ClearEquipmentUI();
            EquipmentUI.Instance.ClearStatComparisons();

            InventoryUI.Refresh();
        }




    }
}
