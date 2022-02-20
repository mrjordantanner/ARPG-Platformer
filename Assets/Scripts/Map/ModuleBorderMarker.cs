using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ModuleBorderMarker : MonoBehaviour {

    public enum Type { Top, Left, Right, Bottom }
    public Type type;
    float scale;
    public Color color = Color.white;

    void Start()
    {
        if (type == Type.Top || type == Type.Bottom)
                scale = 64f;

        if (type == Type.Left || type == Type.Right)
            scale = 42f;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * scale);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * -scale);








    }
}
