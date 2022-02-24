using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Weapon : IEquippable
    {

        public Guid ID { get; set; }

        public string Name { get; set; }

        public ItemQuality quality { get; set; }

        public WeaponType type { get; set; }

        public List<StatModifier> attributes { get; set; }

        public float damageRange { get; set; }


        public Weapon(ItemQuality _quality, WeaponType _type)
        {
            quality = _quality;
            type = _type;
        }
    }
}
