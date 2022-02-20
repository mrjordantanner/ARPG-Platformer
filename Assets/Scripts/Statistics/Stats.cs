using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{

    public enum Resource { Health, Magic, Stamina }

    /// <summary>
    /// Controls and stores all Player Statistics
    /// </summary>
    public class Stats : Statistics, ITargetable
    {
        public bool IsPlayer { get { return true; } }
        public GameObject Entity { get { return PlayerRef.Player.gameObject; } }
        public StatusEffectUI EffectUI { get { return HUD.Instance.statusEffectUI; } }

        /*   Example of exposing getter/setter properties in Inspector
        [SerializeField]
        private int _SomeProperty;

        public int SomeProperty
        {
            get { return _SomeProperty; }
            set { }
        }
        */

        // TODO: Break these two into separate class - PlayerHealth?
        public void GainHP(float amount)
        {
            currentHP += amount;
            if (currentHP > MaxHealth.Value)
                currentHP = MaxHealth.Value;

            LowHealthTest();
        }

        public void TakeDamage(float damage, bool ignoreInvulnerability)
        {
            if (invulnerable) return;

            if (player != null)
            {
                AudioManager.Instance.Play("Hurt");

                currentHP -= (int)(damage);
                if (!ignoreInvulnerability)
                {
                    player.invulnerable = true;
                    Combat.Instance.playerInvulnerabilityTimer = true;
                }

                player.StartFlickering();
                StartCoroutine(player.Hitflash(0.2f));

                LowHealthTest();

                if (currentHP <= 0 && !player.dead)
                    StartCoroutine(Death());
            }
        }



        // //////////////////////////////////////////////
        // Core stats
        public CharacterStat Strength, Constitution, Agility, Intelligence, Luck;

        // CharacterStats
        public CharacterStat
            WeaponDamage,           // base value rolled for on hit with MeleeAttack between the WeaponDamageMin/Max values on the Item attribute (Stat Modifier)       
            MagicDamage,            // base value rolled for on hit with Magic Attack between the MagicDamageMin/Max values in the Skill script
            WeaponDamageBonus,
            MagicDamageBonus,
            MaxMagic,               // Magic point capacity 
            // Stamina,             // Used for mobility skills - has Charges / Cooldown functionality
            CritChance,             // Chance on hit (either Melee or Magic) to do CritChance% more damage
            CritDamage,             // Damage is multiplied by this %factor on CriticalHit
            LifeRegen,              // Amount of health points automatically gained over time
            MagicRegen,             // Amount of magic points automatically gained over time
            CooldownReduction,      // Time for skills to become available again after use are reduced by this amount%
            LifeOnHit,              //
            MagicOnHit,
            PickupRadius,
            DodgeChance,
            TreasureBonus,
            GoldBonus,
            XPBonus;

        // Core StatModifiers
        [HideInInspector]
        public StatModifier 
            strength_StatPoints, 
            constitution_StatPoints, 
            agility_StatPoints, 
            intelligence_StatPoints, 
            luck_StatPoints;

        // StatModifiers
        StatModifier
             weaponDamageBonusModifier_Strength,
             maxHealthBonusModifier_Constitution,
             maxMagicBonusModifier_Intelligence,
             magicDamageBonusModifier_Intelligence,
             magicRegenBonusModifier_Intelligence,
             lifeRegenBonusModifier_Constitution,

             critDamageModifier_Strength,
             defenseModifier_Constitution,
             moveSpeedModifier_Agility,
             dodgeChanceModifier_Agility,
             critChanceModifier_Luck,
             goldModifier_Luck,
             pickupRadiusModifier_Luck;


        // Set default weapon traits 
        // Instead of being read from Weapon.cs, each Item will actually have these CharacterStats and StatMods on them

        // what determines Weapon and Spell accuracy?  Should it be slightly randomized?  Or hard set for each Spell and Weapon type?

        // attackDelay and attackReset will really just be calculated behind the scenes based on attackSpeed
        // e.g. attackDelay = attackSpeed * 0.01f;    delay is 1% of the attackSpeed value, and reset is 10% of the attackSpeed value
        // attackReset = attackSpeed * 0.1f;

        // If Weapon Damage is the Max Damage a hit will cause, 
        // weaponAccuracy indicates the damage range BELOW that number that could be rolled
        // e.g. Weapon Damage 1000, WeaponAccuracy 90% = Max Damage 1000 / Min Damage 900


        [Header("Base Stats")]
        public bool invulnerable;
        public bool resourceCostsRemoved;
        public int baseMaxHealth = 120;
        public int baseMaxMagic = 30;
        public float
            baseMoveSpeed = 3,
            baseDefense = 0.01f,
            baseDodgeChance = 0.01f,
            baseCritChance = 0.005f,
            baseCritDamage = 0.05f,
            baseLifeRegen = 0.01f,
            baseMagicRegen = 0.01f,
            basePickupRadius = 3.5f;

        public float hpRegenRate = 0.25f;
        public float mpRegenRate = 0.25f;

        // HEALTH
        public float potionHealPercentage = 0.6f;
        public float potionBaseCooldown = 30f;
        public float healthPickupPercentage = 0.10f;
        bool lowHealth;
        float lowHealthPercent = 0.25f;
        float healthAlertInterval = 1.5f;
        float healthAlertTimer;
        float hpRegenTimer;

        public float respawnDuration = 3f;
        public float deathDuration = 6f;

        [HideInInspector] public bool potionReady;
        [HideInInspector] public bool regen;

        //[ReadOnly] public float averageMagicDamage_skillA;
        //[ReadOnly] public float averageMagicDamage_skillB;

        // LEVEL & EXPERIENCE
        float baseNextLevelXP = 500;
        public float nextLevelXPIncrease = 1.85f;
        [ReadOnly] public int playerLevel;
        [ReadOnly] public float XP;
        [ReadOnly] public float lastLevelXP;
        [ReadOnly] public float nextLevelXP;
        [ReadOnly] public float nextXPRemaining;
        [ReadOnly] public float xpProgressPercentage;


        public float currentMP;


        // found in the world, allocate into Skill Tree to unlock and upgrade abilities
        public int skillPoints;
        // awarded on leveling up, allocate into Main Stats to improve character
        public int statPoints;            

        // STAMINA
        //public int maxStaminaCharges;
        //public int currentStaminaCharges;
        //public float staminaBaseCooldownDuration;
        //public float staminaCooldownDuration;
        //float staminaCooldownTimer;

        // STAT POINT Increments
        // how many are awarded on LevelUp?  This could be adjusted in 
        // higher player lv/difficulties, or affected by other things
        public int statPointsOnLevelUp = 5;  
        // STR
        public float weaponDamageBonusIncrement = 0.01f;
        public float critDamageIncrement = 0.05f;
        // CON
        public float maxHealthBonusIncrement = 0.01f;
        public float defenseIncrement = 0.005f;
        public float lifeRegenIncrement = 1;
        // AGL
        public float moveSpeedIncrement = 0.02f;
        public float dodgeChanceIncrement = 0.005f;
        // INT
        public float maxMagicBonusIncrement = 0.01f;
        public float magicDamageBonusIncrement = 0.01f;
        public float magicRegenIncrement = 1;
        // LCK
        public float pickupRadiusIncrement = 0.25f;
        public float critChanceIncrement = 0.005f;
        public float goldIncrement = 0.025f;

        // SKILL POINT Increments
        public float superJumpIncrement = 0.05f;       // per skill point invested

        [HideInInspector]
        public float mpPercentage;
        float mpRegenTimer;
        public int gold;
        // public int jewels;
        // public int difficultyKeys;          // used to permanently unlock higher diff. levels


        [Header("Misc")]
        public int kills;
        public float killsPerMinute;  // round these to 2 decimal places
        public float xpPerMinute;
        public int deaths;
        public Vector3 levelPortalLocation;


 


        public static float clock;
        public bool clockRunning;
        float fastestTime;
        public float timeThisRun;
        public static float timePreviousRuns;

        float pickupTimer;
        double startTime;
        bool healthAlert;

        [HideInInspector]
        public PlayerCharacter player;
        [HideInInspector]
        public StatsGUI statsGUI;
        public static Stats Instance;

        // Core Stats from Base and from StatPoints invested
        [Header("Core")]
        public float baseStrength = 5;
        public float
            //strength_StatPoints,
            baseConstitution = 5,
            // constitution_StatPoints,
            baseAgility = 5,
            //agility_StatPoints,
            baseIntelligence = 5,
            //intelligence_StatPoints,
            baseLuck = 5;
        //luck_StatPoints;

        // These represent the total Core Stats
        // Core Stats from Base and from StatPoints invested
        // [Header("Core Stats Total")]
        // [ReadOnly]
        //  public float Strength;
        //  [ReadOnly] public float Constitution, Agility, Intelligence, Luck;



        public override void Awake()
        {
            base.Awake();

            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            statsGUI = GetComponent<StatsGUI>();
        }

        void Start()
        {
            clock = 0;
            clockRunning = true;

            player = PlayerRef.Player;

            Time.timeScale = 1f;
            playerLevel = 1;
            CalculateNextLevelXP();

            hpRegenTimer = hpRegenRate;
            mpRegenTimer = mpRegenRate;

            InitializePlayerStats();
            CalculateCoreStats();

            currentHP = MaxHealth.Value;
            currentMP = MaxMagic.Value;

        }

        void Update()
        {
            if (!player.dead)
            {
                HPRegen();
                MPRegen();
            }

            // if (Input.GetKeyDown(KeyCode.R))
            //ReloadScene();

            if (Input.GetKey(KeyCode.Escape))
                Quit();

            pickupTimer += Time.deltaTime;
            UpdateClock();

            killsPerMinute = (float)Math.Round(kills / (Time.time / 60f), 2);
            xpPerMinute = (float)Math.Round(XP / (Time.time / 60f), 2);
        }


        void InitializePlayerStats()
        {
            // CORE STATS
            Strength = new CharacterStat(CharacterStat.Type.Strength, baseStrength);
            strength_StatPoints = new StatModifier(CharacterStat.Type.Strength, StatModType.Flat, StatModifier.Source.StatPoints);

            Constitution = new CharacterStat(CharacterStat.Type.Constitution, baseConstitution);
            constitution_StatPoints = new StatModifier(CharacterStat.Type.Constitution, StatModType.Flat, StatModifier.Source.StatPoints);

            Agility = new CharacterStat(CharacterStat.Type.Agility, baseAgility);
            agility_StatPoints = new StatModifier(CharacterStat.Type.Agility, StatModType.Flat, StatModifier.Source.StatPoints);

            Intelligence = new CharacterStat(CharacterStat.Type.Intelligence, baseIntelligence);
            intelligence_StatPoints = new StatModifier(CharacterStat.Type.Intelligence, StatModType.Flat, StatModifier.Source.StatPoints);

            Luck = new CharacterStat(CharacterStat.Type.Luck, baseLuck);
            luck_StatPoints = new StatModifier(CharacterStat.Type.Luck, StatModType.Flat, StatModifier.Source.StatPoints);

            CharacterStats.Add(Strength);
            CharacterStats.Add(Constitution);
            CharacterStats.Add(Agility);
            CharacterStats.Add(Intelligence);
            CharacterStats.Add(Luck);

            Strength.AddModifier(strength_StatPoints);
            Constitution.AddModifier(constitution_StatPoints);
            Agility.AddModifier(agility_StatPoints);
            Intelligence.AddModifier(intelligence_StatPoints);
            Luck.AddModifier(luck_StatPoints);


            // CHARACTER STATS
            WeaponDamage = new CharacterStat(CharacterStat.Type.WeaponDamage, 0);                  // Base value dictated by equipped weapon -    // Equipping a weapon creates a Flat StatModifier that adds the base Weapon Damage - 
            MagicDamage = new CharacterStat(CharacterStat.Type.MagicDamage, 0);
            WeaponDamageBonus = new CharacterStat(CharacterStat.Type.WeaponDamageBonus, 0);
            MagicDamageBonus = new CharacterStat(CharacterStat.Type.MagicDamageBonus, 0);
            MaxHealth = new CharacterStat(CharacterStat.Type.MaxHealth, baseMaxHealth);
            MaxMagic = new CharacterStat(CharacterStat.Type.MaxMagic, baseMaxMagic);
            //  Stamina = new CharacterStat(CharacterStat.Type.Stamina, 3.0f);
            MoveSpeed = new CharacterStat(CharacterStat.Type.MoveSpeed, baseMoveSpeed);
            Defense = new CharacterStat(CharacterStat.Type.Defense, baseDefense);                  // % Damage absorbed when hit
            DodgeChance = new CharacterStat(CharacterStat.Type.DodgeChance, baseDodgeChance);      // % chance to avoid damage
            CritChance = new CharacterStat(CharacterStat.Type.CritChance, baseCritChance);
            CritDamage = new CharacterStat(CharacterStat.Type.CritDamage, baseCritDamage);
            LifeRegen = new CharacterStat(CharacterStat.Type.LifeRegen, baseLifeRegen);            // Life and
            MagicRegen = new CharacterStat(CharacterStat.Type.MagicRegen, baseMagicRegen);         // Magic "per second"
           // CooldownReduction = new CharacterStat(CharacterStat.Type.CooldownReduction, 0);       // All skill cooldowns -%Time
            LifeOnHit = new CharacterStat(CharacterStat.Type.LifeOnHit, 0);                        // HP given per weapon hit
            MagicOnHit = new CharacterStat(CharacterStat.Type.MagicOnHit, 0);                      // MP given per weapon hit
            TreasureBonus = new CharacterStat(CharacterStat.Type.TreasureBonus, 0.0f);             // Improves item drop rate and quality
            GoldBonus = new CharacterStat(CharacterStat.Type.GoldBonus, 0.0f);                     // %Gold drop increase
            XPBonus = new CharacterStat(CharacterStat.Type.XPBonus, 0.0f);                         // %Bonus to XP earned from all sources
            PickupRadius = new CharacterStat(CharacterStat.Type.PickupRadius, basePickupRadius);   // Pickup radius for gold and health pickups

            CharacterStats.Add(WeaponDamage);
            CharacterStats.Add(MagicDamage);
            CharacterStats.Add(WeaponDamageBonus);
            CharacterStats.Add(MagicDamageBonus);
            CharacterStats.Add(MaxHealth);
            CharacterStats.Add(MaxMagic);
            // CharacterStats.Add(Stamina);
            CharacterStats.Add(MoveSpeed);
            CharacterStats.Add(Defense);
            CharacterStats.Add(DodgeChance);
            CharacterStats.Add(CritChance);
            CharacterStats.Add(CritDamage);
            CharacterStats.Add(LifeRegen);
            CharacterStats.Add(MagicRegen);
            CharacterStats.Add(CooldownReduction);
            CharacterStats.Add(LifeOnHit);
            CharacterStats.Add(MagicOnHit);
            CharacterStats.Add(TreasureBonus);
            CharacterStats.Add(GoldBonus);
            CharacterStats.Add(XPBonus);
            CharacterStats.Add(PickupRadius);

            // STAT MODIFIERS
            // Stat Modifiers from CoreStats
            // use PercentAdd for modifiers that are acting as % bonuses of an existing flat stat - have BONUS in the name
            weaponDamageBonusModifier_Strength = new StatModifier(CharacterStat.Type.WeaponDamageBonus, StatModType.Flat, StatModifier.Source.CoreStat);
            maxHealthBonusModifier_Constitution = new StatModifier(CharacterStat.Type.MaxHealth, StatModType.PercentAdd, StatModifier.Source.CoreStat);
            maxMagicBonusModifier_Intelligence = new StatModifier(CharacterStat.Type.MaxMagic, StatModType.PercentAdd, StatModifier.Source.CoreStat);
            magicDamageBonusModifier_Intelligence = new StatModifier(CharacterStat.Type.MagicDamageBonus, StatModType.Flat, StatModifier.Source.CoreStat);
            magicRegenBonusModifier_Intelligence = new StatModifier(CharacterStat.Type.MagicRegen, StatModType.Flat, StatModifier.Source.CoreStat);
            lifeRegenBonusModifier_Constitution = new StatModifier(CharacterStat.Type.LifeRegen, StatModType.Flat, StatModifier.Source.CoreStat);

            // use Flat for modifiers that are simply added to, even if they're acting as bonuses
            critDamageModifier_Strength = new StatModifier(CharacterStat.Type.CritDamage, StatModType.Flat, StatModifier.Source.CoreStat);
            defenseModifier_Constitution = new StatModifier(CharacterStat.Type.Defense, StatModType.Flat, StatModifier.Source.CoreStat);
            moveSpeedModifier_Agility = new StatModifier(CharacterStat.Type.MoveSpeed, StatModType.Flat, StatModifier.Source.CoreStat);
            dodgeChanceModifier_Agility = new StatModifier(CharacterStat.Type.DodgeChance, StatModType.Flat, StatModifier.Source.CoreStat);
            critChanceModifier_Luck = new StatModifier(CharacterStat.Type.CritChance, StatModType.Flat, StatModifier.Source.CoreStat);
            pickupRadiusModifier_Luck = new StatModifier(CharacterStat.Type.PickupRadius, StatModType.Flat, StatModifier.Source.CoreStat);
            goldModifier_Luck = new StatModifier(CharacterStat.Type.GoldBonus, StatModType.Flat, StatModifier.Source.CoreStat);

            // Add modifiers from CoreStats
            WeaponDamageBonus.AddModifier(weaponDamageBonusModifier_Strength);
            CritDamage.AddModifier(critDamageModifier_Strength);

            MaxHealth.AddModifier(maxHealthBonusModifier_Constitution);
            LifeRegen.AddModifier(lifeRegenBonusModifier_Constitution);
            Defense.AddModifier(defenseModifier_Constitution);

            MoveSpeed.AddModifier(moveSpeedModifier_Agility);
            DodgeChance.AddModifier(dodgeChanceModifier_Agility);

            MagicDamageBonus.AddModifier(magicDamageBonusModifier_Intelligence);
            MaxMagic.AddModifier(maxMagicBonusModifier_Intelligence);
            MagicRegen.AddModifier(magicRegenBonusModifier_Intelligence);

            GoldBonus.AddModifier(goldModifier_Luck);
            CritChance.AddModifier(critChanceModifier_Luck);
            PickupRadius.AddModifier(pickupRadiusModifier_Luck);

            // StatModifier Bonus% stats will come from :
            //      Equipped Items (stat attributes actually on the items)
            //      Buffs granted by Unique Items(like stat stacks on weapon hit, etc)
            //      Temporary Debuffs from Enemies (like lowered DEF%, etc)
            // Create the individual modifiers there and let the CharacterStat do the math

            // Equip starting weapon
            ItemCreator.Instance.CreateWeapon(Item.Quality.Common, ItemCreator.Instance.GetRandomWeaponType());
            Inventory.Instance.EquipItem(Inventory.Instance.inventoryItems.Last());

            // Other Stats
            currentHP = MaxHealth.Value;
            currentMP = MaxMagic.Value;
            player.pickupRadius = PickupRadius.Value;
            player.pickupRadiusCollider.radius = player.pickupRadius;

            //staminaCooldownDuration = CalculateCooldown(staminaBaseCooldownDuration);
            //currentStaminaCharges = maxStaminaCharges;

            statsGUI.UpdateStatsGUI();
            HUD.Instance.UpdateUI();
        }



        public void AddStatsFromItem(Item item)
        {
            // For every Attribute on the Item being equipped,
            foreach (var statMod in item.itemAttributes)
            {
                // Look through the master list of Player's CharacterStats
                foreach (var stat in CharacterStats)
                    if (statMod.StatType == stat.type)
                        stat.AddModifier(statMod);
            }


            statsGUI.UpdateStatsGUI();
        }

        /*
        public void RemoveStatModifiers(Item item)
        {
            tempStat = null;
            foreach (var statMod in item.itemAttributes)
            {
                foreach (var stat in Instance.characterStats)
                {
                    if (statMod.StatType == stat.type)
                    {
                        tempStat = stat;
                        Instance.tempStat.RemoveModifier(statMod);
                        tempStat = null;
                    }
                }
            }

            statsGUI.UpdateStatsGUI();

        }
        */

        public void RemoveStatModifiersFromSource(Item item, StatModifier.Source source)
        {
            // For every StatModifier on the item we're unequipping,
            foreach (var statMod in item.itemAttributes)
            {
                // Search through player's master list of CharacterStats
                foreach (var stat in CharacterStats)
                {
                    // If stat type and source match the stat on the item we're unequipping, remove the stat
                    if (statMod.StatType == stat.type && statMod.source == source)
                        stat.RemoveModifier(statMod);
                }
            }

            statsGUI.UpdateStatsGUI();

        }



        public void CalculateCoreModifier(CharacterStat characterStat, StatModifier statMod, CharacterStat coreStat, float increment)
        {
            statMod.Value = (coreStat.Value * increment);
            characterStat.isDirty = true;
        }



        // Called when items are equipped/unequipped and when Stat Points are invested/changed/reset
        // Calculates Stat bonuses not granted from items
        public void CalculateCoreStats()
        {
            // STR 
            CalculateCoreModifier(WeaponDamage, weaponDamageBonusModifier_Strength, Strength, weaponDamageBonusIncrement);
            CalculateCoreModifier(CritDamage, critDamageModifier_Strength, Strength, critDamageIncrement);

            // CON
            CalculateCoreModifier(MaxHealth, maxHealthBonusModifier_Constitution, Constitution, maxHealthBonusIncrement);
            CalculateCoreModifier(Defense, defenseModifier_Constitution, Constitution, defenseIncrement);
            CalculateCoreModifier(LifeRegen, lifeRegenBonusModifier_Constitution, Constitution, lifeRegenIncrement);

            // AGL
            CalculateCoreModifier(MoveSpeed, moveSpeedModifier_Agility, Agility, moveSpeedIncrement);
            CalculateCoreModifier(DodgeChance, dodgeChanceModifier_Agility, Agility, dodgeChanceIncrement);

            // INT
            CalculateCoreModifier(MaxMagic, maxMagicBonusModifier_Intelligence, Intelligence, maxMagicBonusIncrement);
            CalculateCoreModifier(MagicDamage, magicDamageBonusModifier_Intelligence, Intelligence, magicDamageBonusIncrement);
            CalculateCoreModifier(MagicRegen, magicRegenBonusModifier_Intelligence, Intelligence, magicRegenIncrement);

            // LCK
            CalculateCoreModifier(GoldBonus, goldModifier_Luck, Luck, goldIncrement);
            CalculateCoreModifier(CritChance, critChanceModifier_Luck, Luck, critChanceIncrement);
            CalculateCoreModifier(PickupRadius, pickupRadiusModifier_Luck, Luck, pickupRadiusIncrement);
            // TreasureBonus?

            // Stamina
            //staminaCooldownDuration = CalculateCooldown(staminaBaseCooldownDuration);  

            statsGUI.UpdateStatsGUI();
        }


        public float EstimateWeaponDamage()
        {
            return WeaponDamage.Value * (1 + (CritChance.Value * CritDamage.Value * (1 + WeaponDamageBonus.Value)));
        }


        public void AllocateStatPoint(CharacterStat coreStat)
        {
            foreach (var statMod in coreStat.statModifiers)
                if (statMod.source == StatModifier.Source.StatPoints)
                {
                    if (statPoints > 0)
                    {
                        statPoints--;
                        statMod.Value++;
                        coreStat.isDirty = true;
                    }
                }

            CalculateCoreStats();
        }

        public void DeallocateStatPoint(CharacterStat coreStat)
        {
            foreach (var statMod in coreStat.statModifiers)
                if (statMod.source == StatModifier.Source.StatPoints)
                {
                    if (statMod.Value >= 1)
                    {
                        statPoints++;
                        statMod.Value--;
                        coreStat.isDirty = true;
                    }
                }

            CalculateCoreStats();
        }

        // Reset the stat points of all core stats
        public void ResetStatPoints()
        {
            foreach (var characterStat in CharacterStats)
                foreach (var statMod in characterStat.statModifiers)
                    if (statMod.source == StatModifier.Source.StatPoints)
                    {
                        statPoints += (int)statMod.Value;
                        statMod.Value = 0;
                        characterStat.isDirty = true;
                    }

            CalculateCoreStats();
        }


        // Reset the stat points of a particular core stat
        public void ResetStatPoints(CharacterStat coreStat)
        {
            foreach (var statMod in coreStat.statModifiers)
                if (statMod.source == StatModifier.Source.StatPoints)
                {
                    statPoints += (int)statMod.Value;
                    statMod.Value = 0;
                    coreStat.isDirty = true;
                }

            CalculateCoreStats();
        }



        // STAMINA========================

        //public void CalculateStamina()
        //{
        //    if (currentStaminaCharges == maxStaminaCharges) return;

        //    staminaCooldownTimer -= Time.deltaTime;
        //    //UpdateStaminaCooldown UI element



        //    if (staminaCooldownTimer <= 0)
        //    {
        //        GainStaminaCharge(1);
        //        staminaCooldownTimer = staminaCooldownDuration;
        //    }

        //}

        //public void GainStaminaCharge(int number)
        //{
        //    currentStaminaCharges += number;
        //    if (currentStaminaCharges >= maxStaminaCharges) currentStaminaCharges = maxStaminaCharges;

        //    HUD.Instance.staminaCooldownRadial.fillAmount = 0;

        //}

        //public void LoseStaminaCharge(int number)
        //{
        //    currentStaminaCharges -= number;
        //    if (currentStaminaCharges <= 0) currentStaminaCharges = 0;

        //    HUD.Instance.staminaCooldownRadial.fillAmount = 1;
        //}




        //  E X P E R I E N C E   &   L E V E L
        // XP total experience points earned
        // nextLevelXP   total experience points required to reach next level
        // nextXPRemaining      how many experience points to go ...  nextLevelXP - XP
        // xpProgressPercentage  -   nextXP expressed as percentage - use for XP bar

        public void GainXP(float amount)
        {
            XP += Mathf.RoundToInt(amount * (1 + XPBonus.Value));

            // Check for level up
            if (XP >= nextLevelXP)
            {
                var rolloverXP = XP - nextLevelXP;
                XP -= rolloverXP;

                CalculateNextLevelXP();
                LevelUp();
                GainXP(rolloverXP);
            }

            nextXPRemaining = nextLevelXP - XP;
            CalculateXPProgress();
            statsGUI.UpdateStatsGUI();
        }

        void CalculateNextLevelXP()
        {
            lastLevelXP = nextLevelXP;
            nextLevelXP = Mathf.RoundToInt(baseNextLevelXP * Mathf.Pow(nextLevelXPIncrease, playerLevel));
            if (playerLevel == 1)
            {
                nextLevelXP = baseNextLevelXP;
                lastLevelXP = 0;
            }
        }

        void CalculateXPProgress()
        {
            xpProgressPercentage = ((XP - lastLevelXP) / (nextLevelXP - lastLevelXP)) * 100;
        }

        public void LevelUp()
        {
            playerLevel++;
            statPoints += statPointsOnLevelUp;
            statsGUI.UpdateStatsGUI();
            AudioManager.Instance.Play("Level Up");
            //StartCoroutine(SlowMotion(0.1f, 1.5f));
            Combat.Instance.CreateFloatingText("LEVEL UP", 42, Color.cyan, 3f, player.gameObject, 1f);
        }

        //public void CalculateLevelProgress(int difficultyLevel)
        //{
        //    levelProgressGoal = Mathf.RoundToInt(baseLevelProgressGoal * Mathf.Pow(Difficulty.Instance.levelProgressIncrease, difficultyLevel));
        //}

        // Reset stats on quit
        private void OnApplicationQuit()
        {
            Inventory.Instance.UnequipAllItems();
            //CalculateStats();
            InitializePlayerStats();

        }


        // COLLECTIBLES
        public void PickupCollectible(Collectible collectible)
        {
            switch (collectible.type)
            {
                case Collectible.Type.Health:
                    var healthPickupValue = Mathf.RoundToInt(MaxHealth.Value * healthPickupPercentage);
                    if (currentHP != MaxHealth.Value)
                    {
                        PickupHealth(player.pickupDelay, healthPickupValue);
                        Combat.Instance.CreateFloatingText("+" + Mathf.RoundToInt(healthPickupValue) + " Health", 6, Color.green, 1.5f, player.gameObject, 1.5f);
                    }
                    break;

                case Collectible.Type.Gold:
                    GainGold(collectible.value);
                    Combat.Instance.CreateFloatingText("+" + collectible.value + " Gold", 6, Color.yellow, 1.5f, player.gameObject, 1.5f);
                    break;

                case Collectible.Type.CollectibleA:
                    //AudioManager.Instance.Play("CollectibleA");
                    //collectibleA++;
                    //GainLevelProgress(collectible.value, true);
                    break;

                case Collectible.Type.CollectibleB:
                    //AudioManager.Instance.Play("CollectibleB");
                    //collectibleB++;
                    break;

                case Collectible.Type.CollectibleC:
                    //AudioManager.Instance.Play("CollectibleC");
                    //  collectibleC++;
                    break;

                case Collectible.Type.CollectibleD:
                    //AudioManager.Instance.Play("CollectibleD");
                    // collectibleD++;
                    break;

                default:
                    break;
            }

            statsGUI.UpdateStatsGUI();
        }


        public void GainGold(int amount)
        {
            gold += amount;
            statsGUI.UpdateStatsGUI();
            // AudioManager.Play("Gain Currency");

        }

        public void LoseGold(int amount)
        {
            gold -= amount;
            statsGUI.UpdateStatsGUI();
            // AudioManager.Play("Lose Currency");
        }








        //float currentLevelProgress;
        //[HideInInspector]
        //public float levelProgressGoal;

        //// LEVEL PROGRESS
        //public void GainLevelProgress(float xp, bool fromGlobe)
        //{
        //    var levelProgressGained = xp * Random.Range(0.10f, 0.20f);
        //    if (fromGlobe) levelProgressGained = xp * Random.Range(0.30f, 0.50f);

        //    currentLevelProgress += levelProgressGained;

        //    // Check for level complete
        //    if (currentLevelProgress >= levelProgressGoal)
        //    {
        //        //LevelComplete();
        //    }

        //    if (playerLevel == 1)
        //    {
        //        nextLevelXP = baseNextLevelXP;
        //        lastLevelXP = 0;
        //    }

        //    levelProgressPercentage = ((XP - lastLevelXP) / (nextLevelXP - lastLevelXP)) * 100;
        //}











        //  H E A L T H
        void HPRegen()
        {
            if (!player.dead)
            {
                hpRegenTimer -= Time.deltaTime;
                if (hpRegenTimer <= 0)
                {
                    // Regen gives a percentage of the max stat per regenrate amt of time
                    GainHP(LifeRegen.Value * MaxHealth.Value * hpRegenRate);
                    hpRegenTimer = hpRegenRate;
                }
            }
        }

        void LowHealthAlert()
        {
            if (healthAlertTimer < healthAlertInterval)
                healthAlertTimer += Time.deltaTime;

            if (healthAlertTimer >= healthAlertInterval)
            {
                AudioManager.Instance.Play("Low Health Alert");
                healthAlertTimer = 0;
            }
        }

        public void PickupHealth(float delay, int healthPickupValue)
        {
            AudioManager.Instance.Play("Health Pickup");
            GainHP(healthPickupValue);
        }



        void LowHealthTest()
        {
            // Low health test
            float curHP = currentHP;
            float mHP = MaxHealth.Value;
            var injuryPercent = curHP / mHP;

            if (injuryPercent <= lowHealthPercent)
                lowHealth = true;
            else
                lowHealth = false;

            if (lowHealth && !player.dead)
            {
                // damageTint.SetActive(true);
                HUD.Instance.healthBarFill.color = HUD.Instance.damaged;
                LowHealthAlert();
            }
            else
                HUD.Instance.healthBarFill.color = HUD.Instance.healthy;
               // damageTint.SetActive(false);
        }

        // Player death
        IEnumerator Death()
        {
            player.anim.SetBool("Dead", true);
            player.velocity.x = 0;
            player.velocity.y = 0;
            player.dead = true;
            player.canMove = false;
            player.canDash = false;
            player.canAttack = false;
            player.inputSuspended = true;
            AudioManager.Instance.Play("Player Death");
            Instantiate(player.DeathExplosion, transform.position, Quaternion.identity);
            StartCoroutine(HUD.Instance.ShowMessageCenter(("YOU HAVE DIED."), Color.white, 100, deathDuration));
            HUD.Instance.EffectsFade(HUD.Instance.effectsVolume_Normal, HUD.Instance.effectsVolume_Death, 0.25f);
            currentHP = 0;
            regen = false;
            deaths++;

            foreach (var effector in player.pointEffectors)
                effector.enabled = false;

            // player.gameObject.SetActive(false);

            // Make all enemies lose aggro
            Combat.Instance.AllEnemiesIdle();

            // TODO: Other penalty?  Lose gold?  Lose % of collected items? Add time to timer?
            // TODO: Add respawn timer on screen
            yield return new WaitForSecondsRealtime(deathDuration);

            // Start Respawn
            // player.gameObject.SetActive(true);
            regen = true;
            GainHP(MaxHealth.Value);
            player.canAttack = false;
            player.anim.SetBool("Dead", false);
            player.canMove = true;
            player.canDash = false;
            player.inputSuspended = false;
            player.dead = false;
            player.respawning = true;
            player.invulnerable = true;

            Combat.Instance.respawnInvulnerabilityTimer = true;
            player.StartRespawnFlickering();

            yield return new WaitForSeconds(respawnDuration);
            player.canDash = true;
            player.canAttack = true;
            player.invulnerable = false;
            player.respawning = false;

            foreach (var effector in player.pointEffectors)
                effector.enabled = true;

            Combat.Instance.playerInvulnerabilityTimer = false;
            HUD.Instance.EffectsFade(HUD.Instance.effectsVolume_Death, HUD.Instance.effectsVolume_Normal, 1.5f);

        }


        // M A G I C
        public void GainMP(float amount)
        {
            currentMP += amount;
            if (currentMP >= MaxMagic.Value)
                currentMP = MaxMagic.Value;

        }

        public void LoseMP(float amount)
        {
            currentMP -= amount;
            if (currentMP <= 0)
                currentMP = 0;
        }

        void MPRegen()
        {
            mpRegenTimer -= Time.deltaTime;
            if (mpRegenTimer <= 0)
            {
                GainMP(MagicRegen.Value * MaxMagic.Value * mpRegenRate);
                mpRegenTimer = mpRegenRate;
            }

            mpPercentage = MPPercentage();
        }

        public float MPPercentage()
        {
            float cMP = currentMP;
            float mMP = MaxMagic.Value;
            float mpPercent = cMP / mMP;
            return mpPercent;
        }


        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        // public void ReloadScene()
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // }


        public float CalculateCooldown(float value)
        {
            var cooldown = value * (1 - CooldownReduction.Value);
            return cooldown;
        }

        void UpdateClock()
        {
            if (clockRunning)
            {
                timeThisRun = Time.timeSinceLevelLoad;
                clock = timeThisRun + timePreviousRuns;
                //clock = Time.fixedUnscaledTime;
                HUD.Instance.clockText.text = TimeFormat(clock).ToString();
            }
        }

        public string TimeFormat(float clock)
        {
            int minutes = Mathf.FloorToInt(clock / 60F);
            int seconds = Mathf.FloorToInt(clock - minutes * 60);
            string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            return formattedTime;
        }


        public class ReadOnlyAttribute : PropertyAttribute
        {

        }

        [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
        public class ReadOnlyDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property,
                                                    GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            public override void OnGUI(Rect position,
                                       SerializedProperty property,
                                       GUIContent label)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }




    }

}
