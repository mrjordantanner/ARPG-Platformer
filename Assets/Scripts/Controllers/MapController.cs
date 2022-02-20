using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts
{
    public class MapController : MonoBehaviour
    {
        public static MapController Instance;

        // Controls which Modules get loaded into the ModuleNodes, hold references to Modules
        // Controls which Layouts get loaded into the LayoutNodes, holds references to Layouts
        // Layout gameObjects hold possible enemyLayouts as children

        GameObject[] MapConfigs;

        GameObject[] Modules;
        GameObject CurrentModule;
        ModuleNode currentModuleNode;

        GameObject[] Layouts;
        LayoutNode currentLayoutNode;

        GameObject CurrentEnemyLayout;
        EnemyLayoutMember[] enemyLayouts;

        public bool destroyExistingMap = false;

        // TODO: Add TreasureLayout layer



        // EnemyLayoutMembers will still physically be children of the Layout Object
        // possibility to enable multiple enemy layouts at once, overlapping them and increasing enemy density
        // another way to control density would be to have a chance to double the enemy type spawned at each spawner,
        // offsetting physical position slightly




        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);


            // Loads all GameObjects in Assets/Resources/... into arrays
            Layouts = Resources.LoadAll("Layouts").Cast<GameObject>().ToArray();
            Modules = Resources.LoadAll("Modules").Cast<GameObject>().ToArray();
            MapConfigs = Resources.LoadAll("MapConfigs").Cast<GameObject>().ToArray();

            //foreach (var layout in Layouts)
            //{
            //    Debug.Log("Loaded layout: " + layout.gameObject.name);
            //}
        }


        // Map Generation process

        // 1. Randomly select and Instantiate a MapConfig prefab, which is a hand-built map made up of many ModuleNodes,
        //       which are in turn made up of many LayoutNodes
        // 2. Iterate through each ModuleNode, reading its parameters and comparing them against all the possible Modules loaded into Modules[]
        // 3. Any Modules matching the criteria the ModuleNode calls for are added to a list
        // 4. A Module is randomly selected from the list and Instantiated over top of the current ModuleNode

        // 5. Iterate though each LayoutNode contained in the current ModuleNode, 
        //      reading its parameters and comparing them against all the possible Layouts loaded into Layouts[]
        // 6. Any Layouts matching the criteria are added to a list
        // 7. A Layout is randomly selected from the list and is Instantiated
        // 8. EnemyLayout is randomly selected from children and enabled
        // 9. Repeat 2-8 for next ModuleNode until all Nodes have Modules loaded


        void Start()
        {

        }

        void GenerateMap()
        {
            // generate MapConfig
        }


        public void DestroyAllModules()
        {
            var modules = FindObjectsOfType<Module>();
            foreach (Module module in modules)
                DestroyImmediate(module.gameObject);

        }


        public void DestroyAllLayouts()
        {
            var layouts = FindObjectsOfType<Layout>();
            foreach (Layout layout in layouts)
                DestroyImmediate(layout.gameObject);

        }

        // TODO:  Experiment with making this a Coroutine.  Would this allow the map to generate without locking the game?
        //  Find a way to make this work without locking the game

        public void GenerateAll()
        {
            // Reset
            if (destroyExistingMap)
            {
                DestroyAllModules();
                DestroyAllLayouts();
            }

            // M A P   C O N F I G
            // TODO: Randomly select and generate MapConfig prefab

            // M O D U L E
            // Get all ModuleNodes in scene
            ModuleNode[] moduleNodes = FindObjectsOfType<ModuleNode>();

            // Iterate through each ModuleNode
            foreach (ModuleNode moduleNode in moduleNodes)
            {
                // New list to store any Modules that match
                var matchingModules = new List<Module>();

                // Find matching modules
                foreach (GameObject Module in Modules)
                {
                    // Get module script off of gameobject prefab
                    var module = Module.GetComponent<Module>();
                    // Compare Module against ModuleNode
                    if (module.top == moduleNode.top &&
                        module.bottom == moduleNode.bottom &&
                        module.left == moduleNode.left &&
                        module.right == moduleNode.right &&
                        (int)module.areaType == (int)moduleNode.areaType)   // cast to int for enum comparison

                        for (int i = 0; i < module.frequencyMultiplier; i++)
                        {
                            // Add matching Module to list the number of times its frequencyMultiplier dictates                                                    
                            matchingModules.Add(module);
                        }
                }

                if (matchingModules.Count > 0)
                {
                    // Randomly select Module from list of matching modules
                    var randomModule = matchingModules[Random.Range(0, matchingModules.Count)];

                    // Instantiate Module at ModuleNode's position
                    var newModule = Instantiate(randomModule, moduleNode.transform.position, moduleNode.transform.rotation);

                    // L A Y O U T
                    // Create array of all LayoutNodes in the new Module that was created
                    LayoutNode[] layoutNodes = newModule.GetComponentsInChildren<LayoutNode>();

                    // Iterate through each LayoutNode in current Module
                    foreach (LayoutNode layoutNode in layoutNodes)
                    {
                        // New list to store any layouts that match
                        var matchingLayouts = new List<Layout>();

                        // Find matching layouts
                        foreach (GameObject Layout in Layouts)
                        {
                            // Get Layout script off of gameobject prefab
                            var layout = Layout.GetComponent<Layout>();
                            // Compare Layout against LayoutNode
                            if (layout.top == layoutNode.top &&
                                layout.topLeft == layoutNode.topLeft &&
                                layout.topRight == layoutNode.topRight &&
                                layout.bottom == layoutNode.bottom &&
                                layout.bottomLeft == layoutNode.bottomLeft &&
                                layout.bottomRight == layoutNode.bottomRight &&
                                layout.left == layoutNode.left &&
                                layout.leftUpper == layoutNode.leftUpper &&
                                layout.leftLower == layoutNode.leftLower &&
                                layout.right == layoutNode.right &&
                                layout.rightUpper == layoutNode.rightUpper &&
                                layout.rightLower == layoutNode.rightLower &&
                                (int)layout.layoutShape == (int)layoutNode.layoutShape)   // cast to int for enum comparison
                                                                                          //TODO: Specify Area type in Layout Prefab then check for matching AreaType here too

                                for (int i = 0; i < layout.frequencyMultiplier; i++)
                                {
                                    // Add matching Layout to list the number of times its frequencyMultiplier dictates                                                    
                                    matchingLayouts.Add(layout);
                                }
                        }

                        if (matchingLayouts.Count > 0)
                        {
                            // Randomly select Layout from list of matching modules
                            var randomLayout = matchingLayouts[Random.Range(0, matchingLayouts.Count)];

                            // Instantiate Layout at LayoutNode's position
                            var newLayout = Instantiate(randomLayout, layoutNode.transform.position, layoutNode.transform.rotation);

                            // GenerateEnemyLayout();
                        }

                        matchingLayouts.Clear();

                    }

                }

                matchingModules.Clear();

            }



        }




        /*
        void GenerateLayout()
        {

            CurrentLayout = RandomizeLayout();
            //DisableAllLayouts();
            CurrentLayout.SetActive(true);
            GenerateEnemyLayout();
        }
        */

        /*
    GameObject RandomizeLayout()
    {
        int randomIndex = Random.Range(0, Layouts.Length);
        return Layouts[randomIndex];
    }
    */

        /*void DisableAllLayouts()
        {
            for (int i = 0; i < Layouts.Length; i++)
            {
                Layouts[i].SetActive(false);
            }
        }
        */

        // Just generates Enemy Layer within current layout
        /*
            public void GenerateEnemyLayouts()
            {
                enemyLayouts = GetComponentsInChildren<EnemyLayoutMember>(true);
                CurrentEnemyLayout = RandomizeEnemyLayout();
                DisableAllEnemyLayouts();
                if (CurrentEnemyLayout != null)
                    CurrentEnemyLayout.SetActive(true);

            }

            GameObject RandomizeEnemyLayout()
            {
                int randomIndex = Random.Range(0, enemyLayouts.Length);
                if (enemyLayouts.Length == 0) return null;
                return enemyLayouts[randomIndex].gameObject;
            }

            void DisableAllEnemyLayouts()
            {
                for (int i = 0; i < enemyLayouts.Length; i++)
                {
                    enemyLayouts[i].gameObject.SetActive(false);
                }
            }
            */

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
                GenerateAll();

            // if (Input.GetKeyDown(KeyCode.Keypad1))
            //     GenerateModules();

            // if (Input.GetKeyDown(KeyCode.Keypad2))
            //     GenerateLayouts();

            // if (Input.GetKeyDown(KeyCode.Keypad3))
            //     GenerateEnemyLayouts();

        }
    }


}