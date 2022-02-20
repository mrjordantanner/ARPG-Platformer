using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyAttackBase : DamagerBase//, IEnemyAttack
    {
        // Base code for all enemy attacks

        [HideInInspector]
        public EnemyAttackController enemyAttackController;
        [HideInInspector]
        public PlayerCharacter player;
        [HideInInspector]
        public EnemyCharacter enemy;
        [HideInInspector]
        public PolygonCollider2D attackRangeCollider;

        // public virtual void Attack() { }
        // public virtual float AnticipationTime { get; set; }
        // public virtual float AttackTime { get; set; }
        // public virtual float ResetTime { get; set; }

        //public virtual float KnockBack { get; set; }


        // TODO: Randomly select a position within the enemy's Min and Max attack radius


        public float CalculateDistanceFromPlayerSquared()
        {
            var distanceFromPlayerSquared = (player.transform.position - transform.position).sqrMagnitude;
            return distanceFromPlayerSquared;
        }

    }

    public interface IEnemyAttack
    {
        void Attack();
        float AnticipationTime { get; }
        float AttackTime { get; }
        float ResetTime { get; }
        //float KnockBack { get; set; }


    }
}