using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour {

    public Color color = Color.yellow;
    Vector2 topLeft, topRight;
    Vector2 bottomLeft, bottomRight;

    [HideInInspector]
    PolygonCollider2D collider;

    void OnDrawGizmos()
    {
        Gizmos.color = color;

        collider = GetComponent<PolygonCollider2D>();
        Bounds bounds = GetComponent<Collider>().bounds;

        bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        topLeft = new Vector2(bounds.min.x, bounds.max.y);
        topRight = new Vector2(bounds.max.x, bounds.max.y);

        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);

    }
}
