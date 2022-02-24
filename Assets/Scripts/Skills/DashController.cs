using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class DashController : Skill, ISkill
    {
        public override int Index { get { return 100; } }
        public override string SkillName { get { return "Dash Attack"; } }
        // public override Item.Spec SkillSpec { get { return Item.Spec.Blood; } }
        public override Resource SkillResource { get { return Resource.Stamina; } }
        public override float Range { get { return 4f; } }

        public LayerMask obstacleLayer;
        public LayerMask enemyLayer;

        float horiz, vert;

        [Header("Rigidbody Dash")]
        public float rigidbodyDashDuration = 0.3f;
        public Vector2 rigidBodyDashVelocity = new Vector2(200, 200);
        public float dashTrailDuration = 0.3f;

        [Header("Misc")]
        public bool dashAttack = true;
        public bool onCooldown = false;
        //public int dashCost = 5;
        public float dashCooldown = 3f;
        public float dashCooldownTimer = 0f;
        public bool enemyHitsResetCooldown = true;
        public int enemiesToHit = 5;

        bool backdashing;

        // [HideInInspector]
        // public Slider dashCooldownBar;

        Animator anim;
        GhostTrails ghostTrails;
        Controller2D controller;
        SpriteRenderer spriteRenderer;
        Rigidbody2D rb;

        [HideInInspector]
        public Vector2 dashDir;

        private Vector2 _startPosition, _endPosition;

        [HideInInspector]
        public BoxCollider2D collider;

        AreaOfEffect aoe;
        public GameObject DashTrailRenderer;
        [HideInInspector]
        public GameObject DashTrailInstance;
       // KeyCombo dashLeftCombo, dashRightCombo, dashUpLeftCombo, dashUpRightCombo, dashDownLeftCombo, dashDownRightCombo, backDashCombo;
        float pythag;

        public void Start()
        {
            player = PlayerRef.Player;
            controller = player.gameObject.GetComponent<Controller2D>();
            ghostTrails = player.gameObject.GetComponent<GhostTrails>();
            anim = player.PlayerGraphics.GetComponent<Animator>();
            //spriteRenderer = player.PlayerGraphicsGetComponent<SpriteRenderer>();
            rb = player.GetComponent<Rigidbody2D>();

            EnemyHits = new List<EnemyCharacter>();

            ResetDashCooldown();

            //dashLeftCombo = new KeyCombo(new string[] { "left", "left" });
            //dashRightCombo = new KeyCombo(new string[] { "right", "right" });
            //dashUpLeftCombo = new KeyCombo(new string[] { "left", "up" });
            //dashUpRightCombo = new KeyCombo(new string[] { "right", "up" });
            //dashDownLeftCombo = new KeyCombo(new string[] { "left", "down" });
            //dashDownRightCombo = new KeyCombo(new string[] { "right", "down" });
            //backDashCombo = new KeyCombo(new string[] { "down", "down" });

            pythag = (Pythag(rigidBodyDashVelocity.x, rigidBodyDashVelocity.y) / 2);
        }


        void Update()
        {
            // DASH COOLDOWN
            if (onCooldown && (dashCooldownTimer < dashCooldown))
                dashCooldownTimer += Time.deltaTime;
            else if (dashCooldownTimer >= dashCooldown)
                ResetDashCooldown();

            horiz = player.horiz;
            vert = player.vert;

        }

        void FixedUpdate()
        {

            if ((Input.GetKeyDown(InputManager.Instance.dash_keyboard) ||
                (Input.GetKeyDown(InputManager.Instance.dash_gamepad))
                && player.canAttack && player.canDash && player.canMove && !player.inputSuspended))  //&& !crouching  
            {

                // Triangle input dash
                var p = (Pythag(rigidBodyDashVelocity.x, rigidBodyDashVelocity.y) / 2);

                //right
                if (horiz > 0 && vert == 0)
                {
                    dashDir = new Vector2(rigidBodyDashVelocity.x, 0);
                    if (player.grounded) player.transform.Translate(0, 0.25f, 0);  // lift slightly off ground
                }
                //left
                if (horiz < 0 && vert == 0)
                {
                    dashDir = new Vector2(-rigidBodyDashVelocity.x, 0);
                    if (player.grounded) player.transform.Translate(0, 0.25f, 0);
                }

                //up left
                if (horiz < 0 && vert > 0 && (player.grounded))
                    dashDir = new Vector2(-p, p);
                //up right
                if (horiz > 0 && vert > 0 && (player.grounded))
                    dashDir = new Vector2(p, p);
                //air down-right
                if (horiz > 0 && vert < 0 && !player.grounded)
                    dashDir = new Vector2(p, -p);
                //air down-left
                if (horiz < 0 && vert < 0 && !player.grounded)
                    dashDir = new Vector2(-p, -p);
                // vertical
                // if (vert > 0 && horiz == 0 && player.grounded)
                //     dashDir = new Vector2(0, rigidBodyDashVelocity.y);

                // Backdash when facing right
                if (horiz == 0 && vert < 0 && player.facingRight && player.grounded)
                {
                    dashDir = new Vector2(-rigidBodyDashVelocity.x / 5, 0);
                    if (player.grounded) player.transform.Translate(0, 0.25f, 0);
                    backdashing = true;
                }

                // Backdash when facing left
                if (horiz == 0 && vert < 0 && !player.facingRight && player.grounded)
                {
                    dashDir = new Vector2(rigidBodyDashVelocity.x / 5, 0);
                    if (player.grounded) player.transform.Translate(0, 0.25f, 0);
                    backdashing = true;
                }

                if ((!onCooldown && Stats.Instance.currentMP >= BaseResourceCost) || Stats.Instance.resourceCostsRemoved)
                {
                    if (dashDir != Vector2.zero)
                    {
                        StartCoroutine(RigidBodyDash(dashDir));
                    }
                }
                else
                    HUD.Instance.NotEnoughMP();
            }



            // Combo Input dash
            /*
            if (player.canAttack && player.canDash && player.canMove && !player.inputSuspended)
            {
                //right
                if (dashRightCombo.Check())
                {
                    dashDir = new Vector2(rigidBodyDashVelocity.x, 0);
                    if (player.grounded) player.transform.Translate(0, 0.25f, 0);  // lift slightly off ground
                }
                //left
                if (dashLeftCombo.Check())
                {
                    dashDir = new Vector2(-rigidBodyDashVelocity.x, 0);
                    if (player.grounded) player.transform.Translate(0, 0.25f, 0);
                }

                //up left
                if (dashUpLeftCombo.Check() && (player.grounded))
                    dashDir = new Vector2(-pythag, pythag);
                //up right
                if (dashUpRightCombo.Check() && (player.grounded))
                    dashDir = new Vector2(pythag, pythag);
                //air down-right
                if (dashDownRightCombo.Check() && !player.grounded)
                    dashDir = new Vector2(pythag, -pythag);
                //air down-left
                if (dashDownLeftCombo.Check() && !player.grounded)
                    dashDir = new Vector2(-pythag, -pythag);
                // vertical
                // if (vert > 0 && horiz == 0 && player.grounded)
                //     dashDir = new Vector2(0, rigidBodyDashVelocity.y);

                // Backdash when facing right
                if (backDashCombo.Check() && player.facingRight && player.grounded)
                {
                    dashDir = new Vector2(-rigidBodyDashVelocity.x / 5, 0);
                    if (player.grounded) player.transform.Translate(0, 0.25f, 0);
                    backdashing = true;
                }

                // Backdash when facing left
                if (backDashCombo.Check() && !player.facingRight && player.grounded)
                {
                    dashDir = new Vector2(rigidBodyDashVelocity.x / 5, 0);
                    if (player.grounded) player.transform.Translate(0, 0.25f, 0);
                    backdashing = true;
                }

                StartDash();
            }
            */


        }


        void StartDash()
        {

            //if (Stats.Instance.currentStaminaCharges >= BaseResourceCost || Stats.Instance.resourceCostsRemoved)
            if (Stats.Instance.currentMP >= BaseResourceCost || Stats.Instance.resourceCostsRemoved)
            {
                if (dashDir != Vector2.zero)
                    StartCoroutine(RigidBodyDash(dashDir));
            }
            else
            {
                //HUD.Instance.NotEnoughStamina();
                HUD.Instance.NotEnoughMP();
            }

        }


        public void CreateTrail(float duration)
        {
            DashTrailInstance = null;
            DashTrailInstance = Instantiate(DashTrailRenderer, player.transform.position, Quaternion.identity, player.transform);
            DashTrailInstance.GetComponent<TrailRenderer>().time = dashTrailDuration;
            Destroy(DashTrailInstance, duration * 1.2f);
        }

        IEnumerator RigidBodyDash(Vector2 rbVelocity)
        {
            // Don't lose resource if...
            if (!backdashing || !Stats.Instance.resourceCostsRemoved)
                Stats.Instance.LoseMP(BaseResourceCost);

            controller.suspendGravity = true;
            player.calculateVelocity = false;
            player.controller.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            player.velocity = Vector2.zero;
            rb.velocity = Vector2.zero;
            rb.freezeRotation = true;
            rb.gravityScale = 0;

            // Don't add DashAttack or DashTrail when backdashing
            if (!backdashing)
            {
                StartCoroutine(CreateTrigger());
                CreateTrail(dashTrailDuration * 1.2f);
                player.invulnerable = true;
            }

            rb.AddForce(rbVelocity);
            player.CollidersDashing();
            player.isDashing = true;
            player.canDash = false;
            player.inputSuspended = true;
            player.canAttack = false;
            anim.SetBool("Dashing", true);

            yield return new WaitForSeconds(rigidbodyDashDuration);
            ResetDash();


        }


        public IEnumerator CreateTrigger()
        {
            // Create dash attack trigger
            var newEffectObject = new GameObject("Dash Attack Trigger");
            newEffectObject.transform.SetParent(player.transform);
            newEffectObject.transform.position = player.transform.position;
            newEffectObject.layer = 13;

            CapsuleCollider2D capsuleCollider = newEffectObject.AddComponent<CapsuleCollider2D>();
            capsuleCollider.isTrigger = true;
            capsuleCollider.size = new Vector2(1, 2);

            aoe = newEffectObject.AddComponent<AreaOfEffect>();
            aoe.parentSkill = this;

            yield return new WaitForSeconds(rigidbodyDashDuration);

            if (EnemyHits != null && EnemyHits.Count > 0)
            {
                foreach (var enemy in EnemyHits)
                {
                    if (!enemy.dead)
                    {
                        Combat.Instance.EnemyHitByMagic(this, enemy);
                        //print(SkillName + " hit: " + enemy.name);
                    }
                }
            }

            Destroy(newEffectObject);
        }


        public void ResetDash()
        {
            player.inputSuspended = false;
            // onCooldown = true;
            rb.velocity = Vector2.zero;
            player.velocity = Vector2.zero;

            player.controller.enabled = true;

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.None;

            if (EnemyHits.Count >= enemiesToHit)
                ResetDashCooldown();
            EnemyHits.Clear();

            if (DashTrailInstance != null)
                DashTrailInstance.transform.parent = null;

            controller.suspendGravity = false;
            player.calculateVelocity = true;
            player.canDash = true;
            player.invulnerable = false;
            player.canAttack = true;
            player.CollidersNormal();
            player.isDashing = false;
            backdashing = false;
            anim.SetBool("Dashing", false);
            anim.ResetTrigger("SwingSword");
            player.velocity.y = 0;
            dashDir = Vector2.zero;
        }


        void ResetDashCooldown()
        {
            onCooldown = false;
            dashCooldownTimer = 0f;
        }

        float Pythag(float a, float b)
        {
            var c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
            return c;
        }

        public struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

    }

}