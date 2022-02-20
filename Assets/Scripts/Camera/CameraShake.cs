using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public float power= .7f;
    public float duration = 1.0f;
    public Transform camera;
    public float slowDownAmount = 1.0f;
    public bool shouldShake = false;

    Vector2 startPosition;
    float initialDuration;
    
	void Start () {
        camera = Camera.main.transform;
        startPosition = camera.localPosition;
        initialDuration = duration;
	}
	
	void Update ()
    {
		if (shouldShake)
        {
            if(duration > 0)
            {
                camera.localPosition = startPosition + Random.insideUnitCircle * power;
                duration -= Time.deltaTime * slowDownAmount;
            }
            else
            {
                shouldShake = false;
                duration = initialDuration;
                camera.localPosition = startPosition;
            }
                  
        }
	}
}
