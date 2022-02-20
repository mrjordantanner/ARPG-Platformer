using System;
using UnityEngine;

//https://stackoverflow.com/questions/59170811/find-reference-in-difference-scene-unity

namespace Assets.Scripts
{
    public static class PlayerReferenceStatic
    {
        private static PlayerCharacter player;

        // other classes will always access and set the value through this property
        public static PlayerCharacter Player
        {
            get
            {
                // if the reference exists return it right away
                if (player) return player;

                // as a fallback try to find it 
                player = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();

                return player;
            }

            set
            {
                player = value;

                // invoke an event to tell all listeners that the player
                // was just assigned
                OnPlayerLoaded?.Invoke(player);
            }
        }

        // An event you will invoke after assigning a value making sure that
        // other scripts only access this value after it has been set
        // you can even directly pass the reference in
        public static event Action<PlayerCharacter> OnPlayerLoaded;


    }
}
