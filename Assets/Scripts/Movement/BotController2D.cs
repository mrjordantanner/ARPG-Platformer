using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class BotController2D : MonoBehaviour
    {
        //This script will handle bot control
        public enum BotType { Enemy, Friendly }
        public BotType botType = BotType.Enemy;
        public enum BotDifficulty { Easy, Medium, Hard }
        public BotDifficulty botDifficulty = BotDifficulty.Medium;
        public enum InitialState { Idle, Explore }
        public InitialState initialState = InitialState.Idle; //Should the Bot stand in place until approached or begin exploring the level right away
        public bool canJump = true; //Can this bot jump?

        public enum CurrentState { Idle, MovingLeft, MovingRight, Attack }
        CurrentState currentState;
        InitialState appliedState;
        PlayerController2D pc2d;
        RaycastHit2D hitLeft;
        RaycastHit2D hitRight;
        RaycastHit2D groundHit;
        Vector3 leftOrigin;
        Vector3 rightOrigin;
        float distanceLeft = -1;
        float distanceRight = -1;

        public LayerMask layerMask;
        public LayerMask playerLayerMask;


        float timeMotionless = 0.0f;
        bool statePause = false;

        float trVelocity;
        Vector3 previousPos;

        Collider2D[] detectedPlayers = new Collider2D[0];
        Collider2D[] previousDetectedPlayers = new Collider2D[0];
        PlayerController2D enemyToFollow;
        int followPriority = 0; //0 = Easy, 1 = Medium (This player inflicted the damage)

        [HideInInspector]
        public Transform t;

        bool checkingTotalEnemies = false;
        bool runAway = false;

        int attackingFromLeft = 0;
        int attackingFromRight = 0;

        //Limit attack rate for easy bots
        float attackTimer = 0;
        float nextAttackTime = 0;

        Camera mainCamera;
        float cameraWidth;

        void Start()
        {
            pc2d = GetComponent<PlayerController2D>();
            t = transform;
            appliedState = initialState;

            if (Random.Range(-10, 10) > 0)
            {
                StartCoroutine(StatePause(CurrentState.Idle, true));
            }
        }

        void FixedUpdate()
        {
            //Draw rays back and forth
            if (!mainCamera)
            {
                mainCamera = Camera.main;
                cameraWidth = mainCamera.aspect * mainCamera.orthographicSize;
            }

            rightOrigin = t.position + t.right * (pc2d.playerDimensions.x / 2f);
            hitRight = Physics2D.Raycast(rightOrigin, t.right, cameraWidth, layerMask);
            if (hitRight)
            {
                Debug.DrawLine(rightOrigin, hitRight.point, Color.red);
                distanceRight = hitRight.distance;
            }
            else
            {
                Debug.DrawLine(rightOrigin, rightOrigin + t.right * cameraWidth, Color.cyan);
                distanceRight = -1;
            }

            leftOrigin = t.position - t.right * (pc2d.playerDimensions.x / 2f);
            hitLeft = Physics2D.Raycast(leftOrigin, -t.right, cameraWidth, layerMask);
            if (hitLeft)
            {
                Debug.DrawLine(leftOrigin, hitLeft.point, Color.red);
                distanceLeft = hitLeft.distance;
            }
            else
            {
                Debug.DrawLine(leftOrigin, leftOrigin - t.right * cameraWidth, Color.cyan);
                distanceLeft = -1;
            }

            if (appliedState == InitialState.Explore)
            {
                if (currentState == CurrentState.Idle)
                {
                    if (!statePause)
                    {
                        //Decide which direction to move
                        if (distanceRight == -1 && distanceLeft == -1)
                        {
                            //Decide random direaction
                            currentState = Random.Range(-10, 10) > 0 ? CurrentState.MovingRight : CurrentState.MovingLeft;
                        }
                        else if (distanceRight == -1 && distanceLeft >= 0)
                        {
                            currentState = CurrentState.MovingRight;
                        }
                        else if (distanceRight >= 0 && distanceLeft == -1)
                        {
                            currentState = CurrentState.MovingLeft;
                        }
                        else if (distanceRight > distanceLeft)
                        {
                            currentState = CurrentState.MovingRight;
                        }
                        else if (distanceRight < distanceLeft)
                        {
                            currentState = CurrentState.MovingLeft;
                        }
                    }
                }
                else if (currentState == CurrentState.MovingLeft)
                {
                    if (!statePause && pc2d.isGrounded)
                    {
                        pc2d.botMovement = -1;
                        float jumpHeightTmp = pc2d.jumpHeight * 0.25f;

                        if (distanceLeft > 0 && distanceLeft < pc2d.playerDimensions.x)
                        {
                            if (hitLeft && canJump &&
                                !Physics2D.Linecast(t.position + t.up * jumpHeightTmp, (t.position + t.up * jumpHeightTmp) - t.right * pc2d.playerDimensions.x * 2, layerMask) &&
                                Random.Range(-2, 10) > 0
                            )
                            {
                                StartCoroutine(DoJump());
                            }
                            else
                            {
                                if (!enemyToFollow)
                                {
                                    StartCoroutine(StatePause(CurrentState.Idle, true));
                                }
                            }
                        }

                        /*if(!Physics2D.Linecast(t.position + t.up * jumpHeightTmp, (t.position + t.up * jumpHeightTmp) - t.right * pc2d.playerDimensions.x * 2)){
                            Debug.DrawLine(t.position + t.up * jumpHeightTmp, (t.position + t.up * jumpHeightTmp) - t.right * pc2d.playerDimensions.x * 2, Color.yellow);
                        }
                        else
                        {
                            Debug.DrawLine(t.position + t.up * jumpHeightTmp, (t.position + t.up * jumpHeightTmp) - t.right * pc2d.playerDimensions.x * 2, Color.red);
                        }*/

                        //Jump if there is no groun in front
                        groundHit = Physics2D.Raycast(leftOrigin, -t.up, pc2d.playerDimensions.y * 2.1f, layerMask);
                        if (groundHit)
                        {
                            Debug.DrawLine(leftOrigin, groundHit.point, Color.red);
                        }
                        else
                        {
                            Debug.DrawLine(leftOrigin, leftOrigin - t.up * (pc2d.playerDimensions.y * 2.1f), Color.blue);
                            if (canJump)
                            {
                                StartCoroutine(DoJump());
                            }
                            else
                            {
                                //StartCoroutine(StatePause(CurrentState.MovingRight, true));
                                StartCoroutine(CheckEnemiesEnumerator(1, 0, false));
                            }
                        }
                    }
                }
                else if (currentState == CurrentState.MovingRight)
                {
                    if (!statePause && pc2d.isGrounded)
                    {
                        pc2d.botMovement = 1;
                        float jumpHeightTmp = pc2d.jumpHeight * 0.25f;

                        if (distanceRight > 0 && distanceRight < pc2d.playerDimensions.x)
                        {
                            if (hitRight && canJump &&
                                !Physics2D.Linecast(t.position + t.up * jumpHeightTmp, (t.position + t.up * jumpHeightTmp) + t.right * pc2d.playerDimensions.x * 2, layerMask) &&
                                Random.Range(-2, 10) > 0
                            )
                            {
                                StartCoroutine(DoJump());
                            }
                            else
                            {
                                if (!enemyToFollow)
                                {
                                    StartCoroutine(StatePause(CurrentState.Idle, true));
                                }
                            }
                        }

                        //Jump if there is no ground in front
                        groundHit = Physics2D.Raycast(rightOrigin, -t.up, pc2d.playerDimensions.y * 2.1f, layerMask);
                        if (groundHit)
                        {
                            Debug.DrawLine(rightOrigin, groundHit.point, Color.red);
                        }
                        else
                        {
                            Debug.DrawLine(rightOrigin, rightOrigin - t.up * (pc2d.playerDimensions.y * 2.1f), Color.blue);
                            if (canJump)
                            {
                                StartCoroutine(DoJump());
                            }
                            else
                            {
                                StartCoroutine(CheckEnemiesEnumerator(0, 1, false));
                            }
                        }
                    }
                }
                else if (currentState == CurrentState.Attack)
                {
                    if (!statePause)
                    {
                        if (!enemyToFollow)
                        {
                            StartCoroutine(StatePause(CurrentState.Idle, true));
                        }
                        else
                        {
                            //Firing weapon
                            if (attackTimer >= nextAttackTime)
                            {
                                //Check if player is above us and jump
                                if (enemyToFollow.t.position.y > t.position.y && enemyToFollow.t.position.y - t.position.y > pc2d.playerDimensions.y * 0.95f)
                                {
                                    if (Random.Range(-5, 10) > 0)
                                    {
                                        StartCoroutine(DoJump());
                                    }
                                }

                                pc2d.Attack();

                                if (botDifficulty == BotDifficulty.Easy)
                                {
                                    attackTimer = 0;
                                    nextAttackTime = Random.Range(0.25f, 0.95f);
                                }
                                if (botDifficulty == BotDifficulty.Medium)
                                {
                                    attackTimer = 0;
                                    nextAttackTime = Random.Range(0.01f, 0.37f);
                                }
                                if (botDifficulty == BotDifficulty.Hard)
                                {
                                    attackTimer = 0;
                                    nextAttackTime = Random.Range(0.01f, 0.24f);
                                }
                            }
                            else
                            {
                                attackTimer += Time.deltaTime;
                            }


                            if (enemyToFollow && !checkingTotalEnemies)
                            {
                                if (enemyToFollow.t.position.x > t.position.x && !pc2d.facingRight)
                                {
                                    pc2d.facingRight = true;
                                }
                                if (enemyToFollow.t.position.x < t.position.x && pc2d.facingRight)
                                {
                                    pc2d.facingRight = false;
                                }

                                attackingFromLeft = 0;
                                attackingFromRight = 0;

                                //Check if there too many player attacking us and run away
                                for (int i = 0; i < detectedPlayers.Length; i++)
                                {
                                    if (detectedPlayers[i])
                                    {
                                        BotController2D bcTmp = detectedPlayers[i].GetComponent<BotController2D>();
                                        if (bcTmp && bcTmp.botType != botType && bcTmp.enemyToFollow == pc2d && bcTmp.currentState == CurrentState.Attack)
                                        {
                                            if (bcTmp.t.position.x > t.position.x)
                                            {
                                                attackingFromRight++;
                                            }
                                            else
                                            {
                                                attackingFromLeft++;
                                            }
                                        }
                                    }
                                }

                                //If the value playerHP from PlayerController2D get too low, and the bot is being attacked, increase the probability to run away
                                if (attackingFromRight >= 2 || attackingFromLeft >= 2 || (pc2d.playerHP < 70 && botDifficulty == BotDifficulty.Hard && (attackingFromRight > 0 || attackingFromLeft > 0)) || (pc2d.playerHP < 40 && botDifficulty == BotDifficulty.Medium && (attackingFromRight > 0 || attackingFromLeft > 0)))
                                {
                                    StartCoroutine(CheckEnemiesEnumerator(attackingFromLeft, attackingFromRight, false));
                                }
                            }
                        }
                    }
                }
            }

            trVelocity = ((t.position - previousPos).magnitude) / Time.deltaTime;
            previousPos = t.position;

            if (trVelocity < 0.01f && !statePause)
            {
                timeMotionless += Time.deltaTime;

                if (timeMotionless > 0.5f)
                {
                    StartCoroutine(StatePause(CurrentState.Idle, true));
                }
            }
            else
            {
                timeMotionless = 0;
            }

            //Detect and attack enemy players
            detectedPlayers = Physics2D.OverlapCircleAll(t.position, cameraWidth, playerLayerMask);

            if (!enemyToFollow)
            {
                if (!runAway)
                {
                    if (previousDetectedPlayers.Length != detectedPlayers.Length || previousDetectedPlayers[0] != detectedPlayers[0])
                    {
                        previousDetectedPlayers = detectedPlayers;

                        for (int i = 0; i < detectedPlayers.Length; i++)
                        {
                            BotController2D bcTmp = detectedPlayers[i].GetComponent<BotController2D>();
                            PlayerController2D pc2dTmp = null;
                            if (!bcTmp)
                            {
                                pc2dTmp = detectedPlayers[i].GetComponent<PlayerController2D>();
                            }
                            if ((pc2dTmp && botType == BotType.Enemy) || (bcTmp && bcTmp.botType != botType))
                            {
                                Vector3 enemyPos = bcTmp ? bcTmp.t.position : pc2dTmp.t.position;
                                float yDistance = Mathf.Abs(enemyPos.y - t.position.y);
                                if (yDistance < pc2d.playerDimensions.y * 2)
                                {
                                    if (!enemyToFollow || Mathf.Abs(enemyPos.x - t.position.x) < Mathf.Abs(enemyToFollow.t.position.x - t.position.x))
                                    {
                                        enemyToFollow = bcTmp ? bcTmp.pc2d : pc2dTmp;
                                        appliedState = InitialState.Explore;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                float yDistance = enemyToFollow.t.position.y - t.position.y;
                float xDistance = enemyToFollow.t.position.x - t.position.x;
                if (Mathf.Abs(yDistance) >= pc2d.playerDimensions.y * 2 || Mathf.Abs(xDistance) > cameraWidth || !enemyToFollow.enabled)
                {
                    enemyToFollow = null;
                }
                else
                {
                    if (Mathf.Abs(xDistance) > pc2d.playerDimensions.x * 1.45f)
                    {
                        if (!statePause && pc2d.botVerticalMovement == 0)
                        {
                            if (xDistance > 0)
                            {
                                if (currentState != CurrentState.MovingRight)
                                {
                                    statePauseCoroutine = StartCoroutine(StatePause(CurrentState.MovingRight, false));
                                }
                            }
                            else if (xDistance < 0)
                            {
                                if (currentState != CurrentState.MovingLeft)
                                {
                                    statePauseCoroutine = StartCoroutine(StatePause(CurrentState.MovingLeft, false));
                                }
                            }
                        }
                        else
                        {
                            StopPauseCoroutine();
                        }
                    }
                    else
                    {
                        if (pc2d.botVerticalMovement == 0)
                        {
                            if (currentState != CurrentState.Attack)
                            {
                                if (!statePause)
                                {
                                    statePauseCoroutine = StartCoroutine(StatePause(CurrentState.Attack, true));
                                }
                            }
                            else
                            {
                                if (Mathf.Abs(xDistance) < pc2d.playerDimensions.x / 3 && !checkingTotalEnemies)
                                {
                                    //print("Enemies are too close!!!");
                                    StartCoroutine(CheckEnemiesEnumerator(attackingFromLeft, attackingFromRight, true));
                                }
                            }
                        }
                    }
                }
            }
        }

        Coroutine statePauseCoroutine = null;

        void StopPauseCoroutine()
        {
            if (statePauseCoroutine != null)
            {
                StopCoroutine(statePauseCoroutine);
                statePauseCoroutine = null;
                statePause = false;
            }
        }

        IEnumerator StatePause(CurrentState newState, bool stopMovement)
        {
            //print("State pause");
            statePause = true;
            if (stopMovement)
            {
                pc2d.botMovement = 0;
                pc2d.botVerticalMovement = 0;
            }
            currentState = newState;

            if (newState == CurrentState.Attack && botDifficulty == BotDifficulty.Hard)
            {
                yield return new WaitForSeconds(Random.Range(0.15f, 0.45f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0.45f, 0.75f));
            }


            statePause = false;
        }

        IEnumerator DoJump()
        {
            //print("Do jump");
            statePause = true;
            pc2d.botJump = true;

            yield return new WaitForSeconds(0.65f);

            statePause = false;
        }

        IEnumerator CheckEnemiesEnumerator(int attackingFromLeft, int attackingFromRight, bool doNotRunAway)
        {
            checkingTotalEnemies = true;

            yield return new WaitForSeconds(Random.Range(0.27f, 0.75f));

            if (Random.Range(-10, 10) > 0)
            {
                runAway = true;
                enemyToFollow = null;

                if (attackingFromLeft > attackingFromRight)
                {
                    currentState = CurrentState.MovingRight;
                }
                else
                {
                    currentState = CurrentState.MovingLeft;
                }
            }

            checkingTotalEnemies = false;

            if (runAway)
            {
                if (doNotRunAway)
                {
                    //Simply walk away a bit
                    yield return new WaitForSeconds(Random.Range(0.37f, 0.75f));
                }
                else
                {
                    //Run away
                    yield return new WaitForSeconds(Random.Range(1.57f, 2.45f));
                }

                runAway = false;
            }
        }

        public static Vector2 ColliderDimensions(Collider2D sp)
        {
            return new Vector2(sp.bounds.max.x - sp.bounds.min.x, sp.bounds.max.y - sp.bounds.min.y);
        }
    }

}