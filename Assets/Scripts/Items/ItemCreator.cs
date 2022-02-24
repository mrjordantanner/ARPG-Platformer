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


        // Create an item of random quality and type
        public void CreateItem()
        {

        }

        // Create an item of given quality of random type
        public void CreateItem(ItemQuality quality)
        {
            if (LootController.Instance.ShouldRollWeapon())
            {
                CreateWeapon(quality);
            }
            else
            {
                CreateArmor(quality);
            }
        }

        #region CreateArmor
        public void CreateArmor(ItemQuality quality, ArmorType type)
        {
            Armor newArmor = new Armor(quality, type);
            newArmor.attributes = new List<StatModifier>();
            newArmor.Name = GenerateArmorName(type, quality);
            //GenerateItemID(newArmor);
            newArmor.ID = new Guid();

            switch (quality)
            {
                case ItemQuality.Common:

                    switch (type)
                    {
                        case ArmorType.Helm:
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.Defense, common_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.Intelligence, common_coreStat);
                            break;

                        case ArmorType.Mail:
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.Defense, common_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.Constitution, common_coreStat);
                            break;

                        case ArmorType.Cloak:
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.Defense, common_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.Luck, common_coreStat);
                            break;

                        case ArmorType.Bracers:
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.Defense, common_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.Strength, common_coreStat);
                            break;

                        case ArmorType.Boots:
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.Defense, common_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.Agility, common_coreStat);
                            break;
                    }
                    break;

                case ItemQuality.Rare:

                    switch (type)
                    {
                        case ArmorType.Helm:
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.Defense, rare_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.Intelligence, rare_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.MaxMagic, rare_Small);
                            break;

                        case ArmorType.Mail:
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.Defense, rare_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.Constitution, rare_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.MaxHealth, rare_Large);
                            break;

                        case ArmorType.Cloak:
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.Defense, rare_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.Luck, rare_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.DodgeChance, rare_Small);
                            break;

                        case ArmorType.Bracers:
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.Defense, rare_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.Strength, rare_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.CritDamage, rare_Large);
                            break;

                        case ArmorType.Boots:
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.Defense, rare_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.Agility, rare_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.MoveSpeed, rare_Small);
                            break;
                    }
                    break;

                case ItemQuality.Magical:

                    switch (type)
                    {
                        case ArmorType.Helm:
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.Defense, magical_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.Intelligence, magical_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.MaxMagic, magical_Small);
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, FiftyFiftyAttribute(Stats.Instance.MagicOnHit, Stats.Instance.MagicRegen), magical_Small, StatModType.Flat, 0.75f);
                            CreateNewStatModifier(StatModifierSource.EquippedHelm, newArmor, Stats.Instance.MagicDamageBonus, magical_Small, StatModType.PercentAdd, 0.5f);
                            break;

                        case ArmorType.Mail:
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.Defense, magical_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.Constitution, magical_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.MaxHealth, magical_Large, StatModType.PercentAdd, 1f);
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.LifeRegen), magical_Small, StatModType.Flat, 0.75f);
                            CreateNewStatModifier(StatModifierSource.EquippedMail, newArmor, Stats.Instance.Defense, magical_Armor, StatModType.Flat, 0.5f);
                            break;

                        case ArmorType.Cloak:
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.Defense, magical_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.Luck, magical_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.DodgeChance, magical_Small);
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, FiftyFiftyAttribute(Stats.Instance.GoldBonus, Stats.Instance.TreasureBonus), magical_Large, StatModType.Flat, 0.75f);
                            CreateNewStatModifier(StatModifierSource.EquippedCloak, newArmor, Stats.Instance.CritChance, magical_Large, StatModType.Flat, 0.5f);
                            break;

                        case ArmorType.Bracers:
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.Defense, magical_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.Strength, magical_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.CritDamage, magical_Large);
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, FiftyFiftyAttribute(Stats.Instance.WeaponDamageBonus, Stats.Instance.MagicDamageBonus), magical_Large, StatModType.PercentAdd, 0.75f);
                            CreateNewStatModifier(StatModifierSource.EquippedBracers, newArmor, Stats.Instance.XPBonus, magical_Small, StatModType.Flat, 0.5f);
                            break;

                        case ArmorType.Boots:
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.Defense, magical_Armor);
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.Agility, magical_coreStat);
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.MoveSpeed, magical_Small);
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, FiftyFiftyAttribute(Stats.Instance.PickupRadius, Stats.Instance.GoldBonus), magical_Large, StatModType.Flat, 0.75f);
                            CreateNewStatModifier(StatModifierSource.EquippedBoots, newArmor, Stats.Instance.DodgeChance, magical_Small, StatModType.Flat, 0.5f);
                            break;
                    }
                    break;
            }

            Inventory.Instance.Add(newArmor);

            //print("Armor Name: " + newArmor.Name + "/ ID: " + newArmor.ID + "/Icon: " + newArmor.icon.bg);
            //foreach (var attribute in newArmor.attributes)
            //    print("" + attribute.StatType + " " + attribute.Value);
        }

        public void CreateArmor(ItemQuality quality)
        {
            CreateArmor(quality, GetRandomArmorType());
        }
        #endregion




        #region CreateWeapon
        public Weapon CreateWeapon(ItemQuality quality, WeaponType type)
        {
           var newWeapon = new Weapon(quality, type);
           newWeapon.attributes = new List<StatModifier>();
           newWeapon.Name = GenerateWeaponName(type, quality);
            //GenerateItemID(newWeapon);
           newWeapon.ID = new Guid();
            //AssignWeaponAttackObject(newWeapon);

            switch (quality)
            {
                case ItemQuality.Common:

                    CreateNewStatModifier(newWeapon, Stats.Instance.WeaponDamage, common_WeaponDamage);
                    CreateNewStatModifier(newWeapon, Stats.Instance.WeaponDamageBonus, common_Large, StatModType.Flat, 0.10f);

                    switch (type)
                    {
                        case WeaponType.Sword:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Luck, common_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.CritChance, common_Small, StatModType.Flat, 0.5f);
                            break;

                        case WeaponType.Axe:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Strength, common_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.CritDamage, common_Large, StatModType.Flat, 0.5f);
                            break;

                        case WeaponType.Mace:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Constitution, common_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.LifeRegen, common_Small, StatModType.Flat, 0.5f);
                            break;

                        case WeaponType.Staff:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Intelligence, common_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.MagicDamageBonus, common_Large, StatModType.Flat, 0.5f);
                            break;

                        case WeaponType.Dagger:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Agility, common_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.DodgeChance, common_Small, StatModType.Flat, 0.5f);
                            break;
                    }
                    break;

                case ItemQuality.Rare:

                    CreateNewStatModifier(newWeapon, Stats.Instance.WeaponDamage, rare_WeaponDamage);
                    CreateNewStatModifier(newWeapon, Stats.Instance.WeaponDamageBonus, rare_Large, StatModType.Flat, 0.35f);

                    switch (type)
                    {
                        case WeaponType.Sword:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Luck, rare_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.CritChance, rare_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.MagicOnHit), rare_Small);
                            break;

                        case WeaponType.Axe:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Strength, rare_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.CritDamage, rare_Large);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.CritChance, Stats.Instance.MagicDamageBonus), rare_Small);
                            break;

                        case WeaponType.Mace:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Constitution, rare_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.LifeRegen, rare_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.MaxMagic, Stats.Instance.MaxHealth), rare_Small);
                            break;

                        case WeaponType.Staff:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Intelligence, rare_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.MagicDamageBonus, rare_Large);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.MagicOnHit, Stats.Instance.MagicRegen), rare_Small);
                            break;

                        case WeaponType.Dagger:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Agility, rare_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.DodgeChance, rare_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.CritChance), rare_Small);
                            break;
                    }
                    break;

                case ItemQuality.Magical:

                    CreateNewStatModifier(newWeapon, Stats.Instance.WeaponDamage, magical_WeaponDamage);
                    CreateNewStatModifier(newWeapon, Stats.Instance.WeaponDamageBonus, magical_Large, StatModType.Flat, 0.65f);

                    switch (type)
                    {
                        case WeaponType.Sword:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Luck, magical_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.CritChance, magical_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.MagicOnHit), magical_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Strength, Stats.Instance.Intelligence), magical_coreStat);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Agility, Stats.Instance.Constitution), magical_coreStat, StatModType.Flat, 0.5f);
                            break;

                        case WeaponType.Axe:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Strength, magical_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.CritDamage, magical_Large);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.CritChance, Stats.Instance.MagicDamageBonus), magical_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Luck, Stats.Instance.Agility), magical_coreStat);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Constitution, Stats.Instance.Intelligence), magical_coreStat, StatModType.Flat, 0.5f);
                            break;

                        case WeaponType.Mace:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Constitution, magical_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.LifeRegen, magical_coreStat);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.MaxMagic, Stats.Instance.MaxHealth), magical_Small, StatModType.PercentAdd, 1f);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Agility, Stats.Instance.Intelligence), magical_coreStat);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Strength, Stats.Instance.Luck), magical_coreStat, StatModType.Flat, 0.5f);
                            break;

                        case WeaponType.Staff:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Intelligence, magical_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.MagicDamageBonus, magical_Large);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.MagicOnHit, Stats.Instance.MagicRegen), magical_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Strength, Stats.Instance.Constitution), magical_coreStat);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Agility, Stats.Instance.Luck), magical_coreStat, StatModType.Flat, 0.5f);
                            break;

                        case WeaponType.Dagger:
                            CreateNewStatModifier(newWeapon, Stats.Instance.Agility, magical_coreStat);
                            CreateNewStatModifier(newWeapon, Stats.Instance.DodgeChance, magical_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.LifeOnHit, Stats.Instance.CritChance), magical_Small);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Luck, Stats.Instance.Intelligence), magical_coreStat);
                            CreateNewStatModifier(newWeapon, FiftyFiftyAttribute(Stats.Instance.Constitution, Stats.Instance.Strength), magical_coreStat, StatModType.Flat, 0.5f);
                            break;
                    }
                    break;
            }

            Inventory.Instance.Add(newWeapon);
            InventoryUI.Refresh();

            //print("Weapon Name: " + newWeapon.Name + "/ ID: " +newWeapon.ID + "/Icon: " +newWeapon.icon.bg);
            //foreach (var attribute in newWeapon.attributes)
            //    print("" + attribute.StatType + " " + attribute.Value);

            return newWeapon;
        }

        public void CreateWeapon(ItemQuality quality)
        {
            CreateWeapon(quality, GetRandomWeaponType());
        }

        #endregion


        //void AssignWeaponAttackObject(Weapon weapon)
        //{
        //    weapon.weaponAttackObject = Resources.Load<WeaponAttackObject>("MeleeWeapons/Sword_Slash-Wide-1");
        //}

        // CreateStatMod with 100% guaranteed attribute and a Flat StatModType
        public void CreateNewStatModifier(
            IEquippable newItem,
            CharacterStat characterStat,
            StatRange range)
        {

            var randomValue = RandomRoundedWithinRange(range);

            var newAttribute = new StatModifier(characterStat.type, StatModType.Flat, randomValue, StatModifierSource.EquippedWeapon);

            newItem.attributes.Add(newAttribute);

        }


        // CreateStatMod from a specified Source with 100% guaranteed attribute and a Flat StatModType
        public void CreateNewStatModifier(
            StatModifierSource source,
            IEquippable newItem,
            CharacterStat characterStat,
            StatRange range)
        {

            var randomValue = RandomRoundedWithinRange(range);

            var newAttribute = new StatModifier(characterStat.type, StatModType.Flat, randomValue, StatModifierSource.EquippedWeapon);

            newItem.attributes.Add(newAttribute);

        }

        // CreateStatMod with source of EquippedWeapon
        public void CreateNewStatModifier(
            IEquippable newItem,
            CharacterStat characterStat, 
            StatRange range,
            StatModType statModType, 
            float chance)
        {
            if (Random.value <= chance)
            {
                var randomValue = RandomRoundedWithinRange(range);

                var newAttribute = new StatModifier(characterStat.type, statModType, randomValue, StatModifierSource.EquippedWeapon);

                newItem.attributes.Add(newAttribute);
            }
        }

        // CreateStatMod with given source
        public void CreateNewStatModifier(
            StatModifierSource source,
            IEquippable newItem, 
            CharacterStat characterStat,
            StatRange range,
            StatModType statModType,
            float chance)
        {
            if (Random.value <= chance)
            {
                var randomValue = RandomRoundedWithinRange(range);

                var newAttribute = new StatModifier(characterStat.type, statModType, randomValue, source);

                newItem.attributes.Add(newAttribute);
            }
        }

        float RandomRoundedWithinRange(StatRange range)
        {
            float roundedValue = 0;

            var randomValue = Random.Range(range.min, range.max);

            if (randomValue < 1)
            {
                roundedValue = (float)Math.Round(randomValue, 2);
            }
            else
            {
                roundedValue = (float)Math.Round(randomValue, 0);
            }

            return roundedValue;
        }

        // TODO: Add word bank for item names
        string GenerateArmorName(ArmorType type, ItemQuality quality)
        {
            string armorName = $"{quality} {type}";
            return armorName;
        }

        string GenerateWeaponName(WeaponType type, ItemQuality quality)
        {
            string weaponName = $"{quality} {type}";
            return weaponName;
        }


        #region Helper Functions

        public WeaponType GetRandomWeaponType()
        {
            WeaponType type = (WeaponType)Random.Range(0, Enum.GetNames(typeof(WeaponType)).Length);
            //print("Rolled weapon type: " + type);
            return type;
        }

        public ArmorType GetRandomArmorType()
        {
            ArmorType type = (ArmorType)Random.Range(0, Enum.GetNames(typeof(ArmorType)).Length);
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


        //void GenerateItemID(IEquippable newItem)
        //{
        //    var id = Random.Range(1000, 9999);

        //    // Generate Item ID until it's unique compared to all existing items
        //    foreach (Item item in Inventory.Instance.inventoryItems)
        //        if (id == item.ID)
        //            GenerateItemID(newItem);

        //    newItem.ID = id;
        //}


        public ItemQuality RandomItemQuality()
        {
            var q = (ItemQuality)Random.Range(0, 2);
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
#endregion