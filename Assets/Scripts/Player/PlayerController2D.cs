//Copyright @2018 sharpcoderr.com
//You are free to use this script in free or commercial projects
//Selling the source code of this script is not allowed

using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController2D : MonoBehaviour
    {
        public float maxSpeed = 2.57f;
        public float jumpHeight = 6.47f;
        public float playerHP = 100;
        public float gravity = -16f;
        public float maxJumpVelocity = 20f;
        [HideInInspector]
        public bool facingRight = true;
        public float moveDirection = 0;
        [HideInInspector]
        public Rigidbody2D r2d;
        [HideInInspector]
        public Collider2D mainCollider;
        [HideInInspector]
        public Vector2 playerDimensions;
        public bool isGrounded = false;
        public LayerMask layerMask;

        //Bot movement directions
        [HideInInspector]
        public float botMovement = 0;
        [HideInInspector]
        public float botVerticalMovement = 0;
        [HideInInspector]
        public bool botJump = false;
        [HideInInspector]
        public Transform t;
        [HideInInspector]
        public int selectedWeaponTmp = 0;

        float gravityScale;

        Controller2D controller2D;

        void Start()
        {
            controller2D = GetComponent<Controller2D>();
            mainCollider = GetComponent<Collider2D>();
            t = transform;

            selectedWeaponTmp = -100;

            facingRight = t.localScale.x > 0;

            playerDimensions = BotController2D.ColliderDimensions(GetComponent<Collider2D>());
        }

        void Update()
        {
            if (botMovement != 0 && (isGrounded || controller2D.moveAmt.x > 0.01f))
            {
                moveDirection = botMovement < 0 ? -1 : 1;
            }
            else
            {
                if (isGrounded || controller2D.moveAmt.x < 0.01f)
                    moveDirection = 0;
            }

            //Change facing position
            if (moveDirection != 0)
            {
                if (moveDirection > 0 && !facingRight)
                {
                    facingRight = true;
                }
                if (moveDirection < 0 && facingRight)
                {
                    facingRight = false;
                }
            }

            if (facingRight)
            {
                if (t.localScale.x < 0)
                {
                    t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                if (t.localScale.x > 0)
                {
                    t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
                }
            }

            if (botJump)
            {
                botJump = false;
                Jump();
            }

        }

        void FixedUpdate()
        {
            CalculateGravity();

            Bounds colliderBounds = mainCollider.bounds;
            Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0.1f, 0);
            //Check if player is grounded
            isGrounded = Physics2D.OverlapCircle(groundCheckPos, 0.25f, layerMask);

            Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, 0.25f, 0), isGrounded ? Color.green : Color.red);

            //Apply player velocity
            controller2D.Move(new Vector2(moveDirection, 0) * maxSpeed);

        }

        public void Jump()
        {
            if (isGrounded)
                controller2D.moveAmt.y = maxJumpVelocity;

        }

        public void Attack()
        {
            print(gameObject.name + " is Attacking");
            //TODO: Attack code
        }

        void CalculateGravity()
        {
            controller2D.moveAmt.y += gravity * Time.deltaTime;
        }
    }

}