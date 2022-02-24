using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WeaponTraits
    {
        public static Dictionary<WeaponType, Dictionary<WeaponTrait, float>> traits = new Dictionary<WeaponType, Dictionary<WeaponTrait, float>>();

        public WeaponTraits()
        {
            // Initialize a dictionary for each WeaponType with each WeaponTrait having an initial value of 0
            foreach (int type in Enum.GetValues(typeof(WeaponType)))
            {
                foreach (int trait in Enum.GetValues(typeof(WeaponTrait)))
                {
                    traits[(WeaponType)type].Add((WeaponTrait)trait, 0f);
                }
            }

            // Sword
            traits[WeaponType.Sword][WeaponTrait.AttackDelay] = 0.1f;
            traits[WeaponType.Sword][WeaponTrait.AttackDuration] = 0.6f;
            traits[WeaponType.Sword][WeaponTrait.AttackReset] = 0.3f;

            traits[WeaponType.Sword][WeaponTrait.AirHover] = 0f;
            traits[WeaponType.Sword][WeaponTrait.PlayerHitStop] = 0.1f;
            traits[WeaponType.Sword][WeaponTrait.EnemyHitStop] = 0.1f;
            traits[WeaponType.Sword][WeaponTrait.MaxHits] = 3f;

            traits[WeaponType.Sword][WeaponTrait.GroundAttackX] = 3f;
            traits[WeaponType.Sword][WeaponTrait.GroundAttackY] = 3f;
            traits[WeaponType.Sword][WeaponTrait.AirAttackX] = 3f;
            traits[WeaponType.Sword][WeaponTrait.AirAttackY] = 3f;
            traits[WeaponType.Sword][WeaponTrait.AirAttackDownX] = 3f;
            traits[WeaponType.Sword][WeaponTrait.AirAttackDownY] = 3f;
            traits[WeaponType.Sword][WeaponTrait.CrouchAttackX] = 3f;
            traits[WeaponType.Sword][WeaponTrait.CrouchAttackY] = 3f;
            traits[WeaponType.Sword][WeaponTrait.CrouchAttackDownX] = 3f;
            traits[WeaponType.Sword][WeaponTrait.CrouchAttackDownY] = 3f;


        }

        public float GetValue(WeaponType type, WeaponTrait trait)
        {
            return traits[type][trait];
        }

        public Dictionary<WeaponTrait, float> GetTraits(WeaponType type)
        {
            return traits[type];
        }



    }
}
