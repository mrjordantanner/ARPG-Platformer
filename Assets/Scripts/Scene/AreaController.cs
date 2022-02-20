using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using Cinemachine;

namespace Assets.Scripts
{
    public class AreaController : MonoBehaviour
    {

        public static AreaController Instance;
        // Controls camera confiner, segment transitions,
        // Holds empty "frames" for static and parallax art layers
        // Handles parallax motion
        // Active Segment holds references to the art for that area 
        // AreaController does the actual loading on SegmentTransition
        [HideInInspector]
        public Module.AreaType activeArea;
        [HideInInspector]
        public Module activeModule;
        [HideInInspector]
        public Segment previousSegment, activeSegment;
        [HideInInspector]
        public Layout activeLayout;
        // public GridCell activeGridCell; //TODO
        [HideInInspector]
        public CinemachineConfiner cinemachineConfiner;
        [HideInInspector]
        public bool segmentTransition = false;


        // empty frame attached to Main Camera that is 
        // filled with the appropriate sprite from the activeSegment
        public SpriteRenderer StaticBackgroundFrame;

        /*
        // Empty array to be loaded into from ActiveSegment
        [HideInInspector]
        public GameObject[] ActiveParallaxElements;

        // empty parent gameobject attached to camera - 
        // instantiate parallax elements and make this the parent


        // the proportion of the camera's movement based on the layer's Z position
        private float[] parallaxScales;    

        // must be positive
        // TODO:  add separate vertical and horizontal smoothing
        public float smoothing = 1f;

        private Transform cam;
        private Vector3 previousCamPos;
        */
        public GameObject ParallaxElementsHolder;

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

        void Start()
        {
            cinemachineConfiner = FindObjectOfType<CinemachineConfiner>();

            if (activeModule != null)
                activeArea = activeModule.areaType;

            /*
            // Parallax
            cam = Camera.main.transform;    // use main cam or use cinemachine cam?
            previousCamPos = cam.position;
            InitializeParallax();
            */
        }

        /*
        void InitializeParallax()
        {
            parallaxScales = new float[ActiveParallaxElements.Length];

            for (int i = 0; i < ActiveParallaxElements.Length; i++)
            {
                parallaxScales[i] = ActiveParallaxElements[i].transform.position.z * -1;
            }
        }
        */
        private void Update()
        {
            // if (ActiveParallaxElements.Length > 0)
            //     ParallaxMotion();
        }
        /*
        void ParallaxMotion()
        {

            //for each background
            for (int i = 0; i < ActiveParallaxElements.Length; i++)
            {
                if (ActiveParallaxElements[i] == null) continue;

                //the parallax is the opposite of the camera movement b/c the previous frame multiplied by the scale
                float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScales[i];
                float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScales[i];

                //set a target x position which is the current position plus the parallax
                float backgroundTargetPosX = ActiveParallaxElements[i].transform.position.x + parallaxX;
                float backgroundTargetPosY = ActiveParallaxElements[i].transform.position.y + parallaxY;

                //create a target position which is the background's current position with it's target x position
                Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgroundTargetPosY, ActiveParallaxElements[i].transform.position.z);

                //fade between current position and the target position using lerp
                ActiveParallaxElements[i].transform.position = Vector3.Lerp(ActiveParallaxElements[i].transform.position, backgroundTargetPos, smoothing * Time.deltaTime);

            }
            //set the previous cam position to the camera's position at the end of the frame
            previousCamPos = cam.position;
        }
        */

        public IEnumerator SegmentTransition()
        {
            //TODO: Check what scene new segment is in
            // and set it to be active scene if not currently the active scene

            segmentTransition = true;
            StartCoroutine(ScreenFader.FadeSceneOut(0.1f));
            if (previousSegment != null) previousSegment.DisableParallax();

            yield return new WaitForSeconds(0.12f);
            activeSegment.DisableMask();
            cinemachineConfiner.InvalidatePathCache(); // clears cached confiner data
            cinemachineConfiner.m_BoundingShape2D = activeSegment.cameraConfiner;
            if (previousSegment != null) previousSegment.EnableMask();
            activeSegment.LoadStaticBackground();

            yield return new WaitForSeconds(0.12f);
            StartCoroutine(ScreenFader.FadeSceneIn(0.15f));
            segmentTransition = false;

            // Zoom in automatically if we are zoomed out and the new segment doesn't allow zooming
            if (!activeSegment.allowZooming && CameraZoom.Instance.currentZoom > CameraZoom.Instance.zoomedInSize)
                CameraZoom.Instance.zooming = true;

            // TODO: Ensure parallax layers don't get shifted around, create some limiting factor, or reset their position
            // Probably Has to do with camera position being set incorrectly at times when transitioning quickly
            activeSegment.SetParallaxCamPos();
            activeSegment.EnableParallax();

        }



        /*
        public void LoadParallaxElements(Segment segment)
        {
            if (segment.parallaxElements.Length > 0)
                ActiveParallaxElements = segment.parallaxElements;

            InstantiateParallaxElements();
        }

        public void InstantiateParallaxElements()
        {
            if (ActiveParallaxElements.Length > 0)
            {
                foreach (var LayerObject in ActiveParallaxElements)
                {
                    if (LayerObject == null) continue;  // skip this array element if it exists but there's no Parallax prefab assigned to it
                    var newParallaxElement = LayerObject.GetComponent<ParallaxElements>();
                    var layerPosition = new Vector3(0, 0, newParallaxElement.position.z);
                    //  var newLayer = Instantiate(LayerObject, layerPosition, Quaternion.identity, ParallaxElementsHolder.transform);
                    var NewLayer = Instantiate(LayerObject, layerPosition, Quaternion.identity);
                    //NewLayer.transform.SetParent(ParallaxElementsHolder.transform, true);   // could use this to keep child's position when parenting to holder object
                    print(LayerObject + " position is " + LayerObject.transform.position);
                    // TODO:  Position and scale of instantiated object still not working properly
                    // Is it being reset when parented to the ParallaxElementsHolder?
                    // Position/scale possibly being "reset" in the ParallaxMotion method

                    // var newParallaxElement = newLayer.GetComponent<ParallaxElements>();
                    //newLayer.transform.localScale = newParallaxElement.scale;
                    // transform.position = newParallaxElement.position;

                }
            }

        }

        public void ClearParallaxElements()
        {
            var elements = ParallaxElementsHolder.GetComponentsInChildren<ParallaxElements>();
            foreach (var element in elements)
            {
                Destroy(element.gameObject);
            }

        }
        */



        public void AreaTransition(Module activeMod, Module.AreaType areaType)
        {
            // Transition code here
            // TODO: Fade music and ambient sound, change backgrounds
            // longer screen fade? - display area name on enter?
            activeArea = areaType;

        }


    }

}