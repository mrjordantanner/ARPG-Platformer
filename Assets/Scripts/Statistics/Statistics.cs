using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Statistics : MonoBehaviour
    {
        // Base class to be inherited from by Character classes
        // Common statistics between PlayerStats and EnemyCharacter

        public float currentHP;
        public CharacterStat MaxHealth, MoveSpeed, Defense;

        public List<CharacterStat> CharacterStats { get; set; }
        public List<StatusEffect> ActiveStatusEffects { get; set; }

        public virtual void Awake()
        {
            ActiveStatusEffects = new List<StatusEffect>();
            CharacterStats = new List<CharacterStat>();
        }

    }




}