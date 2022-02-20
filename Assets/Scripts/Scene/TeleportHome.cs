using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Gamekit2D;

namespace Assets.Scripts
{
    public class TeleportHome : MonoBehaviour
    {

        public GameObject ReturnDestinationPrefab;

        void Update()
        {
            if (Input.GetKeyDown(InputManager.Instance.homePortal_keyboard) ||
                Input.GetKeyDown(InputManager.Instance.homePortal_keyboard))
            {
                if (SceneManager.GetActiveScene().name == SceneController.Instance.homeSceneName)
                    return;
                else
                {
                    //Destroy any existing return destinations
                    var returnDest = SceneController.Instance.GetDestination(SceneTransitionDestination.DestinationTag.D);
                    if (returnDest != null)
                    {
                        print(returnDest.name + " destroyed.");
                        Destroy(returnDest.gameObject);
                    }

                    GoHome();
                }
            }

        }

        void GoHome()
        {
            // Instantiate return destination point
            var ReturnDest = Instantiate(ReturnDestinationPrefab, transform.position, transform.localRotation);

            // Set the return destination in the SceneController for later use
            SceneController.Instance.ReturnDestination = ReturnDest;

            // transition to Home return portal
            StartCoroutine(SceneController.Instance.Transition(SceneController.Instance.homeSceneName, false, SceneTransitionDestination.DestinationTag.B, TransitionPoint.TransitionType.DifferentZone));

        }
    }
}