using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlasher : MonoBehaviour {

    public float flashRate;
    float timer;
    SpriteRenderer renderer;

	void Start ()
    {
        renderer = GetComponent<SpriteRenderer>();
	}
	

	void Update ()
    {
        if (timer < flashRate)
            timer += Time.deltaTime;

        if (timer >= flashRate)
            Flash();
	}

    void Flash()
    {
        renderer.enabled = !renderer.enabled;
        timer = 0;
    }
}
