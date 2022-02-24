using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class EnemyCharacter : Statistics, ITargetable
    {
        public GameObject Entity { get { return gameObject; } }

        public bool IsPlayer { get { return false; } }

        public void GainHP(float amount)
        {
            currentHP += amount;
            if (currentHP > MaxHealth.Value)
                currentHP = MaxHealth.Value;

        }

        public void TakeDamage(float damage, bool ignoreInvulnerability)    
        {
            if (dead) return;

            if (enemyBehavior!=null)
                enemyBehavior.Aggro = true;

            currentHP -= (int)(damage);
            if (currentHP <= 0)
                EnemyDeath();

            if (ignoreInvulnerability)
                StartHitFlash();

            if (enemyUI != null)
                enemyUI.canvasGroup.alpha = 1;

            UpdateEnemyUI();
        }


        //////////////////////////////////////////////////////////
        public CharacterStat
            XP,    // XP Reward given by Enemy that is modified by a StatModifier.PercentAdd based on Difficulty level
            MagicDefense,
            ContactDamage,
            WeaponDamage;

        public float
            baseMaxHealth,
            baseMoveSpeed,
            baseDefense,
            baseMagicDefense,
            baseXP,
            baseContactDamage,
            baseWeaponDamage;

        public float damageRange = 0.04f;

        public List<StatusEffect> TempStatusEffectsList = new List<StatusEffect>();

        StatModifier
            maxHealthModifier_Difficulty,
            contactDamageModifier_Difficulty,
            weaponDamageModifier_Difficulty,
            xpModifier_Difficulty;

        public GameObject EnemyGraphics;
        [HideInInspector]
        public Animator anim;

        public enum Category { Swarmer, Ranged, MeleeStrong }
        public Category category;

        public enum Tier { Normal, Elite, Nemesis }
        public Tier tier;

        [HideInInspector]
        public bool dead;
        public float lifeSpan;
        public float injuryPercent;
        public bool dealContactDamage = true;
        public bool enemyFrozen = false;
        //public bool invulnerable;
        public string immunity;
        bool knockBack;

        [Header("Drops")]
        public GameObject HealthPickup;
        public float healthDropRate;

        [Header("Loot")]
        public GameObject LootA;
        public float lootADropRate;
        public GameObject LootB;
        public float lootBDropRate;

        [Header("Freeze")]
        public float freezeDuration = 3.5f;
        public float freezeTimer = 0f;
        float unfreezeFlasher = 0f;
        public float unfreezeFlashRate = 0.03f;
        [HideInInspector]
        public int prevMoveVectorX;

        [Header("Enemy Audio")]
        public string moveSound;
        public string attackSound;
        public string deathSound;

        [HideInInspector]
        public Material currentMaterial;

        [Header("Prefabs")]
        public GameObject[] Explosions;
        public GameObject[] IceExplosions;
        [HideInInspector]
        public EnemyUI enemyUI;
        public StatusEffectUI EffectUI { get; set; }
        [HideInInspector] public Slider healthBar;

        ShootProjectile shootProjectile;
        [HideInInspector]
        public SpriteRenderer spriteRenderer;
        new Collider2D collider;
        Controller2D controller;

        [HideInInspector]
        public IEnemyBehavior enemyBehavior;
        [HideInInspector]
        public EnemyMovement enemyMovement;
        PlayerCharacter player;
        CapsuleCollider2D ContactTrigger;
        [HideInInspector]
        public Renderer renderer;
        Rigidbody2D rb;

        // Hit flash
        [HideInInspector]
        public bool hitFlash = false;
        float flashTimer = 0f;
        float flasher = 0f;
        float flashRate = 0.05f;
        float flashDuration = 0.3f;

        [HideInInspector]
        public bool isColliding;

        public override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            if (lifeSpan > 0)
                Destroy(gameObject, lifeSpan);

            currentMaterial = MaterialRef.Instance.normal;
            player = PlayerRef.Player;

            GetComponents();
            InitializeEnemyStats();
            UpdateEnemyUI();

        }

        void GetComponents()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            enemyBehavior = GetComponentInChildren<IEnemyBehavior>();
            controller = GetComponent<Controller2D>();
            rb = GetComponent<Rigidbody2D>();
            renderer = EnemyGraphics.GetComponent<Renderer>();
            spriteRenderer = EnemyGraphics.GetComponent<SpriteRenderer>();
            anim = EnemyGraphics.GetComponent<Animator>();
            enemyUI = GetComponentInChildren<EnemyUI>();
            healthBar = GetComponentInChildren<Slider>();
            EffectUI = GetComponentInChildren<StatusEffectUI>();
        }

        void InitializeEnemyStats()
        {
            MaxHealth = new CharacterStat(CharacterStatType.MaxHealth, baseMaxHealth);
            MoveSpeed = new CharacterStat(CharacterStatType.MoveSpeed, baseMoveSpeed);
            Defense = new CharacterStat(CharacterStatType.Defense, baseDefense);
           // MagicDefense = new CharacterStat(CharacterStatType.MagicDefense, baseMagicDefense);
            ContactDamage = new CharacterStat(CharacterStatType.ContactDamage, baseContactDamage);
            XP = new CharacterStat(CharacterStatType.XP, baseXP);
            WeaponDamage = new CharacterStat(CharacterStatType.WeaponDamage, baseWeaponDamage);

            CharacterStats.Add(MaxHealth);
            CharacterStats.Add(MoveSpeed);
            CharacterStats.Add(Defense);
            CharacterStats.Add(MagicDefense);
            CharacterStats.Add(XP);
            CharacterStats.Add(ContactDamage);
            CharacterStats.Add(WeaponDamage);

            maxHealthModifier_Difficulty = new StatModifier(CharacterStatType.MaxHealth, StatModType.PercentAdd, StatModifierSource.Difficulty);
            contactDamageModifier_Difficulty = new StatModifier(CharacterStatType.ContactDamage, StatModType.PercentAdd, StatModifierSource.Difficulty);
            weaponDamageModifier_Difficulty = new StatModifier(CharacterStatType.WeaponDamage, StatModType.PercentAdd, StatModifierSource.Difficulty);
            xpModifier_Difficulty = new StatModifier(CharacterStatType.XP, StatModType.PercentAdd, StatModifierSource.Difficulty);

            if (Difficulty.Instance.difficultyLevel > 1)
                CalculateStatsBasedOnDifficulty();

            MaxHealth.AddModifier(maxHealthModifier_Difficulty);
            ContactDamage.AddModifier(contactDamageModifier_Difficulty);
            XP.AddModifier(xpModifier_Difficulty);
            WeaponDamage.AddModifier(weaponDamageModifier_Difficulty);

            currentHP = MaxHealth.Value;

        }

        public void CalculateStatsBasedOnDifficulty()
        {
            var currentHealthPercentage = currentHP / MaxHealth.Value;

            var maxHealthModified = Difficulty.Instance.enemyHealth * Difficulty.Instance.difficultyLevel;
            maxHealthModifier_Difficulty.Value = maxHealthModified;
            MaxHealth.isDirty = true;      
                
            if (currentHP < MaxHealth.Value)
                currentHP = currentHealthPercentage * MaxHealth.Value;

            var damageModified = Difficulty.Instance.enemyDamage * Difficulty.Instance.difficultyLevel;
            contactDamageModifier_Difficulty.Value = damageModified;
            ContactDamage.isDirty = true;
            weaponDamageModifier_Difficulty.Value = damageModified;
            WeaponDamage.isDirty = true;

            var XPModified = Difficulty.Instance.enemyXP * Difficulty.Instance.difficultyLevel;
            xpModifier_Difficulty.Value = XPModified;
            XP.isDirty = true;

         }


        void Update()
        {
            TempStatusEffectsList = ActiveStatusEffects;

            isColliding = false;

            CalculateInjuryPercentage();
            HandleFreeze();
            HandleHitFlash();
        }

        void CalculateInjuryPercentage()
        {
            float curHP = currentHP;
            float mHP = MaxHealth.Value;
            injuryPercent = (curHP / mHP) * 100;
        }

        // TODO:  Make Freeze a separate Status Effect
        // Make the freeze time dynamic based on the abillty that caused it,
        // resistances to freeze or other CC effects, etc.
        void HandleFreeze()
        {

            if (enemyFrozen && (freezeTimer < freezeDuration))
            {
                // gameObject.layer = 29;    // change layer from Enemy to Obstacle
                // if (enemyWeapon != null)
                //     enemyWeapon.gameObject.layer = 29;

                freezeTimer += Time.deltaTime;

                // enemyMovement.velocity = Vector2.zero;

                enemyMovement.canMove = false;
                anim.enabled = false;   // pause animator

                //  if (enemyWeapon != null)
                //      enemyWeapon.canMove = false;

                // if (enemyBehavior.followPlayer)
                //     enemyBehavior.following = false;

                if (enemyMovement != null)
                    enemyMovement.moveVectorX = 0;

                //  if (anim != null)
                // { anim.enabled = false; }

                dealContactDamage = false;
            }

            // unfreeze warning flash
            // between 75% and 100% complete
            if (freezeTimer >= (freezeDuration * .75))
            {
                unfreezeFlasher += Time.deltaTime;

                if (unfreezeFlasher >= unfreezeFlashRate)
                {
                    unfreezeFlasher = 0f;

                    if (currentMaterial == MaterialRef.Instance.normal)
                        renderer.material = MaterialRef.Instance.frozen;
                    else
                        renderer.material = MaterialRef.Instance.normal;

                    currentMaterial = renderer.material;

                    //  if (enemyWeapon != null)
                    //  {
                    //      if (enemyWeapon.currentMaterial == MaterialRef.Instance.normal)
                    //         enemyWeapon.spriteRenderer.material = MaterialRef.Instance.frozen;
                    //     else
                    //         enemyWeapon.spriteRenderer.material = MaterialRef.Instance.normal;
                    //
                    //     enemyWeapon.currentMaterial = enemyWeapon.spriteRenderer.material;
                    //   }

                }
            }

            // unfreeze reset
            if (freezeTimer >= freezeDuration)
                UnFreeze();
        }

        void HandleHitFlash()
        {
            if (hitFlash && (flashTimer < flashDuration))
            {
                flashTimer += Time.deltaTime;
                flasher += Time.deltaTime;

                if (flasher >= flashRate)
                {
                    flasher = 0;

                    if (enemyFrozen)
                        HitFlashFrozen();
                    else
                        HitFlash();
                }
            }

            // stop flashing
            if (flashTimer >= flashDuration && !enemyFrozen)
            {
                flashTimer = 0;
                flasher = 0;
                hitFlash = false;
                renderer.material = MaterialRef.Instance.normal;
                currentMaterial = MaterialRef.Instance.normal;

                // if (enemyWeapon != null)
                // {
                //     enemyWeapon.spriteRenderer.material = MaterialRef.Instance.normal;
                //     enemyWeapon.currentMaterial = MaterialRef.Instance.normal;
                // }
            }
            else if (flashTimer >= flashDuration && enemyFrozen)
            {
                flashTimer = 0;
                flasher = 0;
                hitFlash = false;
                renderer.material = MaterialRef.Instance.frozen;
                currentMaterial = MaterialRef.Instance.frozen;
                // if (enemyWeapon != null)
                // {
                //     enemyWeapon.spriteRenderer.material = MaterialRef.Instance.frozen;
                //     enemyWeapon.currentMaterial = MaterialRef.Instance.frozen;
                // }
            }
        }

        void HitFlash()
        {
            if (currentMaterial == MaterialRef.Instance.normal)
            {
                renderer.material = MaterialRef.Instance.white;
                currentMaterial = MaterialRef.Instance.white;
                // if (enemyWeapon != null)
                //  {
                //     enemyWeapon.spriteRenderer.material = MaterialRef.Instance.white;
                //      enemyWeapon.currentMaterial = MaterialRef.Instance.white;
                // }
            }
            else
            {
                renderer.material = MaterialRef.Instance.normal;
                currentMaterial = MaterialRef.Instance.normal;
                // if (enemyWeapon != null)
                // {
                //     enemyWeapon.spriteRenderer.material = MaterialRef.Instance.normal;
                //     enemyWeapon.currentMaterial = MaterialRef.Instance.normal;
                //}
            }

        }

        void HitFlashFrozen()
        {
            if (currentMaterial == MaterialRef.Instance.frozen)
            {
                renderer.material = MaterialRef.Instance.white;
                currentMaterial = MaterialRef.Instance.white;
                // if (enemyWeapon != null)
                // {
                //     enemyWeapon.spriteRenderer.material = MaterialRef.Instance.white;
                //    enemyWeapon.currentMaterial = MaterialRef.Instance.white;
                // }
            }
            else
            {
                renderer.material = MaterialRef.Instance.frozen;
                currentMaterial = MaterialRef.Instance.frozen;
                //  if (enemyWeapon != null)
                //  {
                //      enemyWeapon.spriteRenderer.material = MaterialRef.Instance.frozen;
                //     enemyWeapon.currentMaterial = MaterialRef.Instance.frozen;
                // }
            }

        }

        public void UnFreeze()
        {
            // if (enemyMovement.followPlayer)
            //     enemyMovement.following = true;

            enemyMovement.moveVectorX = prevMoveVectorX;
            // gameObject.layer = 9;
            unfreezeFlasher = 0f;
            freezeTimer = 0;
            enemyFrozen = false;
            enemyMovement.canMove = true;
            anim.enabled = true;     // unpause animator

            //  if (enemyMovement.terrainType == EnemyMovement.TerrainType.Air)
            //       enemyMovement.calculateGravity = false;
            //   else enemyMovement.calculateGravity = true;

            if (anim != null)
            { anim.enabled = true; }

            dealContactDamage = true;
            renderer.material = MaterialRef.Instance.normal;
            currentMaterial = MaterialRef.Instance.normal;
            spriteRenderer.enabled = true;

            //  if (enemyWeapon != null)
            //  {
            //      enemyWeapon.canMove = true;
            // enemyWeapon.gameObject.layer = 9;
            //      enemyWeapon.spriteRenderer.material = MaterialRef.Instance.normal;
            //      enemyWeapon.currentMaterial = MaterialRef.Instance.normal;
            //     enemyWeapon.spriteRenderer.enabled = true;
            // }
        }

        public IEnumerator KnockBack(Vector2 knockBackVelocity, int knockBackDirection, float duration)
        {
            anim.SetTrigger("Hurt");
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = true;
            var knockbackVel = knockBackDirection * knockBackVelocity;
            rb.mass = 1f;
            rb.gravityScale = 1f;
            rb.AddForce(knockbackVel);
            knockBack = true;
            yield return new WaitForSeconds(duration);
            rb.bodyType = RigidbodyType2D.Kinematic;
            anim.ResetTrigger("Hurt");
            knockBack = false;
        }


        public void UpdateEnemyUI()
        {
            if (healthBar != null)
            {
                healthBar.maxValue = MaxHealth.Value;
                healthBar.value = currentHP;
            }
        }

        public void StartHitFlash()
        {
            hitFlash = true;
        }

        public void EnemyDeath()
        {
            if (enemyBehavior != null)
                enemyBehavior.Disable();
            if (collider != null)
                collider.enabled = false;
            dealContactDamage = false;
            dead = true;

            //  enemyFrozen = false;
            //   if (anim != null) anim.enabled = true;
            //   if (spriteRenderer != null) spriteRenderer.enabled = true;

            // Frozen explosion
            if (enemyFrozen)   
            {
                AudioManager.Instance.Play("IceExplosion");
                AudioManager.Instance.Play("Enemy Death");
                AudioManager.Instance.Play("Shimmer1");
                AudioManager.Instance.Play("Metallic");

                foreach (var Explosion in IceExplosions)
                {
                    var iceExplosion = Instantiate(Explosion, transform.position, Quaternion.identity);
                    iceExplosion.transform.SetParent(Combat.Instance.VFXContainer.transform);
                }

            }
            // Normal explosion
            else
            {

                AudioManager.Instance.Play("Enemy Death");

                if (tier == Tier.Elite)
                {
                    StartCoroutine(ExplosionRepeat(0.15f, 1f));
                    CameraShaker.Shake(0.1f, 0.35f);
                }
                else
                    ExplosionSingle();

            }

            Stats.Instance.kills++;
            Stats.Instance.GainXP(XP.Value);

            Log.Instance.Output($"{name} dies and awards {XP.Value} XP.");

            CalculateDrops();
            if (anim != null)
                anim.SetBool("Dead", true);

            if (enemyUI.gameObject != null)
                Destroy(enemyUI.gameObject);

            gameObject.SetActive(false);
            Destroy(gameObject, 1f);

        }

        void ExplosionSingle()
        {
            foreach (var Explosion in Explosions)
            {
                var explosionInstance = Instantiate(Explosion, transform.position, Quaternion.identity);
                explosionInstance.transform.SetParent(Combat.Instance.VFXContainer.transform);
            }

        }

        IEnumerator ExplosionRepeat(float repeatRate, float duration)
        {
            InvokeRepeating("Explode", 0, repeatRate);
            yield return new WaitForSeconds(duration);
            CancelInvoke("Explode");
        }

        void Explode()
        {
            foreach (var Explosion in Explosions)
            {
                Vector2 randomPos = new Vector2(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f));
                var explosion = Instantiate(Explosion, randomPos, Quaternion.identity);
                explosion.transform.SetParent(Combat.Instance.VFXContainer.transform);
            }
        }

        public void CalculateDrops()
        {
            // Health pickup - Don't drop if at full health
            if (Stats.Instance.currentHP != Stats.Instance.MaxHealth.Value)
            {
                var healthDropRoll = Random.value;
                if (healthDropRoll <= healthDropRate && HealthPickup != null)
                    Instantiate(HealthPickup, (Vector2)transform.position * transform.localScale.x, Quaternion.identity);
            }

            //
            var dropLoot = Random.value;
            if (LootA != null && dropLoot <= lootADropRate)
            {
                //drop LootA
                var LootObject = Instantiate(LootA, transform.position, Quaternion.identity);
                LootObject.GetComponent<Collectible>().value = Mathf.RoundToInt(XP.Value * Random.Range(0.30f, 0.50f));
            }
            if (LootB != null && dropLoot <= lootBDropRate)
            {
                //drop LootB
                var lootObject = Instantiate(LootB, transform.position, Quaternion.identity);
            }

            // Procedurally Generated Equipment drops
            LootController.Instance.CalculateEnemyDrops(this);
        }


        void ScreenFlash()
        {
            StartCoroutine(Flash());
        }

        IEnumerator Flash()
        {
            HUD.Instance.ScreenFlash.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
            HUD.Instance.ScreenFlash.SetActive(false);
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !player.invulnerable && !player.dead && !enemyFrozen && dealContactDamage)
            {
                Combat.Instance.PlayerHit(ContactDamage.Value, damageRange, false, 0);
            }

        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !player.invulnerable && !player.dead && !enemyFrozen && dealContactDamage)
            {
                Combat.Instance.PlayerHit(ContactDamage.Value, damageRange, false, 0);
            }

        }

        //public class ReadOnlyAttribute : PropertyAttribute
        //{

        //}

        //[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
        //public class ReadOnlyDrawer : PropertyDrawer
        //{
        //    public override float GetPropertyHeight(SerializedProperty property,
        //                                            GUIContent label)
        //    {
        //        return EditorGUI.GetPropertyHeight(property, label, true);
        //    }

        //    public override void OnGUI(Rect position,
        //                               SerializedProperty property,
        //                               GUIContent label)
        //    {
        //        GUI.enabled = false;
        //        EditorGUI.PropertyField(position, property, label, true);
        //        GUI.enabled = true;
        //    }
        //}

    }

}

