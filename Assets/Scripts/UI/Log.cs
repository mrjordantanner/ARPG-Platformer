using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour {

    public static Log Instance;
    public Text logText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        logText.text = "";
    }

    public void Output(string newLine)
    {

        logText.text = logText.text + "\n" + newLine;

    }

    



}
