using UnityEngine;

[ExecuteInEditMode]
public class DrawBorder : MonoBehaviour {

    Vector3 topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner;
    Vector3 confiner_topLeftCorner, confiner_topRightCorner, confiner_bottomLeftCorner, confiner_bottomRightCorner;
    MeshRenderer segmentMask;
    MeshFilter filter;
    BoxCollider2D roomVolume;
    PolygonCollider2D cameraConfiner;
    Bounds bounds;

    [Header("Gizmos")]
    public bool showBorder = true;
    public Color borderColor = Color.yellow;

    void Awake()
    {
        cameraConfiner = GetComponentInChildren<PolygonCollider2D>();
        segmentMask = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        if (showBorder)
        {
            CalcBorderPositions();
            DrawMeshBorder();
        }

    }

    void CalcBorderPositions()
    {
        // Border around Mesh Renderer
        if (filter != null)
            bounds = filter.sharedMesh.bounds;

        // Border around Box Collider
        //Bounds bounds;
        //BoxCollider bc = GetComponent<BoxCollider2D>();
        //if (bc != null)
        //    bounds = bc.bounds;
        //else
        //return;

        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;

        topLeftCorner = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
        topRightCorner = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
        bottomLeftCorner = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
        bottomRightCorner = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner

        topLeftCorner = transform.TransformPoint(topLeftCorner);
        topRightCorner = transform.TransformPoint(topRightCorner);
        bottomLeftCorner = transform.TransformPoint(bottomLeftCorner);
        bottomRightCorner = transform.TransformPoint(bottomRightCorner);

    }

    void DrawMeshBorder()
    {
        Debug.DrawLine(topLeftCorner, topRightCorner, borderColor);
        Debug.DrawLine(topRightCorner, bottomRightCorner, borderColor);
        Debug.DrawLine(bottomLeftCorner, bottomRightCorner, borderColor);
        Debug.DrawLine(bottomLeftCorner, topLeftCorner, borderColor);

    }

}
