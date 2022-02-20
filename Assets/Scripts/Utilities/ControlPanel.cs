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
                ItemCreator.Instance.CreateWeapon(Item.Quality.Magical, Item.Weapon.Type.Sword);
                var sword = Inventory.Instance.inventoryItems.Last();
                Inventory.Instance.EquipItem(sword);

                ItemCreator.Instance.CreateArmor(Item.Quality.Magical, Item.Armor.Type.Helm);
                var helm = Inventory.Instance.inventoryItems.Last();
                Inventory.Instance.EquipItem(helm);

                ItemCreator.Instance.CreateArmor(Item.Quality.Magical, Item.Armor.Type.Mail);
                var mail = Inventory.Instance.inventoryItems.Last();
                Inventory.Instance.EquipItem(mail);

                ItemCreator.Instance.CreateArmor(Item.Quality.Magical, Item.Armor.Type.Cloak);
                var cloak = Inventory.Instance.inventoryItems.Last();
                Inventory.Instance.EquipItem(cloak);

                ItemCreator.Instance.CreateArmor(Item.Quality.Magical, Item.Armor.Type.Bracers);
                var bracers = Inventory.Instance.inventoryItems.Last();
                Inventory.Instance.EquipItem(bracers);

                ItemCreator.Instance.CreateArmor(Item.Quality.Magical, Item.Armor.Type.Boots);
                var boots = Inventory.Instance.inventoryItems.Last();
                Inventory.Instance.EquipItem(boots);
            }

            // Create Weapon of random type and quality
            if (Input.GetKeyDown(KeyCode.Alpha2))
                ItemCreator.Instance.CreateWeapon(ItemCreator.Instance.RandomItemQuality(), ItemCreator.Instance.GetRandomWeaponType());

            // Create Armor of random type and quality
            if (Input.GetKeyDown(KeyCode.Alpha3))
                ItemCreator.Instance.CreateArmor(ItemCreator.Instance.RandomItemQuality(), ItemCreator.Instance.GetRandomArmorType());


            // Unequip all
            if (Input.GetKeyDown(KeyCode.Alpha4))
                Inventory.Instance.UnequipAllItems();

            // Clear inventory and console
            if (Input.GetKeyDown(KeyCode.Alpha5))
                Inventory.Instance.Clear();

            //Equip last item
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                var lastItem = Inventory.Instance.inventoryItems.Last();
                if (lastItem != null)
                    Inventory.Instance.EquipItem(lastItem);
            }

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
