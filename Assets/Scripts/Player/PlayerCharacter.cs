using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerCharacter : MonoBehaviour
    {
        public static PlayerCharacter Instance;

        public GameObject PlayerGraphics;
        public Animator anim;
        public SpriteRenderer spriteRenderer;

        public bool grounded, ceilinged;

        [Header("Movement")]
        public bool useDpad;
        [HideInInspector]
        public float horiz, vert;
        public Vector2 moveVector;
        float accelerationTimeAirborne = 0;
        float accelerationTimeGrounded = 0;
        float velocityXSmoothing;
        public float basePickupRadius = 3f;
        public float pickupRadius;

        [Header("Jump")]
        public float maxJumpHeight = 3f;
        public float doubleJumpReduction = 2.45f;
        public float baseSuperJumpVelocity = 10;
        public float superJumpVelocity = 10;
        public float superJumpCost;
        public int jumpState;
        float minJumpHeight = 0.01f;
        float timeToJumpApex = 0.6f;
        float maxJumpVelocity;
        float minJumpVelocity;

        [Header("Physics")]
        public float fallMultiplier = 2.25f;
        public float terminalVelocity = -70f;
        public Vector2 velocity;
        [HideInInspector]
        public float velocityY, velocityX;
        float gravity;
        [HideInInspector]
        public bool calculateVelocity = true;
        float deadFallVelocity;

        [Header("Knockback")]
        public bool knockback;
        public float knockbackMass = 1f;
        public float knockbackGravity = 1f;
        public Vector2 knockbackVelocity;
        public float knockbackDuration;
        public float invulnerabilityDuration = 2f;
        public float flickeringDuration = 0.1f;
        Vector2 knockbackVel;

        [Header("Misc")]
        public GameObject DeathExplosion;

        [HideInInspector]
        public bool invulnerable, canDoubleJump, canSuperJump = true, canMove = true,
            canTurnAround = true, crouching, canDash = true, canAttack = true,
            isDashing, inputSuspended, respawning, dead, wasAirborneLastFrame, facingRight;

        public float pickupDelay = 0.1f;
        float potionTimer;

        [HideInInspector]
        public bool spriteOriginallyFacesLeft;

        protected Transform m_Transform;

        [HideInInspector]
        public Vector2 m_MoveVector;
        // protected float m_TanHurtJumpAngle;
        protected WaitForSeconds m_FlickeringWait;
        protected Coroutine m_FlickerCoroutine;
        protected Vector2 m_StartingPosition = Vector2.zero;
        protected bool m_StartingFacingLeft = false;
        protected bool m_InPause = false;

        protected readonly int m_HashHorizontalSpeedPara = Animator.StringToHash("HorizontalSpeed");
        protected readonly int m_HashVerticalSpeedPara = Animator.StringToHash("VerticalSpeed");
        protected readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");
        protected readonly int m_HashDeadPara = Animator.StringToHash("Dead");
        protected readonly int m_HashMeleeAttackPara = Animator.StringToHash("MeleeAttack");

        protected const float k_MinHurtJumpAngle = 0.001f;
        protected const float k_MaxHurtJumpAngle = 89.999f;
        protected const float k_GroundedStickingVelocityMultiplier = 3f;
        // This is to help the character stick to vertically moving platforms.

        [Header("Collider Sizes")]
        public Vector2 colliderStandingSize = new Vector2(0.5f, 1.6f);
        public Vector2 colliderStandingOffset = new Vector2(0f, 0f),
                colliderCrouchingSize = new Vector2(0.5f, 1f), colliderCrouchingOffset = new Vector2(0f, -.25f),
                dashColliderSize, dashColliderOffset;

        [HideInInspector]
        public Controller2D controller;
        [HideInInspector]
        public GhostTrails ghostTrails;

        DashController dashController;
        Vector2 directionalInput;

        public CircleCollider2D pickupRadiusCollider;

        Rigidbody2D rb;
        [HideInInspector]
        public CapsuleCollider2D physicsCollider;
        [HideInInspector]
        public CapsuleCollider2D triggerCollider;
        public PointEffector2D[] pointEffectors;
        /*
         [HideInInspector]
        public KeyCode
            buttonSquare = KeyCode.Joystick1Button2,
            buttonX = KeyCode.Joystick1Button0,
            buttonCircle = KeyCode.Joystick1Button1,
            buttonTriangle = KeyCode.Joystick1Button3,
            buttonL1 = KeyCode.Joystick1Button4,
            buttonR1 = KeyCode.Joystick1Button5,
            buttonL2 = KeyCode.Joystick1Button6,
            buttonR2 = KeyCode.Joystick1Button7,
            buttonOptions = KeyCode.Joystick1Button8,
            buttonShare = KeyCode.Joystick1Button9,
            buttonL3 = KeyCode.Joystick1Button12,
            buttonR3 = KeyCode.Joystick1Button13;
            */

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                throw new UnityException("There cannot be more than one PlayerCharacter script.  The instances are " + Instance.name + " and " + name + ".");
                // print("Duplicate PlayerCharacter script destroyed.");
                // Destroy(gameObject);
                // return;
            }

            DontDestroyOnLoad(gameObject);

            gameObject.name = "Player";
            GetComponents();
            SetProperties();

            potionTimer = Stats.Instance.CalculateCooldown(Stats.Instance.potionBaseCooldown);
        }

        void GetComponents()
        {
            controller = GetComponent<Controller2D>();
            ghostTrails = GetComponent<GhostTrails>();
            pickupRadiusCollider = GetComponentInChildren<CircleCollider2D>();
            anim = PlayerGraphics.GetComponent<Animator>();
            spriteRenderer = PlayerGraphics.GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            dashController = FindObjectOfType<DashController>();
            triggerCollider = GetComponent<CapsuleCollider2D>();
            physicsCollider = GetComponent<CapsuleCollider2D>();
            pointEffectors = GetComponentsInChildren<PointEffector2D>();
        }

        void SetProperties()
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

            m_FlickeringWait = new WaitForSeconds(flickeringDuration);
            m_StartingPosition = transform.position;

            anim.SetFloat("AttackSpeed", 1.0f);

            //ghostTrails.on = true;
            //ghostTrails.enabled = true;
            //invulnerable = false;

            canMove = true;
            spriteRenderer.enabled = true;
            inputSuspended = false;

        }

        void Start()
        {

        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Camera shake when dashing into obstacles
            //  if (isDashing && collision.gameObject.layer == 29)
            //      CameraShaker.Shake(0.30f, 0.12f);

            // Stop dash early if collide with obstacle - also end trail early?
            if (isDashing && collision.gameObject.layer == 29)
            {
                dashController.ResetDash();
                if (dashController.DashTrailInstance != null)
                    Destroy(dashController.DashTrailInstance, 0.1f);
            }

            // Ignore enemy colliders while dashing
            if (isDashing && (collision.gameObject.layer == 8 || collision.gameObject.layer == 9))
            {
                //print("Dash collided with " + collision.gameObject.name);
                Physics2D.IgnoreCollision(collision.collider, physicsCollider);
            }

            // Ignore special platform collision while Superjumping
            // TODO: This doesn't work the first time colliding with a platform, but it does after that
            if (anim.GetBool("IsSuperJumping") && (collision.gameObject.CompareTag("One Way")) || collision.gameObject.CompareTag("Fallthrough"))
            {
                Physics2D.IgnoreCollision(collision.collider, physicsCollider);
            }

        }

        public void SetDirectionalInput(Vector2 input)
        {
            directionalInput = input;
        }


        void Update()
        {
            grounded = controller.collisions.below;
            ceilinged = controller.collisions.above;

            anim.SetFloat(m_HashVerticalSpeedPara, velocity.y);
            anim.SetBool("SuperJump", canSuperJump);
            anim.SetBool(m_HashGroundedPara, grounded);

            // DONT CHANGE THIS
            if (controller.collisions.below || controller.collisions.above)
                velocity.y = 0;

            HandleJump();

            if (velocity.y < 0 && !isDashing)
            {
                anim.SetBool("IsSuperJumping", false);
            }

            if (velocity.y > 0)
            {
                canSuperJump = false;
                anim.SetBool("SuperJump", canSuperJump);
            }

            if (calculateVelocity)
                CalculateVelocity();

            if (!controller.suspendGravity)
                CalculateGravity();

            if (!inputSuspended && !GameManager.Instance.gamePaused)
            {
                HandleInput();
                HandlePotion();
            }

            if (canMove && !inputSuspended && !GameManager.Instance.gamePaused)
            {
                HandleMovement();
                HandleCrouchMechanics();
            }

            if (dead)
            {
                inputSuspended = true;
                deadFallVelocity += gravity * Time.deltaTime;
                controller.Move(new Vector2(0, deadFallVelocity) * Time.deltaTime);
            }


        }

        void HandleJump()
        {
            if (canMove)
            {
                // Jump 
                // State 0
                if (grounded)
                {
                    jumpState = 0;
                    canDoubleJump = false;
                    canSuperJump = true;
                    anim.SetBool("DoubleJump", false);
                    anim.SetInteger("JumpState", jumpState);

                    // Prevents attacks from triggering twice when attacking in air just before landing
                    if (wasAirborneLastFrame && anim.GetBool("Attacking"))
                    {
                        anim.SetBool("Attacking", false);
                        canAttack = true;
                        wasAirborneLastFrame = false;
                    }
                    else if (wasAirborneLastFrame) wasAirborneLastFrame = false;
                }

                // State 1                     
                if (Input.GetKeyDown(InputManager.Instance.jump_keyboard) ||
                    Input.GetKeyDown(InputManager.Instance.jump_gamepad) ||
                    Input.GetKeyDown(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.Space)
                    && !anim.GetBool("Attacking") && !inputSuspended)
                {
                    if (grounded)
                    {
                        velocity.y = maxJumpVelocity;
                        StartCoroutine(DoubleJumpDelay(0.1f));   // wait briefly then set jumpState to 1
                        wasAirborneLastFrame = true;
                    }

                    // Double Jump    
                    // State 2               
                    else
                    {
                        if (!grounded && canDoubleJump && jumpState == 1)
                        {
                            jumpState = 2;
                            velocity.y = (maxJumpVelocity - doubleJumpReduction);
                            canDoubleJump = false;
                            anim.SetBool("DoubleJump", true);
                            anim.SetInteger("JumpState", jumpState);
                            ghostTrails.on = true;
                            ghostTrails.enabled = true;
                        }
                    }

                }

                // Stop double jump animation once descending again
                if (jumpState == 2 && velocity.y < 0)  
                    anim.SetBool("DoubleJump", false);

                // Jump cancel
                if (Input.GetKeyUp(InputManager.Instance.jump_keyboard) ||
                    Input.GetKeyUp(InputManager.Instance.jump_gamepad) ||
                    Input.GetKeyUp(KeyCode.W) ||
                    Input.GetKeyUp(KeyCode.UpArrow) ||
                    Input.GetKeyUp(KeyCode.Space))
                {
                    if (velocity.y > 0.01f)
                        velocity.y = 0.01f;
                }
            }
        }

        void HandleInput()
        {
            // Get/Set Input
            if (!inputSuspended)
            {
                // Dpad 
                if (useDpad)
                {
                    horiz = Input.GetAxisRaw("DpadHoriz");
                    vert = Input.GetAxisRaw("DpadVert");
                    // print("Dpad Horiz: " + horiz.ToString());
                    // print("Dpad Vert: " + vert.ToString());
                }
                // Analog Stick
                else
                {
                    horiz = Input.GetAxisRaw("Horizontal");
                    vert = Input.GetAxisRaw("Vertical");
                }

                Vector2 directionalInput = new Vector2(horiz, vert);
                SetDirectionalInput(directionalInput);
            }
        }

        void HandleMovement()
        {
            // Movement
            if (!crouching)
            {
                if (!inputSuspended && canMove)
                {
                    moveVector = new Vector2(velocityX, velocityY);
                    controller.Move(moveVector * Time.deltaTime, directionalInput);
                }

                anim.SetFloat(m_HashHorizontalSpeedPara, Mathf.Abs(horiz));
            }

            // Turnaround
            if (canTurnAround && canMove && !inputSuspended)// && !backflip)
            {
                if (horiz > 0 && !facingRight)
                    TurnAround();
                else if (horiz < 0 && facingRight)
                    TurnAround();
            }
        }

        public void CollidersNormal()
        {
            triggerCollider.size = physicsCollider.size = colliderStandingSize;
            triggerCollider.offset = physicsCollider.offset = colliderStandingOffset;
        }

        public void CollidersCrouching()
        {

            triggerCollider.size = physicsCollider.size = colliderCrouchingSize;
            triggerCollider.offset = physicsCollider.offset = colliderCrouchingOffset;
        }
        public void CollidersDashing()
        {

            triggerCollider.size = physicsCollider.size = dashColliderSize;
            triggerCollider.offset = physicsCollider.offset = dashColliderOffset;

        }

        void HandleCrouchMechanics()
        {
            if ((!inputSuspended && !isDashing && grounded && vert < 0) ||
                (!inputSuspended && !isDashing && grounded && vert < 0 && horiz > 0) ||
                (!inputSuspended && !isDashing && grounded && vert < 0 && horiz < 0))
            {
                anim.SetBool("Crouching", true);
                crouching = true;
                CollidersCrouching();
            }
            else
            {
                anim.SetBool("Crouching", false);
                crouching = false;
                CollidersNormal();
            }

        }

        void HandlePotion()
        {
            // Potion cooldown timer
            if (!Stats.Instance.potionReady)
            {
                potionTimer -= Time.deltaTime;
                HUD.Instance.potion.text = Mathf.RoundToInt(potionTimer).ToString();
                HUD.Instance.potion.color = Color.gray;
            }

            if (potionTimer <= 0)
            {
                Stats.Instance.potionReady = true;
                HUD.Instance.potion.text = "POTION";
                HUD.Instance.potion.color = Color.red;
            }

            // Use potion
            if (Input.GetKeyDown(InputManager.Instance.potion_gamepad) || Input.GetKeyDown(InputManager.Instance.potion_keyboard))
            {
                if (!Stats.Instance.potionReady)
                {
                    HUD.Instance.ShowMessage("Potion not ready.", Color.red, 10, 3f);
                    return;

                }
                else if (Stats.Instance.currentHP == Stats.Instance.MaxHealth.Value)
                {
                    HUD.Instance.ShowMessage("Already at full health.", Color.red, 10, 3f);
                    return;
                }
                else
                {
                    Stats.Instance.GainHP(Mathf.RoundToInt(Stats.Instance.MaxHealth.Value * Stats.Instance.potionHealPercentage));
                    potionTimer = Stats.Instance.CalculateCooldown(Stats.Instance.potionBaseCooldown);
                    Stats.Instance.potionReady = false;
                }

            }

        }


        public IEnumerator PauseX(float waitTime)
        {
            canMove = false;
            yield return new WaitForSeconds(waitTime);
            canMove = true;
        }

        public IEnumerator PauseY(float waitTime)
        {
            controller.suspendGravity = true;
            yield return new WaitForSeconds(waitTime);
            controller.suspendGravity = false;
        }

        IEnumerator DoubleJumpDelay(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            jumpState = 1;
            canDoubleJump = true;
            anim.SetInteger("JumpState", jumpState);

        }

        public IEnumerator Hitflash(float duration)
        {
            Material oldMat;

            // Flash healthbar
            StartCoroutine(HUD.Instance.HealthBarFlash(MaterialRef.Instance.white, duration));

            // Hitflash
            if (!spriteRenderer.material == MaterialRef.Instance.white)
                oldMat = spriteRenderer.material;
            else
                oldMat = MaterialRef.Instance.normal;

            spriteRenderer.material = MaterialRef.Instance.white;
            yield return new WaitForSeconds(duration);
            spriteRenderer.material = oldMat;

        }

        protected IEnumerator Flicker(float duration)
        {
            // Flicker
            float timer = 0f;
            while (timer < duration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return m_FlickeringWait;
                timer += flickeringDuration;
            }

            spriteRenderer.enabled = true;
        }

        public IEnumerator Knockback(int knockbackDirection)
        {
            // TODO:  FIX THIS!
            // Dynamic rigidbody falling through obstacles
            // Similar problem with the Dash
            // use Translate?

            // Could be a cool idea to penalize player when knocked back to lose altitude, but need to control it

            anim.SetBool("KnockedBack", true);
            // v
            // rb.freezeRotation = true;
            //var direction = enemyDamager.gameObject.GetComponent<EnemyMovement>().GetPlayerDirection();
            //var direction = 1;
            // rb.mass = knockbackMass;
            // rb.gravityScale = knockbackGravity;
            // knockbackVel = knockbackDirection * knockbackVelocity;
            // rb.AddForce(knockbackVel);
            rb.velocity = knockbackDirection * knockbackVelocity;
            knockback = true;
            inputSuspended = true;
            yield return new WaitForSeconds(knockbackDuration);
            //rb.bodyType = RigidbodyType2D.Kinematic;
            anim.SetBool("KnockedBack", false);
            knockback = false;
            inputSuspended = false;

            // yield return new WaitForSeconds(0);
        }

        void CalculateVelocity()
        {
            float targetVelocityX = directionalInput.x * Stats.Instance.MoveSpeed.Value;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocityX = velocity.x;
        }

        void CalculateGravity()
        {

            // Normal Gravity
            velocity.y += gravity * Time.deltaTime;

            // Fall Multiplier
            if (velocity.y < -3f)
                velocity.y += gravity * (fallMultiplier - 2f) * Time.deltaTime;
            if (velocity.y < -13f)
                velocity.y += gravity * (fallMultiplier - 1.85f) * Time.deltaTime;
            if (velocity.y < -19)
                velocity.y += gravity * (fallMultiplier - 1.7f) * Time.deltaTime;
            if (velocity.y < terminalVelocity && !grounded)
                velocity.y = terminalVelocity;

            velocityY = velocity.y;

        }


        void TurnAround()
        {
            if (canTurnAround)
            {
                facingRight = !facingRight;
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;

                if (grounded)
                    anim.SetTrigger("TurnAround");
            }
        }

        public void StartRespawnFlickering()
        {
            m_FlickerCoroutine = StartCoroutine(Flicker(Stats.Instance.respawnDuration));
        }

        public void StartFlickering()
        {
            m_FlickerCoroutine = StartCoroutine(Flicker(invulnerabilityDuration));
        }

        public void StopFlickering()
        {
            if (m_FlickerCoroutine != null)
                StopCoroutine(m_FlickerCoroutine);
            spriteRenderer.enabled = true;
        }

        public void MeleeAttack()
        {
            anim.SetTrigger(m_HashMeleeAttackPara);
            anim.SetTrigger("SwingSword");
        }


    }


}