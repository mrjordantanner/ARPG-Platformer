using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    /// <summary>
    /// Controls player/enemy interactions
    /// </summary>
    public class Combat : MonoBehaviour
    {
        public static Combat Instance;

        // TODO: Put this in UI
        public GameObject FloatingText;
        public Vector2 floatingTextOffset;

        [HideInInspector]
        public PlayerCharacter player;
        MeleeAttack playerMeleeAttack;
        EnemyMovement enemyMovement;
        IEnemyBehavior enemyBehavior;
        EnemyCharacter enemy;

        public float onHitCooldown = 0.25f;
        float onHitTimer;

        // TODO: Break this into a StatusEffectController
        [Header("Status Effects")]
        public float globalDotTick = 0.2f;
        public float curseBaseDamage = 60f;
        public float curseBaseDuration = 5f;
        public GameObject CurseIcon;

        public float poisonBaseDamage = 60f;
        public float poisonBaseDuration = 5f;
        public GameObject PoisonIcon;
        public GameObject SlowIcon;
        public GameObject FreezeIcon;
        public GameObject SpeedUpIcon;
        public GameObject MagicDamageBonusIcon;
        public GameObject CooldownReductionIcon;

        public float regenerationBaseHeal = 60f;
        public float regenerationBaseDuration = 5f;
        public GameObject RegenerationIcon;
        //[HideInInspector]
        //public Freeze freeze;
        //[HideInInspector]
        //public AffectStat affectMovementSpeed, affectWeaponDamage, affectMagicDamage, 
        //affectDefense, affectMagicDefense, affectDodgeChance, affectCritChance, affectCritDamage;

        // Invulnerability timers
        [HideInInspector]
        public bool playerInvulnerabilityTimer, enemyInvulnerabilityTimer, respawnInvulnerabilityTimer;
        float playerInvTimer = 0f, enemyInvTimer = 0f, respawnInvTimer = 0f;

        bool criticalHitEffects;
        float critEffectsTimer = 0f;
        public float critEffectsDuration = 0.2f;
        public float critEffectsSlowAmount = 0.2f;
        bool freezeFrame;
        bool slowFrames;

        // TODO: Break this into a ComboController?
        [Header("Combo")]
        public int comboHits;
        public int maxComboHits = 3;
        public float comboGapTime = 0.35f;
        float timeOfLastHit;

        // Container GameObjects, simply for heirarchy organization
        [HideInInspector]
        public GameObject FloatingTextContainer, VFXContainer;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            player = PlayerRef.Player;
            playerMeleeAttack = FindObjectOfType<MeleeAttack>();
            FloatingTextContainer = new GameObject("FloatingTextContainer");
            VFXContainer = new GameObject("VFXContainer");

        }


        // ORDER OF DAMAGE CALCULATION WHEN PLAYER DAMAGES ENEMY
        // Base damage is rolled from within a range on the damaging weapon or skill
        // Multiply by WeaponDamageBonus or MagicDamageBonus depending on damage source
        // If the source canCrit and rolls a Crit, multiply by Crit Damage
        // Finally, enemy defense% is subtracted from the damage 

        // ABOUT STAT RANGES
        // Any stat that can roll within a range will really just have a base stat, which represents the max for that stat
        // Create the min value by multiplying the base value by X% - aka Accuracy rating


        // Used to roll for random value with a given base Value and range percentage, e.g. Roll within a min/max of 2% of 100 Base Value
        public float RandomizeValueWithinRange(float maxValue, float valueRange)
        {
            var value = Random.Range(maxValue * valueRange, maxValue);
            return value;
        }



        // public void PlayerHit(DamagerBase enemyDamager)
        // {

        // }

        // Enemy deals contact or weapon damage
        public void PlayerHit(float baseDamageMax, float damageRange, bool knockback, int knockbackDirection)
        {
            if (player.dead || player.respawning || Stats.Instance.invulnerable) return;
            if (player != null)
            {
                // Calculate chance to dodge
                var dodgeRoll = Random.value;
                if (dodgeRoll <= Stats.Instance.DodgeChance.Value)
                {
                    player.invulnerable = true;
                    Instance.playerInvulnerabilityTimer = true;
                    StartCoroutine(player.Hitflash(0.2f));
                    CreateFloatingText("Dodge", 4, Color.white, 0.7f, player.gameObject, 1f);
                    Log.Instance.Output($"Player dodges enemy hit.");
                }
                else
                {
                    // If no dodge, calculate enemy attack damage - player's defense % 
                    var damage = RandomizeValueWithinRange(baseDamageMax, damageRange);
                    var damageBeforeDefense = damage;
                    damage *= (1 - Stats.Instance.Defense.Value);
                    if (damage <= 0) damage = 1;
                    Stats.Instance.TakeDamage(damage, false);
                    CreateFloatingText("" + (int)damage, 3, Color.red, 0.7f, player.gameObject, 1f);
                    // Log.Instance.Output("Hit by " + enemy.name + ". Damage before/after defense: " + (int)(damageBeforeDefense) + "/" + (int)(damage));
                    //  if (player.hurtSound != null)
                    //    AudioManager.Instance.Play(player.hurtSound);
                    Log.Instance.Output($"Enemy hits player for {damage} damage.");
                }
            }

            if (knockback)
                StartCoroutine(player.Knockback(knockbackDirection));

        }




        // TODO:  Consolidate these methods by passing in the DamagerBase type, then we can process any kind of damager in one method
        // (if that makes sense to do)
        // PLAYER WEAPON STRIKES ENEMY
        public void EnemyHitByWeapon(Item.Weapon weapon, EnemyCharacter enemy)
        {
            bool criticalHit = false;
            enemyBehavior = enemy.gameObject.GetComponentInChildren<IEnemyBehavior>();

            // Roll for damage from within valueRange% higher and lower than Value of WeaponDamage (WeaponDamageBonus already calculated)
            var damage = RandomizeValueWithinRange(Stats.Instance.WeaponDamage.Value, weapon.accuracy);

            // Roll for crit
            var crit = Random.value;
            //HUD.Instance.currentCritRoll.text = Math.Round(crit, 2).ToString();
            if (crit <= Stats.Instance.CritChance.Value)
                criticalHit = true;
            else
                criticalHit = false;

            // Deal Damage
            DamageEnemy(damage, enemy, criticalHit, false);

            Log.Instance.Output($"Player hits {enemy.name} for {damage} damage. Crit: {criticalHit}");

            // Calculate OnHit effects 
            if (onHitTimer <= 0)
            {
                Stats.Instance.currentHP += Mathf.RoundToInt(Stats.Instance.LifeOnHit.Value * Stats.Instance.MaxHealth.Value);
                Stats.Instance.currentMP += Mathf.RoundToInt(Stats.Instance.MagicOnHit.Value * Stats.Instance.MaxMagic.Value);
                //foreach (var effect in weapon.onHitEffects)
                //{
                //    switch (effect)
                //    {
                //        case Item.Weapon.OnHitEffects.SpeedOnHit:
                //            ApplyStatusEffect(StatusEffect.EffectType.SpeedUp, Stats.Instance);
                //            break;

                //        case Item.Weapon.OnHitEffects.MagicDamageBonusOnHit:
                //            ApplyStatusEffect(StatusEffect.EffectType.MagicDamageBonus, Stats.Instance);
                //            break;

                //        case Item.Weapon.OnHitEffects.CooldownReductionOnHit:
                //            ApplyStatusEffect(StatusEffect.EffectType.CooldownReduction, Stats.Instance);
                //            break;
                //    }

                //}

                onHitTimer = onHitCooldown;
            }

            // Combo
            if (weapon != null)
            {
                if (weapon.combos)
                {
                    ComboCheck(weapon);
                    timeOfLastHit = Time.time;
                }

                if (weapon.hitStun > 0)
                    StartCoroutine(HitStun(enemy, weapon.hitStun));
            }

            // Air Hover
            if (!player.grounded && weapon.airHover > 0 && player.velocity.y < 0)  // change so AirHover only happens while falling?
            {
                player.velocity.y = 0;
                StartCoroutine(player.PauseY(weapon.airHover));
                StartCoroutine(player.PauseX(weapon.airHover));
            }


            // MELEE PLAYER HIT STOP
            StartCoroutine(PlayerHitStop(weapon.playerHitStop));

            // Reset damage for next hit
            damage = 0;
            criticalHit = false;
        }


        // PLAYER MAGIC SKILL STRIKES ENEMY
        public void EnemyHitByMagic(Skill magicSkill, EnemyCharacter enemy)
        {
            bool criticalHit = false;
            // if (enemy != null)
            //  {
            //     enemyBehavior = enemy.gameObject.GetComponent<IEnemyBehavior>();
            //     enemyMovement = enemy.gameObject.GetComponent<EnemyMovement>();
            // }

            var damage = RandomizeValueWithinRange(magicSkill.BaseDamageMax, magicSkill.BaseAccuracy);
            //Stats.Instance.MagicDamage.BaseValue = damage;   // TODO: should this be assigned on Skill equip, maybe?

            damage *= (1 + Stats.Instance.MagicDamageBonus.Value);

            // Roll for crit
            if (magicSkill.CanCrit)
            {
                var crit = Random.value;
                if (crit < Stats.Instance.CritChance.Value)
                    criticalHit = true;
                else
                    criticalHit = false;
            }

            if (magicSkill.hitStun > 0)
                StartCoroutine(HitStun(enemy, magicSkill.hitStun));

            Log.Instance.Output($"{magicSkill.SkillName} hits {enemy.name} for {damage} damage. Crit: {criticalHit}");

            DamageEnemy(damage, enemy, criticalHit, true);
            criticalHit = false;
        }




        // PLAYER PROJECTILE STRIKES ENEMY
        public void EnemyHitByProjectile(PlayerProjectile projectile, EnemyCharacter enemy)
        {
            bool criticalHit = false;
            // if (enemy != null)
            // {
            //     enemyBehavior = enemy.gameObject.GetComponent<IEnemyBehavior>();
            //     enemyMovement = enemy.gameObject.GetComponent<EnemyMovement>();
            // }

            var damage = RandomizeValueWithinRange(projectile.BaseDamageMax, projectile.DamageRange);

            damage *= (1 + Stats.Instance.MagicDamageBonus.Value);

            // Roll for crit
            if (projectile.canCrit)
            {
                var crit = Random.value;
                if (crit < Stats.Instance.CritChance.Value)
                    criticalHit = true;
                else
                    criticalHit = false;
            }

            if (projectile.HitStun > 0)
                StartCoroutine(HitStun(enemy, projectile.HitStun));

            DamageEnemy(damage, enemy, criticalHit, true);
            criticalHit = false;

            if (Random.value <= projectile.chanceToFreeze)
                Freeze(enemy);

            if (projectile.applyStatusEffect)
                ApplyStatusEffect(projectile.statusEffect, enemy);

            Log.Instance.Output($"{projectile.name} hits {enemy.name} for {damage} damage. Crit: {criticalHit}");
        }


        // TODO:  Why are there so many damage enemy scripts??
        public void DamageEnemy(float damage, EnemyCharacter enemy, bool isCritical, bool isMagic)
        {
            float damageBeforeDefense = 0;

            if (!enemy.dead)
            {
                enemy.hitFlash = true;
                //enemy.invulnerable = true;
                enemyInvulnerabilityTimer = true;
                enemyInvTimer = 0f;

                if (isCritical)
                {
                    // Add Crit Damage multiplier and Weapon/MagicDamageBonus
                    if (!isMagic) damage *= ((1 + Stats.Instance.CritDamage.Value) * (1 + Stats.Instance.WeaponDamageBonus.Value));
                    else damage *= (1 + Stats.Instance.MagicDamageBonus.Value);

                    damageBeforeDefense = (int)(damage);
                    StartCoroutine(CritEffects(enemy.gameObject));
                    CreateFloatingText("" + (int)damage, 5, Color.red, 1.0f, enemy.gameObject, 1f);
                    CameraShaker.Shake(0.10f, 0.08f);
                    AudioManager.Instance.Play("Punch");
                    AudioManager.Instance.Play("Enemy Hit Critical");
                    AudioManager.Instance.Play("Hit2");
                }
                else
                {
                    // Just add Weapon/MagicDamageBonus
                    if (!isMagic) damage *= (1 + Stats.Instance.WeaponDamageBonus.Value);
                    else damage *= (1 + Stats.Instance.MagicDamageBonus.Value);
                    damageBeforeDefense = (int)(damage);
                    if (enemy != null)
                        CreateFloatingText("" + (int)damage, 3, Color.yellow, 0.7f, enemy.gameObject, 1f);
                    AudioManager.Instance.Play("Punch");
                    AudioManager.Instance.Play("Enemy Hit Normal");
                    AudioManager.Instance.Play("Hit1");
                }

                if (isMagic)
                    damage *= (1 - enemy.MagicDefense.Value);
                else
                    damage *= (1 - enemy.Defense.Value);

                if (damage <= 0) damage = 1;
                enemy.TakeDamage(damage, true);
                //Log.Instance.Output("Hit " + enemy.name + ". Damage before/after defense: " + Mathf.RoundToInt(damageBeforeDefense) + "/" + Mathf.RoundToInt(damage));
            }
        }

        // PLAYER ORB PROJECTILE HITS ENEMY
        public void EnemyHitByOrb(Orb orb, GameObject CurrentTarget)
        {
            enemy = CurrentTarget.GetComponent<EnemyCharacter>();

            if (enemy != null)
            {
                enemyBehavior = enemy.gameObject.GetComponent<IEnemyBehavior>();
                enemyMovement = enemy.gameObject.GetComponent<EnemyMovement>();
            }

            float damage = 0;

            if (!enemy.dead)
            {
                switch (orb.orbType)
                {
                    case Orb.OrbType.GainDamageOverTime:
                        damage = orb.damage * (1 + Stats.Instance.MagicDamageBonus.Value);
                        DamageEnemy(damage, enemy, false, true);
                        break;

                    case Orb.OrbType.RandomDamageRange:
                        damage = Random.Range(orb.damageRange.min, orb.damageRange.max) * (1 + Stats.Instance.MagicDamageBonus.Value);
                        DamageEnemy(damage, enemy, false, true);
                        break;

                    case Orb.OrbType.Freeze:
                        //damage = 0;
                        damage = Random.Range(orb.damageRange.min, orb.damageRange.max) * (1 + Stats.Instance.MagicDamageBonus.Value);
                        DamageEnemy(damage, enemy, false, true);
                        // if (enemy.immunity == "Freeze")
                        //     CreateFloatingText("IMMUNE", 3, Color.cyan, 1f, CurrentTarget, 1f);
                        // else 
                        if (Random.value <= orb.chanceToFreeze)
                            Freeze(enemy);

                        ApplyStatusEffect(StatusEffect.EffectType.Slow, enemy);
                        break;
                }
            }

            // Reset damage for next hit
            damage = 0;

            Log.Instance.Output($"{orb.name} hits {enemy.name} for {damage} damage.");
        }

        public Vector2 GetPlayerDirection(Transform otherObject)
        {
            var playerDirection = (player.transform.position - otherObject.position).normalized;
            return playerDirection;
        }

        IEnumerator CritEffects(GameObject CurrentTarget)
        {
            var oldVel = player.velocity;
            player.canMove = false;
            player.velocity.x = 0;
            player.velocity.y = 0;
            var target = CurrentTarget.GetComponent<EnemyCharacter>();
            yield return new WaitForSeconds(critEffectsDuration);
            player.canMove = true;
            player.velocity.x = oldVel.x;
            player.velocity.y = oldVel.y;
        }

        public void Freeze(EnemyCharacter enemy)
        {
            if (!enemy.dead)
            {
                enemy.prevMoveVectorX = enemy.GetComponent<EnemyMovement>().moveVectorX;
                enemy.enemyFrozen = true;
                enemy.freezeTimer = 0;
                enemy.renderer.material = MaterialRef.Instance.frozen;
                enemy.currentMaterial = MaterialRef.Instance.frozen;
                enemy.hitFlash = false;
                AudioManager.Instance.Play("Metallic");
                AudioManager.Instance.Play("Shimmer2");
                AudioManager.Instance.Play("ReverseShort");
            }
        }


        IEnumerator HitStun(EnemyCharacter enemy, float duration)
        {
            var enemyMovement = enemy.GetComponent<EnemyMovement>();
            var oldCanMove = enemyMovement.canMove;
            enemyMovement.canMove = false;
            enemy.anim.SetTrigger("Hurt");
            yield return new WaitForSeconds(duration);
            if (enemy != null)
            {
                enemyMovement.canMove = oldCanMove;
                enemy.anim.ResetTrigger("Hurt");
            }
        }

        public void CreateFloatingText(string textString, int fontSize, Color textColor, float textDuration, GameObject TargetObject, float moveSpeed)
        {
            Vector2 textPos = new Vector2(TargetObject.transform.position.x + floatingTextOffset.x, TargetObject.transform.position.y + floatingTextOffset.y);
            GameObject NewFloatingText = Instantiate(FloatingText, textPos, Quaternion.identity);
            NewFloatingText.transform.SetParent(FloatingTextContainer.transform);
            NewFloatingText.SetActive(true);
            FloatingText newFloatingText = NewFloatingText.GetComponent<FloatingText>();
            newFloatingText.SetFloatingText(textString, fontSize, textColor);
            newFloatingText.moveSpeed = moveSpeed;
            Destroy(NewFloatingText, textDuration);

        }


        // Projectile struck by player weapon
        public void ProjectileHit(GameObject currentTarget)
        {
            var enemyProjectile = currentTarget.GetComponent<EnemyProjectile>();
            if (!enemyProjectile.invulnerable)
            {
                Destroy(currentTarget);
            }
        }





        public void ApplyStatusEffect(StatusEffect.EffectType effectType, ITargetable target)
        {
            StatusEffect NewEffectInstance = null;

            // Create new Effect
            switch (effectType)
            {
                case StatusEffect.EffectType.Curse:
                    NewEffectInstance = Curse(target);
                    break;

                case StatusEffect.EffectType.Poison:
                    NewEffectInstance = Poison(target);
                    break;

                case StatusEffect.EffectType.Slow:
                    NewEffectInstance = Slow(target);
                    break;

                case StatusEffect.EffectType.SpeedUp:
                    NewEffectInstance = SpeedUp(target);
                    break;

                case StatusEffect.EffectType.MagicDamageBonus:
                    NewEffectInstance = MagicDamageBonus(target);
                    break;

                case StatusEffect.EffectType.Regeneration:
                    NewEffectInstance = Regeneration(target);
                    break;

                case StatusEffect.EffectType.CooldownReduction:
                    NewEffectInstance = CooldownReduction(target);
                    break;

            }

            // Handle stacking
            var existingEffect = FindStatusEffectByType(NewEffectInstance._EffectType, target.ActiveStatusEffects);

            switch (NewEffectInstance.Mode)
            {
                // If same effect already exists on target, refresh it and destroy new instance
                case StatusEffect.ApplyMode.Single:

                    if (existingEffect != null)
                    {
                        existingEffect.RefreshEffect();
                        Destroy(NewEffectInstance);
                    }
                    else
                        NewEffectInstance.StartEffect();
                    break;

                // Start the effect as a separate effect
                case StatusEffect.ApplyMode.Multiple:
                    NewEffectInstance.StartEffect();
                    break;

                // If same effect exists on target, add 1 to the stacks and restart the effect
                case StatusEffect.ApplyMode.Stack:
                    if (existingEffect != null)
                    {
                        if (existingEffect.CurrentStacks < existingEffect.MaxStacks)
                            existingEffect.AddStack(1);

                        existingEffect.RefreshEffect();
                        Destroy(NewEffectInstance);

                    }
                    else
                        NewEffectInstance.StartEffect();

                    break;

            }

        }


        public StatusEffect FindStatusEffectByType(StatusEffect.EffectType type, List<StatusEffect> activeEffects)
        {
            foreach (var effect in activeEffects)
            {
                if (effect._EffectType == type)
                    return effect;
                else return null;
            }
            return null;

        }

        public StatusEffect FindStatusEffectByName(string statusEffectName, List<StatusEffect> activeEffects)
        {
            foreach (var effect in activeEffects)
            {
                if (effect.Name == statusEffectName)
                    return effect;
                else return null;
            }
            return null;

        }











        // COMBOS

        // ANATOMY OF A COMBO HIT
        // GameObject MeleeWeapon;
        // RB, Collider Trigger;
        // Weapon Animation clip
        // Player Animation clip
        // Enemy Animation clip (or behavior, e.g. knocked into air by uppercut)

        void ComboCheck(Item.Weapon weapon)
        {
            if ((Time.time - timeOfLastHit) <= comboGapTime)
            {
                comboHits++;

                if (comboHits > maxComboHits)
                    comboHits = 1;

                switch (comboHits)
                {
                    case 1: // "Left Swing"
                            // Player Animation 1
                            // Weapon Animation 1
                            // Start Attack 1 Coroutine
                        break;

                    case 2:  // "Right Swing"
                             // Player Animation 2
                             // Weapon Animation 2
                             // Start Attack 2 Coroutine
                        break;

                    case 3:  // "Uppercut Swing"
                             // Player Animation 3
                             // Weapon Animation 3
                             // Start Attack 3 Coroutine
                             // Add RB to enemy and Addforce
                        break;
                }



            }
            else
            {
                comboHits = 1;  // since we're checking onHit, we have to award one hit here
                timeOfLastHit = 0;
            }

            weapon.comboHits = comboHits;
            HUD.Instance.comboHits.text = comboHits.ToString();
        }


        IEnumerator PlayerHitStop(float duration)
        {
            player.canMove = false;
            player.inputSuspended = true;
            player.anim.enabled = false;
            playerMeleeAttack.weaponAnim.enabled = false;

            yield return new WaitForSeconds(duration);

            player.canMove = true;
            player.inputSuspended = false;
            player.anim.enabled = true;
            if (playerMeleeAttack.weaponAnim != null)
                playerMeleeAttack.weaponAnim.enabled = true;

        }



        private void Update()
        {

            // OnHit cooldown timer - prevents gaining onHit effects from multiple hits at once
            if (onHitTimer > 0)
                onHitTimer -= Time.deltaTime;

            if (criticalHitEffects)
            {
                if (slowFrames && !freezeFrame)   // this happens second
                {
                    critEffectsTimer += Time.unscaledDeltaTime;

                    if (critEffectsTimer < critEffectsDuration)
                        Time.timeScale = critEffectsSlowAmount;

                    if (critEffectsTimer >= critEffectsDuration)
                    {
                        Time.timeScale = 1.0f;
                        critEffectsTimer = 0f;
                        criticalHitEffects = false;
                        slowFrames = false;
                        freezeFrame = false;
                    }
                }

                if (freezeFrame)  // this happens first
                {
                    Time.timeScale = 0;
                    freezeFrame = false;
                    slowFrames = true;
                }
            }

            if (playerInvulnerabilityTimer)
            {
                if (playerInvTimer < player.invulnerabilityDuration)
                {
                    playerInvTimer += Time.deltaTime;
                    player.invulnerable = true;
                }

                if (playerInvTimer >= player.invulnerabilityDuration)
                {
                    player.invulnerable = false;
                    playerInvTimer = 0f;
                    playerInvulnerabilityTimer = false;
                    player.StopFlickering();
                }
            }

            if (respawnInvulnerabilityTimer)
            {
                if (respawnInvTimer < Stats.Instance.respawnDuration)
                {
                    respawnInvTimer += Time.deltaTime;
                    player.invulnerable = true;
                }

                if (respawnInvTimer >= Stats.Instance.respawnDuration)
                {
                    player.invulnerable = false;
                    respawnInvTimer = 0f;
                    respawnInvulnerabilityTimer = false;
                    player.StopFlickering();
                }
            }

            /*
            if (enemyInvulnerabilityTimer && enemy != null)
            {
                if (enemyInvTimer < .05f)
                {
                    enemyInvTimer += Time.deltaTime;
                    enemy.invulnerable = true;
                }

                if (enemyInvTimer >= .05f)
                {
                    enemy.invulnerable = false;
                    enemyInvTimer = 0f;
                    enemyInvulnerabilityTimer = false;
                }
            }

            if (!enemyInvulnerabilityTimer && enemy != null)
                enemy.invulnerable = false;
                */
        }

        public void AllEnemiesIdle()
        {
            // TODO: FIX THIS
            //  EnemyCharacter[] allEnemies = FindObjectsOfType<EnemyCharacter>();
            //  foreach (var enemy in allEnemies)
            //  {
            //      IEnemyBehavior enemyBehavior = enemy.gameObject.GetComponentInChildren<IEnemyBehavior>();
            //      enemyBehavior.Aggro = false;
            // enemyBehavior.wasAggroed = false;
            // enemy.idle = true;
            //  }

        }




        // STATUS EFFECTS
        public ValueOverTime Curse(ITargetable target)
        {
            ValueOverTime NewEffectInstance = null;

            if (target.IsPlayer)
                NewEffectInstance = player.gameObject.AddComponent<ValueOverTime>();
            else
                NewEffectInstance = target.Entity.AddComponent<ValueOverTime>();

            NewEffectInstance.Name = "Curse";
            NewEffectInstance._EffectType = StatusEffect.EffectType.Curse;
            NewEffectInstance.Value = curseBaseDamage;
            NewEffectInstance.Duration = curseBaseDuration;
            NewEffectInstance.Mode = StatusEffect.ApplyMode.Stack;
            NewEffectInstance.MaxStacks = 13;
            NewEffectInstance.IconPrefab = CurseIcon;
            NewEffectInstance.Target = target;
            return NewEffectInstance;
        }

        public ValueOverTime Poison(ITargetable target)
        {
            ValueOverTime NewEffectInstance = null;

            if (target.IsPlayer)
                NewEffectInstance = player.gameObject.AddComponent<ValueOverTime>();
            else
                NewEffectInstance = target.Entity.AddComponent<ValueOverTime>();

            NewEffectInstance.Name = "Poison";
            NewEffectInstance._EffectType = StatusEffect.EffectType.Poison;
            NewEffectInstance.Value = poisonBaseDamage;
            NewEffectInstance.Duration = poisonBaseDuration;
            NewEffectInstance.Mode = StatusEffect.ApplyMode.Multiple;
            NewEffectInstance.MaxStacks = 3;
            NewEffectInstance.IconPrefab = PoisonIcon;
            NewEffectInstance.Target = target;
            return NewEffectInstance;
        }

        public ValueOverTime Regeneration(ITargetable target)
        {
            ValueOverTime NewEffectInstance = null;

            if (target.IsPlayer)
                NewEffectInstance = player.gameObject.AddComponent<ValueOverTime>();
            else
                NewEffectInstance = target.Entity.AddComponent<ValueOverTime>();

            NewEffectInstance.Name = "Regeneration";
            NewEffectInstance._EffectType = StatusEffect.EffectType.Regeneration;
            NewEffectInstance.Value = regenerationBaseHeal;
            NewEffectInstance.Duration = regenerationBaseDuration;
            NewEffectInstance.Mode = StatusEffect.ApplyMode.Single;
            NewEffectInstance.MaxStacks = 1;
            NewEffectInstance.IconPrefab = RegenerationIcon;
            NewEffectInstance.Target = target;
            return NewEffectInstance;
        }

        public AffectCharacterStat Slow(ITargetable target)
        {
            AffectCharacterStat NewEffectInstance = null;

            if (target.IsPlayer)
                NewEffectInstance = player.gameObject.AddComponent<AffectCharacterStat>();
            else
                NewEffectInstance = target.Entity.AddComponent<AffectCharacterStat>();

            NewEffectInstance.Name = "Slow";
            NewEffectInstance._EffectType = StatusEffect.EffectType.Slow;
            NewEffectInstance.StatType = CharacterStat.Type.MoveSpeed;
            NewEffectInstance.Value = -0.5f;
            NewEffectInstance.Duration = 4;
            NewEffectInstance.Mode = StatusEffect.ApplyMode.Single;
            NewEffectInstance.MaxStacks = 1;
            NewEffectInstance.IconPrefab = SlowIcon;
            NewEffectInstance.Target = target;
            return NewEffectInstance;
        }

        public AffectCharacterStat SpeedUp(ITargetable target)
        {
            AffectCharacterStat NewEffectInstance = null;

            if (target.IsPlayer)
                NewEffectInstance = player.gameObject.AddComponent<AffectCharacterStat>();
            else
                NewEffectInstance = target.Entity.AddComponent<AffectCharacterStat>();

            NewEffectInstance.Name = "SpeedUp";
            NewEffectInstance._EffectType = StatusEffect.EffectType.SpeedUp;
            NewEffectInstance.StatType = CharacterStat.Type.MoveSpeed;
            NewEffectInstance._StatModType = StatModType.Flat;
            NewEffectInstance.Value = 0.075f;
            NewEffectInstance.Duration = 1f;
            NewEffectInstance.Mode = StatusEffect.ApplyMode.Stack;
            NewEffectInstance.MaxStacks = 30;
            NewEffectInstance.IconPrefab = SpeedUpIcon;
            NewEffectInstance.Target = target;
            return NewEffectInstance;
        }

        // Player only effects
        public AffectCharacterStat MagicDamageBonus(ITargetable target)
        {
            AffectCharacterStat NewEffectInstance = null;

            if (target.IsPlayer)
            {
                NewEffectInstance = player.gameObject.AddComponent<AffectCharacterStat>();
                NewEffectInstance.Name = "MagicDamageBonus";
                NewEffectInstance._EffectType = StatusEffect.EffectType.MagicDamageBonus;
                NewEffectInstance.StatType = CharacterStat.Type.MagicDamageBonus;
                NewEffectInstance.Mode = StatusEffect.ApplyMode.Stack;
                NewEffectInstance._StatModType = StatModType.Flat;
                NewEffectInstance.Value = 0.05f;
                NewEffectInstance.Duration = 2f;
                NewEffectInstance.MaxStacks = 20;
                NewEffectInstance.IconPrefab = MagicDamageBonusIcon;
                NewEffectInstance.Target = target;
            }

            return NewEffectInstance;
        }

        public AffectCharacterStat CooldownReduction(ITargetable target)
        {
            AffectCharacterStat NewEffectInstance = null;

            if (target.IsPlayer)
            {
                NewEffectInstance = player.gameObject.AddComponent<AffectCharacterStat>();
                NewEffectInstance.Name = "CooldownReduction";
                NewEffectInstance._EffectType = StatusEffect.EffectType.CooldownReduction;
                //NewEffectInstance.StatType = CharacterStat.Type.CooldownReduction;
                NewEffectInstance.Mode = StatusEffect.ApplyMode.Stack;
                NewEffectInstance._StatModType = StatModType.Flat;
                NewEffectInstance.Value = 0.01f;
                NewEffectInstance.Duration = 0.5f;
                NewEffectInstance.MaxStacks = 50;
                NewEffectInstance.IconPrefab = CooldownReductionIcon;
                NewEffectInstance.Target = target;
            }

            return NewEffectInstance;
        }

    }

}