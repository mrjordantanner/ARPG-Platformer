using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TreasureChest : MonoBehaviour
    {
        bool isColliding;
        public bool hasBeenOpened;

        SpriteRenderer spriteRenderer;
        Animator animator;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            // InvokeRepeating("TreasureTest", 0, 2f);
        }

        private void Update()
        {
            isColliding = false;
        }


        // For Interaction
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                // TODO: Display popup message like, "Press R1/Spacebar to open" etc

                if (isColliding || hasBeenOpened) return;
                isColliding = true;

                // Open Chest
                if (Input.GetKeyDown(InputManager.Instance.interact_keyboard) || (Input.GetKeyDown(InputManager.Instance.interact_gamepad)))
                {
                    //LootController.Instance.DropEquipment(LootController.Instance.treasureChestSmall, gameObject.transform.position, true);
                    LootController.Instance.DropGold(LootController.Instance.goldSmall, transform.position, false);
                    spriteRenderer.sprite = spriteRenderer.sprite = Resources.Load<Sprite>("ItemIcons/Chest-1_Open");
                    hasBeenOpened = true;

                }

            }


        }


    }

}