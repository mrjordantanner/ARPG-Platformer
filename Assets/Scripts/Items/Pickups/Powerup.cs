using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

namespace Assets.Scripts
{
    public class Powerup : MonoBehaviour
    {

        public int value = 1;

        [Header("Sine Motion")]
        public float frequency = 0f;
        public float motionX = 0f;
        public float motionY = 0f;
        public float motionZ = 0f;
        float timeCounter = 0f;

        PlayerCharacter player;

        bool isColliding;

        void Start()
        {
            player = FindObjectOfType<PlayerCharacter>();

        }

        private void Update()
        {
            timeCounter += Time.deltaTime * frequency;
            transform.position += new Vector3(Mathf.Cos(timeCounter) * motionX, Mathf.Sin(timeCounter) * motionY, Mathf.Sin(timeCounter) * motionZ);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (isColliding) return;
                isColliding = true;

                //  stats.PickupPowerup(player.pickupDelay, value, gameObject);
            }
        }
    }
}