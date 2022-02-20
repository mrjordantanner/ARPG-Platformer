//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Gamekit2D;

//public class EnemyWeapon : MonoBehaviour {

//    // Attached to enemy Melee weapon to control its behavior and trigger contact with player

//    [Header("Damage")]
//    public bool onlyHurtWhileAttacking = true;
//    public Vector2 baseDamageRange, damageRange;

//    [Header("Position and Rotation")]
//    public float idleRotation = -30f;
//    public float resetSpeed = 2f;
//    public float anticipationRotation = -65f;
//    public float anticipationSpeed = 1f;
//    public float attackRotation = 144f;
//    public float attackSpeed = 8f;



//    [HideInInspector]
//    public bool canMove;
//    bool moving;
//    float speed = 1f;
//    float targetRotationZ;

//    Vector3 targetAngle;
//    Vector3 currentAngle;
//    PolygonCollider2D collider;

//    //public Quaternion resetRotation = Quaternion.Euler(0, 0, -180);
//    // public Vector3 anticipationRotation = new Vector3(0, 0, -30);
//    // public Vector3 idleRotation = new Vector3(0, 0, 0);
//    //public Vector3 attackRotation = new Vector3(0, 0, 210);
//    //public Vector3 resetRotation;
//    //public Vector2 weaponAttackPosition;
//    //public Vector2 weaponIdlePosition;

//    EnemyCharacter enemy;
//   // EnemyAttack enemyAttack;
//    IEnemyBehavior enemyBehavior;
//    PlayerCharacter player;
//    [HideInInspector]
//    public SpriteRenderer renderer;
//    [HideInInspector]
//    public Material currentMaterial;
//    [HideInInspector]
//    public SpriteRenderer spriteRenderer;
//    bool isColliding;

//    void Start ()
//    {
//        currentAngle = transform.eulerAngles;
//        collider = GetComponent<PolygonCollider2D>();
//        player = PlayerRef.Player;
//        enemy = GetComponentInParent<EnemyCharacter>();
//        // enemyAttack = GetComponentInParent<EnemyAttack>();
//        enemyBehavior = GetComponentInParent<IEnemyBehavior>();
//        renderer = GetComponent<SpriteRenderer>();
//        currentMaterial = MaterialRef.Instance.normal;
//        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
       
//    }


//    void Update ()
//    {
//        isColliding = false;

//        if (canMove)
//        {
//            // Flip sprite
//            if (enemyMovement.alwaysFacePlayer)
//                enemyMovement.FacePlayer();
//            else if (enemyMovement.moveVectorX > 0 && !enemyMovement.facingRight)
//                enemyMovement.Flip();
//            else if (enemyMovement.moveVectorX < 0 && enemyMovement.facingRight)
//                enemyMovement.Flip();

//            if (onlyHurtWhileAttacking && !moving)
//                collider.enabled = false;
//            else
//                collider.enabled = true;
///*
//            if (enemyAttack.anticipating)
//            {
//                targetRotationZ = anticipationRotation * -enemyMovement.facingDirection;
//                speed = anticipationSpeed;
//                moving = true;
//            }

//            else if (enemyAttack.swingingWeapon)
//            {
//                targetRotationZ = -attackRotation * -enemyMovement.facingDirection;
//                speed = attackSpeed;
//                moving = true;
//            }

//            else if (enemyAttack.resetting)
//            {
//                targetRotationZ = idleRotation * -enemyMovement.facingDirection;
//                speed = resetSpeed;
//                moving = true;
//            }
//            else
//                moving = false;
//                */
//            if (moving)
//            {
//                currentAngle = new Vector3(0, 0,
//                Mathf.LerpAngle(currentAngle.z, targetRotationZ, Time.deltaTime * speed));
//                transform.eulerAngles = currentAngle;
//            }
//        }
//    }

//    void OnTriggerEnter2D(Collider2D other)
//    {
//       // if (!enemy.dead)
//       // {
//            if (other.gameObject.CompareTag("Player") && !player.invulnerable && !player.dead && !enemy.enemyFrozen)
//            {
//                if (isColliding) return;
//                Combat.Instance.PlayerHit(enemy, enemy.WeaponDamage.Value);
//                isColliding = true;

//            }
//      //  }

//    }

//    void OnTriggerStay2D(Collider2D other)
//    {

//     //   if (!enemy.dead)
//      //  {
//            if (other.gameObject.CompareTag("Player") && !player.invulnerable && !player.dead && !enemy.enemyFrozen)
//            {
//                if (isColliding) return;
//                Combat.Instance.PlayerHit(enemy, enemy.WeaponDamage.Value);
//                isColliding = true;
//            }
//      //  }

//    }

//}
