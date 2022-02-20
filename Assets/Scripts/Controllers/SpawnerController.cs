using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class SpawnerController : MonoBehaviour
    {
        // [Header("Enemy Pack Size")]
        // public int swarmerPackMin;
        // public int swarmerPackMax;
        // public int rangedPackMin, rangedPackMax;
        // public int meleeStrongPackMin, meleeStrongPackMax;

        public GameObject[] EnemyBank;

        public EnemySpawner[] enemySpawners;
        // public TreasureSpawner[] treasureSpawners;

        #region Singleton
        public static SpawnerController Instance;
        private void Awake()
        {
            if (Application.isEditor)
                Instance = this;
            else
            {
                if (Instance == null)
                    Instance = this;
                else
                {
                    Destroy(gameObject);
                    return;
                }

                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion


        void Start()
        {
            EnemyBank = Resources.LoadAll<GameObject>("Enemies");
            enemySpawners = FindObjectsOfType<EnemySpawner>();
            //treasureSpawners = FindObjectsOfType<TreasureSpawner>();
        }


        public void StartEnemySpawn()
        {
            foreach (var enemySpawner in enemySpawners)
                enemySpawner.StartSpawn();

        }

        public void DestroyEnemyChildren()
        {
            foreach (var enemySpawner in enemySpawners)
            {
                enemySpawner.DestroyChildren();
                enemySpawner.EnemyPool.Clear();
            }
        }

        private void OnApplicationQuit()
        {
            DestroyEnemyChildren();
        }
        /*
        public void StartTreasureSpawn()
        {
            foreach (var treasureSpawner in treasureSpawners)
                treasureSpawner.StartSpawn();

        }

        public void DestroyTreasureChildren()
        {
            foreach (var treasureSpawner in treasureSpawners)
            {
                treasureSpawner.DestroyChildren();
                treasureSpawner.EnemyPool.Clear();
            }
        }
        */


    }

}