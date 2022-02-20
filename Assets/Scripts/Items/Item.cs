using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Item
    {
        public enum Quality { Common, Rare, Magical }//, Set }
        public enum Type { Weapon, Armor }
        public enum Spec { Bone, Blood, Ghost }

        public Quality quality { get; set; }
        public Type type;
        public Spec spec;

        public int ID { get; set; }
        public string itemName { get; set; }
        public bool equipped { get; set; }

        public Icon icon { get; set; }

        public ItemObject itemObject;

        [HideInInspector]
        public ItemSlotGUI itemSlotGUI { get; set; }

        [SerializeField]
        public virtual List<StatModifier> itemAttributes { get; set; }





    }
}