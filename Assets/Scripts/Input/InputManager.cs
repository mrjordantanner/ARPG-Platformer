using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        [Header("Keyboard Input")]
        public KeyCode jump_keyboard;
        public KeyCode meleeAttack_keyboard, skillA_keyboard, skillB_keyboard, dash_keyboard, homePortal_keyboard, interact_keyboard, potion_keyboard, menu_keyboard,
            nextSkillA_keyboard, previousSkillA_keyboard, nextSkillB_keyboard, previousSkillB_keyboard, zoom_keyboard, nextWeapon_keyboard, previousWeapon_keyboard;

        [Header("Gamepad Input")]
        public KeyCode jump_gamepad;
        public KeyCode meleeAttack_gamepad, skillA_gamepad, skillB_gamepad, dash_gamepad, homePortal_gamepad, interact_gamepad, potion_gamepad, menu_gamepad,
            nextSkillA_gamepad, previousSkillA_gamepad, nextSkillB_gamepad, previousSkillB_gamepad, zoom_gamepad,
            nextWeapon_gamePad, previousWeapon_gamePad, spawnEnemies_gamePad;


        private float chargeCounter;


        public bool ButtonHoldCheck(KeyCode button, float chargeDuration)
        {
            if (Input.GetKey(button))
            {
                chargeCounter += Time.deltaTime;

                if (chargeCounter < chargeDuration)
                {
                    // Charging...
                    return false;
                }
                else
                {
                    // Charged
                    return true;
                }
            }

            if (Input.GetKeyUp(button))
            {
                // Charged attack
                //  if (chargeCounter >= chargeDuration)
                //  {
                //
                //  }

                // Button released
                chargeCounter = 0;

            }

            return false;
        }


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

        }


    }
}
