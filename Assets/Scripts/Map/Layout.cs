using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Layout : MonoBehaviour {

    public enum AreaType { A, B, C, D, E }

    [Header("Area")]
    public AreaType areaType;

    // Placed on actual Layout gameobjects
    public enum LayoutShape { _1x1, _1x3, _1x6, _2x2, _2x4, _3x1, _4x2, _4x4, _4x8, _6x1, J_B, J_L, J_R, J_T }

    [Header("Shape")]
    public LayoutShape layoutShape;

    [Header("Connections")]
    public bool top;
    public bool bottom;
    public bool left;
    public bool right;
    public bool topLeft, topRight, rightUpper, rightLower, bottomRight, bottomLeft, leftLower, leftUpper;
    LayoutConnectionMarker[] connectors;

    // Affects how often the layout will occur by adding it to the list of matches this many more times
    [Header("Frequency")]
    [Range(1, 10)] public int frequencyMultiplier = 1;

    private void Start()
    {
        connectors = GetConnectors();
        CheckConnectors();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            // This will run in Edit Mode
            // TODO: Perhaps put this on a timer instead of running every frame
           // CheckConnectors();
        }

    }

    public LayoutConnectionMarker[] GetConnectors()
    {
        return GetComponentsInChildren<LayoutConnectionMarker>(true);  //includes inactive markers
    }

    // Enable/Disable Connector Gizmos based on the Module's ConnectionType
    void CheckConnectors()
    {
        connectors = GetConnectors();

        foreach (LayoutConnectionMarker connector in connectors)
        {
            switch (connector.type)
            {
                case LayoutConnectionMarker.Type.Bottom:
                    if (!bottom)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.BottomLeft:
                    if (!bottomLeft)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.BottomRight:
                    if (!bottomRight)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.Top:
                    if (!top)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.TopLeft:
                    if (!topLeft)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.TopRight:
                    if (!topRight)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.Left:
                    if (!left)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.LeftUpper:
                    if (!leftUpper)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.LeftLower:
                    if (!leftLower)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.Right:
                    if (!right)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.RightUpper:
                    if (!rightUpper)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case LayoutConnectionMarker.Type.RightLower:
                    if (!rightLower)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;
            }


        }


    }

}
