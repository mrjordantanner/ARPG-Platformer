using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class EnergyBall : Skill, ISkill
    {
        // TODO: Probably don't need a unique script for this anymore
        // Eventually just rework this as PlayerProjectile launched by FireProjectile.cs
        // Make Orb prefabs and let each prefab's Orb.cs dictate its behavior

        public override int Index { get { return 3; } }
        public override string SkillName { get { return "Orb"; } }
        public override Item.Spec SkillSpec { get { return Item.Spec.Ghost; } }
        public override Resource SkillResource { get { return Resource.Magic; } }
        //public override float Range { get { return 0; } }

        public float particleLifetime = 5.0f;
        [Header("Projectiles")]
        public GameObject[] Projectiles;
        GameObject CurrentProjectile;
        int projectileIndex;
        Orb orb;

        [Header("Gravity")]
        public bool retract = true;
        public float forceMagnitudeMin = 45f;
        public float forceMagnitudeMax = 100f;
        public float dragMin = 1f;
        public float dragMax = 5f;
        public float angularDragMin = 0.25f;
        public float angularDragMax = 1f;

        [Header("Range")]
        public float maxDistance = 15;
        float currentDistance;

        [HideInInspector]
        public Vector2 playerPos, projectilePos;

        Orb orbInstance;
        GameObject ProjectileGameObject;
        Animator anim;
        //Weapon weapon;
        GameObject GravityField;
        PointEffector2D gravityField;
        GhostTrails ghostTrails;

        [HideInInspector]
        public bool canShoot = true;
        float horiz, vert;
        Vector2 velocity;

        private void Start()
        {
            //player = GetComponent<PlayerCharacter>();
            player = PlayerRef.Player;
            anim = GetComponent<Animator>();
            GravityField = GameObject.FindGameObjectWithTag("Gravity Field");
            gravityField = GravityField.GetComponent<PointEffector2D>();

            // damageTimer = 0f;
            if (gravityField != null)
                gravityField.forceMagnitude = forceMagnitudeMin;

            // Assign default projectile
            CurrentProjectile = Projectiles[projectileIndex];
            GetProjectileStats();
            //UpdateUI();

        }


        // Switch Projectile
        public void SwitchProjectile()
        {
            // Cycle through array using Index
            projectileIndex++;
            if (projectileIndex > Projectiles.Length - 1)
                projectileIndex = 0;

            CurrentProjectile = Projectiles[projectileIndex];
            GetProjectileStats();
            // UpdateUI();

        }


        void GetProjectileStats()
        {
            orb = CurrentProjectile.GetComponent<Orb>();
        }

        // Shoot projectile
        public void LaunchProjectile(Vector2 velocity)
        {
            if (!Stats.Instance.resourceCostsRemoved)
                Stats.Instance.LoseMP(orb.cost);

            //default - need to switch based on FacingRight
            if (horiz == 0 && vert == 0)
                velocity = new Vector2(orb.velocity.min, 0);
            //right
            if (horiz > 0 && vert == 0)
                velocity = new Vector2(orb.velocity.min, 0);
            //left
            if (horiz < 0 && vert == 0)
                velocity = new Vector2(orb.velocity.min, 0);
            //up left
            if (horiz < 0 && vert > 0)
                velocity = new Vector2(orb.velocity.min, orb.velocity.max);
            //up right
            if (horiz > 0 && vert > 0)
                velocity = new Vector2(orb.velocity.min, orb.velocity.max);
            //down-right
            if (horiz > 0 && vert < 0)
                velocity = new Vector2(orb.velocity.min, -orb.velocity.max);
            //down-left
            if (horiz < 0 && vert < 0)
                velocity = new Vector2(orb.velocity.min, -orb.velocity.max);
            // up
            if (vert > 0 && horiz == 0)
                velocity = new Vector2(0, orb.velocity.min);
            // down
            if (vert < 0 && horiz == 0)
                velocity = new Vector2(0, -orb.velocity.min);

            // AudioManager.Instance.Play("Enemy Frozen");
            player.anim.SetBool("Attacking", true);
            player.anim.SetTrigger("SwingSword");
            StartCoroutine(AttackReset());

            if (!orb.multiBall)
                canShoot = false;

            if (gravityField != null)
            {
                gravityField.forceMagnitude = -forceMagnitudeMin;
                gravityField.angularDrag = angularDragMin;
                gravityField.drag = dragMin;
            }

            ProjectileGameObject = Instantiate(CurrentProjectile, (Vector2)player.transform.position + orb.offset * player.transform.localScale.x, Quaternion.identity);
            ProjectileGameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * player.transform.localScale.x, velocity.y);
            orbInstance = ProjectileGameObject.GetComponent<Orb>();
        }

        public override void Use()
        {
            // Retract
            if (!canShoot && ProjectileGameObject != null)
            {
                gravityField.drag = dragMax;
                gravityField.angularDrag = angularDragMax;
                gravityField.forceMagnitude = forceMagnitudeMax;
                return;
            }

            if (Stats.Instance.currentMP >= BaseResourceCost || Stats.Instance.resourceCostsRemoved)
                if (canShoot && !player.inputSuspended)
                    LaunchProjectile(velocity);
                else
                    HUD.Instance.NotEnoughMP();

        }


        void Update()
        {
            horiz = player.horiz;
            vert = player.vert;

            // Switch projectile input
            if (Input.GetKeyDown(KeyCode.B))
                SwitchProjectile();


            // Retract
            // if ((Input.GetKeyDown(player.buttonCircle) || Input.GetKeyDown(InputManager.Instance.skillB)) && !canShoot)
            // {
            //           gravityField.drag = dragMax;
            //     gravityField.angularDrag = angularDragMax;
            // }

            // Destroy projectile if it gets out of range
            if (ProjectileGameObject != null)
            {
                playerPos = player.transform.position;
                projectilePos = ProjectileGameObject.transform.position;
                currentDistance = Vector2.Distance(playerPos, projectilePos);
                if (currentDistance > maxDistance)
                {
                    DisconnectParticles();
                    Destroy(ProjectileGameObject);
                    currentDistance = 0;
                    canShoot = true;
                }
            }
            else
                canShoot = true;

        }


        void DisconnectParticles()
        {
            // unparent the particles so they will persist after the orb is collected
            if (orbInstance.Particles != null)
            {
                orbInstance.Particles.transform.parent = null;
                Destroy(orbInstance.Particles.gameObject, particleLifetime);
                var particleSystemEmission = orbInstance.Particles.GetComponent<ParticleSystem>().emission;
                particleSystemEmission.enabled = false;
            }

        }

        public void Collect(GameObject Projectile)
        {
            DisconnectParticles();

            Destroy(Projectile);
            canShoot = true;

            if (gravityField != null)
                gravityField.forceMagnitude = -forceMagnitudeMin;

        }

        public IEnumerator AttackReset()
        {
            yield return new WaitForSeconds(0.2f);
            player.anim.SetBool("Attacking", false);
        }




    }




}