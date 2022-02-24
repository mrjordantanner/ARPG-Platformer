using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CoreStatDeallocateButtonGUI : MonoBehaviour
    {
        public CharacterStatType type;
        Button button;

        private void Start()
        {
            button = GetComponent<Button>();

            switch (type)
            {
                case CharacterStatType.Strength:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Strength));
                    break;

                case CharacterStatType.Constitution:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Constitution));
                    break;

                case CharacterStatType.Agility:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Agility));
                    break;

                case CharacterStatType.Intelligence:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Intelligence));
                    break;

                case CharacterStatType.Luck:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Luck));
                    break;

            }
        }


    }






}