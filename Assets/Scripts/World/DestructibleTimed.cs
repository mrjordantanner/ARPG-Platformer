using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleTimed : MonoBehaviour {

    public float respawnTime = 3f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player Weapon"))
        {
            gameObject.SetActive(false);
            Invoke("SpawnObject", respawnTime);
        }

    }

    void SpawnObject()
    {
        gameObject.SetActive(true);
    }
}
