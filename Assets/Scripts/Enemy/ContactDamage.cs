using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ContactDamage : DamagerBase
    {
        // may not need this

        // Sits on Enemy collider gameObject
        EnemyCharacter enemy;


        private void Start()
        {
            enemy = GetComponentInParent<EnemyCharacter>();
            UpdateContactDamage();
        }


        public void UpdateContactDamage()
        {
            BaseDamageMax = enemy.ContactDamage.Value;
        }


    }
}