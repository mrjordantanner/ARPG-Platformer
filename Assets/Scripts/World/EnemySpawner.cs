using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using UnityEditor;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class EnemySpawner : MonoBehaviour
    {
        public enum Type { Single, Pack, Elite }
        public Type type;

        public EnemyMovement.TerrainType terrainType;

        public EnemyCharacter.Category category;

        public bool spawnOnStart;
        public List<GameObject> EnemyPool;

        [HideInInspector]
        public GameObject[] AllEnemies;
        [HideInInspector]
        public GameObject[] SpawnPool;
        //  GameObject ObjectToSpawn;

        [Header("Enemy Pack Size")]
        public Vector2 swarmerPackSize = new Vector2(12, 32);
        public Vector2 rangedPackSize = new Vector2(4, 12);
        public Vector2 meleeStrongPackSize = new Vector2(3, 9);

        [Header("Range from Player")]
        public bool useRange = false;
        public bool playerInRange;
        public float spawnRange = 20f;
        [HideInInspector]
        public float distanceFromPlayerSquared;

        [Header("Position Randomization")]
        public float minRangeX = -2f;
        public float maxRangeX = 2f;
        public float minRangeY = -2f;
        public float maxRangeY = 2f;
        Vector2 offsetX = new Vector2(0.0f, 0.0f);
        Vector2 offsetY = new Vector2(0.0f, 0.0f);
        public float gizmoSize = 2;

        [HideInInspector]
        public EnemyCharacter[] enemyChildren;
        int enemiesMin, enemiesMax;
        int newEnemyIndex;
        bool dontSpawn;

        PlayerCharacter player;
        GameObject EnemyInstance;
        EnemyCharacter enemy;
        SpriteRenderer renderer;

        Transform parent;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position - Vector3.right * minRangeX, transform.position + Vector3.right * maxRangeX);
            Gizmos.DrawLine(transform.position - Vector3.up * minRangeY, transform.position + Vector3.up * maxRangeY);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position - Vector3.right * gizmoSize, transform.position + Vector3.right * gizmoSize);
            Gizmos.DrawLine(transform.position - Vector3.up * gizmoSize, transform.position + Vector3.up * gizmoSize);
        }

        private void Start()
        {
            if (!Application.isEditor)
                player = PlayerRef.Player;

            AllEnemies = SpawnerController.Instance.EnemyBank;
            EnemyPool = new List<GameObject>();
            if (spawnOnStart)
                StartSpawn();

        }



        public void StartSpawn()
        {
            if (!dontSpawn)
            {
                EnemyPool.Clear();
                DestroyChildren();

                switch (category)
                {
                    case EnemyCharacter.Category.Swarmer:
                        enemiesMin = Mathf.RoundToInt(swarmerPackSize.x);
                        enemiesMax = Mathf.RoundToInt(swarmerPackSize.y); ;
                        break;

                    case EnemyCharacter.Category.Ranged:
                        enemiesMin = Mathf.RoundToInt(rangedPackSize.x);
                        enemiesMax = Mathf.RoundToInt(rangedPackSize.y);
                        break;

                    case EnemyCharacter.Category.MeleeStrong:
                        enemiesMin = Mathf.RoundToInt(meleeStrongPackSize.x);
                        enemiesMax = Mathf.RoundToInt(meleeStrongPackSize.y);
                        break;
                }

                foreach (GameObject Enemy in AllEnemies)
                {
                    enemy = Enemy.GetComponent<EnemyCharacter>();
                    if (enemy.category == category)
                        EnemyPool.Add(Enemy);
                }

                // Decide which EnemyPrefab to spawn within selected Category
                foreach (GameObject Enemy in EnemyPool)
                    newEnemyIndex = Random.Range(0, EnemyPool.Count);
                var NewEnemy = EnemyPool[newEnemyIndex];

                // Decide how many enemies to spawn
                var spawnIterations = Random.Range(enemiesMin, enemiesMax);
                SpawnObject(NewEnemy, spawnIterations);

                // "Register" children so they can be reset within Editor if desired
                //if (Application.isEditor)
                //    GetChildren();
                //else
                //    Destroy(gameObject);    // At runtime, destroy Spawner after spawning


                // "Register" children 
                GetChildren();

                // At runtime, destroy Spawner after spawning
                if (!Application.isEditor)
                    Destroy(gameObject);
            }
        }

        void GetChildren()
        {
            enemyChildren = GetComponentsInChildren<EnemyCharacter>();
        }

        public void DestroyChildren()
        {
            //GetChildren();

            if (enemyChildren != null)
            {
                foreach (var enemy in enemyChildren)
                {
                    if (enemy != null)
                        DestroyImmediate(enemy.gameObject);
                }
            }

        }

        void Update()
        {
            // if (useRange)
            //     CalculatePlayerRange();

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                StartSpawn();
            }
        }

        GameObject NewObject;
        GameObject ObjectInstance;

        void SpawnObject(GameObject NewObject, int iterations)
        {
            if (!dontSpawn)
            {
                for (int i = 0; i <= iterations; i++)
                {
                    Vector2 offsetX = new Vector2(Random.Range(-minRangeX, maxRangeX), 0);   // Random X offset
                    Vector2 offsetY = new Vector2(0, Random.Range(-minRangeY, maxRangeY));   // Random Y offset

                    // Makes new object a child of the spawner in Edit Mode only
                    if (!Application.isEditor)
                        ObjectInstance = Instantiate(NewObject, (Vector2)transform.position + (offsetX + offsetY) * transform.localScale.x, Quaternion.identity);
                    else
                        ObjectInstance = Instantiate(NewObject, (Vector2)transform.position + (offsetX + offsetY) * transform.localScale.x, Quaternion.identity, gameObject.transform);

                }

                //print("Spawned " + iterations + " " + NewObject.name + "s");
            }
        }

        void CalculatePlayerRange()
        {
            Vector2 offset = player.transform.position - transform.position;
            distanceFromPlayerSquared = offset.sqrMagnitude;

            if (distanceFromPlayerSquared < (spawnRange * spawnRange))
                playerInRange = true;
            else
                playerInRange = false;

        }

        private void OnApplicationQuit()
        {
            spawnOnStart = false;
            dontSpawn = true;
            DestroyChildren();
            EnemyPool.Clear();
        }

        /*
        public class ReadOnlyAttribute : PropertyAttribute
        {

        }

        [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
        public class ReadOnlyDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property,
                                                    GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            public override void OnGUI(Rect position,
                                       SerializedProperty property,
                                       GUIContent label)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }
        */
    }

}