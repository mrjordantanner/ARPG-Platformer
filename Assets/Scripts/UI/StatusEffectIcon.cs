using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StatusEffectIcon : MonoBehaviour
    {
        // Sits on Buff and Debuff Icon gameObjects in player UI and enemy UI 

        public Text iconText;
        public Image durationRadial;

        [HideInInspector]
        public StatusEffect statusEffect;
        [HideInInspector]
        public bool durationRadialActive;
        [HideInInspector]
        public float duration;
        // Vector2 textPos = new Vector2(5, 2);
        bool showStacks = true;

        private void Awake()
        {
            iconText = GetComponentInChildren<Text>();
            durationRadial = GetComponentInChildren<Image>();

            iconText.text = "";
            durationRadial.fillAmount = 1;
            //iconText.gameObject.transform.position = textPos;

        }

        private void Update()
        {
            if (durationRadialActive && durationRadial.fillAmount < duration)
            {
                // TODO: Consider using this formula for all ValueOverTime calculations
                durationRadial.fillAmount += 1.0f / duration * Time.deltaTime;
            }

            if (showStacks && statusEffect != null)// && statusEffect.CurrentStacks > 1)
                iconText.text = statusEffect.CurrentStacks.ToString();
            else
                iconText.text = "";


        }




    }
}