using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyProjectile : ProjectileBase
    {

        [Header("Projectile Behavior")]
        public bool invulnerable = true;
        public bool collision = true;
        public bool seeker = false;

        PlayerCharacter player;
        // Transform target;
        // PointEffector2D pointEffector;

        public override void Start()
        {
            base.Start();

            rb = GetComponent<Rigidbody2D>();
            player = PlayerRef.Player;

            if (seeker) gameObject.layer = 26;   ///seekTarget layer
            else gameObject.layer = 9;    //enemy layer


        }


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !player.invulnerable)
            {
                Combat.Instance.PlayerHit(BaseDamageMax, DamageRange, Knockback, (int)Combat.Instance.GetPlayerDirection(transform).x);
                DestroyOnImpact();
            }

            if (other.gameObject.layer == 29)// && obstacleCollision)
            {
                DestroyOnImpact();
            }

        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !player.invulnerable)
            {
                Combat.Instance.PlayerHit(BaseDamageMax, DamageRange, Knockback, (int)Combat.Instance.GetPlayerDirection(transform).x);
                DestroyOnImpact();
            }



        }

        // TYPES OF PROJECTILE PATHS

        // fire in single direction
        // fire in multiple directions simultaneously (e.g. spread fire)
        // lock on to player position and fire directly to that position
        // lock on to player and continue to seek player after being fired (loose or tight homing)
        // geometric patterns, e.g. wave, spiral, etc
        // physics-based movement like Energy Ball
        // arc, like a mortar or grenade

        // TYPES OF FIRING

        // set to timer
        // based on player proximity
        // randomized


    }

}