using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts
{/// <summary>
/// Developer hotkeys and shortcuts
/// </summary>
    public class ControlPanel : MonoBehaviour
    {
        private void Update()
        {
            // Max Equipment
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ItemCreator.Instance.CreateWeapon(ItemQuality.Magical, WeaponType.Sword);
                var sword = Inventory.Instance.inventoryWeaponSlots.Last();
                Equipment.Instance.EquipWeapon((Weapon)sword);

                ItemCreator.Instance.CreateArmor(ItemQuality.Magical, ArmorType.Helm);
                var helm = Inventory.Instance.inventoryArmorSlots[ArmorType.Helm].Last();
                Equipment.Instance.EquipArmor((Armor)helm);

                ItemCreator.Instance.CreateArmor(ItemQuality.Magical, ArmorType.Mail);
                var mail = Inventory.Instance.inventoryArmorSlots[ArmorType.Mail].Last();
                Equipment.Instance.EquipArmor((Armor)mail);

                ItemCreator.Instance.CreateArmor(ItemQuality.Magical, ArmorType.Cloak);
                var cloak = Inventory.Instance.inventoryArmorSlots[ArmorType.Cloak].Last();
                Equipment.Instance.EquipArmor((Armor)cloak);

                ItemCreator.Instance.CreateArmor(ItemQuality.Magical, ArmorType.Bracers);
                var bracers = Inventory.Instance.inventoryArmorSlots[ArmorType.Bracers].Last();
                Equipment.Instance.EquipArmor((Armor)bracers);

                ItemCreator.Instance.CreateArmor(ItemQuality.Magical, ArmorType.Boots);
                var boots = Inventory.Instance.inventoryArmorSlots[ArmorType.Boots].Last();
                Equipment.Instance.EquipArmor((Armor)boots);
            }

            // Create Weapon of random type and quality
            if (Input.GetKeyDown(KeyCode.Alpha2))
                ItemCreator.Instance.CreateWeapon(ItemCreator.Instance.RandomItemQuality(), ItemCreator.Instance.GetRandomWeaponType());

            // Create Armor of random type and quality
            if (Input.GetKeyDown(KeyCode.Alpha3))
                ItemCreator.Instance.CreateArmor(ItemCreator.Instance.RandomItemQuality(), ItemCreator.Instance.GetRandomArmorType());


            // Unequip all
            if (Input.GetKeyDown(KeyCode.Alpha4))
                Equipment.Instance.UnequipAllItems();

            // Clear inventory and console
            if (Input.GetKeyDown(KeyCode.Alpha5))
                Inventory.Instance.ClearInventory();

            //Equip last item
            //if (Input.GetKeyDown(KeyCode.Alpha7))
            //{
            //    var lastItem = Inventory.Instance.inventoryItems.Last();
            //    if (lastItem != null)
            //        Inventory.Instance.EquipItem(lastItem);
            //}

            // Clear Enemies
            if (Input.GetKeyDown(KeyCode.Alpha9))
                SpawnerController.Instance.DestroyEnemyChildren();

            // Spawn Enemies
            if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(InputManager.Instance.spawnEnemies_gamePad))
                SpawnerController.Instance.StartEnemySpawn();

            // Increase Difficulty
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                Difficulty.Instance.Raise();

            //  Decrease Difficulty
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                Difficulty.Instance.Lower();







        }

    }
}
