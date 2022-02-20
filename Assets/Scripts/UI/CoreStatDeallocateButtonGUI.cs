using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CoreStatDeallocateButtonGUI : MonoBehaviour
    {
        public CharacterStat.Type type;
        Button button;

        private void Start()
        {
            button = GetComponent<Button>();

            switch (type)
            {
                case CharacterStat.Type.Strength:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Strength));
                    break;

                case CharacterStat.Type.Constitution:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Constitution));
                    break;

                case CharacterStat.Type.Agility:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Agility));
                    break;

                case CharacterStat.Type.Intelligence:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Intelligence));
                    break;

                case CharacterStat.Type.Luck:
                    button.onClick.AddListener(() => Stats.Instance.DeallocateStatPoint(Stats.Instance.Luck));
                    break;

            }
        }


    }






}