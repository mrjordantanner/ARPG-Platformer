using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts
{
    [RequireComponent(typeof(Controller2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyMovement : MonoBehaviour
    {
        // Responsible for basic enemy physics and movement control, not behavior
        /*
        Types of Enemy Physics

        Type 1 - Small-Medium size enemies that fully collide with each other and can climb each other
            - Can result in large piles of enemies to have fun with physics 
            - blowing them away with point effector, dashing through, etc
            - Kinematic/Dynamic Rigidbody
        Type 2 - Medium-Large size enemies that collide horizontally with each other but do not climb
        - Kinematic Rigidbody
        */

        public enum TerrainType { Ground, Air }
        public TerrainType terrainType;

        public bool alwaysFacePlayer;
        int playerDirection;
        int facingDirection;

        [Header("Physics")]
        //public bool canStack;        // Will be used to control enemy collision/"climbing" behavior
        float gravity = -15.27f;
        float fallMultiplier = 2.25f;
        float terminalVelocity = -30f;

        // [HideInInspector]
        public Vector2 velocity;

        //[HideInInspector]
        [Range(-1, 1)] public int moveVectorX;
        //[HideInInspector]
        [Range(-1, 1)] public int moveVectorY;

        [Header("Follow Lock")]
        public bool airFollowLockX;
        public bool airFollowLockY;

        PlayerCharacter player;
        EnemyCharacter enemy;

        /*
        [Header("Move Then Wait")]
        public float minWanderMoveTime = 0.2f;
        public float maxWanderMoveTime = 1f;
        public float minWanderWaitTime = 1.5f;
        public float maxWanderWaitTime = 5f;
        public float waitTimer, waitTime, moveTimer, moveTime;
        public bool waiting = true;
        public bool moving;

        [Header("Avoid Player")]
        public bool movingAway;
        public float minAvoidMoveTime = 0.5f, maxAvoidMoveTime = 1.5f, minAvoidWaitTime = 2f, maxAvoidWaitTime = 5f;

        [Header("Follow Behavior")]
        public float smoothTime = 2.0f;
        public bool lockXAxis;
        public bool lockYAxis;

        [Header("Patrol Movement")]
        public bool patrolMovement;
        public Vector3[] localWaypoints;
        Vector3[] globalWaypoints;
        public float patrolSpeed;
        public bool cyclic;
        public float patrolWaitTime;
        [Range(0, 2)]
        public float easeAmount;
        int fromWaypointIndex;
        float percentBetweenWaypoints;
        float nextMoveTime;

        [Header("Jump Movement")]
        public bool randomizeJumpTime;
        public bool randomizeJumpDirection;
        public bool jumpTowardsPlayer;
        public Vector2 jumpVelocity;
        public float jumpTimer;
        public float jumpTime;
        public float minJumpWaitTime;
        public float maxJumpWaitTime;
        public int jumpState;

        [Header("Behavior States")]
        public bool move;
        public bool attacking;
        public bool patrolling;
        public bool following;
        */

        [Header("Misc States")]
        public bool canMove = true;
        public bool dead;
        public bool grounded;
        public bool ceilinged;
        public bool facingRight;
        public bool suspendGravity;

        [HideInInspector]
        public Controller2D controller;
        IEnemyBehavior enemyBehavior;
        // public Animator anim;

        Rigidbody2D rb;
        // GameObject EnemyUI;
        // float uiScale;


        void Start()
        {
            enemyBehavior = GetComponentInChildren<IEnemyBehavior>();
            //enemyBehavior = GetComponentInChildren(typeof(IEnemyBehavior)) as IEnemyBehavior;

            player = PlayerRef.Player;
            enemy = GetComponent<EnemyCharacter>();
            //anim = enemy.gameObject.GetComponent<Animator>();
            controller = GetComponent<Controller2D>();
            rb = GetComponent<Rigidbody2D>();

            //uiScale = enemy.enemyUI.gameObject.transform.localScale.x;
        }



        void Update()
        {
            // grounded = controller.grounded;
            grounded = controller.collisions.below;
            ceilinged = controller.collisions.above;

            if (controller.collisions.below || controller.collisions.above)  // DONT CHANGE THIS
                velocity.y = 0;

            // Set animator params
            enemy.anim.SetInteger("moveVectorX", moveVectorX);
            enemy.anim.SetInteger("moveVectorY", moveVectorY);
            enemy.anim.SetBool("Grounded", grounded);
            enemy.anim.SetFloat("VelocityY", velocity.y);

            if (!suspendGravity && terrainType == TerrainType.Ground && canMove)
                CalculateGravity();

            velocity.x = enemy.MoveSpeed.Value * moveVectorX;

            if (terrainType == TerrainType.Air && canMove)
                velocity.y = enemy.MoveSpeed.Value * moveVectorY;

            if (canMove)
                controller.Move(velocity * Time.deltaTime, Vector2.zero);

            /*
            // Ground Movement
            if (terrainType == TerrainType.Ground && canMove)
            {
                MoveGround(enemy.MoveSpeed.Value, moveVectorX);


            }
            // Air Movment
            else if (terrainType == TerrainType.Air && canMove)
            {
                MoveAir(enemy.MoveSpeed.Value, moveVectorX, moveVectorY);
            }
            */

            // Flip sprite
            // if (enemyBehavior.Aggro && alwaysFacePlayer)
            if (alwaysFacePlayer)
                FacePlayer();
            else if (moveVectorX > 0 && !facingRight)
                Flip();
            else if (moveVectorX < 0 && facingRight)
                Flip();

            if (facingRight)
                facingDirection = 1;
            else
                facingDirection = -1;

            // Reverse direction on collision
            //if (reverseOnCollision && (controller.collisions.left || controller.collisions.right))
            ////   moveVectorX *= -1;


        }


        // MOVEMENT CONTROLLERS
        // Ground Movement
        public void MoveGround(float speed, int vectorX)
        {
            // reverseOnCollision = true;
            //CalculateHorizontalVelocitySmooth();
            velocity.x = speed * vectorX;
            controller.Move(velocity * Time.deltaTime, Vector2.zero);
        }
        // Air Movement
        public void MoveAir(float speed, int vectorX, int vectorY)
        {
            velocity.x = speed * vectorX;
            velocity.y = speed * vectorY;
            controller.Move(velocity * Time.deltaTime, Vector2.zero);
        }

        // BEHAVIORS move to other script
        void MoveTowardsPlayer()
        {
            // Need to be able to stop enemies from stacking if desired - if (canStack) etc.
            // Experiment with alternate move code, combine with OnTriggerEnter2D or both

            if (airFollowLockX)
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, player.transform.position.y), enemy.MoveSpeed.Value * Time.deltaTime);
            else if (airFollowLockY)
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), enemy.MoveSpeed.Value * Time.deltaTime);
            else
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.MoveSpeed.Value * Time.deltaTime);
        }

        void MoveAwayFromPlayer()
        {
            // Mathf.Clamp(moveVectorX, -1, 1);
            //Mathf.Clamp(moveVectorY, -1, 1);

            if (airFollowLockX)
                transform.position = -Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, player.transform.position.y), enemy.MoveSpeed.Value * Time.deltaTime);
            else if (airFollowLockY)
                transform.position = -Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), enemy.MoveSpeed.Value * Time.deltaTime);
            else
                transform.position = -Vector2.MoveTowards(transform.position, player.transform.position, enemy.MoveSpeed.Value * Time.deltaTime);
        }




        // TODO: ENEMY WANDER within CONTAINING AREA dicated by spawner -
        // Spawner could act as containment area for spawned enemies while idle
        /*
    void Wander()
    {
        MoveThenWait("Wander", minWanderMoveTime, maxWanderMoveTime, minWanderWaitTime, maxWanderWaitTime);
    }

    void AvoidPlayer()
    {
        MoveThenWait("Avoid", minAvoidMoveTime, maxAvoidMoveTime, minAvoidWaitTime, maxAvoidWaitTime);
    }

    void ConstantMove()
    {
        if (moveVectorX == 0)
            RandomLeftRight();
        moving = true;
    }

    // MOVE THEN WAIT
    void MoveThenWait(string type, float minMoveTime, float maxMoveTime, float minWaitTime, float maxWaitTime)
    {
        // Increment appropriate timer
        if (waiting)
            waitTimer += Time.deltaTime;

        if (moving)
            moveTimer += Time.deltaTime;

        // Start moving after waitTime is up
        if (waitTimer >= waitTime)
        {
            if (type == "Wander")
                RandomLeftRight();

            if (type == "Avoid")
                movingAway = true;

            //  Choose move Duration
            moveTime = Random.Range(minMoveTime, maxMoveTime);

            // Reset timer and bools
            waitTimer = 0;
            waiting = false;
            moving = true;
            move = true;
        }

        if (movingAway && !calculateGravity)
        {
            // Flying enemy
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -1 * enemy.MoveSpeed.Value * Time.deltaTime);
        }
        else if (movingAway && calculateGravity)
        {
            // Ground enemy
            moveVectorX = -playerDirection;
        }

        // Stop moving after moveTime is up
        if (moving && (moveTimer >= moveTime))
        {
            Stop();
            moveTimer = 0;
            movingAway = false;
            moving = false;
            move = false;
            waiting = true;

            // Set new wait time
            waitTime = Random.Range(minWaitTime, maxWaitTime);
        }

    }

    // RANDOM HOPPER MOVEMENT
    void JumpMovement()
    {
        // moveSpeed = jumpVelocity.x;

        if (randomizeJumpTime && jumpState == 0)
            jumpTime = Random.Range(minJumpWaitTime, maxJumpWaitTime);
        else
            jumpTime = maxJumpWaitTime;

        if (jumpState == 0 && jumpTimer < jumpTime)
            jumpTimer += Time.deltaTime;

        if (jumpState == 0 && jumpTimer >= jumpTime)
        {
            if (!grounded)
                return;

            if (randomizeJumpDirection)
            {
                float randomDir = Random.value;
                if (randomDir < .5)
                    jumpVectorX = -1;
                else
                    jumpVectorX = 1;
            }
            else if (jumpTowardsPlayer)
                jumpVectorX *= playerDirection;

            Jump(jumpVelocity, jumpVectorX);
        }

        if (grounded && jumpState == 1)
            JumpReset();
    }

    public void Jump(Vector2 jVelocity, int jVectorX)
    {
        Vector2 jumpVel = new Vector2(jVelocity.x * jVectorX, jVelocity.y);
        velocity = jumpVel;
        StartCoroutine(JumpDelay(0.1f));
    }

    IEnumerator JumpDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        jumpState = 1;
    }

    void JumpReset()
    {
        jumpTimer = 0;
        jumpState = 0;
        jumpVectorX = 0;
    }


    // PATROL MOVEMENT
    Vector3 CalculatePatrolMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * patrolSpeed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }
    */

        // QUICK ACTIONS

        void MoveLeft()
        {
            moveVectorX = -1;
        }

        void MoveRight()
        {
            moveVectorX = 1;
        }

        void MoveUp()
        {
            moveVectorY = 1;
        }

        void MoveDown()
        {
            moveVectorY = -1;
        }

        void Stop()
        {
            moveVectorX = 0;
            moveVectorY = 0;
        }

        public IEnumerator MoveGroundThenStop(int vectorX, float duration)
        {
            moveVectorX = vectorX;
            yield return new WaitForSeconds(duration);
            Stop();
        }

        public IEnumerator MoveAirThenStop(int vectorX, int vectorY, float duration)
        {
            moveVectorX = vectorX;
            moveVectorY = vectorY;
            yield return new WaitForSeconds(duration);
            Stop();
        }


        // M I S C   C A L C U L A T I O N S

        /*
        float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }
        */

        public void Flip()
        {
            facingRight = !facingRight;
            Vector2 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;

            if (enemy.enemyUI.gameObject != null)
            {
                Vector2 hbScale = enemy.enemyUI.gameObject.transform.localScale;
                hbScale.x *= -1;
                enemy.enemyUI.gameObject.transform.localScale = hbScale;
            }
        }

        void RandomLeftRight()
        {
            var randomDirection = Random.Range(0, 2);
            if (randomDirection == 1)
                moveVectorX = 1;
            else
                moveVectorX = -1;
        }



        void CalculateGravity()
        {
            velocity.y += gravity * Time.deltaTime;

            // Fall Multiplier
            if (velocity.y < -2f)
                velocity.y += gravity * (fallMultiplier - 2f) * Time.deltaTime;
            if (velocity.y < -13f)
                velocity.y += gravity * (fallMultiplier - 1.8f) * Time.deltaTime;
            if (velocity.y < -20)
                velocity.y += gravity * (fallMultiplier - 1.6f) * Time.deltaTime;
            if (velocity.y < terminalVelocity && !grounded)
                velocity.y = terminalVelocity;
        }

        public void FacePlayer()
        {
            if (canMove && player != null)
            {
                Vector2 faceDirection = Combat.Instance.GetPlayerDirection(transform);
                if (faceDirection.x > 0 && !facingRight)
                    Flip();
                else if (faceDirection.x < 0 && facingRight)
                    Flip();
            }
        }

        /*
        void OnDrawGizmosSelected()
        {
            if (localWaypoints != null)
            {
                Gizmos.color = Color.green;
                float size = .3f;

                for (int i = 0; i < localWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, aggroRadius);

        }

        */


    }
}