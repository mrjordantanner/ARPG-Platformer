using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IPickup
    {
        IEquippable item {get; set;}
        ItemQuality quality { get; set; }

        Rigidbody2D rb { get; set; }
        LayerMask layerMask { get; set; }
        SpriteRenderer iconSpriteRenderer { get; set; }
        SpriteRenderer bgSpriteRenderer { get; set; }
        Sprite icon { get; set; }
        Sprite qualityBG { get; set; }
        Animator animator { get; set; }

        bool isColliding { get; set; }
        bool hasDropped { get; set; }
        float lifespan { get; set; }




    }
}
