using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class LayoutNode : MonoBehaviour
    {

        // Map-building placeholder for the actual Layouts
        // These define what layouts can be placed here

        //[HideInInspector]
        public Module.AreaType areaType;

        public enum LayoutShape { _1x1, _1x3, _1x6, _2x2, _2x4, _3x1, _4x2, _4x4, _4x8, _6x1, J_B, J_L, J_R, J_T }

        [Header("Shape")]
        public LayoutShape layoutShape;

        [Header("Connections")]
        public bool top;
        public bool right;
        public bool bottom;
        public bool left;

        public bool topLeft, topRight, rightUpper, rightLower, bottomRight, bottomLeft, leftLower, leftUpper;

        LayoutConnectionMarker[] connectors;
        EnemyLayoutMember[] enemyLayouts;
        GameObject CurrentLayout;
        GameObject CurrentEnemyLayout;
        Module parentModule;

        private void Start()
        {
            connectors = GetConnectors();

            // Set compatible area for this layoutnode to the area specified by the parent Module
            parentModule = GetComponentInParent<Module>();
            if (parentModule != null)
                areaType = parentModule.areaType;
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                // This will run in Edit Mode
                CheckConnectors();
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






}