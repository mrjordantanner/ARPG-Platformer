using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Hazard : MonoBehaviour
    {

        public float DamagePercentage;
        public float DamageInterval = 1f;
        public bool PlayerPresent;
        float DamageTimer;
        [HideInInspector]
        public int Damage;

        PlayerCharacter player;

        void Start()
        {
            player = PlayerRef.Player;
        }

        private void Update()
        {
            DamagePlayerOverTime();
        }

        public void DamagePlayerOverTime()
        {

            if (PlayerPresent && !player.dead && !player.respawning)
            {
                if (DamageTimer < DamageInterval)
                {
                    DamageTimer += Time.deltaTime;
                    return;
                }

                if (DamageTimer >= DamageInterval)
                {
                    // Damage is a constant based on percentage of player's max HP
                    Damage = Mathf.RoundToInt(Stats.Instance.MaxHealth.Value * DamagePercentage);
                    Stats.Instance.TakeDamage(Damage, true);
                    DamageTimer = 0;
                }

            }

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                PlayerPresent = true;

        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                PlayerPresent = true;

        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                PlayerPresent = false;

        }

    }

}