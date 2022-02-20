using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WeaponObject : ItemObject
    {

        public Item.Weapon.Type type;


        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                // TODO: Display popup message like, "Press R1/Spacebar to pick up" etc

                if (isColliding) return;
                isColliding = true;

                // Pick up Item
                //if (Input.GetKeyDown(InputManager.Instance.interact_keyboard) ||
                //    (Input.GetKeyDown(InputManager.Instance.interact_gamepad)))
                //{
                //    if (isColliding && (Inventory.Instance.inventoryItems.Count < Inventory.Instance.inventorySpace))
                //    {
                //        ItemCreator.Instance.CreateWeapon(quality, type);
                //        Destroy(iconSpriteRenderer.gameObject);
                //        // TODO: After picking up, briefly display a small panel to the side showing the item details
                //    }
                //    else
                //        print("Not enough inventory space!");
                //}

            }


        }




    }
}
