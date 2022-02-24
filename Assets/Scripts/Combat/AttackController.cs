using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    /// <summary>
    /// Controls weapon attacks; receives input, instantiates AttackObject, stores WeaponTraits
    /// </summary>
    public class AttackController : MonoBehaviour
    {
        #region Singleton
        public static AttackController Instance;
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
        #endregion

        float offsetX, offsetY;
        Vector2 weaponPosition;
        Quaternion weaponRotation;

        //public bool isCharging;
        //public bool meleeAttackCharged;
        //public float chargeTime = 2f;
        //public float chargeDelayTime = 0.5f;

        // Set on item equip, read from WeaponTraits
        float attackDelay, attackDuration, attackReset;

        [HideInInspector]
        public Animator weaponAnim;
        Animator anim;
        PlayerCharacter player;

        Vector3 attackAngle = new Vector3(0, 0, -26);

        bool colliderActive;

        WeaponTraits weaponTraits = new WeaponTraits();
        public Dictionary<WeaponTrait, float> equippedWeaponTraits = new Dictionary<WeaponTrait, float>();

        AttackObject equippedAttackObject;

        private void Start()
        {
            player = PlayerRef.Player;
            UpdateEquippedWeapon();
        }

        void Update()
        {
            HandleAttackInput();
            // HandleButtonCharging();
        }

        void HandleAttackInput()
        {
            if (((Input.GetKeyDown(InputManager.Instance.meleeAttack_keyboard) ||
                (Input.GetKeyDown(InputManager.Instance.meleeAttack_gamepad))
                && player.vert <= 0)
                && !player.inputSuspended && player.canAttack
                && player.canMove && !player.dead &&
                !player.respawning && !GameManager.Instance.gamePaused))// && !player.backflip))
            {
                if (Equipment.Instance._weaponSlot == null)
                    Debug.LogWarning("Tried to attack but no weapon is equipped.");
                else
                    StartCoroutine(Attack());
            }
        }

        public void UpdateEquippedWeapon()
        {
            if (Equipment.Instance._weaponSlot != null)
            {
                Weapon equippedWeapon = (Weapon)Equipment.Instance._weaponSlot;
                equippedWeaponTraits = weaponTraits.GetTraits(equippedWeapon.type);

                var path = $"AttackObjects/{equippedWeapon.type.ToString()}";
                equippedAttackObject = Resources.Load<AttackObject>(path);
          
            }
            else
            {
                Debug.LogWarning("Tried to update equipped WeaponTraits and AttackObject but no weapon is equipped.");
            }
        }

        //IEnumerator ChargeDelay(float delay)
        //{
        //    isCharging = false;
        //    yield return new WaitForSeconds(delay);
        //    isCharging = true;
        //}

        //void HandleButtonCharging()
        //{
        //    if (Input.GetKeyDown(InputManager.Instance.meleeAttack_gamepad))
        //    {
        //        StartCoroutine(ChargeDelay(chargeDelayTime));
        //    }

        //    player.anim.SetBool("Charging", isCharging);

        //    if (InputManager.Instance.ButtonHoldCheck(InputManager.Instance.meleeAttack_gamepad, chargeTime - chargeDelayTime))
        //        meleeAttackCharged = true;

        //    if (meleeAttackCharged)
        //    {
        //        print("Melee Attack Charged");
        //        // TODO:  Play charged VFX and SFX

        //        // Charge released
        //        if (Input.GetKeyUp(InputManager.Instance.meleeAttack_gamepad))
        //        {

        //            if (player.grounded)
        //            {
        //                print("CHARGE ATTACK!!");
        //                // StartCoroutine(ChargedAttackGround());
        //                StartCoroutine(Attack());
        //            }
        //            else
        //            {
        //                print("AERIAL CHARGE ATTACK!!");
        //                // StartCoroutine(ChargedAttackAir());
        //                StartCoroutine(Attack());
        //            }

        //            meleeAttackCharged = false;
        //        }

        //    }

        //}

        IEnumerator Attack()
        {
            // Control player behavior during attack
            player.canAttack = false;
            player.canTurnAround = false;
            if (player.grounded)
                player.canMove = false;

            // Attack delay
            yield return new WaitForSeconds(equippedWeaponTraits[WeaponTrait.AttackDelay]);

            // Player Animation settings
            player.anim.SetBool("Attacking", true);
            player.anim.SetTrigger("SwingSword");

            player.anim.SetFloat("AttackSpeed", Stats.Instance.AttackSpeed.Value);

            // To sync instantiation with sword swing animation
            yield return new WaitForSeconds(0.07f);

            Quaternion angle = Quaternion.identity;

            // DETERMINE ATTACK POSITION AND ROTATION
            // Ground attack
            if (player.grounded)
            {
                if (player.facingRight)
                    offsetY = equippedWeaponTraits[WeaponTrait.GroundAttackY];
                else
                    offsetY = -equippedWeaponTraits[WeaponTrait.GroundAttackY];

                offsetX = equippedWeaponTraits[WeaponTrait.GroundAttackX];
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = Quaternion.identity;

            }

            // Air attack
            if (!player.grounded)
            {
                if (player.facingRight)
                    offsetY = equippedWeaponTraits[WeaponTrait.AirAttackY];
                else
                    offsetY = -equippedWeaponTraits[WeaponTrait.AirAttackY];

                offsetX = equippedWeaponTraits[WeaponTrait.AirAttackX];
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = Quaternion.identity;

            }

            // Air attack down
            if (!player.grounded && player.horiz != 0 && player.vert < 0)
            {
                if (player.facingRight)
                {
                    offsetY = equippedWeaponTraits[WeaponTrait.AirAttackDownY];
                    angle = Quaternion.Euler(attackAngle);
                }
                else
                {
                    offsetY = -equippedWeaponTraits[WeaponTrait.AirAttackDownY];
                    angle = Quaternion.Euler(attackAngle);
                }

                offsetX = equippedWeaponTraits[WeaponTrait.AirAttackDownX];
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = angle;

            }

            // Crouch attack
            if (player.crouching)
            {
                if (player.facingRight)
                    offsetY = equippedWeaponTraits[WeaponTrait.CrouchAttackY];
                else
                    offsetY = -equippedWeaponTraits[WeaponTrait.CrouchAttackY];

                offsetX = equippedWeaponTraits[WeaponTrait.CrouchAttackX];
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = Quaternion.identity;

            }

            // Crouch attack down
            if (player.crouching && player.horiz != 0 && player.vert < 0)
            {
                if (player.facingRight)
                {
                    offsetY = equippedWeaponTraits[WeaponTrait.CrouchAttackDownY];
                    angle = Quaternion.Euler(attackAngle);
                }
                else
                {
                    offsetY = -equippedWeaponTraits[WeaponTrait.CrouchAttackDownY];
                    angle = Quaternion.Euler(-attackAngle);
                }

                offsetX = equippedWeaponTraits[WeaponTrait.CrouchAttackDownX];
                Vector2 offset = new Vector2(offsetX, offsetY);
                weaponPosition = (Vector2)player.transform.position + offset * player.transform.localScale.x;
                weaponRotation = angle;

            }

            // Instantiate Weapon Prefab
            var WeaponInstance = Instantiate(equippedAttackObject.gameObject, weaponPosition, weaponRotation, player.transform);
            weaponAnim = WeaponInstance.GetComponent<Animator>();
            colliderActive = true;

            // Weapon Animation settings
            if (Stats.Instance.AttackSpeed.Value > 0)
                weaponAnim.SetFloat("AttackSpeed", Stats.Instance.AttackSpeed.Value);
            else
                weaponAnim.SetFloat("AttackSpeed", Stats.Instance.baseAttackSpeed);

            weaponAnim.SetTrigger("Slash");

            // Audio
            //var swingSound = Random.Range(0, 2);
            //if (swingSound == 0)
            //    AudioManager.Instance.Play("Swing1");
            //else
            //    AudioManager.Instance.Play("Swing2");

            // Attack Reset 
            yield return new WaitForSeconds(equippedWeaponTraits[WeaponTrait.AttackReset]);
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