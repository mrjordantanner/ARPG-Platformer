using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StartOnPressEnter : MonoBehaviour
    {

        public GameObject screenFade;
        public float startDelay = 2.5f;
        public string sceneToLoad;

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                screenFade.SetActive(true);
                AudioManager.Instance.Stop("BG Music");
                Invoke("PressStart", startDelay);
            }
        }

        void PressStart()
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}