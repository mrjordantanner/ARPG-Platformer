using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{ 
    /// <summary>
    /// Holds values for increasing stats based on Difficulty
    /// </summary>
    public class Difficulty : MonoBehaviour
    {
        public static Difficulty Instance;

        public int difficultyLevel = 1;

        [Header("Elite Enemy Stats")]
        public float eliteHealth = 10f;
        public float eliteDamage = 4f;
        public float eliteXP = 3f;
        public float eliteSize = 1.35f;
        public Color eliteTint;

        [Header("Bonuses per Difficulty level")]
        [Header("Enemy Stat Bonuses")]
        public float enemyHealth = 1.40f;
        public float enemyDamage = 1.30f;
        public float enemyXP = 1.35f;
        public float hazardDamage = 1.3f;

        // How much extra xp is needed to level up per diff level
        public float levelProgressIncrease = 1.75f; 

        [Header("Drop Rate Bonuses")]
        public float commonDropRate = 1.0f;     // stays constant
        public float rareDropRate = 0.04f;
        public float magicalDropRate = 0.02f;
        public float uniqueDropRate = 0.01f;
        public float goldBonus = 0.15f;

        PlayerCharacter player;
        // EnemySpawner enemySpawner;

        //int newEnemyHealth;
        //int newEnemyDamageh


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
            player = PlayerRef.Player;
            Set(1);  
        }

        public void Raise()
        {
            difficultyLevel++;
            Set(difficultyLevel);

        }

        public void Lower()
        {
            if (difficultyLevel > 1)
            {
                difficultyLevel--;
                Set(difficultyLevel);
            }
        }

        public void Set(int diffLevel)
        {
            difficultyLevel = diffLevel;

            CalculateEnemyStats();
            //Stats.Instance.CalculateLevelProgress(difficultyLevel);
            Stats.Instance.statsGUI.UpdateStatsGUI();
        }

        void CalculateEnemyStats()
        {
            var allEnemiesInPlay = FindObjectsOfType<EnemyCharacter>();
            foreach (var enemy in allEnemiesInPlay)
            {
                enemy.CalculateStatsBasedOnDifficulty();
            }
        }

        // Enemy stats formulas
        // This may need adjusting, or create a table
        // public int CalculateBasedOnLevel(int baseStat, float statIncrease)
        // {
        //     int newStat = Mathf.RoundToInt(baseStat * Mathf.Pow(statIncrease, difficultyLevel));
        //     return newStat;
        // }

    }


}