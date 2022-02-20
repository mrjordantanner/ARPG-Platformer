using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum StatModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
}

[Serializable]
public class StatModifier
{
    public enum Source
    {
        CoreStat,   // one of the 5 Core Stats
        StatPoints, // a note to be able to reference the actual StatPoints modifier on a Core Stat
        EquippedWeapon, EquippedMail, EquippedCloak, EquippedBracers, EquippedHelm, EquippedBoots,    
        StatusEffect,
        Difficulty // for enemies only
    }

    public Source source;
    public CharacterStat.Type StatType;
    public float Value;

    public readonly StatModType Type;
    public readonly int Order;

    /*
    public StatModifier(CharacterStat.Type statType, StatModType type, float value, Source _source, int order)
    {
        StatType = statType;
        Value = value;
        Type = type;
        source = _source;
        Order = order;
    }
    */

    // ***
    public StatModifier(CharacterStat.Type statType, StatModType type, float value, Source _source) 
    {
        StatType = statType;
        Value = value;
        Type = type;
        source = _source; 
    }

   
    public StatModifier(CharacterStat.Type statType, StatModType type, float value)
    {
        StatType = statType;
        Value = value;
        Type = type;
    }

    // Used in initialization when no current value is present
    public StatModifier(CharacterStat.Type statType, StatModType type, Source _source)
    {
        StatType = statType;
        Type = type;
        source = _source;
        Value = 0;
    }

    // Used in item creation
    // Uses specified Stat Type and chooses from within specified Value Range 
    public StatModifier(CharacterStat.Type statType, float baseStatvalue, float valueRange, Source _source) 
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
    public CharacterStat.Type RandomAttributeWithinEnum(float min, float max)
    {
        var t = (CharacterStat.Type)Random.Range(min, max);
        return t;
    }

}


