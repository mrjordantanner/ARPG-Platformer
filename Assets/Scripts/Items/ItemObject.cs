using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class ItemObject : MonoBehaviour
    {
        // Attached to ItemObject prefab
        // represents a collectible Item in the game world
        // Gets quality, type, sprite, and any animations assigned to it upon dropping
        // All attributes and stats are rolled upon picking up the item

        public Item.Quality quality;


        [HideInInspector]
        public Rigidbody2D rb;
        public SpriteRenderer iconSpriteRenderer, bgSpriteRenderer;
        Animator animator;

        public Sprite icon, qualityBG;

        public bool isColliding;
        public LayerMask layerMask;
        public bool grounded;
        bool hasDropped;
        public float lifespan;

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
            isColliding = false;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.5f, layerMask);
            if (hit) grounded = true;

            if (grounded)
            {
                hasDropped = true;
                Destroy(rb);
            }
        }









    }
}