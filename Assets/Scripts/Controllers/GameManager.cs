using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Configuration;

namespace Assets.Scripts
{
public class GameManager : MonoBehaviour
    {
        public bool enteringText;
        public bool debugMode;
        public bool canPause;
        public bool playerHasControl;
        public bool gamePaused;
        public bool sceneTransition;

        PlayerCharacter player;
        Animator playerAnim;

        #region Singleton
        public static GameManager Instance;
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
            #endregion

            Initialize();
        }

        //void Start()
        //{


        //}

        public void Initialize()
        {
            player = PlayerRef.Player;
            player.gameObject.SetActive(true);

            canPause = true;

            MenuController.Instance.CloseMenuPanel(MenuController.Instance.gameMenu);
            //MenuController.Instance.SwapButtons(startButton.gameObject, resumeButton.gameObject);
            //returnToMenuButton.gameObject.SetActive(true);

            //PoolController.Instance.CreateEnemyPools();

            if (gamePaused)
                Unpause();

            // AudioManager.Instance.RestoreMusicVolume();   

            player.anim.ResetTrigger("StartAttacking");
            player.anim.SetBool("Attacking", false);

            player.spriteRenderer.enabled = true;
            player.dead = false;
            player.canMove = true;
            player.inputSuspended = false;

            Debug.Log("Game Initialized.");

        }

        //public void StartReload()
        //{
        //    StartCoroutine(Reload());
        //}

        //IEnumerator Reload()
        //{
        //    yield return StartCoroutine(ScreenFader.FadeSceneOut(1f));
        //    ReloadGame();
        //    StartCoroutine(ScreenFader.FadeSceneIn(1f));

        //}

        //public void ReloadGame()
        //{


        //}

        void Update()
        {
            // Toggle HUD
            if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Z) && canPause) MenuController.Instance.ToggleHUD();

            // Pause and Show Main Menu
            if (Input.GetKeyDown(KeyCode.M) && !gamePaused && canPause && !enteringText)
            {
                Pause();
            }
            else if (Input.GetKeyDown(KeyCode.M) && gamePaused && canPause && !enteringText)
            {
                Unpause();
            }

            // Reload game
            //if (Input.GetKeyDown(KeyCode.R) && !enteringText)
            //{
            //    SceneController.Instance.Initialize();
            //    // TODO: Add other controller init's here as necessary
            //}
        }

            public void SetEnteringTextState(bool state)
        {
            enteringText = state;
        }

        public void SetCursorVisibility(bool visible)
        {
            Cursor.visible = visible;
        }

        public void SetPausableState(bool isPausable)
        {
            canPause = isPausable;
        }

        //public IEnumerator SceneTransition(float fadeDuration)
        //{
        //    SetPausableState(false);
        //    yield return StartCoroutine(ScreenFader.FadeSceneOut(fadeDuration));
        //    Initialize();
        //    yield return StartCoroutine(ScreenFader.FadeSceneIn(fadeDuration));
        //    SetPausableState(true);
        //}

        public void Pause()
        {
            MenuController.Instance.CloseMenuPanel(MenuController.Instance.hudCanvasGroup);
            MenuController.Instance.OpenMenuPanel(MenuController.Instance.gameMenu);
            AudioManager.Instance.ReduceMusicVolume();

            SetCursorVisibility(true);
            gamePaused = true;
            Time.timeScale = 0;
            playerHasControl = false;
        }

        public void Unpause()
        {
            MenuController.Instance.OpenMenuPanel(MenuController.Instance.hudCanvasGroup);
            MenuController.Instance.CloseMenuPanel(MenuController.Instance.gameMenu);
            AudioManager.Instance.RestoreMusicVolume();

            SetCursorVisibility(false);
            gamePaused = false;
            Time.timeScale = 1;
            playerHasControl = true;

        }

        public IEnumerator SlowMotion(float slowMotionAmount, float slowMotionDuration)
        {
            var oldTimeScale = Time.timeScale;
            Time.timeScale = slowMotionAmount;
            yield return new WaitForSecondsRealtime(slowMotionDuration);
            Time.timeScale = oldTimeScale;

        }


        public void Quit()
        {

            //#if UNITY_EDITOR
            //            UnityEditor.EditorApplication.isPlaying = false;
            //#else
            //         Application.Quit();
            //#endif

#if UNITY_STANDALONE
            Application.Quit();
#endif

        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
            Application.Quit();
#endif
        }
    }


}
