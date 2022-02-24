using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class Pickup : MonoBehaviour, IPickup
    {
        /// <summary>
        /// Attached to ItemObject prefab
        /// Controls the interactable GUI for the dropped weapon or armor in the game scene
        /// Gets quality, type, sprite, and any animations assigned to it upon dropping
        /// ** All attributes and stats on the item are rolled upon PICKING UP the item **
        /// </summary>

        public IEquippable item { get; set; }
        public ItemQuality quality { get; set; }

        [HideInInspector]
        public Rigidbody2D rb { get; set; }
        public SpriteRenderer iconSpriteRenderer { get; set; }
        public SpriteRenderer bgSpriteRenderer { get; set; }
        public LayerMask layerMask { get; set; }
        public Sprite icon { get; set; }
        public Sprite  qualityBG { get; set; }
        public Animator animator { get; set; }

        public bool isColliding { get; set; }
        public bool hasDropped { get; set; }
        public float lifespan { get; set; }

        private void Start()
        {
            rb = GetComponentInParent<Rigidbody2D>();
            iconSpriteRenderer = GetComponentInParent<SpriteRenderer>();
            bgSpriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponentInParent<Animator>();

            iconSpriteRenderer.sprite = icon;
            bgSpriteRenderer.sprite = icon;

            if (Application.isPlaying)
                if (lifespan > 0) Destroy(rb.gameObject, lifespan);
 
        }

        private void Update()
        {
            if (hasDropped) return;
            isColliding = false;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.5f, layerMask);

            if (hit)
            {
                hasDropped = true;
                Destroy(rb);
            }

        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                // TODO: Display popup message like, "Press R1/Spacebar to pick up" etc

                if (isColliding) return;
                isColliding = true;

                // Pick up Item
                if (Input.GetKeyDown(InputManager.Instance.interact_keyboard) ||
                    (Input.GetKeyDown(InputManager.Instance.interact_gamepad)))
                {
                    if (Inventory.Instance.CanAddItem(item))
                    {
                        ItemCreator.Instance.CreateItem(quality);
                        Destroy(this.gameObject);

                        // TODO: After picking up, briefly display a small panel to the side showing the item details
                    }

                }

            }


        }








    }
}