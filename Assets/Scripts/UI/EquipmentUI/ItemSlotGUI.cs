using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class ItemSlotGUI : MonoBehaviour
    {
        [HideInInspector]
        public Selectable selectable;
        Button button;

        public IEquippable itemInSlot;
        public Icon icon;

        public enum Type { Weapon, Helm, Mail, Cloak, Bracers, Boots }
        public Type type;

        private void Start()
        {
            button = GetComponent<Button>();
            selectable = button.GetComponent<Selectable>();
            //onButtonPress.AddListener(InventoryItemSelected);
            button.interactable = false;

        }


    }
}