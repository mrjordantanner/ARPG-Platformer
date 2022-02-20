using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MoveToAttackPosition : EnemyBehaviorBase//, IEnemyBehavior
    {
        // Ground behavior - Move to Attack Position, then Attack
        // 1. Move into desired position based on type of attack(melee or ranged)
        // 2. Randomly choose an attack move from the list of available moves and perform it
        // 3. Repeat step 2 until either player dies, enemy dies, or player is out of range
        // 4. If player is out of range, go to Step 1

        // Platform behavior 
        //public bool canFalloff;  // can fall off platforms/ledges?
        //public bool canJump;     // can jump to other platforms?

        public string description;
        public bool loseFocus;
        public float loseFocusTime = 5f;
        public bool shouldMove;
        public bool repositionAfterAttack;

        public override string Description { get { return description; } }
        public override bool LoseFocus { get { return loseFocus; } }
        public override float LoseFocusTime { get { return loseFocusTime; } }

        // public override bool Aggro { get { return aggro; } }
        // public override int MoveVectorX { get { return moveVectorX; } }
        //public override int MoveVectorY { get { return moveVectorY; } }
        // public override bool CanMove { get { return canMove; } }

        // EnemyMeleeAttack meleeAttack;
        // EnemyAttackController attackController;
        //EnemyRangedAttack rangedAttack;

        public override void Start()
        {
            base.Start();
            attackController = GetComponentInChildren<EnemyAttackController>();
            aggroCollider = GetComponent<Collider2D>();

        }



        private void Update()
        {
            // If already attacking, do nothing here
            if (attackController.attackCycleActive)
                return;

            // Move based on whether or not we're in attack range
            if (attackController.inAttackRange)// && !repositionAfterAttack)
            {
                enemyMovement.moveVectorX = 0;
                enemyMovement.moveVectorY = 0;
                shouldMove = false;
            }
            else
                shouldMove = true;

            if (shouldMove && (aggro || wasAggroed))
                MoveTowardsDesiredPosition();

        }

        void MoveTowardsDesiredPosition()
        {

            if (enemyMovement.terrainType == EnemyMovement.TerrainType.Ground)
            {
                enemyMovement.moveVectorX = Mathf.RoundToInt(Combat.Instance.GetPlayerDirection(transform).x);
            }
            else if (enemyMovement.terrainType == EnemyMovement.TerrainType.Air)
            {
                enemyMovement.moveVectorX = Mathf.RoundToInt(Combat.Instance.GetPlayerDirection(transform).x);
                enemyMovement.moveVectorY = Mathf.RoundToInt(Combat.Instance.GetPlayerDirection(transform).y);
            }

        }




    }
}