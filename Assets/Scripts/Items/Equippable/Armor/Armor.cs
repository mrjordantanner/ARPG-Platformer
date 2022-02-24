using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Armor : IEquippable
    {

        public Guid ID { get; set; }

        public string Name { get; set; }

        public ItemQuality quality { get; set; }

        public ArmorType type { get; set; }

        public List<StatModifier> attributes { get; set; }


        public Armor(ItemQuality _quality, ArmorType _type)
        {
            quality = _quality;
            type = _type;
        }


    }
}
