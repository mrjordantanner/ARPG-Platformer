using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyAttackController : MonoBehaviour
    {
        // Enemy Attack Controller
        // Sits on enemy gameobject - has a child object below it for each potential attack:
        //          For Melee attacks: a trigger collider, any vfx/sfx
        //          For Ranged attacks: a reference to the projectile to fired and targeting behavior, any vfx/sfx
        //          Projectile script will determine behavior after firing 
        //           e.g. homing, gravity (arc), rotation,

        //  Enemy attack movements
        //  Slowly back away from player, then lunge forward and attack
        //  Leap towards player to either attack or do contact damage
        //  Jump over player and attack downwards when overhead
        //  Vertical patrol movement, firing burts straight ahead
        //  Horizontal patrol movement, firist bursts downward
        //  Ground effect attack that spreads toward player, eg. flame, evil magic, bone spikes, etc
        // 
        // general note, it's possible to keep track of how many frames have passed
        // by incrementing a variable in Update.. this could be useful for animations
        // or maybe look into SMBs for this

        public enum AttackState { Idle, Anticipating, Attacking, Resetting }   // use these as Ints in animator for Animation Triggers

        public AttackState attackState;
        public bool attackCycleActive;                                // currently attacking?
        public bool inAttackRange;                                    // within the above range?
        float attackWaitTime;                                         // selected waitTime
        public float minAttackWaitTime, maxAttackWaitTime;           // min/max possible wait times
        public float attackTimer;                                    // current timer
                                                                     //public float minDistanceFromPlayer, maxDistanceFromPlayer;

        [SerializeField]
        public IEnemyAttack currentAttack;
        [SerializeField]
        public IEnemyAttack[] enemyAttackPool;

        PlayerCharacter player;
        public EnemyCharacter enemy;
        public EnemyMovement enemyMovement;
        IEnemyBehavior enemyBehavior;
        //EnemyWeapon enemyWeapon;
        Controller2D controller;
        Rigidbody2D rb;


        void Start()
        {
            player = PlayerRef.Player;
            controller = GetComponent<Controller2D>();
            rb = GetComponent<Rigidbody2D>();

            // Manually hooked up in Inspector for now
            // enemy = GetComponentInParent<EnemyCharacter>();
            // enemyMovement = GetComponentInParent<EnemyMovement>();

            enemyBehavior = GetComponentInParent<IEnemyBehavior>();
            enemyAttackPool = GetComponentsInChildren<IEnemyAttack>();

            // Set initial wait time for attacks 
            attackWaitTime = Random.Range(minAttackWaitTime, maxAttackWaitTime);

        }


        void Update()
        {
            // Increment timer between attacks if not currently attacking
            if (enemyMovement.canMove && inAttackRange && !attackCycleActive && !player.dead && !player.respawning)
                HandleAttackTimer();

            if (player.dead || player.respawning)
            {
                attackTimer = 0;
                inAttackRange = false;
            }

            // Attack on keypress for testing
            // if (Input.GetKeyDown(KeyCode.X))
            //     SelectAttack();

        }


        void HandleAttackTimer()
        {
            // TODO: If player was in attack range but moved away, reset the attack timer

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackWaitTime && inAttackRange && enemyMovement.canMove)
            {
                SelectAttack();
                attackCycleActive = true;
                attackTimer = 0;
            }

        }


        public void SelectAttack()
        {
            // Choose an attack to perform from our list of possible attacks
            if (enemyAttackPool.Length > 0)
            {
                currentAttack = enemyAttackPool[Random.Range(0, enemyAttackPool.Length)];
                //print("Current Attack: " + currentAttack.Description);
                StartCoroutine(StartAttackCycle(currentAttack));
            }
            else
                print(enemy.gameObject.ToString() + " has no attacks assigned to it.");
        }


        //  A T T A C K    C Y C L E
        public IEnumerator StartAttackCycle(IEnemyAttack currentAttack)
        {
            attackCycleActive = true;
            enemy.anim.SetBool("AttackIdle", false);

            // Anticipation
            attackState = AttackState.Anticipating;
            enemy.anim.SetBool("Anticipating", true);
            //AudioManager.Instance.Play(currentAttack.anticipationSound);
            yield return new WaitForSeconds(currentAttack.AnticipationTime);
            enemy.anim.SetBool("Anticipating", false);

            // Attacking
            attackState = AttackState.Attacking;
            enemy.anim.SetBool("Attacking", true);
            currentAttack.Attack();
            //AudioManager.Instance.Play(currentAttack.attackSound);
            yield return new WaitForSeconds(currentAttack.AttackTime);
            enemy.anim.SetBool("Attacking", false);

            // Resetting
            attackState = AttackState.Resetting;
            enemy.anim.SetBool("Resetting", true);
            //AudioManager.Instance.Play(currentAttack.resetSound);
            yield return new WaitForSeconds(currentAttack.ResetTime);
            enemy.anim.SetBool("Resetting", false);

            // Return to Idle
            attackState = AttackState.Idle;
            enemy.anim.SetBool("AttackIdle", true);
            attackCycleActive = false;
            attackWaitTime = Random.Range(minAttackWaitTime, maxAttackWaitTime);
        }

        // TODO: Make this more interesting by using distance math to create different ranges enemies
        // could sit at, rather than always at the edge of the collider boundary
        // This will help to prevent enemies from overlapping so much, too

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))// && (!player.dead || !player.respawning))
            {
                inAttackRange = true;
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))// && (!player.dead || !player.respawning))
            {
                inAttackRange = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))// && (!player.dead || !player.respawning))
            {
                inAttackRange = false;
            }
        }



        // Attack State indicator
        private void OnDrawGizmos()
        {
            if (attackState == AttackState.Anticipating)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, new Vector3(3, 3, 0));
            }
            if (attackState == AttackState.Attacking) Gizmos.color = Color.red;
            if (attackState == AttackState.Resetting) Gizmos.color = Color.green;

            Gizmos.DrawSphere(new Vector2(transform.position.x, transform.position.y + 2f), 0.15f);

        }


    }


}