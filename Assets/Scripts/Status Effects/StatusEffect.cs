using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class StatusEffect : MonoBehaviour
    {
        // Need to simplify this
        public enum EffectType { Curse, Poison, Regeneration, Slow, Freeze, SpeedUp, ResourceCostsRemoved, Invulnerability, MagicDamageBonus, CooldownReduction }
        public EffectType _EffectType;

        // public enum AffectableStats { Health, MovementSpeed, ResourceCost, IsInvulnerable, Damage }

        public enum ApplyMode
        {
            Single,          // applying effect again just refreshes it, doesn't stack
            Multiple,        // effects of the same type exist separately and don't affect each other, multiple instances of same effect - e.g. most DOTs
            Stack            // effects of the same type refresh the timer as well as adding another stack - effects consolidate into one - e.g. buffs on hit
        }

        [HideInInspector]
        public ITargetable Target;

        public string Name;
        //public EffectType Type;
        public ApplyMode Mode;
        // public AffectableStats AffectedStat;
        public float Value;
        public float Duration;
        public float Timer;
        public float StartDelay;
        //public bool IsHarmful;
        public bool IsActive;

        public int
            CurrentStacks = 1,
            MaxStacks = 1;

        // Icon
        [HideInInspector]
        public GameObject IconPrefab;             // reference to actual prefab stored in Combat.cs
        [HideInInspector]
        public GameObject IconInstance;           // reference to the new icon object once it's instantiated
        [HideInInspector]
        public StatusEffectIcon icon;                         // reference to icon script on newly instantiated icon object

        public virtual void Start()
        {
            CurrentStacks = 1;
            Timer = Duration;
        }


        public StatusEffect(string name, EffectType effectType, float value, float duration, ApplyMode mode, int maxStacks, GameObject iconPrefab)
        {
            Name = name;
            // AffectedStat = affectedStat;
            // IsHarmful = isHarmful;
            _EffectType = effectType;
            Value = value;
            Duration = duration;
            Mode = mode;
            MaxStacks = maxStacks;
            IconPrefab = iconPrefab;
            Timer = Duration;
        }

        public virtual void RefreshEffect()
        {
            CancelInvoke();
            icon.duration = Duration;
            Timer = Duration;
            icon.durationRadial.fillAmount = 0;
            icon.durationRadialActive = true;
            IsActive = true;
            CalculateEffectAmount();
            //print(Name + " effect refreshed.");
        }

        public virtual void StartEffect()
        {
            CurrentStacks = 1;
            Timer = Duration;
            IsActive = true;
            StatusEffectIcon newIcon = null;
            GameObject NewIconObject = null;

            // Add effect icon to target's UI
            if (Target.IsPlayer)
                NewIconObject = HUD.Instance.statusEffectUI.AddEffectIcon(this, Duration);
            else
                NewIconObject = Target.Entity.gameObject.GetComponentInChildren<StatusEffectUI>().AddEffectIcon(this, Duration);

            newIcon = NewIconObject.GetComponent<StatusEffectIcon>();
            newIcon.statusEffect = this;
            newIcon.durationRadial.fillAmount = 0;
            newIcon.durationRadialActive = true;

            Target.ActiveStatusEffects.Add(this);
            //Debug.Log(Name + " started");

        }

        public virtual void AddStack(int number)
        {
            CurrentStacks += number;
            if (CurrentStacks > MaxStacks) CurrentStacks = MaxStacks;
            CalculateEffectAmount();
        }

        public virtual void RemoveStack(int number)
        {
            CurrentStacks -= number;
            if (CurrentStacks <= 0) EndEffect();
            CalculateEffectAmount();
            Timer = Duration;

            if (icon != null)
            {
                icon.iconText.text = CurrentStacks.ToString();
                icon.durationRadial.fillAmount = 0;
            }
        }

        public virtual void CalculateEffectAmount() { }



        // Timer for effect duration
        public virtual void Update()
        {
            if (IsActive)
            {
                Timer -= Time.deltaTime;

                // Subtract a stack if it's a stackable effect
                if (Timer <= 0)
                {
                    if (Mode == ApplyMode.Stack)
                    {
                        RemoveStack(1);
                    }
                    else
                        EndEffect();
                }

            }

        }

        public virtual void EndEffect()
        {
            CancelInvoke();
            IsActive = false;
            if (Target.IsPlayer)
                HUD.Instance.statusEffectUI.RemoveEffectIcon(IconInstance);
            else
                Target.Entity.gameObject.GetComponentInChildren<StatusEffectUI>().RemoveEffectIcon(IconInstance);

            Target.ActiveStatusEffects.Remove(this);
            //Debug.Log(Name + " ended.");
            Destroy(this);

        }



        protected virtual CharacterStat GetStat(List<CharacterStat> characterStatList, CharacterStat.Type _type)
        {
            foreach (CharacterStat characterStat in characterStatList)
            {
                if (characterStat.type == _type)
                {
                    return characterStat;
                }
            }

            return null;
        }



    }




}