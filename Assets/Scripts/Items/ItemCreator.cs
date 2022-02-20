using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class ItemCreator : MonoBehaviour
    {
        #region Singleton
        public static ItemCreator Instance;
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

        #region Stat Ranges
        StatRange common_Small = new StatRange(0.01f, 0.03f);
        StatRange common_Large = new StatRange(0.1f, 0.2f);
        StatRange common_Armor = new StatRange(0.2f, 0.4f);
        StatRange common_coreStat = new StatRange(5, 10);
        StatRange common_WeaponDamage = new StatRange(20, 40);

        StatRange rare_Small = new StatRange(0.05f, 0.1f);
        StatRange rare_Large = new StatRange(0.2f, 0.5f);
        StatRange rare_Armor = new StatRange(0.3f, 0.6f);
        StatRange rare_coreStat = new StatRange(12, 28);
        StatRange rare_WeaponDamage = new StatRange(50, 100);

        StatRange magical_Small = new StatRange(0.12f, 0.2f);
        StatRange magical_Large = new StatRange(0.5f, 1.0f);
        StatRange magical_Armor = new StatRange(1.0f, 2.0f);
        StatRange magical_coreStat = new StatRange(32, 50);
        StatRange magical_WeaponDamage = new StatRange(125, 175);
        #endregion

        public enum WeaponType { Sword, Axe, Mace, Staff, Dagger }
        public enum ArmorType { Helm, Mail, Cloak, Bracers, Boots }

        // Create an item of random quality and type
        public void CreateItem()
        {

        }

        // Create an item of given quality of random type
        public void CreateItem(Item.Quality quality)
        {
            if (ShouldRollWeapon())
            {
                CreateWeapon(quality);
            }
            else
            {
                CreateArmor(quality);
            }
        }

        #region CreateArmor
        public void CreateArmor(Item.Quality quality, Item.Armor.Type type)
        {
            Item.Armor newArmor = new Item.Armor(quality, type);
            newArmor.itemAttributes = new List<StatModifier>();
            newArmor.itemName = GenerateArmorName(type, quality);
            GenerateItemID(newArmor);
            GenerateItemID(newArmor);

            switch (quality)
            {
                case Item.Quality.Common:

                    newArmor.icon.bg.sprite = Resources.Load<Sprite>("Icons/Backgrounds/white");


                    switch (type)
                    {
                        case Item.Armor.Type.Helm:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.helms);

                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.Defense, common_Armor.min, common_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.Intelligence, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            break;

                        case Item.Armor.Type.Mail:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.mail);

                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.Defense, common_Armor.min, common_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.Constitution, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            break;

                        case Item.Armor.Type.Cloak:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.cloaks);

                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.Defense, common_Armor.min, common_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.Luck, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            break;

                        case Item.Armor.Type.Bracers:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.bracers);

                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.Defense, common_Armor.min, common_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.Strength, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            break;

                        case Item.Armor.Type.Boots:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.boots);

                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.Defense, common_Armor.min, common_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.Agility, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            break;
                    }
                    break;

                case Item.Quality.Rare:

                    newArmor.icon.bg.sprite = Resources.Load<Sprite>("Icons/Backgrounds/gold");

                    switch (type)
                    {
                        case Item.Armor.Type.Helm:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.helms);

                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.Defense, rare_Armor.min, rare_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.Intelligence, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.MaxMagic, rare_Small.min, rare_Small.max, StatModType.PercentAdd, 1f);
                            break;

                        case Item.Armor.Type.Mail:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.mail);

                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.Defense, rare_Armor.min, rare_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.Constitution, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.MaxHealth, rare_Large.min, rare_Large.max, StatModType.PercentAdd, 1f);
                            break;

                        case Item.Armor.Type.Cloak:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.cloaks);

                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.Defense, rare_Armor.min, rare_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.Luck, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.DodgeChance, rare_Small.min, rare_Small.max, StatModType.Flat, 1f);
                            break;

                        case Item.Armor.Type.Bracers:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.bracers);

                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.Defense, rare_Armor.min, rare_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.Strength, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.CritDamage, rare_Large.min, rare_Large.max, StatModType.Flat, 1f);
                            break;

                        case Item.Armor.Type.Boots:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.boots);

                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.Defense, rare_Armor.min, rare_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.Agility, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.MoveSpeed, rare_Small.min, rare_Small.max, StatModType.Flat, 1f);
                            break;
                    }
                    break;

                case Item.Quality.Magical:

                    newArmor.icon.bg.sprite = Resources.Load<Sprite>("Icons/Backgrounds/purple");

                    switch (type)
                    {
                        case Item.Armor.Type.Helm:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.helms);

                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.Defense, magical_Armor.min, magical_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.Intelligence, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.MaxMagic, magical_Small.min, magical_Small.max, StatModType.PercentAdd, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, FiftyFiftyAttribute(Stats.Instance.MagicOnHit, Stats.Instance.MagicRegen), magical_Small.min, magical_Small.max, StatModType.Flat, 0.75f);
                            CreateNewStatModifier(StatModifier.Source.EquippedHelm, newArmor, Stats.Instance.MagicDamageBonus, 0.15f, 0.25f, StatModType.PercentAdd, 0.5f);
                            break;

                        case Item.Armor.Type.Mail:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.mail);

                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.Defense, magical_Armor.min, magical_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.Constitution, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.MaxHealth, magical_Large.min, magical_Large.max, StatModType.PercentAdd, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.LifeRegen), magical_Small.min, magical_Small.max, StatModType.Flat, 0.75f);
                            CreateNewStatModifier(StatModifier.Source.EquippedMail, newArmor, Stats.Instance.Defense, magical_Armor.min, magical_Armor.max, StatModType.Flat, 0.5f);

                            break;

                        case Item.Armor.Type.Cloak:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.cloaks);

                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.Defense, magical_Armor.min, magical_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.Luck, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.DodgeChance, magical_Small.min, magical_Small.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, FiftyFiftyAttribute(Stats.Instance.GoldBonus, Stats.Instance.TreasureBonus), magical_Large.min, magical_Large.max, StatModType.Flat, 0.75f);
                            CreateNewStatModifier(StatModifier.Source.EquippedCloak, newArmor, Stats.Instance.CritChance, magical_Large.min, magical_Large.max, StatModType.Flat, 0.5f);
                            break;

                        case Item.Armor.Type.Bracers:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.bracers);

                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.Defense, magical_Armor.min, magical_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.Strength, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.CritDamage, magical_Large.min, magical_Large.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, FiftyFiftyAttribute(Stats.Instance.WeaponDamageBonus, Stats.Instance.MagicDamageBonus), magical_Large.min, magical_Large.max, StatModType.PercentAdd, 0.75f);
                            CreateNewStatModifier(StatModifier.Source.EquippedBracers, newArmor, Stats.Instance.XPBonus, 0.10f, 0.20f, StatModType.Flat, 0.5f);
                            break;

                        case Item.Armor.Type.Boots:
                            newArmor.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.armor.boots);

                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.Defense, magical_Armor.min, magical_Armor.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.Agility, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.MoveSpeed, magical_Small.min, magical_Small.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, FiftyFiftyAttribute(Stats.Instance.PickupRadius, Stats.Instance.GoldBonus), magical_Large.min, magical_Large.max, StatModType.Flat, 0.75f);
                            CreateNewStatModifier(StatModifier.Source.EquippedBoots, newArmor, Stats.Instance.DodgeChance, 0.05f, 0.10f, StatModType.Flat, 0.5f);
                            break;
                    }
                    break;
            }

            Inventory.Instance.inventoryItems.Add(newArmor);
            Inventory.Instance.UpdateInventoryUI();

            print("Armor Name: " + newArmor.itemName + "/ ID: " + newArmor.ID + "/Icon: " + newArmor.icon.bg);
            foreach (var attribute in newArmor.itemAttributes)
                print("" + attribute.StatType + " " + attribute.Value);
        }

        public void CreateArmor(Item.Quality quality)
        {
            CreateArmor(quality, GetRandomArmorType());
        }
        #endregion

        #region CreateWeapon
        public void CreateWeapon(Item.Quality quality, Item.Weapon.Type type)
        {
            Item.Weapon newWeapon = new Item.Weapon(quality, type);
            newWeapon.itemAttributes = new List<StatModifier>();
            newWeapon.itemName = GenerateWeaponName(type, quality);
            GenerateItemID(newWeapon);
            AssignWeaponAttackObject(newWeapon);

            switch (quality)
            {
                case Item.Quality.Common:

                    newWeapon.icon.bg.sprite = Resources.Load<Sprite>("Icons/Backgrounds/white");

                    CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.WeaponDamage, common_WeaponDamage.min, common_WeaponDamage.max, StatModType.Flat, 1f);
                    CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.WeaponDamageBonus, common_Large.min, common_Large.max, StatModType.Flat, 0.10f);

                    switch (type)
                    {
                        case Item.Weapon.Type.Sword:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.swords);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Luck, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.CritChance, common_Small.min, common_Small.max, StatModType.Flat, 0.5f);
                            break;

                        case Item.Weapon.Type.Axe:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.axes);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Strength, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.CritDamage, common_Large.min, common_Large.max, StatModType.Flat, 0.5f);
                            break;

                        case Item.Weapon.Type.Mace:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.maces);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Constitution, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.LifeRegen, common_Small.min, common_Small.max, StatModType.Flat, 0.5f);
                            break;

                        case Item.Weapon.Type.Staff:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.staves);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Intelligence, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.MagicDamageBonus, common_Large.min, common_Large.max, StatModType.Flat, 0.5f);
                            break;

                        case Item.Weapon.Type.Dagger:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.daggers);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Agility, common_coreStat.min, common_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.DodgeChance, common_Small.min, common_Small.max, StatModType.Flat, 0.5f);
                            break;
                    }
                    break;

                case Item.Quality.Rare:

                    newWeapon.icon.bg.sprite = Resources.Load<Sprite>("Icons/Backgrounds/gold");

                    CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.WeaponDamage, rare_WeaponDamage.min, rare_WeaponDamage.max, StatModType.Flat, 1f);
                    CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.WeaponDamageBonus, rare_Large.min, rare_Large.max, StatModType.Flat, 0.35f);

                    switch (type)
                    {
                        case Item.Weapon.Type.Sword:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.swords);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Luck, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.CritChance, rare_Small.min, rare_Small.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.MagicOnHit), rare_Small.min, rare_Small.max, StatModType.Flat, 1f);
                            break;

                        case Item.Weapon.Type.Axe:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.axes);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Strength, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.CritDamage, rare_Large.min, rare_Large.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.CritChance, Stats.Instance.MagicDamageBonus), rare_Small.min, rare_Small.max, StatModType.Flat, 1f);

                            break;

                        case Item.Weapon.Type.Mace:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.maces);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Constitution, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.LifeRegen, rare_Small.min, rare_Small.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.MaxMagic, Stats.Instance.MaxHealth), rare_Small.min, rare_Small.max, StatModType.PercentAdd, 1f);


                            break;

                        case Item.Weapon.Type.Staff:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.staves);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Intelligence, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.MagicDamageBonus, rare_Large.min, rare_Large.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.MagicOnHit, Stats.Instance.MagicRegen), rare_Small.min, rare_Small.max, StatModType.Flat, 1f);

                            break;

                        case Item.Weapon.Type.Dagger:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.daggers);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Agility, rare_coreStat.min, rare_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.DodgeChance, rare_Small.min, rare_Small.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.CritChance), rare_Small.min, rare_Small.max, StatModType.Flat, 1f);

                            break;
                    }
                    break;

                case Item.Quality.Magical:

                    newWeapon.icon.bg.sprite = Resources.Load<Sprite>("Icons/Backgrounds/purple");

                    CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.WeaponDamage, magical_WeaponDamage.min, magical_WeaponDamage.max, StatModType.Flat, 1f);
                    CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.WeaponDamageBonus, magical_Large.min, magical_Large.max, StatModType.Flat, 0.65f);

                    switch (type)
                    {
                        case Item.Weapon.Type.Sword:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.swords);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Luck, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.CritChance, magical_Small.min, magical_Small.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.MagicOnHit), magical_Small.min, magical_Small.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Strength, Stats.Instance.Intelligence), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Agility, Stats.Instance.Constitution), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 0.5f);
                            break;

                        case Item.Weapon.Type.Axe:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.axes);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Strength, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.CritDamage, magical_Large.min, magical_Large.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.CritChance, Stats.Instance.MagicDamageBonus), magical_Small.min, magical_Small.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Luck, Stats.Instance.Agility), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Constitution, Stats.Instance.Intelligence), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 0.5f);
                            break;

                        case Item.Weapon.Type.Mace:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.maces);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Constitution, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.LifeRegen, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.MaxMagic, Stats.Instance.MaxHealth), magical_Small.min, magical_Small.max, StatModType.PercentAdd, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Agility, Stats.Instance.Intelligence), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Strength, Stats.Instance.Luck), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 0.5f);
                            break;

                        case Item.Weapon.Type.Staff:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.staves);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Intelligence, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.MagicDamageBonus, magical_Large.min, magical_Large.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.MagicOnHit, Stats.Instance.MagicRegen), magical_Small.min, magical_Small.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Strength, Stats.Instance.Constitution), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Agility, Stats.Instance.Luck), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 0.5f);

                            break;

                        case Item.Weapon.Type.Dagger:
                            newWeapon.icon.image.sprite = ItemGraphics.Instance.GetRandomIcon(ItemGraphics.Instance.weapons.daggers);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.Agility, magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, Stats.Instance.DodgeChance, magical_Small.min, magical_Small.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.CritChance), magical_Small.min, magical_Small.max, StatModType.Flat, 1f);

                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Luck, Stats.Instance.Intelligence), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 1f);
                            CreateNewStatModifier(StatModifier.Source.EquippedWeapon, newWeapon, FiftyFiftyAttribute(Stats.Instance.Constitution, Stats.Instance.Strength), magical_coreStat.min, magical_coreStat.max, StatModType.Flat, 0.5f);

                            break;
                    }
                    break;
            }

            Inventory.Instance.inventoryItems.Add(newWeapon);
            Inventory.Instance.UpdateInventoryUI();

            print("Weapon Name: " + newWeapon.itemName + "/ ID: " + newWeapon.ID + "/Icon: " + newWeapon.icon.bg);
            foreach (var attribute in newWeapon.itemAttributes)
                print("" + attribute.StatType + " " + attribute.Value);
        }

        public void CreateWeapon(Item.Quality quality)
        {
            CreateWeapon(quality, GetRandomWeaponType());
        }

        #endregion


        void AssignWeaponAttackObject(Item.Weapon weapon)
        {
            weapon.weaponAttackObject = Resources.Load<WeaponAttackObject>("MeleeWeapons/Sword_Slash-Wide-1");
        }


        void CreateNewStatModifier(StatModifier.Source source, Item newItem, CharacterStat characterStat, float minValue, float maxValue, StatModType statModType, float chance)
        {
            if (Random.value <= chance)
            {
                float roundedValue = 0;

                var randomValue = Random.Range(minValue, maxValue);

                if (randomValue < 1)
                {
                    roundedValue = (float)Math.Round(randomValue, 2);
                }
                else
                {
                    roundedValue = (int)Math.Round(randomValue, 0);
                }

                StatModifier newAttribute = new StatModifier(characterStat.type, statModType, roundedValue, source);

                newItem.itemAttributes.Add(newAttribute);
            }
        }

        // TODO: Add word bank for item names
        string GenerateArmorName(Item.Armor.Type type, Item.Quality quality)
        {
            string armorName = $"{quality} {type}";
            return armorName;
        }

        string GenerateWeaponName(Item.Weapon.Type type, Item.Quality quality)
        {
            string weaponName = $"{quality} {type}";
            return weaponName;
        }


        #region Helper Functions
        // Determine whether to roll weapon or armor
        bool ShouldRollWeapon()
        {
            var rnd = Random.Range(0, 100);
            if (rnd <= LootController.Instance.weaponDropChance) return true;
            return false;
        }

        public Item.Weapon.Type GetRandomWeaponType()
        {
            Item.Weapon.Type type = (Item.Weapon.Type)Random.Range(0, Enum.GetNames(typeof(Item.Weapon.Type)).Length);
            //print("Rolled weapon type: " + type);
            return type;
        }

        public Item.Armor.Type GetRandomArmorType()
        {
            Item.Armor.Type type = (Item.Armor.Type)Random.Range(0, Enum.GetNames(typeof(Item.Armor.Type)).Length);
            //print("Rolled armor type: " + type);
            return type;
        }

        public CharacterStat FiftyFiftyAttribute(CharacterStat statA, CharacterStat statB)
        {
            CharacterStat tempStat = null;
            var rnd = Random.value;
            if (rnd < 0.50) tempStat = statA;
            else tempStat = statB;
            return tempStat;
        }


        void GenerateItemID(Item newItem)
        {
            var id = Random.Range(1000, 9999);

            // Generate Item ID until it's unique compared to all existing items
            foreach (Item item in Inventory.Instance.inventoryItems)
                if (id == item.ID)
                    GenerateItemID(newItem);

            newItem.ID = id;
        }


        public Item.Quality RandomItemQuality()
        {
            var q = (Item.Quality)Random.Range(0, 2);
            return q;
        }

        //public Item.Type RandomItemType()
        //{
        //    var t = (Item.Type)Random.Range(0, 4);
        //    return t;
        //}


#endregion


    }


}

public class StatRange
{
    public float min, max;

    public StatRange(float _min, float _max)
    {
        min = _min;
        max = _max;
    }
}