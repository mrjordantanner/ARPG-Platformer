using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public interface ITargetable
    {
        GameObject Entity { get; }
        List<CharacterStat> CharacterStats { get; set; }
        List<StatusEffect> ActiveStatusEffects { get; set; }
        void TakeDamage(float damage, bool shouldHitFlash);
        void GainHP(float health);
        bool IsPlayer { get; }
        StatusEffectUI EffectUI { get; }

    }
}
