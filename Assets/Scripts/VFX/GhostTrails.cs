using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GhostTrails : MonoBehaviour
    {

        public bool on;
        float timer = 0f;
        float spawnTimer = 0f;
        public int sortingOrder = 12;
        // public float onDuration = 0f;

        List<GameObject> trailParts = new List<GameObject>();
        PlayerCharacter player;

        public float duration = 0.3f;

        public float repeatRate = 0.01f;

        [Range(0.0f, 1.0f)]
        public float trailOpacity = 0.8f;

        [Range(0.0f, 1.0f)]
        public float trailColorRed;

        [Range(0.0f, 1.0f)]
        public float trailColorGreen;

        [Range(0.0f, 1.0f)]
        public float trailColorBlue;

        GameObject GhostTrailsContainer;

        private void Start()
        {
            player = GetComponent<PlayerCharacter>();
            GhostTrailsContainer = new GameObject("GhostTrails");   // container object for sprite trails
        }


        private void Update()
        {

            if (on) // && onDuration > 0)
            {
                timer += Time.deltaTime;
                spawnTimer += Time.deltaTime;

                if (spawnTimer >= repeatRate)
                {
                    SpawnTrailPart();
                    spawnTimer = 0f;
                }

            }

            // if (timer >= onDuration) Reset();


        }

        void SpawnTrailPart()
        {


            GameObject trailPart = new GameObject();

            // Make trail parts children of an object so as not to clutter up the heirarchy
            trailPart.transform.SetParent(GhostTrailsContainer.transform);

            SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
            trailPart.gameObject.layer = 13;  // player layer
            trailPart.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            trailPartRenderer.sortingOrder = sortingOrder;
            trailPartRenderer.sortingLayerName = "Level";
            trailPartRenderer.sprite = player.PlayerGraphics.GetComponent<SpriteRenderer>().sprite;
            trailPart.transform.position = transform.position;
            trailPart.transform.localScale = transform.localScale;
            trailParts.Add(trailPart);

            StartCoroutine(FadeTrailPart(trailPartRenderer));
            // add an opacity fade out here before destroying
            Destroy(trailPart, duration);

        }


        IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
        {
            //Color color = trailPartRenderer.color;
            Color trailColor = new Color(trailColorRed, trailColorGreen, trailColorBlue, trailOpacity);
            //trailColor.a = trailOpacity; 
            trailPartRenderer.color = trailColor;
            yield return new WaitForEndOfFrame();
        }
        /*
        public void Reset()
        {
            on = false;
            timer = 0f;
            spawnTimer = 0f;
            duration = 0.3f;
            repeatRate = 0.01f;
            //onDuration = 0f;

        }
        */

    }

}