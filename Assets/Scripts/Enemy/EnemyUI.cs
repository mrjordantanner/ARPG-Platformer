using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    // Sits on EnemyUI gameobject
    // StatusEffectUI object is child of this object

    [HideInInspector] public CanvasGroup canvasGroup;
    Slider healthBar;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        healthBar = GetComponentInChildren<Slider>();
        canvasGroup.alpha = 0;
    }
}
