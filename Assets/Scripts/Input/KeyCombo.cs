using UnityEngine;

public class KeyCombo : MonoBehaviour
{
    [HideInInspector]
    public string[] buttons;
    private int currentIndex = 0;                       //moves along the array as buttons are pressed

    public float allowedTimeBetweenButtons = 0.3f; 
    private float timeLastButtonPressed;
    public float inputBuffer = 0.1f;
    string previousButton;
    public bool buttonHeld;

    public KeyCombo(string[] b)
    {
        buttons = b;
    }


    public bool Check()
    {
        var horiz = Input.GetAxisRaw("DpadHoriz");
        var vert = Input.GetAxisRaw("DpadVert");

        if (Time.time > timeLastButtonPressed + allowedTimeBetweenButtons) currentIndex = 0;
        {                               
            if (currentIndex < buttons.Length && buttons != null && (Time.time > timeLastButtonPressed + inputBuffer))   //JT
            {
                if ((buttons[currentIndex] == "down" &&  vert == -1) ||
                (buttons[currentIndex] == "up" && vert == 1) ||
                (buttons[currentIndex] == "left" && horiz == -1) ||
                (buttons[currentIndex] == "right" && horiz == 1) ||
                (buttons[currentIndex] != "down" && buttons[currentIndex] != "up" && buttons[currentIndex] != "left" && buttons[currentIndex] != "right" && Input.GetButtonDown(buttons[currentIndex])))
                {
                    timeLastButtonPressed = Time.time;
                    currentIndex++;

                    //previousButton = buttons[currentIndex];  //JT
                }

                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                    return true;
                }
                else return false;
            }
        }

        return false;
    }
}
