using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFade : MonoBehaviour {

    public float fadeInTime;
    public float fadeOutTime;
    public Text i;

    void Start()
    {
        i = GetComponent<Text>();
    }

    public IEnumerator FadeTextIn(float fadeInTime, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / fadeInTime));
            yield return null;
        }
    }

    public IEnumerator FadeTextOut(float fadeOutTime, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / fadeOutTime));
            yield return null;
        }
    }
}
