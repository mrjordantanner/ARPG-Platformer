using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

[ExecuteInEditMode]
public class LightSpritePreview : MonoBehaviour
{
    SpriteRenderer spritePreview;
    LightSprite lightSprite;

    void Start()
    {
        spritePreview = GetComponent<SpriteRenderer>();
        lightSprite = GetComponentInChildren<LightSprite>();

        if (!Application.isPlaying)
            spritePreview.enabled = true;
        else
            spritePreview.enabled = false;
    }


    void Update()
    {
        if (!Application.isPlaying && lightSprite != null && spritePreview != null)
            UpdatePreview();

    }

    public void UpdatePreview()
    {
        spritePreview.color = lightSprite.Color;
        spritePreview.sprite = lightSprite.Sprite;
    }
}
