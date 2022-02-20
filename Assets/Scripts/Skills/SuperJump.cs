using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class SuperJump : MonoBehaviour
    {
        [Header("Super Jump")]
        public float jumpDuration = 0.2f;
        public Vector2 jumpVelocity = new Vector2(0, 1000);
        public bool superJumping = false;
        public int jumpCost = 1;

        PlayerCharacter player;
        Rigidbody2D rb;
        Animator anim;
        KeyCombo superJumpCombo;
        TrailRenderer dashTrail;
        Controller2D controller;
        DashController dashController;

        private void Start()
        {
            player = PlayerRef.Player;
            controller = player.gameObject.GetComponent<Controller2D>();
            rb = player.gameObject.GetComponent<Rigidbody2D>();
            anim = player.PlayerGraphics.GetComponent<Animator>();
            dashController = GetComponent<DashController>();
            superJumpCombo = new KeyCombo(new string[] { "down", "up" });

        }

        void FixedUpdate()
        {
            if (superJumpCombo.Check() && player.canSuperJump && player.canMove)
            {
                if (jumpCost <= Stats.Instance.currentMP || Stats.Instance.resourceCostsRemoved)
                    StartCoroutine(StartSuperJump(jumpVelocity, jumpDuration));
                else
                    HUD.Instance.NotEnoughMP();

            }
        }

        IEnumerator StartSuperJump(Vector2 velocity, float duration)
        {
            if (!Stats.Instance.resourceCostsRemoved)
                Stats.Instance.LoseMP(jumpCost);

            dashController.CreateTrail(duration);

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            rb.freezeRotation = true;
            rb.gravityScale = 0;
            rb.AddForce(velocity);

            superJumping = true;
            controller.suspendGravity = true;
            player.calculateVelocity = false;
            player.inputSuspended = true;
            player.invulnerable = true;
            player.canAttack = false;
            player.canSuperJump = false;
            player.canDash = false;

            anim.SetBool("SuperJump", player.canSuperJump);
            anim.SetBool("IsSuperJumping", superJumping);

            yield return new WaitForSeconds(duration);
            ResetSuperJump();

        }

        void ResetSuperJump()
        {
            // onCooldown = true;

            rb.velocity = Vector2.zero;
            player.velocity.y = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.None;
            superJumping = false;
            controller.suspendGravity = false;
            player.calculateVelocity = true;
            player.inputSuspended = false;
            player.invulnerable = false;
            player.canAttack = true;
            player.canSuperJump = true;
            player.canDash = true;

            anim.SetBool("SuperJump", player.canSuperJump);
            anim.SetBool("IsSuperJumping", superJumping);

        }

    }
}