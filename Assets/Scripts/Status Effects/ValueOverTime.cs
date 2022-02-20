using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ValueOverTime : StatusEffect
    {
        public float valuePerTick;
        float dotTimer;

        public override void Start()
        {
            base.Start();
            dotTimer = Combat.Instance.globalDotTick;
            CurrentStacks = 1;
            Timer = Duration;
        }

        public override void Update()
        {
            base.Update();

        }


        public ValueOverTime(string name, EffectType type, float value, float duration, ApplyMode mode, int maxStacks, GameObject iconPrefab) : base(name, type, value, duration, mode, maxStacks, iconPrefab)
        {

        }


        public override void CalculateEffectAmount()
        {
            valuePerTick = (Value * CurrentStacks * (1 + Stats.Instance.MagicDamageBonus.Value)) / (Duration / Combat.Instance.globalDotTick);
        }

        public override void StartEffect()
        {
            base.StartEffect();
            CalculateEffectAmount();
            //Debug.Log(Name + " / Duration: " + Duration + " / val: " + valuePerTick + " / CurrentStacks: " + CurrentStacks);
            InvokeRepeating("EffectTick", Combat.Instance.globalDotTick, Combat.Instance.globalDotTick);
        }

        public override void RefreshEffect()
        {
            base.RefreshEffect();
            CalculateEffectAmount();
            InvokeRepeating("EffectTick", Combat.Instance.globalDotTick, Combat.Instance.globalDotTick);

        }

        public void EffectTick()
        {
            if (Target == null)
            {
                Debug.Log("EffectTick: Target is Null");
                EndEffect();
            }

            if (_EffectType == EffectType.Regeneration)
                Target.GainHP(valuePerTick);
            else
                Target.TakeDamage(valuePerTick, false);

        }


    }
}