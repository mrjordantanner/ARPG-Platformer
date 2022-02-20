using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class LootController : MonoBehaviour
    {
        // Have drop conditions based on Player Level, Difficulty Level, or both
        // Unique items don't start dropping until Difficulty Level 20 etc

        // Have loot tables be dynamically created based on conditions?


        /// <summary>
        /// 
        /// Procedural Item "Traits"
        /// Procedural Item Name Prefixes/Suffixes and Effects
        /// Weapon TRAITS affect the attributes that roll and the name the item is assigned
        /// Weapon TYPE affects Collider Shape/Size, Base Attack Speed, Base Weapon Damage range (and possibly character animation later on!), and a basic melee attack mechanic
        ///      Weapon Types - Sword - Medium range, Medium Attack Speed, Medium Damage range, Linear BoxCollider                         - ALLOWS AIR HOVER
        ///                   - Lance - Long range, Slow Attack Speed, High Damage range,  Elliptical Collider (like Slash-Ellipse now)    - ALLOWS BACKFLIP SLASH
        ///                   - Claw  - Short range, Fast Attack Speed, Low Damage range, Circle Collider                                  - ALLOWS COMBOS, KNOCKBACK -  Physics-emphasis
        /// 
        /// PREFIXES - Roll on Specific Item Types
        /// 
        /// Weapon: - 
        /// - "Swift" (+Attack Speed) 
        /// - "Icy" (+Chance to Freeze)  
        /// - "Bloodthirsty" (+Life Drain)
        /// - "Venomous" (+Chance to Poison)
        /// 
        /// 
        /// Armor 
        /// - "Iron" (+DEF%/-MoveSpeed)

        /// 
        /// 
        /// Cloak
        /// - "Tattered"   
        /// - "Bloodstained"
        /// - "Velvet" 
        /// 
        /// 
        /// Glove
        /// - "Stealthy" (+DOD)
        /// - "Blessed" (+CC%)
        /// - "Gilded" (+TrsBonus%)
        /// 
        /// 
        /// 
        /// Ring
        /// - "Fortunate" (+%TrsBonus)  
        /// - "Golden" (%GoldBonus)
        /// - "Sorcerous" (%Mag Dmg)
        /// 
        /// 
        /// SUFFIXES - Can Roll on all Item Types    Prefix + Item.Type + " of " + Suffix
        /// 
        /// "Bleeding" - +%ChanceToBleed
        /// "Strength", Constitution, Agility, Intelligence, Luck - +STR etc  Rolls bonus Main Stat
        /// "Wealth" - +%GoldBonus
        /// "Death" 
        /// 
        /// 



        // RESOURCE 
        // How to get more resource - HP, MP, STAM
        // What currently gives MP?  only MP regen from equipped items and any temporary buffs
        // Increase the Main Stat that gives regen of that resource - although keep "Max Gear" in mind - all those stats
        // should end up being balanced out in this situation, so "stacking INT" doesn't really happen
        // What about MP and/or STAM pickups?  
        //  OR
        // Alter how Health Pickups can work based on item attributes or Passive Skills
        //      - Health pickups grant half as much health, but also give Stamina
        //      - Health pickups grant half as much health, but also grant MP

        // chance for weapons to drop vs armor
        public float weaponDropChance = 16.66f;

        public GameObject ArmorObjectPrefab, WeaponObjectPrefab, GoldObjectPrefab;
        [HideInInspector]
        public float dropForceX, dropForceY;

        [Header("Drop Roll Weights")]
        public float baseWeightCommon;
        public float baseWeightRare, baseWeightMagical,
            weightSword,
            weightAxe,
            weightMace,
            weightStaff,
            weightDagger,
            weightHelm,
            weightMail,
            weightCloak,
            weightBracers,
            weightBoots;




        // Drop ranges
        [HideInInspector]
        public Vector2
            treasureChestSmall = new Vector2(1, 2),
            treasureChestLarge = new Vector2(2, 5),
            goldSmall = new Vector2(100, 500),
            goldMedium = new Vector2(500, 5000),
            goldLarge = new Vector2(5000, 100000);

        Item newItem;
        Vector2 dropForce;

        public static LootController Instance;
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


        private void Start()
        {
            InitializeLootTables();
        }

        LootTable qualityTable, weaponTable, armorTable;
        LootTableMember
            common,
            rare,
            magical,
            sword,
            axe,
            mace,
            staff,
            dagger,
            helm,
            mail,
            cloak,
            bracers,
            boots;
            

        public float RoundToNearestHalf(float number)
        {
            float f = number * 2;
            f = Mathf.RoundToInt(f);
            f = f / 2;
            return f;

        }



        void InitializeLootTables()
        {
            // Quality Loot Table
            common = new LootTableMember((int)Item.Quality.Common, baseWeightCommon);
            rare = new LootTableMember((int)Item.Quality.Rare, baseWeightRare);
            magical = new LootTableMember((int)Item.Quality.Magical, baseWeightMagical);
            //unique = new LootTableMember((int)Item.Quality.Unique, baseWeightUnique);

            List<LootTableMember> qualityList = new List<LootTableMember>();
            qualityList.Add(common);
            qualityList.Add(rare);
            qualityList.Add(magical);
            //qualityList.Add(unique);

            qualityTable = new LootTable(qualityList);

            // Type Loot Table
            sword = new LootTableMember((int)Item.Weapon.Type.Sword, weightSword);
            axe = new LootTableMember((int)Item.Weapon.Type.Axe, weightAxe);
            mace = new LootTableMember((int)Item.Weapon.Type.Mace, weightMace);
            staff = new LootTableMember((int)Item.Weapon.Type.Staff, weightStaff);
            dagger = new LootTableMember((int)Item.Weapon.Type.Dagger, weightDagger);

            helm = new LootTableMember((int)Item.Armor.Type.Helm, weightHelm);
            mail = new LootTableMember((int)Item.Armor.Type.Mail, weightMail);
            cloak = new LootTableMember((int)Item.Armor.Type.Cloak, weightCloak);
            bracers = new LootTableMember((int)Item.Armor.Type.Bracers, weightBracers);
            boots = new LootTableMember((int)Item.Armor.Type.Boots, weightBoots);

            List<LootTableMember> weaponList = new List<LootTableMember>();
            List<LootTableMember> armorList = new List<LootTableMember>();

            weaponList.Add(sword);
            weaponList.Add(axe);
            weaponList.Add(mace);
            weaponList.Add(staff);
            weaponList.Add(dagger);

            armorList.Add(helm);
            armorList.Add(mail);
            armorList.Add(cloak);
            armorList.Add(bracers);
            armorList.Add(boots);

            weaponTable = new LootTable(weaponList);
            armorTable = new LootTable(armorList);
        }



        #region Enemy Drop Calculator
        // CALCULATE ENEMY DROPS
        public void CalculateEnemyDrops(EnemyCharacter enemy)
        {
            // PLACEHOLDER ITEM DROP CODE
            var randomDrop = Random.value;
            if (randomDrop <= 0.10f)
                DropEquipment(treasureChestSmall, enemy.transform.position, true);

            var goldDrop = Random.value;
            if (goldDrop <= 0.10f)
                DropGold(goldSmall, enemy.transform.position, true);

            /*

            // TreasureBonus and Difficulty affect Quality
            var rareWeight = rare.probabilityWeight * (1+ Stats.Instance.TreasureBonus.Value + Difficulty.Instance.rareDropRate);
            var magicalWeight = magical.probabilityWeight * (1 + Stats.Instance.TreasureBonus.Value + Difficulty.Instance.magicalDropRate);
            var uniqueWeight = unique.probabilityWeight * (1 + Stats.Instance.TreasureBonus.Value + Difficulty.Instance.uniqueDropRate);

           // eDropBonus_Frequency = eDropBonus_Quantity = eDropBonus_Quality = 0;

            switch (enemy.tier)
            {
                case EnemyCharacter.Tier.Normal:
                    switch(enemy.category)
                    {
                        case EnemyCharacter.Category.Swarmer:
                            break;

                        case EnemyCharacter.Category.Ranged:
                            break;

                        case EnemyCharacter.Category.MeleeStrong:
                            break;
                    }
                    break;

                case EnemyCharacter.Tier.Elite:
                    switch (enemy.category)
                    {
                        case EnemyCharacter.Category.Swarmer:
                            break;

                        case EnemyCharacter.Category.Ranged:
                            break;

                        case EnemyCharacter.Category.MeleeStrong:
                            break;
                    }
                    break;
            }

            // var eDropBonus_Total = eDropBonus_Frequency + eDropBonus_Quantity + eDropBonus_Quality;

        */

            // Calc drops upon enemy death based on enemy tier, difficulty, and player treasure bonus?

            // 1) Calc equipment drops
            // Normal Tier enemy
            // Swarmer
            // Enemy tier and category increase number of equipment items that can drop
            // Diff, Treasure Bonus increase weights of Rare and Magical Item qualities

            //A) How often Items drop (Frequency) - equipmentDropBonus_Frequency (%)
            // Affected by Tier and Category (e.g. Normal Swarmer vs Elite Strong Melee)

            //B) Number of Items that drop   (Quantity) - equipmentDropBonus_Quantity %
            // Affected by Tier, Category, and Treasure Bonus

            //C) Weights of Items that drop  (Quality) - equipmentDropBonus_Quality %
            // Affected by Tier, Category, and Treasure Bonus, and Difficulty

            // OR MAYBE TIER, CATEGORY, TrBONUS AND DIFF    
            //      ALWAYS AFFECT    
            // FREQUENCY, QUANTITY, AND QUALITY    
            //   OF EQUIPMENT ITEM DROPS


            // Only Elites and Treasure Chests can drop Mats

            // 2) Calc any drops unique to this specific monster


            // Calculate Common, Rare, and Magical Equipment Item drops based on Difficulty and Enemy Tier (Trash vs Elite)
            // Calculate Gold drop based on Difficulty, Enemy Tier, and Player Treasure Bonus
            // Calculate Jewel (materials) drop based on Difficulty, Enemy Tier, Player Treasure Bonus, and Enemy Loot Table
            // What else affects drop rate?  
            // - Trash vs. Elite vs. "Super Elite" level monster
            // Or does each enemy type have its own loot table?
            //                  All enemies can drop all 5 main equipment types of all 3 qualities, same drop chance for each enemy (depending on Trash vs Elite though)
            //                  In addition, each enemy has a unique loot table for "Mats"
            //                  .e.g. "Tulip Spider" could drop any of 5 main equipment items plus an Onyx or Garnet, etc

            // Treasure Bonus affects drop rate how?    - drops more frequent, AND slightly better chance for higher quality
            //      - doesn't affect 


            // TODO:  Enemy Loot Types
            // Standard - Randomly spawned enemies that can drop loot from a very large and generic loot table
            // Special - Enemies that are not randomly spawned and always exist in the same place - 
            //         - have a chance to drop a unique item specific to that enemy only
        }
        #endregion

        // EQUIPMENT ROLL AND DROP
        public void DropEquipment(Vector2 itemRange, Vector2 position, bool dropMotion)
        {
            qualityTable.ValidateTable();
            weaponTable.ValidateTable();
            armorTable.ValidateTable();

            var itemsToDrop = (int)Random.Range(itemRange.x, itemRange.y);

            // Roll for Quality and Type
            for (int i = 0; i < itemsToDrop; i++)
            {
                var quality = qualityTable.PickLootTableMember().index;  // TODO: Make difficulty and TreasureBonus% affect this

                if (Random.value <= weaponDropChance)
                {
                    var type = weaponTable.PickLootTableMember().index;
                    DropWeaponItem((Item.Quality)quality, (Item.Weapon.Type)type, position, dropMotion);
                }
                else
                {
                    var type = armorTable.PickLootTableMember().index;
                    DropArmorItem((Item.Quality)quality, (Item.Armor.Type)type, position, dropMotion);
                }

            }
        }

        public void DropWeaponItem(Item.Quality quality, Item.Weapon.Type type, Vector2 position, bool dropMotion)
        {
            var newItemObject = Instantiate(WeaponObjectPrefab, position, Quaternion.identity).GetComponentInChildren<WeaponObject>();
            newItemObject.quality = quality;
            newItemObject.type = type;
            newItemObject.rb = newItemObject.GetComponentInParent<Rigidbody2D>();

            if (newItemObject.rb != null)
                newItemObject.rb.AddForce(CalculateDropForce(dropMotion));

        }


        public void DropArmorItem(Item.Quality quality, Item.Armor.Type type, Vector2 position, bool dropMotion)
        {
            var newItemObject = Instantiate(ArmorObjectPrefab, position, Quaternion.identity).GetComponentInChildren<ArmorObject>();
            newItemObject.quality = quality;
            newItemObject.type = type;
            newItemObject.rb = newItemObject.GetComponentInParent<Rigidbody2D>();

            if (newItemObject.rb != null)
                newItemObject.rb.AddForce(CalculateDropForce(dropMotion));
           
        }

        Vector2 CalculateDropForce(bool dropMotion)
        {
            Vector2 dropDir = Vector2.zero;

            if (Random.value < 0.50f)
                dropDir = Vector2.right;
            else
                dropDir = -Vector2.right;

            Vector2 dropVelocity = new Vector2(Random.Range(500, dropForceX) * dropDir.x, dropForceY);

            if (dropMotion) dropForce = new Vector2(dropForceX, dropForceY);
            else
                dropForce = Vector2.zero;

            return dropForce;
        }


        public void DropGold(Vector2 goldRange, Vector2 position, bool dropMotion)
        {
            // TODO:  To be dropped as objects that use the PickupRadius pointeffector

            var newGoldObject = Instantiate(GoldObjectPrefab, position, Quaternion.identity);
            var collectible = newGoldObject.GetComponentInChildren<Collectible>();
            //var rb = newGoldObject.gameObject.AddComponent<Rigidbody2D>();

            var goldAmount = Mathf.RoundToInt((Random.Range(goldRange.x, goldRange.y)) * (1 + Stats.Instance.GoldBonus.Value + Difficulty.Instance.goldBonus));

            if (collectible != null)
            {
                collectible.value = goldAmount;
                collectible.destroyOnPickup = true;
                collectible.usePhysics = true;
            }

            // if (rb != null && dropMotion)
            //     rb.AddForce(dropForce);

            // print("Dropped " + goldAmount + " gold.");

        }






    }


}