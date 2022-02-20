using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Segment : MonoBehaviour
    {
        // aka "SCENE"
        // Assigned to a Segment of an Area that is continuous and will use the same Camera Confiner

        // Handles screen transitions between Segments - fading, masking, etc
        // Loads and handles backgrounds and parallax motion
        // May control whether zooming is allowed and/or AutoZooming
        // Could control enemy "sleep" mode when player is not present

        Transform cam;
        PlayerCharacter player;
        public bool playerPresent;
        public bool allowZooming;
        //public bool autoZoom;

        BoxCollider2D segmentVolume;                // trigger for actual "physical" bounds of the segment
        [HideInInspector]
        public PolygonCollider2D cameraConfiner;    // confiner bounds
        [HideInInspector]
        public MeshRenderer segmentMask;
        [HideInInspector]
        public bool hasTriggered;
        [HideInInspector]
        public bool parallaxEnabled;

        float parallaxX, parallaxY, targetPosX, targetPosY;
        Vector3 currentCamPosition;
        Vector3 previousCamPosition; //= new Vector3();

        [Header("Artwork")]
        public Sprite staticBackground;

        public ParallaxLayer[] parallaxLayers;

        // must be positive
        // TODO:  add separate vertical and horizontal smoothing
        public float smoothing = 1f;


        private void Update()
        {
            currentCamPosition = cam.transform.position;
            hasTriggered = false;

            if (parallaxLayers.Length > 0 && playerPresent && cam != null && parallaxEnabled)
                HandleParallaxMotion();
        }

        private void Awake()
        {
            //cam = Camera.main.transform;    

            player = PlayerRef.Player;
            cameraConfiner = GetComponentInChildren<PolygonCollider2D>();
            segmentMask = GetComponent<MeshRenderer>();
            segmentVolume = GetComponent<BoxCollider2D>();

            if (segmentMask != null)
            {
                segmentMask.sortingLayerName = "Segment Mask";
                segmentMask.sortingOrder = 0;
                segmentMask.enabled = true;
            }

        }

        private void Start()
        {
            cam = Camera.main.transform;
        }

        public void LoadStaticBackground()
        {
            if (staticBackground != null)
                AreaController.Instance.StaticBackgroundFrame.sprite = staticBackground;
        }

        public void SetParallaxCamPos()
        {
            previousCamPosition = currentCamPosition;
        }

        void HandleParallaxMotion()
        {
            for (int i = 0; i < parallaxLayers.Length; i++)
            {
                if (parallaxLayers[i] == null) continue;

                parallaxX = (previousCamPosition.x - currentCamPosition.x) * -parallaxLayers[i].transform.position.z;
                parallaxY = (previousCamPosition.y - currentCamPosition.y) * -parallaxLayers[i].transform.position.z;

                targetPosX = parallaxLayers[i].transform.position.x + parallaxX;
                targetPosY = parallaxLayers[i].transform.position.y + parallaxY;

                Vector3 backgroundTargetPos = new Vector3(targetPosX, targetPosY, parallaxLayers[i].transform.position.z);

                parallaxLayers[i].transform.position = Vector3.Lerp(parallaxLayers[i].transform.position, backgroundTargetPos, smoothing * Time.deltaTime);

            }

            previousCamPosition = currentCamPosition;
        }

        public void EnableParallax()
        {
            parallaxEnabled = true;
            foreach (var layer in parallaxLayers)
            {
                layer.gameObject.SetActive(true);
            }
        }

        public void DisableParallax()
        {
            parallaxEnabled = false;
            foreach (var layer in parallaxLayers)
            {
                layer.gameObject.SetActive(false);
            }
        }


        /*
        public void InstantiateParallaxElements()
        {
            if (ParallaxObjects.Length > 0)
            {
                foreach (var LayerObject in ParallaxObjects)
                {
                    //if (LayerObject == null) continue;  // skip this array element if it exists but there's no Parallax prefab assigned to it
                    var parallaxElementOnPrefab = LayerObject.GetComponent<ParallaxElements>();
                    if (parallaxElementOnPrefab == null)
                    {
                        print("No ParallaxElement.cs found on" + LayerObject);
                        continue;
                    }

                    // Get stored position from script on prefab
                    var layerPosition = new Vector3(0, 0, parallaxElementOnPrefab.position.z);
                    var NewLayerObject = Instantiate(LayerObject, layerPosition, Quaternion.identity);

                    // NewLayerObject.transform.SetParent(AreaController.Instance.ParallaxElementsHolder.transform, true);   // could use this to keep child's position when parenting to holder object
                    // var newParallaxElement = NewLayerObject.GetComponent<ParallaxElements>();

                    // "Register" new layer
                    ActiveParallaxObjects.Add(NewLayerObject);
                    print(NewLayerObject + " position is " + NewLayerObject.transform.position);

                    // TODO:  Position and scale of instantiated object still not working properly
                    // Is it being reset when parented to the ParallaxElementsHolder?
                    // Position/scale possibly being "reset" in the ParallaxMotion method

                    // var newParallaxElement = newLayer.GetComponent<ParallaxElements>();
                    //newLayer.transform.localScale = newParallaxElement.scale;
                    // transform.position = newParallaxElement.position;


                }
            }

        }

        // Destroy instantiated layers
        public void ClearParallaxElements()
        {
            foreach (var activeObject in ActiveParallaxObjects)
            {
                if (activeObject != null)
                   Destroy(activeObject.gameObject);
            }

            ActiveParallaxObjects.Clear();
        }
        */



        /*
    // Get Enemies in this segment to do things with, for example setting to sleep/awake when player absent/present, etc
    public LayerMask enemyLayers;   //TODO: Assign these layers in code so don't have to do it on every segment manually
    public List<EnemyCharacter> enemiesInSegment = new List<EnemyCharacter>();

    public void GetEnemiesInSegment()
    {
        enemiesInSegment.Clear();
        Collider[] hitColliders = Physics.OverlapBox(segmentVolume.transform.position, segmentVolume.transform.localScale / 2, Quaternion.identity, enemyLayers);
        foreach (var coll in hitColliders)
        {
            var enemy = coll.gameObject.GetComponent<EnemyCharacter>();
            if (enemy != null)
                enemiesInSegment.Add(enemy);
        }

    }

    */



        // TRANSITIONS
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //  if (collision.gameObject.CompareTag("Player") && player.respawning)
            //      return;

            if (AreaController.Instance.activeSegment == this)
                return;

            if (collision.gameObject.CompareTag("Player") && !hasTriggered)
            {
                hasTriggered = true;
                playerPresent = true;

                AreaController.Instance.previousSegment = AreaController.Instance.activeSegment;
                AreaController.Instance.activeSegment = this;
                StartCoroutine(AreaController.Instance.SegmentTransition());
            }

        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerPresent = true;

                if (AreaController.Instance.activeSegment != this)
                    AreaController.Instance.previousSegment = this;

                if (AreaController.Instance.previousSegment == this && !AreaController.Instance.activeSegment.playerPresent)
                {
                    AreaController.Instance.previousSegment = AreaController.Instance.activeSegment;
                    AreaController.Instance.activeSegment = this;
                    StartCoroutine(AreaController.Instance.SegmentTransition());
                }

            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerPresent = false;

                if (AreaController.Instance.previousSegment == this)
                    return;

                if (AreaController.Instance.activeSegment == this && !player.respawning)
                {
                    AreaController.Instance.previousSegment = this;
                }

            }

        }


        IEnumerator EnableSegmentMask()
        {
            yield return new WaitForSeconds(0.12f);
            segmentMask.enabled = true;
        }

        IEnumerator DisableSegmentMask()
        {
            yield return new WaitForSeconds(0.12f);
            segmentMask.enabled = false;
        }

        public void DisableMask()
        {
            if (segmentMask != null)
                segmentMask.enabled = false;

        }

        public void EnableMask()
        {
            if (segmentMask != null)
                segmentMask.enabled = true;

        }




    }



}