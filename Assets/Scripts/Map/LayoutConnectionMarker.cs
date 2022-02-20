using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutConnectionMarker : MonoBehaviour {

    public enum Type { Top, Left, Right, Bottom, TopLeft, TopRight, RightUpper, RightLower, BottomLeft, BottomRight, LeftUpper, LeftLower }
    public Type type;
    public float scale = 2f;
    public Color color = Color.yellow;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
       // Gizmos.DrawLine(transform.position, transform.position + transform.right * scale);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * -scale);
    }
}
