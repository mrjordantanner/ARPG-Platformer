using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineJuice : MonoBehaviour {

    [Header("Motion")]
    public bool sineMotion;
    public float motionFrequency = 0f;
    public float motionX = 0f;
    public float motionY = 0f;
    public float motionZ = 0f;
    float motionTimeCounter = 0f;

    [Header("Scale")]
    public bool sineScale;
    public float scaleFrequency;
    public float scaleAmount;
    float scaleTimeCounter = 0f;

    [Header("Fade")]
    public bool sineFade;
    public float fadeFrequency;
    public float fadeAmount;
    float fadeTimeCounter = 0f;

    SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update ()
    {

        if (sineScale)
        {
            scaleTimeCounter += Time.deltaTime * scaleFrequency;
            transform.localScale = new Vector2(Mathf.Sin(scaleTimeCounter) * scaleAmount, Mathf.Sin(scaleTimeCounter) * scaleAmount);
        }

        if (sineMotion)
        {
            motionTimeCounter += Time.deltaTime * motionFrequency;
            transform.position += new Vector3(Mathf.Cos(motionTimeCounter) * motionX, Mathf.Sin(motionTimeCounter) * motionY, Mathf.Sin(motionTimeCounter) * motionZ);
        }

        if (sineFade)
        {
            fadeTimeCounter += Time.deltaTime * fadeFrequency;
            float alpha = renderer.material.color.a;
            alpha = Mathf.Sin(fadeTimeCounter) * fadeAmount;

        }
    }


}

