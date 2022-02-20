using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WeaponController : MonoBehaviour
    {
        public static WeaponController Instance;
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

        public WeaponAttackObject currentWeaponAttackObject;
        public Item.Weapon currentWeapon;
        public Animator weaponAnim;

        public WeaponAttackObject[] weaponPool;

        int weaponIndex = 0;

        private void Start()
        {
            weaponPool = Resources.LoadAll<WeaponAttackObject>("MeleeWeapons");

            // set default weapon
            SetWeapon(weaponPool[weaponIndex]);
        }

        private void Update()
        {
            if (!GameManager.Instance.gamePaused && GameManager.Instance.playerHasControl)
            {
                HandleInput();
            }

        }

        void HandleInput()
        {
            // Cycle weapon
            if (Input.GetKeyDown(InputManager.Instance.previousWeapon_keyboard) ||
                Input.GetKeyDown(InputManager.Instance.previousWeapon_gamePad))
                PreviousWeapon();

            if (Input.GetKeyDown(InputManager.Instance.nextWeapon_keyboard) ||
                Input.GetKeyDown(InputManager.Instance.nextWeapon_gamePad))
                NextWeapon();
        }

        void PreviousWeapon()
        {
            weaponIndex--;
            if (weaponIndex < 0)
                weaponIndex = weaponPool.Length - 1;

            SetWeapon(weaponPool[weaponIndex]);
        }

        void NextWeapon()
        {
            weaponIndex++;
            if (weaponIndex > weaponPool.Length - 1)
                weaponIndex = 0;

            SetWeapon(weaponPool[weaponIndex]);

        }

        public void SetWeapon(WeaponAttackObject weaponAttackObject)
        {
            currentWeaponAttackObject = weaponAttackObject;
            currentWeapon = currentWeaponAttackObject.weapon;
            weaponAnim = currentWeaponAttackObject.GetComponent<Animator>();

        }

    }
}
