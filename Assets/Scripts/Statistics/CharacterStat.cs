using System;
using System.Collections.Generic;
//using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class CharacterStat 
{
    [HideInInspector]
    public enum Type
    {   // Common between Player and Enemies
        MaxHealth, Defense, MoveSpeed,

        // Player only
        Strength, Constitution, Agility, Intelligence, Luck,
        MaxMagic,
        WeaponDamage,
        MagicDamage,
        WeaponDamageBonus,
        MagicDamageBonus,
        CritChance,
        CritDamage,
        DodgeChance,
        //CooldownReduction,
        LifeRegen,
        MagicRegen,
        LifeOnHit,
        MagicOnHit,
        TreasureBonus,
        GoldBonus,
        XPBonus,
        PickupRadius,

        // Enemies only
        XP,
       // MagicDefense,
        ContactDamage                                        
    }

    [HideInInspector] public Type type;
    [HideInInspector] public float BaseValue;
    public float _value;
    protected float lastBaseValue = float.MinValue;

    [HideInInspector]
    public bool isDirty = true;

    //public readonly ReadOnlyCollection<StatModifier> StatModifiers;
    [SerializeField]
    public List<StatModifier> statModifiers;

    public float Value
    {
        get
        {
            if (isDirty || lastBaseValue != BaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }

    }

    // The ReadOnlyCollection stores a reference to the original List and prohibits changing it.
    // However, if you modify the original statModifiers(lowercase s), 
    // then the StatModifiers(uppercase S) will also change.
    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
       // StatModifiers = statModifiers.AsReadOnly();
    }

    public CharacterStat(Type _type, float baseValue) : this()
    {
        type = _type;
        BaseValue = baseValue;
    }

    public virtual void AddModifier(StatModifier mod)
    {
        statModifiers.Add(mod);
       // statModifiers.Sort(CompareModifierOrder);
        _value = CalculateFinalValue();
        isDirty = true;
    }


    public void RemoveModifier(StatModifier mod)   
    {
        statModifiers.Remove(mod);
        _value = CalculateFinalValue();
        isDirty = true;
    }

   // public virtual void RemoveAllModifiers()
   // {
   //     foreach (var statMod in StatModifiers)
   //         RemoveModifier(statMod);
   // }

        /*
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0; // if (a.Order == b.Order)
    }
    */

    public virtual void RemoveAllModifiersFromSource(StatModifier.Source source) 
    {
        foreach (var statMod in statModifiers)
            if (statMod.source == source)
            {
                RemoveModifier(statMod);
                isDirty = true;
            }

      //  for (int i = statModifiers.Count - 1; i >= 0; i--)
      //  {
       //     if (statModifiers[i].Source == source)
      //      {
       //         Debug.Log("Removed " + statModifiers[i].StatType + " modifier that came from source: " + source.ToString());
       //         isDirty = true;
                //didRemove = true;
       //         statModifiers.RemoveAt(i);
       //     }
      //  }
       // return didRemove;
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0; // This will hold the sum of our "PercentAdd" modifiers

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == StatModType.PercentAdd) // When we encounter a "PercentAdd" modifier
            {
                sumPercentAdd += mod.Value; // Start adding together all modifiers of this type

                // If we're at the end of the list OR the next modifer isn't of this type
                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                    //Debug.Log(type + " sumPercentAdd: " + sumPercentAdd.ToString());
                    sumPercentAdd = 0; // Reset the sum back to 0
                }
            }
            else if (mod.Type == StatModType.PercentMult) 
            {
                finalValue *= 1 + mod.Value;
            }
        }

        return (float)Math.Round(finalValue, 4);
    }
}
