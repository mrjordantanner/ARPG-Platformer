using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sprite = null;
    }

}
