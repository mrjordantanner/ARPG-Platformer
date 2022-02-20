using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

namespace Assets.Scripts
{
    public class GridCell : MonoBehaviour
    {

        Vector2 topLeft, topRight;
        Vector2 bottomLeft, bottomRight;

        [HideInInspector]
        BoxCollider2D collider;

        public bool hasVisited;
        public string ID;

        Stats stats;

        void Start()
        {
            stats = FindObjectOfType<Stats>();
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                //     if (!hasVisited)
                //        stats.totalCellsVisited++;

                //     hasVisited = true;
                //   stats.currentGridCell = ID;

            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // stats.currentGridCell = ID;
        }


        void OnDrawGizmos()
        {
            /*
            if (!hasVisited)
                Gizmos.color = Color.blue;

            else if (hasVisited && stats.currentGridCell != ID)
                Gizmos.color = Color.yellow;

            else if (stats.currentGridCell == ID)
                Gizmos.color = Color.green;

            else Gizmos.color = Color.blue;

            collider = GetComponent<BoxCollider2D>();
            Bounds bounds = collider.bounds;

            bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            topLeft = new Vector2(bounds.min.x, bounds.max.y);
            topRight = new Vector2(bounds.max.x, bounds.max.y);

            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            */

        }


    }
}