using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public float rotationSpeed = 5f;

	void Start ()
    {
       
    }
	

	void Update ()
    {

        transform.Rotate(Vector2.up, rotationSpeed * Time.deltaTime);

    }
}
