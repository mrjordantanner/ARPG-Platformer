using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class Module : MonoBehaviour
    {

        public enum AreaType { A, B, C, D, E }

        [Header("Area")]
        public AreaType areaType;

        [Header("Connections")]
        public bool top;
        public bool left;
        public bool right;
        public bool bottom;

        // Affects how often the module will occur by adding it to the list of matches this many more times
        [Header("Frequency")]
        [Range(1, 10)]
        public int frequencyMultiplier = 1;

        ModuleConnectionMarker[] connectors;
        SpriteRenderer spriteRenderer;
        // BoxCollider2D moduleVolume;

        public ModuleConnectionMarker[] GetConnectors()
        {
            return GetComponentsInChildren<ModuleConnectionMarker>(true);  //includes inactive markers
        }

        private void Start()
        {
            connectors = GetConnectors();
            spriteRenderer = GetComponent<SpriteRenderer>();
            // moduleVolume = GetComponent<BoxCollider2D>();


        }



        void Update()
        {
            if (Application.isEditor)
            {
                // This will run in Edit Mode
                CheckConnectors();
                AreaColor();
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (AreaController.Instance.activeArea != areaType)
                    AreaController.Instance.AreaTransition(this, areaType);
                else
                    AreaController.Instance.activeModule = this;
            }

        }



    }

}