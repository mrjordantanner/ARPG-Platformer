using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Collectible : MonoBehaviour
    {

        // To be used for objects that are collected simply by touching them
        //  Health Pickups, possibly gold or materials

        public enum Type { Health, Gold, CollectibleA, CollectibleB, CollectibleC, CollectibleD }
        public Type type;

        public bool grounded, hasDropped, usePhysics;
        public LayerMask layerMask;

        float timer;
        public float lifespan = 0f;
        public bool destroyOnPickup = true;
        public int value;
        public GameObject VFXPrefab;

        [Header("Motion")]
        public float frequency = 0f;
        public float motionX = 0f;
        public float motionY = 0f;
        public float motionZ = 0f;
        float timeCounter = 0f;

        bool isColliding;

        SpriteRenderer spriteRenderer;
        PlayerCharacter player;
        Rigidbody2D rb;

        void Start()
        {
            player = PlayerRef.Player;
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            if (lifespan != 0)
                Destroy(gameObject, lifespan);

        }


        private void Update()
        {
            // Sine movement
            timeCounter += Time.deltaTime * frequency;
            transform.position += new Vector3(Mathf.Cos(timeCounter) * motionX, Mathf.Sin(timeCounter) * motionY, Mathf.Sin(timeCounter) * motionZ);

            isColliding = false;

            // Flicker
            if (lifespan != 0)
            {
                if (timer < lifespan)
                    timer += Time.deltaTime;

                // Warning flash between 75% and 100% complete
                if (timer >= (lifespan * .75))
                {
                    StartCoroutine(Flicker());
                }
            }

            if (timer >= lifespan)
            {
                timer = 0f;
                StopCoroutine(Flicker());
                spriteRenderer.enabled = true;
            }

            // TODO: Maybe use Translate instead of RB movement for this behavior
            // Not landing correctly?
            // Raycast for grounding
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.1f, layerMask);
            if (hit) grounded = true;

            if (grounded && !hasDropped)
            {
                hasDropped = true;
                Destroy(rb);
            }

        }


        IEnumerator Flicker()
        {
            float timer = 0f;

            while (timer < lifespan)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
                timer += 0.1f;
            }

            spriteRenderer.enabled = true;
        }


        void OnTriggerEnter2D(Collider2D other)
        {

            if (other.gameObject.CompareTag("Player") && !player.respawning)
            {
                if (isColliding) return;
                isColliding = true;

                Stats.Instance.PickupCollectible(this);

                // Spawn VFX Prefab
                if (VFXPrefab != null)
                    Instantiate(VFXPrefab, player.transform.position, Quaternion.identity, player.transform);

                // if (destroyOnPickup)
                Destroy(gameObject);

            }

        }


    }


}