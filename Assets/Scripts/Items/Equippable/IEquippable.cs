using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public interface IEquippable
    {
        Guid ID { get; set; }

        string Name { get; set; }

        ItemQuality quality { get; set; }

        List<StatModifier> attributes { get; set; }

    }
}
