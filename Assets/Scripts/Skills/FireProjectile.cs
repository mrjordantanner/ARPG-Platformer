using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class FireProjectile : Skill, ISkill
    {
        public override int Index { get { return 4; } }
        public override string SkillName { get { return "Fire Projectile"; } }
        public override Resource SkillResource { get { return Resource.Magic; } }

        public string description;
        public Vector2 shootVector;
        public float airHover;
        public float attackResetTime = 0.2f;
        bool canShoot;

        public int projectilesToFire;
        public float multiShotOffsetY = 0.35f;


        [TextArea]
        public string notes;

        [Header("Projectiles")]
        public GameObject[] Projectiles;
        public Vector2 offset;

        GameObject CurrentProjectile;
        PlayerProjectile playerProjectile;
        int projectileIndex;

        GameObject ProjectileGameObject;

        private void Start()
        {
            player = PlayerRef.Player;

            // Assign default projectile
            CurrentProjectile = Projectiles[projectileIndex];
            GetProjectileStats();

            canShoot = true;

        }


        void Update()
        {
            // Switch projectile input
            if (Input.GetKeyDown(KeyCode.B))
                SwitchProjectile();

        }


        public void SwitchProjectile()
        {
            // Cycle through array using Index
            projectileIndex++;
            if (projectileIndex > Projectiles.Length - 1)
                projectileIndex = 0;

            CurrentProjectile = Projectiles[projectileIndex];
            GetProjectileStats();

        }



        public override void Use()
        {

            if (Stats.Instance.currentMP >= playerProjectile.baseResourceCost || Stats.Instance.resourceCostsRemoved)
            {
                if (player.canAttack && !player.inputSuspended && canShoot)
                    LaunchProjectile();
            }

            if (Stats.Instance.currentMP < playerProjectile.baseResourceCost && !Stats.Instance.resourceCostsRemoved)
                HUD.Instance.NotEnoughMP();

        }


        // Shoot projectile
        public void LaunchProjectile()
        {
            if (!Stats.Instance.resourceCostsRemoved)
                Stats.Instance.LoseMP(playerProjectile.baseResourceCost);

            player.anim.SetBool("Attacking", true);
            player.anim.SetTrigger("SwingSword");
            canShoot = false;
            StartCoroutine(AttackReset());

            // Should this "Attack-Pause" be in Combat.cs?
            if (player.grounded)
                player.canMove = false;

            // Air hover
            if (!player.grounded && playerProjectile.airHover > 0 && player.velocity.y < 0)
            {
                player.velocity.y = 0;
                StartCoroutine(player.PauseY(playerProjectile.airHover));
            }

            var tempOffset = offset;
            if (player.facingRight) tempOffset.y = offset.y;
            else tempOffset.y = -offset.y;

            for (int i = 0; i < projectilesToFire; i++)
            {
                ProjectileGameObject = Instantiate(CurrentProjectile, (Vector2)player.transform.position + tempOffset * player.transform.localScale.x, Quaternion.identity);
                ProjectileGameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerProjectile.velocity.x * player.transform.localScale.x, playerProjectile.velocity.y);

                // "Flip" projectile when facing right
                if (player.facingRight)
                {
                    ProjectileGameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                    ProjectileGameObject.GetComponent<PlayerProjectile>().explosionOffset *= -1;
                }

                tempOffset.y -= multiShotOffsetY;
            }


        }

        void GetProjectileStats()
        {
            playerProjectile = CurrentProjectile.GetComponent<PlayerProjectile>();
        }

        public IEnumerator AttackReset()
        {
            yield return new WaitForSeconds(playerProjectile.resetTime);
            player.anim.SetBool("Attacking", false);
            player.canMove = true;
            canShoot = true;
        }




    }

}

