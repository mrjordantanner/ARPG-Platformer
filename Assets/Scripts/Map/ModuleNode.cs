using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ModuleNode : MonoBehaviour
{
    // ModuleNodes are empty map slots with parameters that define what Modules can be loaded into it
    // At runtime, the Module Controller fetches a random module
    // from the appropriate matching category
    // based on what AreaType and ConnectionType are

    public enum AreaType { A, B, C, D, E}
    public AreaType areaType;
    public bool top, left, right, bottom;
    Vector3 topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner;

    ModuleConnectionMarker[] connectors;
    ModuleBorderMarker[] borders;
    SpriteRenderer spriteRenderer;

    public ModuleConnectionMarker[] GetConnectors()
    {
        return GetComponentsInChildren<ModuleConnectionMarker>(true);  //includes inactive markers
    }

    public ModuleBorderMarker[] GetBorders()
    {
        return GetComponentsInChildren<ModuleBorderMarker>(true);  //includes inactive markers
    }

    private void Start()
    {
        connectors = GetConnectors();
        borders = GetBorders();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }



    void Update()
    {
        if (Application.isEditor)
        {
            // This will run in Edit Mode
            CheckConnectors();
            CheckBorders();
            AreaColor();
        }

    }


    void CheckBorders()
    {
        borders = GetBorders();

        foreach (ModuleBorderMarker border in borders)
        {
            switch (border.type)
            {
                case ModuleBorderMarker.Type.Bottom:
                    if (!bottom)
                        border.gameObject.SetActive(true);
                    else
                        border.gameObject.SetActive(false);
                    break;

                case ModuleBorderMarker.Type.Top:
                    if (!top)
                        border.gameObject.SetActive(true);
                    else
                        border.gameObject.SetActive(false);
                    break;

                case ModuleBorderMarker.Type.Left:
                    if (!left)
                        border.gameObject.SetActive(true);
                    else
                        border.gameObject.SetActive(false);
                    break;

                case ModuleBorderMarker.Type.Right:
                    if (!right)
                        border.gameObject.SetActive(true);
                    else
                        border.gameObject.SetActive(false);
                    break;
            }


        }




    }


    // Enable/Disable Connector Gizmos based on the Module's ConnectionType
    void CheckConnectors()
    {
        connectors = GetConnectors();

        foreach (ModuleConnectionMarker connector in connectors)
        {
            switch (connector.type)
            {
                case ModuleConnectionMarker.Type.Bottom:
                    if (!bottom)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case ModuleConnectionMarker.Type.Top:
                    if (!top)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case ModuleConnectionMarker.Type.Left:
                    if (!left)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;

                case ModuleConnectionMarker.Type.Right:
                    if (!right)
                        connector.gameObject.SetActive(false);
                    else
                        connector.gameObject.SetActive(true);
                    break;
            }


        }


    }

    void AreaColor()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch (areaType)
        {
            case AreaType.A:
                spriteRenderer.color = Color.blue;
                break;

            case AreaType.B:
                spriteRenderer.color = Color.grey;
                break;

            case AreaType.C:
                spriteRenderer.color = Color.magenta;
                break;

            case AreaType.D:
                spriteRenderer.color = Color.green;
                break;

            case AreaType.E:
                spriteRenderer.color = Color.red;
                break;

        }
    }


    private void OnDrawGizmos()
    {
       // Gizmos.color = Color.white;
       // Gizmos.DrawLine(topLeftCorner, topRightCorner);    // top
       // Gizmos.DrawLine(topLeftCorner, bottomLeftCorner);  // left
       // Gizmos.DrawLine(topRightCorner, bottomRightCorner);    // right
       // Gizmos.DrawLine(bottomLeftCorner, bottomRightCorner);    // bottom

    }

}


