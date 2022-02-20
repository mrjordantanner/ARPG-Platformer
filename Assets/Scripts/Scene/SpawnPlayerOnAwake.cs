using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Gamekit2D;

namespace Assets.Scripts
{
    public class SpawnPlayerOnAwake : MonoBehaviour
    {

        public bool spawnPlayer;
        public GameObject PlayerPrefab;

        void Awake()
        {
            PlayerCharacter Player = FindObjectOfType<PlayerCharacter>();

            if (Player == null && spawnPlayer)
                SpawnPlayer();

        }

        void SpawnPlayer()
        {
            GameObject PlayerInstance = Instantiate(PlayerPrefab, transform.position, Quaternion.identity);

        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1f);

        }
    }

}