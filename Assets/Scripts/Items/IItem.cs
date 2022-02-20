using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public interface IItem
    {
        int ID { get; set; }
        string itemName { get; set; }
        bool equipped { get; set; }

        Item.Quality quality { get; set; }
       // Item.Type type { get; set; }
        Item.Spec spec { get; set; }

        Image itemImage { get; set; }

        ItemSlotGUI itemSlotGUI { get; set; }
        List<StatModifier> itemAttributes { get; set; }

        Icon icon { get; set; }


    }
}
