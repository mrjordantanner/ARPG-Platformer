using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StatusEffectUI : MonoBehaviour
    {
        // Controls behavior and appearance of StatusEffect Icons
        // for any ITargetable

        [HideInInspector] public CanvasGroup canvasGroup;
        HorizontalLayoutGroup statusEffectIconGroup;
        public List<GameObject> ActiveIconObjects = new List<GameObject>();

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            statusEffectIconGroup = GetComponentInChildren<HorizontalLayoutGroup>();
            // canvasGroup.alpha = 0;
        }

        public GameObject AddEffectIcon(StatusEffect statusEffect, float duration)
        {
            GameObject NewIconObject = Instantiate(statusEffect.IconPrefab, statusEffectIconGroup.gameObject.transform);
            statusEffect.IconInstance = NewIconObject;
            statusEffect.icon = NewIconObject.GetComponent<StatusEffectIcon>();
            statusEffect.icon.duration = statusEffect.Duration;
            statusEffect.icon.durationRadialActive = true;
            statusEffect.icon.iconText.text = statusEffect.CurrentStacks.ToString();

            ActiveIconObjects.Add(NewIconObject);
            // if (enabled)
            //     StartCoroutine(RemoveEffectIcon(NewIconObject, duration));
            //print(statusEffect.Name + " Icon added.");
            return NewIconObject;
        }

        public void RemoveEffectIcon(GameObject IconObject)
        {
            if (IconObject != null)
                Destroy(IconObject);
            ActiveIconObjects.Remove(IconObject);
        }

        /*
        public IEnumerator RemoveEffectIcon(GameObject IconObject, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (IconObject != null)
                Destroy(IconObject);
            ActiveIconObjects.Remove(IconObject);
           // print(IconObject + " Icon removed.");
        }
        */

        // Remove icon by Effect Name
        //public void RemoveEffectIcon(StatusEffectName effectName)
        //{
        //    Icon[] effectIcons = GetComponentsInChildren<Icon>();
        //    foreach (var effectIcon in effectIcons)
        //    {
        //       if (effectIcon.EffectName == effectName)
        //            Destroy(effectIcon.gameObject);
        //   }
        //
        //}

    }

}