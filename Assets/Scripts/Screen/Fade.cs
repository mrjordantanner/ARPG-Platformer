using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Gamekit2D;

public class Fade : MonoBehaviour {

    public Image FadeImg;
    public float fadeSpeed = 1.5f;
    public bool sceneStarting = true;


    void Awake()
    {
        FadeImg.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        // If the scene is starting...
        if (sceneStarting)
            // ... call the StartScene function.
            StartScene();
    }


    void FadeToClear()
    {
        FadeImg.color = Color.Lerp(FadeImg.color, Color.clear, fadeSpeed * Time.deltaTime);
    }


    public void FadeToBlack()
    {
        FadeImg.color = Color.Lerp(FadeImg.color, Color.black, fadeSpeed * Time.deltaTime);
    }


    void StartScene()
    {
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (FadeImg.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the RawImage.
            FadeImg.color = Color.clear;
            FadeImg.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
        }
    }


    public IEnumerator EndSceneRoutine(int SceneNumber)
    {
        // Make sure the RawImage is enabled.
        FadeImg.enabled = true;
        do
        {
            // Start fading towards black.
            FadeToBlack();

            // If the screen is almost black...
            if (FadeImg.color.a >= 0.95f)
            {
                // ... reload the level
                // SceneManager.LoadScene(SceneNumber);
                    // SceneController.RestartZone(true);
                yield break;
            }
            else
            {
                yield return null;
            }
        } while (true);
    }

    public void EndScene(int SceneNumber)
    {
        sceneStarting = false;
        StartCoroutine("EndSceneRoutine", SceneNumber);
    }
}   

