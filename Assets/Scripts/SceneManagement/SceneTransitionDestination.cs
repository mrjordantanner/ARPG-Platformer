using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class SceneTransitionDestination : MonoBehaviour
    {
        public enum DestinationTag
        {
            A, B, C, D, E, F, G,
        }

        GameObject Player;

        public DestinationTag destinationTag;   
        public GameObject transitioningGameObject;
        public UnityEvent OnReachDestination;

    }
}