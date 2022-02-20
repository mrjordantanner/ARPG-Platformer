using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ProjectileBase : DamagerBase
    {
        [Header("Behavior")]
        public bool piercing = false;
        public bool obstacleCollision = true;
        public bool canHitProjectiles = false;
        public Vector2 velocity = new Vector2(15, 0);
        public float rotation, lifespan = 5f;

        [HideInInspector]
        public GameObject CurrentTarget, PreviousTarget;
        [HideInInspector]
        public EnemyCharacter enemy;
        [HideInInspector]
        public Rigidbody2D rb;

        public Vector2 explosionOffset;
        public GameObject[] Explosions;


        public virtual void Start()
        {

            Invoke("DestroyOnImpact", lifespan);
        }

        void Update()
        {
            PreviousTarget = null;
            // Projectile spin
            transform.Rotate(transform.forward, rotation);
        }


        public virtual void DestroyOnImpact()
        {
            if (Explosions.Length > 0)
            {
                // Spawn impact VFX
                foreach (var Explosion in Explosions)
                {
                    var position = new Vector2(transform.position.x + explosionOffset.x, transform.position.y + explosionOffset.y);
                    var explosionInstance = Instantiate(Explosion, position, Quaternion.identity);
                    explosionInstance.transform.SetParent(Combat.Instance.VFXContainer.transform);
                }
            }

            // TODO: Play hit SFX

            Destroy(gameObject);
        }







    }
}