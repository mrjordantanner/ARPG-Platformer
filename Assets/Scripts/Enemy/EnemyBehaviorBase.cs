using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Controller2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyBehaviorBase : MonoBehaviour, IEnemyBehavior
    {
        // Parent Behavior class
        // Detects aggro / player presence
        // Child scripts dictate actual movement

        // TODO: Make enemies aware of other enemies around them and not stack directly on top of each other

        public virtual string Description { get; set; }
        //public virtual bool Aggro { get { return aggro; } }
        public virtual bool Aggro { get; set; }
        public virtual bool LoseFocus { get; set; }
        public virtual float LoseFocusTime { get; set; }
        public virtual void Disable()
        {
            enabled = false;
        }

        [HideInInspector]
        public EnemyAttackController attackController;
        [HideInInspector]
        public PlayerCharacter player;
        [HideInInspector]
        public EnemyCharacter enemy;
        [HideInInspector]
        public Collider2D aggroCollider;
        [HideInInspector]
        public Controller2D controller;
        [HideInInspector]
        public Rigidbody2D rb;
        [HideInInspector]
        public EnemyMovement enemyMovement;

        Vector2 velocity;

        [Header("Aggro")]
        public bool aggro;
        public float loseFocusTimer;
        public bool focusLost;
        public bool wasAggroed;
        public bool playerPresent;

        // Misc States
        [HideInInspector]
        public bool canMove = true;
        [HideInInspector]
        public bool dead;
        [HideInInspector]
        public bool grounded;
        [HideInInspector]
        public bool ceilinged;
        [HideInInspector]
        public bool facingRight;
        [HideInInspector]
        public bool suspendGravity;

        // List of all possible attacks this enemy can perform
        public IEnemyAttack[] enemyAttacks;

        public virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            player = PlayerRef.Player;
            enemy = GetComponent<EnemyCharacter>();
            enemyMovement = GetComponentInParent<EnemyMovement>();
            controller = GetComponentInParent<Controller2D>();

        }

        void Update()
        {
            aggro = Aggro;

            if (player.dead || player.respawning)
            {
                Aggro = false;
                wasAggroed = false;
                playerPresent = false;
            }
        }


        // Aggro indicator
        void OnDrawGizmos()
        {
            if (aggro)
            {
                Gizmos.color = Color.red;
                Vector2 offset = new Vector2(transform.position.x, transform.position.y + 1.25f);
                Gizmos.DrawSphere(offset, 0.15f);
            }
        }



        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !player.dead && !player.respawning)
            {
                aggro = true;
                wasAggroed = true;
                playerPresent = true;
            }

        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !player.dead && !player.respawning)
            {
                aggro = true;
                wasAggroed = true;
                playerPresent = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !player.dead && !player.respawning)
            {
                aggro = false;
                wasAggroed = true;
                playerPresent = false;
            }
        }
    }

    public interface IEnemyBehavior
    {
        string Description { get; set; }
        bool Aggro { get; set; }
        bool LoseFocus { get; set; }
        float LoseFocusTime { get; set; }
        void Disable();

    }
}