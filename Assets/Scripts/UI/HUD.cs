using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

namespace Assets.Scripts
{
    public class HUD : MonoBehaviour
    {
        #region Singleton
        public static HUD Instance;
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
        #endregion

        [Header("Post-Processing")]
        public PostProcessVolume effectsVolume_Normal;
        public PostProcessVolume effectsVolume_Death;

        //[Header("Menu")]
        //public CanvasGroup menuGroup;
        //public RectTransform menu;

        [Header("UI Elements")]
        public Text currentHPText;
        public Slider HealthBar;
        public Image healthBarFill;
        public Slider MagicBar;
        public Text currentMPText;
        public Slider levelProgressBar;
        public Text playerLevelText;
        public Text difficultyLevelText;
        public Text clockText;
        public Slider xpBar;
        public Text xpProgressPercent;
        public Text xpText;
        public Text nextXpText;
        public Image mpRadial;
        public Text potion;
        public Text skillSlotAText, skillSlotBText;
        public StatusEffectUI statusEffectUI;
        public Text comboHits;
        public Image staminaCooldownRadial;
        public Text currentStaminaCharges;
        public Text collectibleAText;
        public Text killsPerMinuteText;
        public Text xpPerMinuteText;
        public Button startButton, resumeButton, returnToMenuButton, addNewUserButton;

        [Header("Messages")]
        public Text message;
        public Text messageCenter;

        [Header("Screen Effects")]
        public GameObject ScreenFlash;

        // Healthbar colors
        [HideInInspector]
        public Color healthy = new Color(170, 0, 0);
        [HideInInspector]
        public Color damaged = new Color(120, 100, 0);

        public EquipmentSlotGUI
            weaponSlotGUI,
            helmSlotGUI,
            mailSlotGUI,
            cloakSlotGUI,
            bracersSlotGUI,
            bootsSlotGUI;

        private void Start()
        {
            xpBar.maxValue = 100;
            //  menuGroup = menu.GetComponent<CanvasGroup>();
            //   menuGroup.interactable = false;
            // menuGroup.alpha = 0;

            // Create blank EquipSlotGUIs for use by Inventory Controller
            //  weaponSlotGUI = new EquipSlotGUI();
            //  armorSlotGUI = new EquipSlotGUI();
            //  cloakSlotGUI = new EquipSlotGUI();
            //  gloveSlotGUI = new EquipSlotGUI();
            //  ringSlotGUI = new EquipSlotGUI();

        }

        void Update()
        {
            UpdateUI();

            if (Input.GetKeyDown(InputManager.Instance.menu_keyboard) ||
                (Input.GetKeyDown(InputManager.Instance.menu_gamepad)))
            {
                if (!EventController.Instance.isPaused)
                {
                    EventController.Instance.isPaused = true;
                    // menuGroup.interactable = true;
                    // menuGroup.alpha = 1;
                    EventController.Instance.firstSelectedGameObject.GetComponent<Selectable>().Select();  //
                    return;
                }

                if (EventController.Instance.isPaused)
                {
                    EventController.Instance.isPaused = false;
                    // menuGroup.interactable = false;
                    //  menuGroup.alpha = 0;
                    return;
                }

            }

            if (effectsFade)
            {
                ProcessEffectsFade();
            }


        }


