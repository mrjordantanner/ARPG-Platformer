using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public interface IWeapon
    {
        //public enum OnHitEffects { None, SpeedOnHit, MagicDamageBonusOnHit, CooldownReductionOnHit }
        //public OnHitEffects[] onHitEffects;
        //int comboHits;
        //bool combos;

        Item.Quality Quality { get; set; }
        WeaponAttackObject WeaponAttackObject { get; set; }
        WeaponObject WeaponObject { get; set; }
        Icon Icon { get; }

        float WeaponDamageBaseCommon { get; }
        float WeaponDamageBaseRare { get; }
        float WeaponDamageBaseMagical { get; }
        float AttackSpeedBase { get; }
        float Accuracy { get; }
        float AirHover { get; }
        float HitStun { get; }
        float PlayerHitStop { get; }
        bool CritEffects { get; }

        void Attack() {}



    }
}
