using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {

    public float scrollSpeed = 0.5f;
    MeshRenderer meshRenderer;
    Material mat;
    Vector2 offset;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
    }

    void Update ()
    {
        offset = mat.mainTextureOffset;
        offset.x += (Time.deltaTime * scrollSpeed) / 10;
        mat.mainTextureOffset = offset;

    }
}
