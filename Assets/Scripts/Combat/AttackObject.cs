using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    /// <summary>
    /// Sits on the actual object prefab that is instantiated by AttackController. 
    /// Triggers contact with damageable entities.
    /// Controls animator/sprite renderer also attached to this gameobject.
    /// </summary>
    public class AttackObject : MonoBehaviour
    {

        public Weapon weapon;
        public WeaponType type;
        // public Item.Spec spec;

        //[Header("Weapon Stats")]
        //public bool combos;
        //public int comboHits;

        public Vector3 attackAngle = new Vector3(0, 0, -26f);

        public float colliderDelay;    
        public float colliderLifespan = 0.03f;

        bool isColliding;

        [HideInInspector]
        public GameObject CurrentTarget, PreviousTarget;
        EnemyCharacter enemy;
        Animator anim;
        SpriteRenderer spriteRenderer;
        CapsuleCollider2D capsuleCollider;
        PolygonCollider2D polygonCollider;

        public GameObject[] ImpactEffects;

        private void Awake()
        {
            capsuleCollider = GetComponent<CapsuleCollider2D>();
            polygonCollider = GetComponent<PolygonCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();

            // Disable sprite preview on play
            spriteRenderer.sprite = null;

            DisableColliders();

            StartCoroutine(ColliderDelay());
            Invoke("DisableColliders", colliderLifespan);

        }

        IEnumerator ColliderDelay()
        {
            yield return new WaitForSeconds(colliderDelay);
            EnableColliders();
        }

        void EnableColliders()
        {
            if (polygonCollider != null) polygonCollider.enabled = true;
            if (capsuleCollider != null) capsuleCollider.enabled = true;
        }

        void DisableColliders()
        {
            if (polygonCollider != null) polygonCollider.enabled = false;
            if (capsuleCollider != null) capsuleCollider.enabled = false;
        }


        void Update()
        {
            PreviousTarget = null;
        }

        void SpawnImpactEffect(Vector2 contactPoint)
        {
            var randomEffectIndex = Random.Range(0, ImpactEffects.Length);
            var impactInstance = Instantiate(ImpactEffects[randomEffectIndex], contactPoint, Quaternion.identity);
            impactInstance.transform.SetParent(Combat.Instance.VFXContainer.transform);

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
                    Combat.Instance.EnemyHitByWeapon(weapon, enemy);

                PreviousTarget = CurrentTarget;

                // these negate each other, resulting in the effect being in the center of the target
                Vector2 tmpDirection = new Vector2(other.transform.position.x - transform.position.x, other.transform.position.y - transform.position.y);
                Vector2 tmpContactPoint = new Vector2(transform.position.x + tmpDirection.x, transform.position.y + tmpDirection.y);

                // Vector2 tmpContactPoint = (other.transform.position - transform.position) * 0.5f;

                if (ImpactEffects.Length > 0)
                    SpawnImpactEffect(tmpContactPoint);

            }

            if (other.gameObject.CompareTag("EnemyProjectile"))
            {
                CurrentTarget = other.gameObject;
                Combat.Instance.ProjectileHit(CurrentTarget);
            }

        }

        void OnCollisionEnter(Collision other)
        {
            print("Points colliding: " + other.contacts.Length);
            print("First point that collided: " + other.contacts[0].point);
        }

    }



}