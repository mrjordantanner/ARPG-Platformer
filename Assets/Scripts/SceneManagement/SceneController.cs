using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Cinemachine;

namespace Assets.Scripts
{
    public class SceneController : MonoBehaviour
    {
        public Segment homeSegment;         
        CinemachineConfiner cinemachineConfiner;
        public GameObject ReturnDestination;
        SceneTransitionDestination destination;

        public Transform startPosition;

        protected Scene currentScene;
        [HideInInspector]
        public SceneTransitionDestination.DestinationTag defaultDestinationTag, returnDestinationTag;
        protected PlayerInput m_PlayerInput;
        protected bool m_Transitioning;

        [SceneName]
        public string homeSceneName = "Home Scene";
        [SceneName]
        public string gameWorldName = "Game World";

        Scene homeScene, globalScene, gameWorldScene;

        public bool Transitioning
        {
            get { return m_Transitioning; }
        }

        public static SceneController Instance;
        private void Awake()
        {
            #region Singleton
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            #endregion

            defaultDestinationTag = SceneTransitionDestination.DestinationTag.A;
            returnDestinationTag = SceneTransitionDestination.DestinationTag.D;

            globalScene = SceneManager.GetSceneByBuildIndex(0);
            homeScene = SceneManager.GetSceneByBuildIndex(1);
            gameWorldScene = SceneManager.GetSceneByBuildIndex(2);

            Initialize();

        }

        private void Start()
        {
           

        }

        public void Initialize()
        {
            SpawnPlayer();

            cinemachineConfiner = FindObjectOfType<CinemachineConfiner>();
            cinemachineConfiner.m_BoundingShape2D = homeSegment.cameraConfiner;
        }

        public void TransitionToScene(TransitionPoint transitionPoint)
        {
            StartCoroutine(Instance.Transition(transitionPoint.newSceneName, transitionPoint.unloadCurrentScene, transitionPoint.transitionDestinationTag, transitionPoint.transitionType));
        }

        public SceneTransitionDestination GetDestinationFromTag(SceneTransitionDestination.DestinationTag destinationTag)
        {
            return GetDestination(destinationTag);
        }

        //StackTrace stackTrace;

        public IEnumerator Transition(string newSceneName, bool unloadCurrentScene, SceneTransitionDestination.DestinationTag destinationTag, TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentZone)
        {
            //stackTrace = new StackTrace();
            //print("stackTrace !! " + stackTrace.GetFrame(1).GetMethod().Name);

            print("Transtition started");

            m_Transitioning = true;
            PersistentDataManager.SaveAllData();

            //yield return StartCoroutine(ScreenFader.FadeSceneOut(0.5f, ScreenFader.FadeType.Black));

            PersistentDataManager.ClearPersisters();
            if (unloadCurrentScene)
            {
                Scene currentScene = SceneManager.GetActiveScene();
                yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync(currentScene);
            }
            PersistentDataManager.LoadAllData();

            // yield return SceneManager.GetSceneByName(newSceneName).isLoaded;

            if (newSceneName != homeSceneName)
                yield return SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
            else yield return null;

            // This takes the player out of DontDestroyOnLoad, so maybe not what we want
            //  SceneManager.MoveGameObjectToScene(Player, SceneManager.GetSceneByName(newSceneName));

            SceneTransitionDestination entrance = GetDestination(destinationTag);
            PlayerRef.Player.gameObject.transform.position = entrance.transform.position;

           // yield return StartCoroutine(ScreenFader.FadeSceneIn(0.5f));

            m_Transitioning = false;

            print("Transtition finished");

            // Destroy return point once we're back from Home
            if (destinationTag == returnDestinationTag && currentScene.name != homeSceneName)
            {
                Destroy(entrance.gameObject);
                print("Return destination reached and destroyed");
            }


        }


        public SceneTransitionDestination GetDestination(SceneTransitionDestination.DestinationTag destinationTag)
        {
            SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();
            for (int i = 0; i < entrances.Length; i++)
            {
                if (entrances[i].destinationTag == destinationTag)
                    return entrances[i];
            }
            print("No entrance was found with the " + destinationTag + " tag.");
            return null;
        }

        public void SetEnteringGameObjectLocation(SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                print("Entering Transform's location has not been set.");
                return;
            }
            Transform entranceLocation = entrance.transform;
            Transform enteringTransform = PlayerRef.Player.gameObject.transform;
            enteringTransform.position = entranceLocation.position;
            enteringTransform.rotation = entranceLocation.rotation;

        }

        public void SpawnPlayer()
        {
            var PlayerPrefab = Resources.Load("Player/Player", typeof(GameObject)) as GameObject;
            var PlayerObject = Instantiate(PlayerPrefab, startPosition.position, Quaternion.identity);

            // Assign static reference to player
            PlayerRef.Player = PlayerObject.GetComponent<PlayerCharacter>();

            // Set up camera follow
            CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
            cam.Follow = PlayerObject.transform;

            var entrance = GetDestination(SceneTransitionDestination.DestinationTag.A);
            PlayerObject.transform.position = entrance.transform.position;



        }

        protected void SetupNewScene(TransitionPoint.TransitionType transitionType, SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                print("Restart information has not been set.");
                return;
            }

            if (transitionType == TransitionPoint.TransitionType.DifferentZone)
                SetZoneStart(entrance);

        }

        protected void SetZoneStart(SceneTransitionDestination entrance)
        {
            currentScene = entrance.gameObject.scene;
            //defaultDestinationTag = entrance.destinationTag;
        }

       // static IEnumerator CallWithDelay<T>(float delay, Action<T> call, T parameter)
       // {
       //     yield return new WaitForSeconds(delay);
       //     call(parameter);
       // }
    }
}