using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerProjectile : ProjectileBase
    {
        [Header("Stats")]
        public bool canCrit;
        public float
            baseResourceCost,
            resetTime,
            airHover,
            chanceToFreeze;

        public bool applyStatusEffect;
        public StatusEffect.EffectType statusEffect;



        public override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody2D>();

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                CurrentTarget = other.gameObject;

                if (CurrentTarget == PreviousTarget)
                    return;

                enemy = CurrentTarget.GetComponent<EnemyCharacter>();
                if (!enemy.dead)
                    Combat.Instance.EnemyHitByProjectile(this, enemy);

                PreviousTarget = CurrentTarget;

                if (!piercing)
                    DestroyOnImpact();

            }

            if (other.gameObject.CompareTag("EnemyProjectile") && canHitProjectiles)
            {
                CurrentTarget = other.gameObject;
                Combat.Instance.ProjectileHit(CurrentTarget);

                if (!piercing)
                    DestroyOnImpact();
            }

            if (other.gameObject.layer == 29)// && obstacleCollision)
            {
                DestroyOnImpact();
            }
        }

    }


}