using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class EventController : MonoBehaviour
    {
        //  Handles menu interaction events, game pausing

        public static EventController Instance;
        public bool isPaused;
        bool pause, unpause;
        PlayerCharacter player;
        EventSystem eventSystem;
        [HideInInspector]
        public GameObject firstSelectedGameObject;
        [HideInInspector]
        public BaseEventData m_BaseEvent;

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

        private void Start()
        {
            player = PlayerRef.Player;
            eventSystem = GetComponent<EventSystem>();
            firstSelectedGameObject = eventSystem.firstSelectedGameObject;

        }

        //void Update()
        //{
        //    if (isPaused)
        //    {
        //        Time.timeScale = 0;
        //        player.inputSuspended = true;
        //    }

        //    else if (!isPaused)
        //    {
        //        Time.timeScale = 1;
        //        player.inputSuspended = false;
        //    }

        //}








    }

}