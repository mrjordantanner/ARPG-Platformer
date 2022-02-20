using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollection : MonoBehaviour {

	void Update ()
    {
        if (Time.frameCount % 60 == 0)
        {
            System.GC.Collect();
        }
    }
}
