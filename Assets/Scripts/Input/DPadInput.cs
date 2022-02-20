using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPadInput : MonoBehaviour {

    public bool dpadUp = false;
    public bool dpadDown = false;
    public bool dpadLeft = false;
    public bool dpadRight = false;
    public bool dpadNW = false;
    public bool dpadNE = false;
    public bool dpadSW = false;
    public bool dpadSE = false;

    void Update () {

       float horiz = Input.GetAxisRaw("Horizontal");
       float vert = Input.GetAxisRaw("Vertical");

        //Debug.Log("H: " + horiz + "  V: " + vert);

        // stopped
        if (horiz == 0 && vert == 0)       
        {
            dpadLeft = dpadUp = dpadDown = dpadRight = dpadNW = dpadNE = dpadSW = dpadSE = false;
        }
        // right
        if (horiz == 1)                  
        {
            dpadRight = true;
            dpadUp = dpadDown = dpadLeft = dpadNW = dpadNE = dpadSW = dpadSE = false;
        }
        //left
        if (horiz == -1)
        {
            dpadLeft = true;
            dpadUp = dpadDown = dpadRight = dpadNW = dpadNE = dpadSW = dpadSE = false;
        }
        //up
        if (vert == 1)
        {
            dpadUp = true;
            dpadLeft = dpadDown = dpadRight = dpadNW = dpadNE = dpadSW = dpadSE = false;
        }
        //down
        if (vert == -1)
        {
            dpadDown = true;
            dpadUp = dpadLeft = dpadRight = dpadNW = dpadNE = dpadSW = dpadSE = false;
        }
        //NE
        if (vert > 0 && horiz > 0)
        {
            dpadNE = true;
            dpadUp = dpadDown = dpadRight = dpadNW = dpadLeft = dpadSW = dpadSE = false;
        }
        // NW
        if (vert > 0 && horiz < 0)
        {
            dpadNW = true;
            dpadUp = dpadDown = dpadRight = dpadLeft = dpadNE = dpadSW = dpadSE = false;
        }
        // SW
        if (vert < 0 && horiz < 0)
        {
            dpadSW = true;
            dpadUp = dpadDown = dpadRight = dpadNW = dpadNE = dpadLeft = dpadSE = false;
        }

        // SE
        if (vert < 0 && horiz > 0)
        {
            dpadSE = true;
            dpadUp = dpadDown = dpadRight = dpadNW = dpadNE = dpadSW = dpadLeft = false;
        }
    }

   
}
