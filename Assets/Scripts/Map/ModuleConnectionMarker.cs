using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleConnectionMarker : MonoBehaviour {

    public enum Type { Top, Left, Right, Bottom}
    public Type type;
    public float scale = 5f;
    public Color color = Color.magenta;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
       // Gizmos.DrawLine(transform.position, transform.position + transform.right * scale);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * -scale);
    }

}
