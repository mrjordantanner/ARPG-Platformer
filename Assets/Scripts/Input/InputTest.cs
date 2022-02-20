using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {

        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Debug.Log("Horiz: " + horiz);
        Debug.Log("Vert: " + vert);

        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            Debug.Log("0");

        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            Debug.Log("1");

        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            Debug.Log("2");

        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
            Debug.Log("3");

        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            Debug.Log("4");

        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            Debug.Log("5");

        if (Input.GetKeyDown(KeyCode.Joystick1Button6))
            Debug.Log("6");

        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
            Debug.Log("7");

        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
            Debug.Log("8");

        if (Input.GetKeyDown(KeyCode.Joystick1Button9))
            Debug.Log("9");

        if (Input.GetKeyDown(KeyCode.Joystick1Button10))
            Debug.Log("10");

        if (Input.GetKeyDown(KeyCode.Joystick1Button11))
            Debug.Log("11");

        if (Input.GetKeyDown(KeyCode.Joystick1Button12))
            Debug.Log("12");

        if (Input.GetKeyDown(KeyCode.Joystick1Button13))
            Debug.Log("13");

        if (Input.GetKeyDown(KeyCode.Joystick1Button14))
            Debug.Log("14");

        if (Input.GetKeyDown(KeyCode.Joystick1Button15))
            Debug.Log("15");

        if (Input.GetKeyDown(KeyCode.Joystick1Button16))
            Debug.Log("16");


           
 
      
    


    }

}
