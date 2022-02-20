using System;
using UnityEngine;

namespace Assets.Scripts
{
    public static class PlayerRef 
    {
        private static PlayerCharacter player;

        // Other classes will always access and set the value through this property
        public static PlayerCharacter Player
        {
            get
            {
                if (player) return player;

                player = GameObject.FindObjectOfType<PlayerCharacter>();

                if (player == null)
                {
                    Debug.Log("Player was null.");
                }

                return player;

            }

            set
            {
                player = value;

                OnPlayerLoaded?.Invoke(player);
            }
        }

        public static event Action<PlayerCharacter> OnPlayerLoaded;
    }

}