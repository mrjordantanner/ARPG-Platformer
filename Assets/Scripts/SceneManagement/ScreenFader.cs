using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public enum FadeType
    {
        Black
    }

    public static ScreenFader ControllerPrefab;

    public static ScreenFader Instance
    {
        get
        {
            if (s_Instance != null)
                return s_Instance;

            s_Instance = FindObjectOfType<ScreenFader> ();

            if (s_Instance != null)
                return s_Instance;

            Create ();

            return s_Instance;
        }
    }

        
    public static bool IsFading
    {
        get { return Instance.m_IsFading; }
    }
        

    protected static ScreenFader s_Instance;


    public static void Create ()
    {
        //ScreenFader controllerPrefab = Resources.Load<ScreenFader> ("ScreenFader");
        // s_Instance = Instantiate (controllerPrefab;
        s_Instance = ControllerPrefab;
    }


    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;
    protected bool m_IsFading;
    
    const int k_MaxSortingLayer = 32767;

    void Awake ()
    {
        if (Instance != this)
        {
            Destroy (gameObject);
            return;
        }
        
        DontDestroyOnLoad (gameObject);
    }

    protected IEnumerator Fade(float fadeDuration, float finalAlpha, CanvasGroup canvasGroup)
    {
        m_IsFading = true;
        canvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);
            yield return null;
        }
        canvasGroup.alpha = finalAlpha;
        m_IsFading = false;
        canvasGroup.blocksRaycasts = false;
    }

    public static void SetAlpha (float alpha)
    {
        Instance.faderCanvasGroup.alpha = alpha;
    }

    public static IEnumerator FadeSceneIn (float duration)
    {
        yield return Instance.StartCoroutine(Instance.Fade(duration, 0f, Instance.faderCanvasGroup));
        Instance.faderCanvasGroup.gameObject.SetActive(false);
    }

    public static IEnumerator FadeSceneOut (float duration, FadeType fadeType = FadeType.Black)
    {
        Instance.faderCanvasGroup.gameObject.SetActive (true);
        yield return Instance.StartCoroutine(Instance.Fade(duration, 1f, Instance.faderCanvasGroup));
    }
}
