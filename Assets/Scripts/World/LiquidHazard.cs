using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

namespace Assets.Scripts
{
    public class LiquidHazard : Hazard
    {
        [Header("Movement")]
        public bool riseOrFall;
        public Vector2 moveDirection;
        public float moveSpeed;
        public float baseMoveSpeed = 0.1f;
        float timer;
        public float interval;
        public float maxSizeY;
        public float cameraShakeAmount;
        float positionReduction = 3.125f;
        //public float reductionLerpTime = 0.5f;
        //float lerpFraction;
        //public bool shouldLerp;
        // Vector3 oldPosition, oldSize, newPosition, newSize;

        [Header("Physics")]
        public float affectMoveSpeed;
        public float affectGravity;

        void Update()
        {
            // Hazard Level Rise or Fall
            if (riseOrFall)
            {
                if (cameraShakeAmount > 0)
                    CameraShaker.Shake(cameraShakeAmount, Time.deltaTime);

                if (timer < interval)
                {
                    timer += Time.deltaTime;
                    return;
                }

                if (timer >= interval)
                {
                    // poison grows in both directions unless position is compensated
                    //   float hazardPositionY = transform.position.y;
                    float hazardSizeY = transform.localScale.y;
                    //   hazardPositionY += (moveDirection.y * moveSpeed) / positionReduction;
                    hazardSizeY += (moveDirection.y * moveSpeed);

                    //   transform.position = new Vector2(transform.position.x, hazardPositionY);
                    transform.localScale = new Vector2(transform.localScale.x, hazardSizeY);

                    if (maxSizeY > 0 && hazardSizeY >= maxSizeY)
                        hazardSizeY = maxSizeY;

                    timer = 0;
                }


            }



        }



        /*
        public void ReducePoisonLevel(float reductionAmount)
        {
            // quick approach
            float hazardSizeY = transform.localScale.y;
            hazardSizeY *= reductionAmount;
            transform.localScale = new Vector2(transform.localScale.x, hazardSizeY);

            // lerp approach
           // oldPosition = transform.position;
           // oldSize = transform.localScale;
           // newPosition = new Vector3(transform.position.x, transform.position.y / reductionAmount) * -positionReduction;
           // newSize = new Vector3(transform.localScale.x, oldSize.y / reductionAmount);
            // shouldLerp = true;


        }
        */



    }

}