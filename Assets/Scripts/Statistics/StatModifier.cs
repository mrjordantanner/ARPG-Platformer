using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    [Serializable]
    public class StatModifier
    {
        public StatModifierSource source;
        public CharacterStatType StatType;
        public readonly StatModType Type;

        public float Value;

        public StatModifier(CharacterStatType statType, StatModType type, float value, StatModifierSource _source)
        {
            StatType = statType;
            Value = value;
            Type = type;
            source = _source;
        }

        public StatModifier(CharacterStatType statType, StatModType type, float value)
        {
            StatType = statType;
            Value = value;
            Type = type;
        }

        // Used in initialization when no current value is present
        public StatModifier(CharacterStatType statType, StatModType type, StatModifierSource _source)
        {
            StatType = statType;
            Type = type;
            source = _source;
            Value = 0;
        }

        // Used in item creation
        // Uses specified Stat Type and chooses from within specified Value Range 
        public StatModifier(CharacterStatType statType, float baseStatvalue, float valueRange, StatModifierSource _source)
        {
            source = _source;
            StatType = statType;
            Value = Random.Range((1 - valueRange) * baseStatvalue, (1 + valueRange) * baseStatvalue);
        }

        // Randomizes Stat Type and chooses from within specified value range
        public StatModifier(Vector2 enumRange, Vector2 valueRange)
        {
            StatType = RandomAttributeWithinEnum(enumRange.x, enumRange.y);
            Value = Random.Range(valueRange.x, valueRange.y);
        }

        // Picks randomly from within a consecutive range of items in an enum
        public CharacterStatType RandomAttributeWithinEnum(float min, float max)
        {
            var t = (CharacterStatType)Random.Range(min, max);
            return t;
        }

    }


}