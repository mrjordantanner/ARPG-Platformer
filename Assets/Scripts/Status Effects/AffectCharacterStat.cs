using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    ///  Inherits from StatusEffect. 
    ///  Creates a temporary StatMod and adds it to Target Entity's appropriate CharacterStat, then removes itself
    /// </summary>
    public class AffectCharacterStat : StatusEffect
    {
        public float value;
        public CharacterStatType StatType;
        public StatModType _StatModType;
        CharacterStat StatToAffect;
        StatModifier StatModInstance;   // reference to the StatMod instance created by the script

        public AffectCharacterStat(string name, EffectType effectType, CharacterStatType statType, StatModType statModType,
            float value, float duration, ApplyMode mode, int maxStacks, GameObject iconPrefab) : base(name, effectType, value, duration, mode, maxStacks, iconPrefab)
        {
            StatType = statType;
            _StatModType = statModType;
        }

        public override void Start()
        {
            base.Start();
            CurrentStacks = 1;
            Timer = Duration;

        }

        public override void Update()
        {
            base.Update();

        }

        public override void StartEffect()
        {
            CreateNewStatModifier();
            base.StartEffect();
            // CalculateEffectAmount();

        }

        public override void EndEffect()
        {
            StatToAffect.RemoveModifier(StatModInstance);
            base.EndEffect();

        }

        void CreateNewStatModifier()
        {
            StatModInstance = new StatModifier(StatType, _StatModType, Value, StatModifierSource.StatusEffect);
            StatToAffect = GetStat(Target.CharacterStats, StatType);

            if (StatToAffect != null)
                StatToAffect.AddModifier(StatModInstance);
            else
            {
                print("StatToAffect is Null");
                return;
            }

        }

        public override void CalculateEffectAmount()
        {
            StatModInstance.Value = Value * CurrentStacks;
            StatToAffect.isDirty = true;
        }
    }
}