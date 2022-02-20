using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MeleeAttack : MonoBehaviour
    {
        // Controls all weapon attacks
        // Receives input
        // Instantiates weapon objects
        // Triggers animations and delays

        // TODO:  Syncing of player animation with appearance/disappearance/animation 
        // of weapon object needs refining

        float horiz, vert;
        float attackDelay, attackReset;
        float offsetX, offsetY;
        Vector2 weaponPosition;
        Quaternion weaponRotation;

        public bool isCharging;
        public bool meleeAttackCharged;
        public float chargeTime = 2f;
        public float chargeDelayTime = 0.5f;

        PlayerCharacter player;

        Animator anim;
        [HideInInspector]
        public Animator weaponAnim;

        bool colliderActive;

        private void Start()
        {
            player = PlayerRef.Player;

        }

        void Update()
        {
            horiz = player.horiz;
            vert = player.vert;

            HandleButtonCharging();

            // Attack input
            if (((Input.GetKeyDown(InputManager.Instance.meleeAttack_keyboard) ||
                (Input.GetKeyDown(InputManager.Instance.meleeAttack_gamepad))
                && player.vert <= 0)
                && !player.inputSuspended && player.canAttack
                && player.canMove && !player.dead && 
                !player.respawning && !GameManager.Instance.gamePaused))// && !player.backflip))
            {
                if (WeaponController.Instance.currentWeapon == null)
                    Debug.LogError("Weapon is null.");
                else
                    StartCoroutine(Attack());
            }

        }

        IEnumerator ChargeDelay(float delay)
        {
            isCharging = false;
            yield return new WaitForSeconds(delay);
            isCharging = true;
        }

        void HandleButtonCharging()
        {
            if (Input.GetKeyDown(InputManager.Instance.meleeAttack_gamepad))
            {
                StartCoroutine(ChargeDelay(chargeDelayTime));
            }

            player.anim.SetBool("Charging", isCharging);

            if (InputManager.Instance.ButtonHoldCheck(InputManager.Instance.meleeAttack_gamepad, chargeTime - chargeDelayTime))
                meleeAttackCharged = true;

            if (meleeAttackCharged)
            {
                print("Melee Attack Charged");
                // TODO:  Play charged VFX and SFX

                // Charge released
                if (Input.GetKeyUp(InputManager.Instance.meleeAttack_gamepad))
                {

                    if (player.grounded)
                    {
                        print("CHARGE ATTACK!!");
                        // StartCoroutine(ChargedAttackGround());
                        StartCoroutine(Attack());
                    }
                    else
                    {
                        print("AERIAL CHARGE ATTACK!!");
                        // StartCoroutine(ChargedAttackAir());
                        StartCoroutine(Attack());
                    }

                    meleeAttackCharged = false;
                }

            }

        }

        IEnumerator Attack()
        {
            // Control player behavior during attack
            player.canAttack = false;
            player.canTurnAround = false;
            if (player.grounded)
                player.canMove = false;

            // Attack delay
            yield return new WaitForSeconds(WeaponController.Instance.currentWeapon.weaponAttackObject.attackDelay);

            // Player Animation settings
            player.anim.SetBool("Attacking", true);
            player.anim.SetTrigger("SwingSword");

            // TODO: ensure this is reading from the right place and calculating properly
            player.anim.SetFloat("AttackSpeed", WeaponController.Instance.currentWeapon.attackSpeedBase);    

            // To sync instantiation with sword swing animation
            yield return new WaitForSeconds(0.07f);

            Quaternion angle = Quaternion.identity;

            // DETERMINE ATTACK POSITION AND ROTATION
            // Ground attack
            if (player.grounded)
            {
                if (player.facingRight)
                    offsetY = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetGroundAttack.y;
                else
                    offsetY = -WeaponController.Instance.currentWeapon.weaponAttackObject.offsetGroundAttack.y;

                offsetX = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetGroundAttack.x;
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = Quaternion.identity;

            }

            // Air attack
            if (!player.grounded)
            {
                if (player.facingRight)
                    offsetY = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetAirAttack.y;
                else
                    offsetY = -WeaponController.Instance.currentWeapon.weaponAttackObject.offsetAirAttack.y;

                offsetX = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetAirAttack.x;
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = Quaternion.identity;

            }

            // Air attack down
            if (!player.grounded && horiz != 0 && vert < 0)
            {
                if (player.facingRight)
                {
                    offsetY = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetAirAttackDown.y;
                    angle = Quaternion.Euler(WeaponController.Instance.currentWeapon.weaponAttackObject.attackAngle);
                }
                else
                {
                    offsetY = -WeaponController.Instance.currentWeapon.weaponAttackObject.offsetAirAttackDown.y;
                    angle = Quaternion.Euler(-WeaponController.Instance.currentWeapon.weaponAttackObject.attackAngle);
                }

                offsetX = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetAirAttackDown.x;
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = angle;

            }

            // TODO: Fix these
            // Crouch attack
            if (player.crouching)
            {
                if (player.facingRight)
                    offsetY = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetCrouchAttack.y;
                else
                    offsetY = -WeaponController.Instance.currentWeapon.weaponAttackObject.offsetCrouchAttack.y;

                offsetX = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetCrouchAttack.x;
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = Quaternion.identity;

            }

            // Crouch attack down
            if (player.crouching && horiz != 0 && vert < 0)
            {
                if (player.facingRight)
                {
                    offsetY = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetCrouchAttackDown.y;
                    angle = Quaternion.Euler(WeaponController.Instance.currentWeapon.weaponAttackObject.attackAngle);
                }
                else
                {
                    offsetY = -WeaponController.Instance.currentWeapon.weaponAttackObject.offsetCrouchAttackDown.y;
                    angle = Quaternion.Euler(-WeaponController.Instance.currentWeapon.weaponAttackObject.attackAngle);
                }

                offsetX = WeaponController.Instance.currentWeapon.weaponAttackObject.offsetCrouchAttackDown.x;
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = angle;

            }

            // Instantiate Weapon Prefab
            var WeaponInstance = Instantiate(WeaponController.Instance.currentWeaponAttackObject.gameObject, weaponPosition, weaponRotation, player.transform);
            weaponAnim = WeaponInstance.GetComponent<Animator>();
            colliderActive = true;

            // Weapon Animation settings
            if (WeaponController.Instance.currentWeapon != null && WeaponController.Instance.currentWeapon.attackSpeedBase > 0)
                weaponAnim.SetFloat("AttackSpeed", WeaponController.Instance.currentWeapon.attackSpeedBase);
            else
                weaponAnim.SetFloat("AttackSpeed", 1.0f);

            weaponAnim.SetTrigger("Slash");

            // Play audio - tweak this as sounds are added
            var swingSound = Random.Range(0, 2);
            if (swingSound == 0)
                AudioManager.Instance.Play("Swing1");
            else
                AudioManager.Instance.Play("Swing2");

            // Attack Reset 
            yield return new WaitForSeconds(WeaponController.Instance.currentWeapon.weaponAttackObject.attackReset);
            player.canAttack = true;
            player.canMove = true;
            player.canTurnAround = true;
            Destroy(WeaponInstance);
            player.anim.SetBool("Attacking", false);
            player.anim.ResetTrigger("SwingSword");
            player.anim.SetFloat("AttackSpeed", 1.0f);

        }


    }


}