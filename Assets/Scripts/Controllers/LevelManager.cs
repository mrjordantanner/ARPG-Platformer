using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gamekit2D;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

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

        /// <summary>
        /// 
        ///  Manages home portal status
        ///  Opens and closes rifts
        ///  Manages rift timer and player progress
        ///  Manages all mechanics of the Rift system
        ///  Difficulty level and stats handled by
        ///     Difficulty.cs
        ///  
        /// 
        /// </summary>
        /// 

        // [Header("Home Scene")]
        // public SceneTransitionDestination HomeDefaultEntrance;  // default Start
        // public SceneTransitionDestination HomeReturnEntrance;   // from Game World
        // public TransitionPoint HomeDefaultPortal;   // to Game World default
        // public TransitionPoint HomeReturnPortal;    // to Game World return

        [Header("Level Mechanics")]
        public bool riftActive;
        public bool riftCompleted;
        public bool riftFailed;
        public int difficultyLevel;
        public int highestDifficultyLevelAvailable;

        public bool riftTimerRunning;
        public float riftTimer;
        public float riftTimeLimit;
        public float riftProgress;

        public int deaths;
        public float deathTimePenalty;
        public float deathTimePenaltyMax;
        public float deathGoldPenalty;

        PlayerCharacter player;

        void Start()
        {
            player = PlayerRef.Player;

        }


        void Update()
        {
            // UpdateRiftTimer();
            // UpdateUI();


        }









    }

}