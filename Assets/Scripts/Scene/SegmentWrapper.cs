using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class SegmentWrapper : MonoBehaviour
    {
        // Sits on the parent object of camera segment and parallax layers

        Segment childSegment;

        private void Awake()
        {
            childSegment = GetComponentInChildren<Segment>();
            childSegment.parallaxLayers = GetComponentsInChildren<ParallaxLayer>();
        }

    }

}