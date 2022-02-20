using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlotGUI : MonoBehaviour
{
   // public Inventory.EquipmentSlot.Type type;
   // public Text itemNameText;
   // public Text[] attributeText;

    /*
    public class AttributeTextLine
    {
        public Text textLine;

        public AttributeTextLine()
        {
            textLine.fontSize = 12;
        }

    }
    */
    

    void Start()
    {
        //attributeText = new Text[5];
       // attributeText = GetComponentsInChildren<Text>();
    }

    // Create blank GUI
    public EquipSlotGUI()
    {
        Clear();
    }

    public void Clear()
    {
       // itemNameText.text = "None";
       // foreach (var _text in attributeText)
       //     _text.text = "";
    }


    //public EquipSlotGUI(
    //    string _itemNameText, 
    //    string _stat1Label, 
    //    string _stat1Text, 
    //    string _stat2Label, 
    //    string _stat2Text, 
    //    string _stat3Label, 
    //    string _stat3Text,
    //    string _stat4Label,
    //    string _stat4Text, 
    //    string _stat5Label, 
    //    string _stat5Text)
    //{
    //    itemNameText.text = _itemNameText;
    //    stat1Label.text = _stat1Label;
    //    stat1Text.text = _stat1Text;
    //    stat2Label.text = _stat2Label;
    //    stat2Text.text = _stat2Text;
    //    stat3Label.text = _stat3Label;
    //    stat3Text.text = _stat3Text;
    //    stat4Label.text = _stat4Label;
    //    stat4Text.text = _stat4Text;
    //    stat5Label.text = _stat5Label;
    //    stat5Text.text = _stat5Text;


    //}




}
