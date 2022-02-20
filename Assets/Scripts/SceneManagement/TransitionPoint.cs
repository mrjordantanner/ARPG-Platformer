using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class TransitionPoint : MonoBehaviour
    {
        public enum TransitionType
        {
            DifferentZone, DifferentNonGameplayScene, SameScene,
        }


        public enum TransitionWhen
        {
            ExternalCall, InteractPressed, OnTriggerEnter,
        }


        public GameObject transitioningGameObject;
        public TransitionType transitionType;
        [SceneName]
        public string newSceneName;
        public SceneTransitionDestination.DestinationTag transitionDestinationTag;
        public TransitionPoint destinationTransform;
        public TransitionWhen transitionWhen;
        public bool resetInputValuesOnTransition = true;
        public bool requiresInventoryCheck;
        public bool unloadCurrentScene;
    
        bool m_PlayerPresent;
        PlayerCharacter player;
        GameObject Player;

        void Start ()
        {
            Player = PlayerRef.Player.gameObject;
            transitioningGameObject = Player;


            if (transitionWhen == TransitionWhen.ExternalCall)
                m_PlayerPresent = true;

        }

        void OnTriggerEnter2D (Collider2D other)
        {

            if (other.gameObject.CompareTag("Player"))
            {
                m_PlayerPresent = true;

                if (ScreenFader.IsFading || SceneController.Instance.Transitioning)
                   return;

                //if (transitionDestinationTag == SceneTransitionDestination.DestinationTag.A)
                //    StartCoroutine(SceneController.TransitionToWorld(transitionDestinationTag));

                //if (newSceneName == SceneController.gameWorldName)
                //    StartCoroutine(SceneController.TransitionToWorld(transitionDestinationTag));
                //else if (newSceneName == SceneController.homeSceneName)
                //    StartCoroutine(SceneController.TransitionToHome(transitionDestinationTag));
                //else
                //    return;

                if (transitionWhen == TransitionWhen.OnTriggerEnter)
                {
                    print("Transition triggered at TransitionPoint: " + gameObject.name);
                    TransitionInternal();
                }

            }
        }

        void OnTriggerExit2D (Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                m_PlayerPresent = false;
            }
        }

        void Update ()
        {
            if (ScreenFader.IsFading || SceneController.Instance.Transitioning)
                return;

            if(!m_PlayerPresent)
                return;

            if (transitionWhen == TransitionWhen.InteractPressed)
            {
                if (PlayerInput.Instance.Interact.Down)
                {
                    TransitionInternal ();
                }
            }
        }

        protected void TransitionInternal ()
        {  

            if (transitionType == TransitionType.SameScene)
            {
                GameObjectTeleporter.Teleport (transitioningGameObject, destinationTransform.transform);
            }
            else
            {
                 SceneController.Instance.TransitionToScene (this);
                 //SceneController.TransitionToScene(this);


            }
        }

       // public void Transition ()
       // {
            // if(!m_PlayerPresent)
            //     return;
         //   print("Scene Transition");
        //    if(transitionWhen == TransitionWhen.ExternalCall)
         //       TransitionInternal ();
        ///
    }
}