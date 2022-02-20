using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyRangedAttack : EnemyAttackBase, IEnemyAttack
    {
        // public override string Description { get { return description; } }
        //public override float BaseDamageMax { get { return baseDamageMax; } }

        public float AnticipationTime { get { return anticipationTime; } }
        public float AttackTime { get { return attackTime; } }
        public float ResetTime { get { return resetTime; } }

        // public override float Knockback { get { return knockback; } }

        public string description;
        public float baseDamageMax;

        public enum FiringStyle { SingleShot, RapidFire, SweepingBurst, Mortar }
        public enum TargetingStyle { ConstantDirection, LockOnPlayer }

        [Header("Firing Behavior")]
        public FiringStyle firingStyle;
        public float startDelay;
        public float rapidFireInterval;
        public float anticipationTime;
        public float attackTime;
        public float resetTime;
        // public float knockback;

        [Header("Targeting")]
        public TargetingStyle targetingStyle;
        public LayerMask HittableLayers;      // set this to Player and Obstacle
                                              // public bool targetPlayer;
        bool targetTracking;                   // targets last player position before firing
                                               // public bool constantTracking;    // targets current player position during firing
        public Vector2 offset = new Vector2(0.0f, 0.0f);
        public float maxAttackRange;
        public bool requireLineOfSight;
        public bool hasLineOfSight;

        [Header("Projectile")]
        public GameObject EnemyProjectile;
        EnemyProjectile enemyProjectile;
        public float projectileSpeed = 6.0f;

        [Header("SFX")]
        public Sound anticipationSound, attackSound, resetSound;

        RaycastHit2D hit;
        Transform target;
        float rapidFireTimer;
        float totalFiringTimer;
        bool canShoot;
        bool onCooldown;



        void Start()
        {
            player = PlayerRef.Player;
            enemy = GetComponentInParent<EnemyCharacter>();
            enemyProjectile = GetComponent<EnemyProjectile>();
            enemyAttackController = GetComponentInParent<EnemyAttackController>();
            StartCoroutine(StartDelay());
            if (targetingStyle == TargetingStyle.LockOnPlayer) target = player.transform;

        }

        IEnumerator StartDelay()
        {
            yield return new WaitForSecondsRealtime(startDelay);
            StartCoroutine(Cooldown(ResetTime));
        }


        public void Attack()
        {
            canShoot = true;
            onCooldown = false;
        }


        void Update()
        {

            //  if (targetPlayer && (targetTracking || constantTracking))
            if (targetingStyle == TargetingStyle.LockOnPlayer && targetTracking && !player.dead && !player.respawning && !enemyAttackController.attackCycleActive)
                UpdateTarget();

            if (requireLineOfSight && !hasLineOfSight) return;

            if (player.dead || player.respawning)
            {
                totalFiringTimer = 0;
                rapidFireTimer = 0;
            }
            else
                HandleFiringStyle();

        }

        void HandleFiringStyle()
        {
            // SINGLE SHOT
            if (firingStyle == FiringStyle.SingleShot && canShoot && !enemy.enemyFrozen && !onCooldown)
            {
                FireProjectile();
                canShoot = false;
                StartCoroutine(Cooldown(ResetTime));
            }


            // RAPID FIRE
            if (firingStyle == FiringStyle.RapidFire && canShoot && !enemy.enemyFrozen && !onCooldown)
            {
                targetTracking = false;  // stop tracking during firing process

                if (totalFiringTimer < AttackTime)
                {
                    // Increment timers
                    totalFiringTimer += Time.deltaTime;
                    rapidFireTimer += Time.deltaTime;

                    // Rapid Fire Interval
                    if (rapidFireTimer >= rapidFireInterval)
                    {
                        rapidFireTimer = 0;
                        FireProjectile();
                    }
                }

                // Total Fire Time complete, start cooldown
                if (totalFiringTimer >= AttackTime)
                {
                    totalFiringTimer = 0;
                    rapidFireTimer = 0;
                    canShoot = false;
                    onCooldown = true;
                    targetTracking = true;
                    StartCoroutine(Cooldown(ResetTime));
                }
            }
        }



        void FireProjectile()
        {
            GameObject ProjectileInstance = Instantiate(EnemyProjectile, (Vector2)transform.position + offset * transform.localScale.x, Quaternion.identity);
            ProjectileInstance.GetComponent<Rigidbody2D>().velocity = transform.right * projectileSpeed;
        }


        // to be called from Update when the target is being tracked
        void UpdateTarget()
        {
            transform.right = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, 0);
            // LineOfSight();
        }

        IEnumerator Cooldown(float resetTime)
        {
            yield return new WaitForSeconds(resetTime);
            onCooldown = false;
            if (targetingStyle == TargetingStyle.LockOnPlayer) targetTracking = true;
        }



        public void LineOfSight()
        {
            hit = Physics2D.Raycast(transform.position, target.position, maxAttackRange, HittableLayers);
            //Debug.DrawRay(transform.position, target.position, Color.yellow);
            if (hit.collider.gameObject.CompareTag("Player"))
                hasLineOfSight = true;
            else
                hasLineOfSight = false;

        }


        // Taken from Gamekit - Calculates lobbed projectile velocity to a varying degree of accuracy towards the target
        // based on shotType variable
        private Vector3 GetProjectileVelocity(Vector3 target, Vector3 origin)
        {
            const float projectileSpeed = 30.0f;

            Vector3 velocity = Vector3.zero;
            Vector3 toTarget = target - origin;

            float gSquared = Physics.gravity.sqrMagnitude;
            float b = projectileSpeed * projectileSpeed + Vector3.Dot(toTarget, Physics.gravity);
            float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

            // Check whether the target is reachable at max speed or less.
            if (discriminant < 0)
            {
                velocity = toTarget;
                velocity.y = 0;
                velocity.Normalize();
                velocity.y = 0.7f;

                velocity *= projectileSpeed;
                return velocity;
            }

            float discRoot = Mathf.Sqrt(discriminant);

            // Highest
            float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

            // Lowest speed arc
            float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

            // Most direct with max speed
            float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

            float T = 0;

            // 0 = highest, 1 = lowest, 2 = most direct
            int shotType = 1;

            switch (shotType)
            {
                case 0:
                    T = T_max;
                    break;
                case 1:
                    T = T_lowEnergy;
                    break;
                case 2:
                    T = T_min;
                    break;
                default:
                    break;
            }

            velocity = toTarget / T - Physics.gravity * T / 2f;

            return velocity;
        }



    }
}