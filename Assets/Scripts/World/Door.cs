using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using Cinemachine;

namespace Assets.Scripts
{
    public class Door : MonoBehaviour
    {
        // Door is a trigger that is a child of a Segment - used to create transitions between Segments
        public Transform destinationPoint;

        // manually assign this in Inspector (for now) - use to determine next Confiner to load
        public Door connectedDoor;
        public float gizmoSize = 1f;

        PlayerCharacter player;
        Segment segment;
        CinemachineConfiner cinemachineConfiner;


        private void Start()
        {
            player = PlayerRef.Player;
            segment = GetComponentInParent<Segment>();
            // destinationPoint = GetComponentInChildren<Transform>();
            cinemachineConfiner = FindObjectOfType<CinemachineConfiner>();
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // StartCoroutine(SegmentTransition());
            }
        }

        public IEnumerator SegmentTransition()
        {
            StartCoroutine(ScreenFader.FadeSceneOut(0.1f));
            // player.transform.position = destinationPoint.transform.position;
            yield return new WaitForSeconds(0.2f);
            //  cinemachineConfiner.InvalidatePathCache();
            //  cinemachineConfiner.m_BoundingShape2D = connectedDoor.segment.cameraConfiner;
            StartCoroutine(ScreenFader.FadeSceneIn(0.1f));

        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position - Vector3.right * gizmoSize, transform.position + Vector3.right * gizmoSize);
            Gizmos.DrawLine(transform.position - Vector3.up * gizmoSize, transform.position + Vector3.up * gizmoSize);
        }

    }



}