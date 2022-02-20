using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public interface IArmor
    {

        public Type type;
        public ArmorObject armorObject;

        [SerializeField]
        public override List<StatModifier> itemAttributes { get; set; }

        public Icon icon = new Icon();



    }
}
