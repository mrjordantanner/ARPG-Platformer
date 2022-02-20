using UnityEngine;
using Cinemachine;

//https://stackoverflow.com/questions/59170811/find-reference-in-difference-scene-unity

namespace Assets.Scripts
{
    public class LoadPlayer : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        private PlayerCharacter player;

        private void Awake()
        {
            //PlayerPrefab = Resources.Load<GameObject>("Player/Player");
            PlayerPrefab = Resources.Load("Player/Player", typeof(GameObject)) as GameObject;

            //SpawnPlayer();
        }

        void SpawnPlayer()
        {
            //SceneTransitionDestination entrance = SceneController.Instance.GetDestination(SceneController.Instance.defaultDestinationTag);

            GameObject PlayerObject = Instantiate(PlayerPrefab, transform.position, Quaternion.identity);

            //PlayerObject.transform.position = entrance.transform.position;

            // Assign static reference to player
            PlayerRef.Player = PlayerObject.GetComponent<PlayerCharacter>();

            // Set up camera follow
            CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
            cam.Follow = PlayerObject.transform;
        }

        private void Start()
        {
            //PlayerRef.OnPlayerLoaded -= OnPlayerLoaded;
            //PlayerRef.OnPlayerLoaded += OnPlayerLoaded;

            if (!PlayerRef.Player)
            {
                PlayerRef.OnPlayerLoaded -= OnPlayerLoaded;
                PlayerRef.OnPlayerLoaded += OnPlayerLoaded;
            }
            else
            {
                OnPlayerLoaded(PlayerRef.Player);
            }
        }

        private void OnPlayerLoaded(PlayerCharacter player)
        {
            // Clean up callback
            PlayerRef.OnPlayerLoaded -= OnPlayerLoaded;

            if (player)
            {
                Debug.Log($"Player loaded: {player}");
            }
        }
    }
}
