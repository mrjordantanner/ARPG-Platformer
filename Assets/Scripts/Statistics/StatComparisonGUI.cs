using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatComparisonGUI : MonoBehaviour
{
    [HideInInspector]
    public Text statComparisonValue;
    public CharacterStat.Type statComparisonType;

    private void Start()
    {
        statComparisonValue = GetComponent<Text>();
    }

    public void SetText(CharacterStat.Type matchingStatModType, float matchingStatModDifference)
    {
        var fontColor = Color.gray;
        var plusMinus = " ";

        //  Assign stat values to proper labels based on StatType
        if (statComparisonType == matchingStatModType)
        {
            // Positive value
            if (matchingStatModDifference > 0)
            {
                fontColor = Color.green;
                plusMinus = "+";

            }
            // Negative value
            else if (matchingStatModDifference < 0)
            {
                fontColor = Color.red;
                plusMinus = "-";
            }
            // Zero
            else
            {
                fontColor = Color.gray;
                plusMinus = " ";
            }

            statComparisonValue.color = fontColor;
            var difference = (float)Math.Round(matchingStatModDifference, 2);
            statComparisonValue.text = plusMinus + Mathf.Abs(difference).ToString();
        }


    }


}