        public void UpdateUI()
        {
            currentHPText.text = Stats.Instance.currentHP.ToString();
            currentMPText.text = Stats.Instance.currentMP.ToString();
            HealthBar.maxValue = Stats.Instance.MaxHealth.Value;
            HealthBar.value = Stats.Instance.currentHP;
            MagicBar.value = Stats.Instance.currentMP;
            MagicBar.maxValue = Stats.Instance.MaxMagic.Value;
            mpRadial.fillAmount = Stats.Instance.mpPercentage;
            playerLevelText.text = Stats.Instance.playerLevel.ToString();
            xpText.text = Stats.Instance.XP.ToString();
            nextXpText.text = Stats.Instance.nextXPRemaining.ToString();
            xpBar.value = Stats.Instance.xpProgressPercentage;
            xpProgressPercent.text = Mathf.RoundToInt(Stats.Instance.xpProgressPercentage).ToString() + "%";
            difficultyLevelText.text = Difficulty.Instance.difficultyLevel.ToString();

            //killsPerMinuteText.text = Stats.Instance.killsPerMinute.ToString();
            //xpPerMinuteText.text = Stats.Instance.xpPerMinute.ToString();
            //collectibleAText.text = Stats.Instance.collectibleA.ToString();
           // levelProgressBar.value = Stats.Instance.levelProgressPercentage;


            // "RIFT" PROGRESS IDEA
            // The level progress bar moves forward based on the amount of XP gained from killing trash mobs or collecting eliteGlobes from killing Elites
            // When trash is killed, levelProgress += enemy.XP * 0.10f;
            // When Elites are killed, they leave behind Elite Globes.  Collecting a globe grants, levelProgress += enemy.XP * 0.25f;


            // currentStaminaCharges.text = Stats.Instance.currentStaminaCharges.ToString();
            //staminaCooldownRadial.fillAmount -= 1.0f / Stats.Instance.staminaCooldownDuration * Time.deltaTime;

        }


        public IEnumerator HealthBarFlash(Material flashMat, float duration)
        {
            var oldMat = Instance.healthBarFill.material;
            Instance.healthBarFill.material = flashMat;
            yield return new WaitForSeconds(duration);
            Instance.healthBarFill.material = oldMat;

        }

        public void NotEnoughMP()
        {
            StartCoroutine(ShowMessage("Not enough MP.", Color.yellow, 32, 2f));
        }

        public void NotEnoughStamina()
        {
            StartCoroutine(ShowMessage("Not enough Stamina.", Color.yellow, 32, 2f));
        }

        public IEnumerator ShowMessage(string text, Color color, int size, float duration)
        {
            if (message != null)
            {
                message.color = color;
                message.text = text;
                message.fontSize = size;
                // textFade.FadeTextIn(textFade.fadeInTime, message);
                yield return new WaitForSeconds(duration);
                // textFade.FadeTextOut(textFade.fadeOutTime, message);
                // StartCoroutine(ClearMessage(textFade.fadeOutTime));
                message.text = " ";
            }

        }

        public IEnumerator ShowMessageCenter(string text, Color color, int size, float duration)
        {
            if (messageCenter != null)
            {
                messageCenter.color = color;
                messageCenter.text = text;
                messageCenter.fontSize = size;
                // textFade.FadeTextIn(textFade.fadeInTime, message);
                yield return new WaitForSeconds(duration);
                // textFade.FadeTextOut(textFade.fadeOutTime, message);
                // StartCoroutine(ClearMessage(textFade.fadeOutTime));
                messageCenter.text = " ";
            }

        }

        public IEnumerator ClearMessage(float duration)
        {
            yield return new WaitForSeconds(duration);
            message.text = " ";
        }

        // FloatParameter colorSaturation = new FloatParameter() { value = 20 };
        // FloatParameter bwSaturation = new FloatParameter() { value = -100 };

        bool effectsFade;
        PostProcessVolume effectsVolume_From;
        PostProcessVolume effectsVolume_To;
        float duration;
        float startTime;

        public void EffectsFade(PostProcessVolume _effectsVolume_From, PostProcessVolume _effectsVolume_To, float _duration)
        {
            effectsFade = true;
            startTime = Time.time;
            effectsVolume_From = _effectsVolume_From;
            effectsVolume_To = _effectsVolume_To;
            duration = _duration;
        }

        void ProcessEffectsFade()
        {
            float t = (Time.time - startTime) / duration;
            effectsVolume_From.weight = Mathf.SmoothStep(1, 0, t);
            effectsVolume_To.weight = Mathf.SmoothStep(0, 1, t);

            if (effectsVolume_From.weight < 0.01f)
            {
                effectsVolume_From.weight = 0;
                effectsFade = false;
            }

            if (effectsVolume_To.weight > 0.99f)
            {
                effectsVolume_To.weight = 1;
                effectsFade = false;
            }
        }
    }

}