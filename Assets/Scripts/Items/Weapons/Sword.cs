using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class Sword : IWeapon
    {
        public Item.Quality Quality { get; set; }
        public WeaponAttackObject WeaponAttackObject { get; set; }
        public WeaponObject WeaponObject { get; set; }
        public Icon Icon { get; }

        public float WeaponDamageBaseCommon { get; }
        public float WeaponDamageBaseRare { get; }
        public float WeaponDamageBaseMagical { get; }
        public float AttackSpeedBase { get; }
        public float Accuracy { get; }
        public float AirHover { get; }
        public float HitStun { get; }
        public float PlayerHitStop { get; }
        public bool CritEffects { get; }

        public Sword() { }

        public Sword(Item.Quality quality)
        {
            Quality = quality;
        }

        // Does IWeapon have any methods?




    }
}
