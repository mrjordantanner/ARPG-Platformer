using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyMeleeAttack : EnemyAttackBase, IEnemyAttack
    {
        // Represent each actual attack
        // each enemy will have one child object for each of its possible attacks with either melee or ranged script attached
        // Controls attack intervals, speeds, animations, sounds, and collider enabling/disabling
        // animation and sfx triggers for anticipation, attacking, and reset phases
        // values regarding child collider enabling/disabling and duration

        //public override string Description { get { return description; } }
        //public override float BaseDamage { get { return baseDamage; } }
        public float AnticipationTime { get { return anticipationTime; } }
        public float AttackTime { get { return attackTime; } }
        public float ResetTime { get { return resetTime; } }
        //public override float Knockback { get { return knockback; } }

        // [Header("Attack")]
        // public string description;    // e.g. Weak , Strong, Knockback, etc
        // public float baseDamage;
        public float anticipationTime;
        public float attackTime = 1f;
        public float resetTime;
        //public float knockback;
        //public float maxAttackRange;

        public Sound anticipationSound, attackSound, resetSound;
        // public AnimationClip anticipationClip, attackClip, resetClip;

        [HideInInspector]
        public Collider2D trigger;

        bool isColliding;

        private void Update()
        {
            isColliding = false;
        }

        public void Start()
        {
            enemyAttackController = GetComponentInParent<EnemyAttackController>();
            enemy = GetComponentInParent<EnemyCharacter>();
            player = PlayerRef.Player;
            trigger = GetComponent<Collider2D>();
            trigger.enabled = false;

        }

        public void Attack()
        {
            trigger.enabled = true;
            StartCoroutine(DisableTrigger());
        }

        public IEnumerator DisableTrigger()
        {
            yield return new WaitForSeconds(attackTime);
            trigger.enabled = false;
        }


        // Collision with player
        void OnTriggerEnter2D(Collider2D other)
        {

            if (other.gameObject.CompareTag("Player") && !player.invulnerable && !player.dead && !enemy.enemyFrozen)
            {
                if (isColliding) return;
                //Combat.Instance.PlayerHit(BaseDamageMax, DamageRange, Knockback, -(int)Combat.Instance.GetPlayerDirection(transform).x);  // TODO: Fully Calculate Damage by difficulty
                Combat.Instance.PlayerHit(enemy.WeaponDamage.Value, DamageRange, Knockback, -(int)Combat.Instance.GetPlayerDirection(transform).x);  // TODO: Fully Calculate Damage by difficulty

                isColliding = true;

            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !player.invulnerable && !player.dead && !enemy.enemyFrozen)
            {
                if (isColliding) return;
                Combat.Instance.PlayerHit(enemy.WeaponDamage.Value, DamageRange, Knockback, -(int)Combat.Instance.GetPlayerDirection(transform).x);   // TODO: Fully Calculate Damage by difficulty
                isColliding = true;
            }
        }

    }
}