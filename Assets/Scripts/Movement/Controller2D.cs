using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Controller2D : MonoBehaviour
    {

        public bool isPlayer;
        //Raycast Controller
        public LayerMask collisionMask;

        public const float skinWidth = .05f;
        const float dstBetweenRays = .25f;
        [HideInInspector]
        public int horizontalRayCount, verticalRayCount;
        [HideInInspector]
        public float horizontalRaySpacing, verticalRaySpacing;
        bool standingOnPlatform = false;
        [HideInInspector]
        public new CapsuleCollider2D collider;
        public RaycastOrigins raycastOrigins;

        [HideInInspector]
        public bool suspendGravity;
        // Collider2D[] m_GroundColliders = new Collider2D[3];
        // public Collider2D[] GroundColliders { get { return m_GroundColliders; } }

        // Controller2D
        public bool grounded;
        public bool climbing;
        public bool descending;
        public int facing;
        public Vector2 moveAmt;

        float maxClimbAngle = 60;
        float maxDescendAngle = 60;

        public CollisionInfo collisions;
        [HideInInspector]
        public Vector2 playerInput;

        Animator anim;
        [HideInInspector]
        public RaycastHit2D hit;

        // public GameObject Log;
        // public Log log;

        Vector2 m_PreviousPosition;
        Vector2 m_CurrentPosition;
        Vector2 m_NextMovement;

        PlayerCharacter player;

        public struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;
            public bool climbingSlope;
            public bool descendingSlope;
            public float slopeAngle, slopeAngleOld;
            public Vector2 moveAmountOld;
            public int faceDir;
            public bool fallingThroughPlatform;

            public void Reset()
            {
                above = below = false;
                left = right = false;
                climbingSlope = false;
                descendingSlope = false;

                slopeAngleOld = slopeAngle;
                slopeAngle = 0;
            }
        }

        public void Awake()
        {
            collider = GetComponent<CapsuleCollider2D>();
        }

        public void Start()
        {
            player = PlayerRef.Player;
            CalculateRaySpacing();
            collisions.faceDir = 1;
        }

        private void Update()
        {
            descending = collisions.descendingSlope;
            climbing = collisions.climbingSlope;
            facing = collisions.faceDir;
            grounded = collisions.below;

            if (grounded)
                DetectEdges();
            else
                standingOnLedge = false;
        }


        public void UpdateRaycastOrigins()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        public void CalculateRaySpacing()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        public void Move(Vector2 moveAmount)
        {
            Move(moveAmount, Vector2.zero, false);
        }


        public void Move(Vector2 moveAmount, bool standingOnPlatform)
        {
            Move(moveAmount, Vector2.zero, standingOnPlatform);
        }

        public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();

            collisions.Reset();
            collisions.moveAmountOld = moveAmount;
            //  playerInput = input;

            moveAmt = moveAmount;

            if (moveAmount.x != 0)
            {
                collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            if (moveAmount.y < 0)
            {
                DescendSlope(ref moveAmount);
            }

            HorizontalCollisions(ref moveAmount);

            if (moveAmount.y != 0)
            {
                VerticalCollisions(ref moveAmount);
            }

            if (suspendGravity) moveAmount.y = 0;

            if (float.IsNaN(moveAmount.x) || float.IsNaN(moveAmount.y))
                return;
            else
                transform.Translate(moveAmount);
        }


        void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

            if (Mathf.Abs(moveAmount.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                //Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                if (hit)
                {
                    if (hit.distance == 0) continue;

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && slopeAngle <= maxClimbAngle)
                    {
                        if (collisions.descendingSlope)
                        {
                            collisions.descendingSlope = false;
                            moveAmount = collisions.moveAmountOld;
                        }
                        float distanceToSlopeStart = 0;
                        if (slopeAngle != collisions.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            moveAmount.x -= distanceToSlopeStart * directionX;
                        }
                        ClimbSlope(ref moveAmount, slopeAngle);
                        moveAmount.x += distanceToSlopeStart * directionX;
                    }

                    if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        if (collisions.climbingSlope)
                        {
                            moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                        }

                        collisions.left = directionX == -1;
                        collisions.right = directionX == 1;
                    }
                }
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                //Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hit)
                {
                    if (hit.collider.tag == "Fallthrough" && isPlayer && player.canMove)
                    {
                        if (directionY == 1 || hit.distance == 0)
                            continue;

                        if (collisions.fallingThroughPlatform)
                            continue;

                        if (player.vert < 0 && player != null)// && ((Input.GetKeyDown(InputManager.Instance.jump_gamepad) || Input.GetKeyDown(InputManager.Instance.jump_keyboard))))
                        {
                            collisions.fallingThroughPlatform = true;
                            StartCoroutine(FallthroughPlatform(hit.collider.gameObject, 0.5f));
                            continue;
                        }
                    }
                    else if (hit.collider.tag == "One Way" && isPlayer && player.canMove)
                    {
                        if (directionY == 1 || hit.distance == 0)
                            continue;

                        if (collisions.fallingThroughPlatform)
                            continue;
                    }

                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                    }

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }

            if (collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != collisions.slopeAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        collisions.slopeAngle = slopeAngle;
                    }
                }
            }
        }

        void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
        {
            float moveDistance = Mathf.Abs(moveAmount.x);
            float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (moveAmount.y <= climbmoveAmountY)
            {
                moveAmount.y = climbmoveAmountY;
                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                collisions.below = true;
                collisions.climbingSlope = true;
                collisions.slopeAngle = slopeAngle;
            }
        }

        void DescendSlope(ref Vector2 moveAmount)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
            //Debug.DrawRay(rayOrigin, -Vector2.up, Color.yellow, 0.1f);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                        }
                    }
                }
            }
        }

        IEnumerator FallthroughPlatform(GameObject currentPlatform, float resetTime)
        {
            collisions.fallingThroughPlatform = true;
            yield return new WaitForSeconds(resetTime);
            collisions.fallingThroughPlatform = false;

        }

        RaycastHit2D edgeTest;
        public Vector2 edgeTestRayTarget;
        public float originOffsetX = 0.1f;
        public bool standingOnLedge;

        void DetectEdges()
        {
            Bounds colliderBounds = collider.bounds;
            colliderBounds.Expand(skinWidth * -2);

            var rayOriginBottomLeft = new Vector2(colliderBounds.min.x - originOffsetX, colliderBounds.min.y);
            var rayOriginBottomRight = new Vector2(colliderBounds.max.x + originOffsetX, colliderBounds.min.y);

            if (!player.facingRight)
            {
                edgeTest = Physics2D.Raycast(rayOriginBottomLeft, edgeTestRayTarget, skinWidth, collisionMask);
                Debug.DrawRay(rayOriginBottomLeft, edgeTestRayTarget, Color.yellow);
            }
            else
            {
                edgeTest = Physics2D.Raycast(rayOriginBottomRight, edgeTestRayTarget, skinWidth, collisionMask);
                Debug.DrawRay(rayOriginBottomRight, edgeTestRayTarget, Color.yellow);
            }

            if (!edgeTest && grounded)
                standingOnLedge = true;
            else
                standingOnLedge = false;
        }




    }


}