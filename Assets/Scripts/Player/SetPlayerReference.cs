using UnityEngine;

//https://stackoverflow.com/questions/59170811/find-reference-in-difference-scene-unity

namespace Assets.Scripts
{
    public class SetPlayerReference : MonoBehaviour
    {
        // already reference it via the Inspector
        [SerializeField] private PlayerCharacter player;

        private void Awake()
        {
            // as a fallback
            if (!player) player = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();

            // assign it to the global class
            PlayerRef.Player = player;
        }

    }
}
