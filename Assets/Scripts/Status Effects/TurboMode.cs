using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

namespace Assets.Scripts
{
    public class TurboMode : MonoBehaviour
    {

        public bool on;

        [Range(1.0f, 2.0f)]
        public float gameSpeedBoost = 1.10f;
        [Range(1.0f, 2.0f)]
        public float animSpeedBoost = 1.50f;
        // public float damageBonusIncrement = 1.0f;       // per game speed unit
        // public float turboModeDamageBonus = 1.0f;      // after calculation
        public float spiritCost = 2.0f;
        public float spiritCostInterval = 0.5f;
        float spiritRegenAmount, resourceRegenAmount;

        public float minMoveSpeed = 3.5f;
        public float maxMoveSpeed = 7f;

        public float timer;
        public float duration;

        public GameObject EffectsVolume;

        GameObject Player;
        PlayerCharacter player;
        GameObject Stats;
        Stats stats;
        Animator anim;
        GhostTrails ghostTrails;
        DashController dashController;
        TurboMode turboMode;

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            player = Player.GetComponent<PlayerCharacter>();
            Stats = GameObject.FindGameObjectWithTag("PlayerStats");
            stats = Stats.GetComponent<Stats>();
            anim = GetComponent<Animator>();
            ghostTrails = GetComponent<GhostTrails>();
            dashController = Player.GetComponent<DashController>();

            //spiritRegenAmount = stats.spiritRegenAmount;

        }

        private void Update()
        {
            if (on)
            {
                Time.timeScale = gameSpeedBoost;
                ghostTrails.on = true;
                anim.SetFloat("SpeedBoost", animSpeedBoost);
                ////  if (stats.currentSpirit < spiritCost)
                //     StopTurboMode();

            }

        }

        public void StartTurboMode()
        {
            if (EffectsVolume != null)
                EffectsVolume.gameObject.SetActive(true);

            on = true;
            ghostTrails.enabled = true;
            ghostTrails.on = true;
            //  dashController.velocity = dashController.turboVelocity;

            //  spiritRegenAmount = stats.spiritRegenAmount;    // store old regen value
            //  stats.spiritRegenAmount = 0f;                  // stop regen

            //  resourceRegenAmount = stats.resourceRegenAmount;   
            //  stats.resourceRegenAmount = 2f;                  

            //  InvokeRepeating("DrainSpirit", 0f, spiritCostInterval);
        }

        private void DrainSpirit()
        {
            //stats.LoseSpirit(spiritCost);
        }

        public void StopTurboMode()
        {
            if (EffectsVolume != null)
                EffectsVolume.gameObject.SetActive(false);

            on = false;
            ghostTrails.on = false;
            ghostTrails.enabled = false;
            // dashController.velocity = dashController.standardVelocity;
            // CancelInvoke("DrainSpirit");

            //  stats.spiritRegenAmount = spiritRegenAmount;    // restore old regen value
            //  stats.resourceRegenAmount = resourceRegenAmount;

            Time.timeScale = 1.0f;
            anim.SetFloat("SpeedBoost", 1.0f);
        }




    }
}
