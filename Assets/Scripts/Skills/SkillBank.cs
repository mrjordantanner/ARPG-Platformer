using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Gamekit2D;

namespace Assets.Scripts
{
    public class SkillBank : MonoBehaviour
    {
        // possible bank (or spec) types
        // elemental - fire, ice, water, stone
        // birds - Albatross, Eagle, Falcon
        // Fighter Jets - Mirage, Demon, Falcon, Eagle, Vampire, 
        // Banshee, Zero, Phantom, Baron, Tempest, Warhawk
         
        // Demon, Blood, Phantom/Ghost
        // Demon, Phantom, Vampire
        // Falcon, Eagle, Warhawk
        // Mirage, Tempest, Banshee

        // Phantom X   Warhawk      

        // FUN WITH PHYSICS2D - Pointeffectors, Wind Zone?
        // ALSO PLAY WITH GRAVITY: - It's one of the main mechanics in Side-Scrollers, and it's one that Top-Down ARPG's don't have
        //      Allow some flying enemies to take off and land - Some enemies "crawl" on the BG wall - another way to have them be "in the air" without necessarily flying
        //      Magic skill that makes all flying enemies within range "Earthbound" for X seconds
        //      Gravity-defying abilities

        // Can experiment with different collider shapes!  Use Polygon2D to create interesting areas of effect

        public static SkillBank Instance;

        public Skill ActiveSkillA, ActiveSkillB;  // was ISkill
        public enum SkillSlot { A, B }

        PlayerCharacter player;
        [HideInInspector]
        public GameObject SkillBankUI;

        // make prefabs of these ui elements and then instantiate them when the corresponding skills are created
        //    public GameObject ui_SkillPanel0, ui_SkillPanel1, ui_SkillPanel2;

        // avoid selection changing too fast
        bool canSelect = true;
        float selectionDelayTime = 0.2f;
        [HideInInspector]
        public int skillCounterA, skillCounterB = 0;

        //public List<Skill> skillsList = new List<Skill>();
        // public List<ISkill> skills = new List<ISkill>();

        // public Dictionary<int, ISkill> allSkills = new Dictionary<int, ISkill>();
        public Dictionary<int, Skill> allSkills = new Dictionary<int, Skill>();   // JT 1/5

        // ChaoticBlast chaoticBlast = SkillFactory.GetSkill<ChaoticBlast>();
        ChaoticBlast chaoticBlast;
        EnergyBall chillingOrb;
        RangedColumnAttack moonStrike;
        FireProjectile fireProjectile;

        //DemonicPresence demonicPresence = SkillFactory.GetSkill<DemonicPresence>();
        // DemonSkill3 demonSkill3 = SkillFactory.GetSkill<DemonSkill3>();

        //AortalConduit aortalConduit = SkillFactory.GetSkill<AortalConduit>();
        // VampiricForce vampiricForce = SkillFactory.GetSkill<VampiricForce>();
        // BloodSkill3 bloodSkill3 = SkillFactory.GetSkill<BloodSkill3>();

        // ChillingOrb chillingOrb = SkillFactory.GetSkill<ChillingOrb>();
        // Moonflight moonflight = SkillFactory.GetSkill<Moonflight>();
        // GhostSkill3 ghostSkill3 = SkillFactory.GetSkill<GhostSkill3>();

        public int skillIndexA, skillIndexB;
        public int totalSkillCount;

        public void InitializeSkills()
        {
            chaoticBlast = GetComponent<ChaoticBlast>();
            chillingOrb = GetComponent<EnergyBall>();
            moonStrike = GetComponent<RangedColumnAttack>();
            fireProjectile = GetComponent<FireProjectile>();



            allSkills.Add(chaoticBlast.Index, chaoticBlast);
            allSkills.Add(chillingOrb.Index, chillingOrb);
            allSkills.Add(moonStrike.Index, moonStrike);
            allSkills.Add(fireProjectile.Index, fireProjectile);

            // allSkills.Add(demonicPresence._index, demonicPresence);
            // allSkills.Add(demonSkill3._index, demonSkill3);

            // allSkills.Add(vampiricForce._index, vampiricForce);
            // allSkills.Add(aortalConduit._index, aortalConduit);
            // allSkills.Add(bloodSkill3._index, bloodSkill3);

            // allSkills.Add(chillingOrb._index, chillingOrb);
            // allSkills.Add(moonflight._index, moonflight);
            // allSkills.Add(ghostSkill3._index, ghostSkill3);

            // Print all skills to console
            // foreach (var skill in allSkills)
            //     print(skill.Key + " : " + skill.Value._skillName);

            // set default skills
            ActiveSkillA = fireProjectile;
            ActiveSkillB = chillingOrb;

            skillIndexA = ActiveSkillA.Index;
            skillIndexB = ActiveSkillB.Index;

            totalSkillCount = allSkills.Count;




        }


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
        }

        private void Start()
        {
            player = PlayerRef.Player;

            InitializeSkills();
            UpdateSkillsUI();
            // SkillBankUI.gameObject.SetActive(false);
        }


        void Update()
        {
            if (player != null)
            {
                float horiz = Input.GetAxis("Horizontal");
                float vert = Input.GetAxis("Vertical");

                /*
                // SKILL SLOT A - hold L1
                if (Input.GetKey(player.buttonL1) && canSelect)
                {
                    player.inputSuspended = true;
                    player.canMove = false;
                    player.canTurnAround = false;
                    player.canAttack = false;
                    //  SkillBankUI.gameObject.SetActive(true);  // Skill Wheel UI element
                    */

                if (Input.GetKeyDown(InputManager.Instance.nextSkillA_keyboard) ||
                    (Input.GetKeyDown(InputManager.Instance.nextSkillA_gamepad)))
                {
                    CycleSkill(SkillSlot.A, "Next");
                    canSelect = false;
                    StartCoroutine(SelectionDelay(selectionDelayTime));
                }

                if (Input.GetKeyDown(InputManager.Instance.previousSkillA_keyboard) ||
                    (Input.GetKeyDown(InputManager.Instance.previousSkillA_gamepad)))
                {
                    CycleSkill(SkillSlot.A, "Previous");
                    canSelect = false;
                    StartCoroutine(SelectionDelay(selectionDelayTime));
                }

                if (Input.GetKeyDown(InputManager.Instance.skillA_keyboard) ||
                   (Input.GetKeyDown(InputManager.Instance.skillA_gamepad)))
                {
                    if (!player.inputSuspended && !player.dead && !player.respawning)
                        ActiveSkillA.Use();
                }

                /*
                }

                // RELEASE L1
                if (Input.GetKeyUp(player.buttonL1))
                {
                    player.inputSuspended = false;
                    player.canMove = true;
                    player.canTurnAround = true;
                    player.canAttack = true;
                    // SkillBankUI.gameObject.SetActive(false);
                }
                */

                // SKILL SLOT B - Hold R1
                /*
                if ((Input.GetKey(player.buttonR1) && canSelect))
                {
                    player.inputSuspended = true;
                    player.canMove = false;
                    player.canTurnAround = false;
                    player.canAttack = false;
                    // SkillBankUI.gameObject.SetActive(true);
                */

                if (Input.GetKeyDown(InputManager.Instance.nextSkillB_keyboard) ||
                    (Input.GetKeyDown(InputManager.Instance.nextSkillB_gamepad)))
                {
                    CycleSkill(SkillSlot.B, "Next");
                    canSelect = false;
                    StartCoroutine(SelectionDelay(selectionDelayTime));
                    print("ActiveSkillB: " + ActiveSkillB.SkillName);
                }

                if (Input.GetKeyDown(InputManager.Instance.previousSkillB_keyboard) ||
                    (Input.GetKeyDown(InputManager.Instance.previousSkillB_gamepad)))
                {
                    CycleSkill(SkillSlot.B, "Previous");
                    canSelect = false;
                    StartCoroutine(SelectionDelay(selectionDelayTime));
                    print("ActiveSkillB: " + ActiveSkillB.SkillName);
                }

                if (Input.GetKeyDown(InputManager.Instance.skillB_keyboard) ||
                   (Input.GetKeyDown(InputManager.Instance.skillB_gamepad)))
                {
                    if (!player.inputSuspended && !player.dead && !player.respawning)
                        ActiveSkillB.Use();
                }

                /*
            }


            // RELEASE R1
            if (Input.GetKeyUp(player.buttonR1))
            {
                player.inputSuspended = false;
                player.canMove = true;
                player.canTurnAround = true;
                player.canAttack = true;
                // SkillBankUI.gameObject.SetActive(false);
            }
            */

            }
        }


        // C Y C L E   S K I L L S
        public void CycleSkill(SkillSlot slot, string cycleDirection)
        {
            switch (slot)
            {
                // CYCLE SLOT A
                case (SkillSlot.A):
                    {
                        if (cycleDirection == "Next")
                        {
                            skillIndexA++;

                            if (skillIndexA > totalSkillCount)
                                skillIndexA = 1;

                            if (skillIndexA == skillIndexB)
                                skillIndexA++;

                            if (skillIndexA > totalSkillCount)
                                skillIndexA = 1;
                        }

                        if (cycleDirection == "Previous")
                        {
                            skillIndexA--;

                            if (skillIndexA < 1)
                                skillIndexA = totalSkillCount;

                            if (skillIndexA == skillIndexB)
                                skillIndexA--;

                            if (skillIndexA < 1)
                                skillIndexA = totalSkillCount;
                        }

                        if (allSkills.ContainsKey(skillIndexA))
                            ActiveSkillA = allSkills[skillIndexA];

                        break;
                    }

                // CYCLE SLOT B
                case (SkillSlot.B):
                    {
                        if (cycleDirection == "Next")
                        {
                            skillIndexB++;

                            if (skillIndexB > totalSkillCount)
                                skillIndexB = 1;

                            if (skillIndexB == skillIndexA)
                                skillIndexB++;


                            if (skillIndexB > totalSkillCount)
                                skillIndexB = 1;
                        }

                        if (cycleDirection == "Previous")
                        {
                            skillIndexB--;

                            if (skillIndexB < 1)
                                skillIndexB = totalSkillCount;

                            if (skillIndexB == skillIndexA)
                                skillIndexB--;

                            if (skillIndexB < 1)
                                skillIndexB = totalSkillCount;
                        }


                        if (allSkills.ContainsKey(skillIndexB))
                            ActiveSkillB = allSkills[skillIndexB];


                        break;
                    }
            }

            UpdateSkillsUI();
        }



        public void UpdateWeaponSpecificSkills()
        {


        }


        public void UpdateSkillsUI()
        {
            HUD.Instance.skillSlotAText.text = ActiveSkillA.SkillName;
            HUD.Instance.skillSlotBText.text = ActiveSkillB.SkillName;
        }



        IEnumerator SelectionDelay(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            canSelect = true;
        }


        // TODO:  Maybe have all Skills as individual scripts on the _Skills global gameObject.

        /*
            // DEMON SKILLS ******************
            public class ChaoticBlast : Skill, ISkill
            {
                // Does instant AoE damage to all enemies within a certain radius
                // Also, Curses enemies hit - Curse does X damage over X seconds
                // Skill can be improved to add Knockback using a PointEffector2D and also increase damage

                void Start()
                {
                   // player = PlayerRef.Player;
                    player = FindObjectOfType<PlayerCharacter>();
                }

                PlayerCharacter player;

                public bool uniqueEffects;      // Unique Equipment affecting behavior?
                public List<EnemyCharacter> enemyHitList;
                public PointEffector2D pointEffector;
                public float blastForce;
                CircleCollider2D circleCollider;
                //GameObject particleEffects;

                public Vector2 baseInstantDamage = new Vector2(40, 60);   // *MAG DMG Bonus%
                public float baseDamageOverTime = 5f;                     // *MAG DMG Bonus%
                public float baseDuration = 5f;
                public float range = 8f;

                public Resource _resource { get { return Resource.Magic; } }
                public Spec _spec { get { return Spec.Blood; } }

                public override int _index { get { return 1; } }
                public override string _skillName { get { return "Chaotic Blast"; } }

                public override void Use()
                {
                    var CBlastObj = Instantiate(Instance.ChaoticBlastObject, Instance.player.transform);
                    var AoE = CBlastObj.GetComponent<AreaOfEffect>();
                    foreach (var e in AoE.enemiesHit)
                    {
                        enemyHitList.Add(e);
                        Debug.Log(e.name);
                    }

                    /*
                    GameObject ChaoticBlastObject = Instantiate(new GameObject("X"));//, player.gameObject.transform);
                    ChaoticBlastObject.AddComponent<CBlast>();   // adds Monobehaviour script to allow Collision detection
                    circleCollider = ChaoticBlastObject.AddComponent<CircleCollider2D>();
                    circleCollider.isTrigger = true;
                    circleCollider.radius = range;


                    Destroy(CBlastObj, 1f);
                    enemyHitList.Clear();

                    Debug.Log("Used : " + _skillName);
                }




            }

            public class DemonicPresence : Skill, ISkill
            {
                public Resource _resource { get { return Resource.Magic; } }
                public Spec _spec { get { return Spec.Demon; } }

                public override int _index { get { return 2; } }
                public override string _skillName { get { return "Demonic Presence"; } }

                public override void Use()
                {
                    Debug.Log("Used : " + _skillName);
                }
            }

            public class DemonSkill3 : Skill, ISkill
            {
                public Resource _resource { get { return Resource.Magic; } }
                public Spec _spec { get { return Spec.Blood; } }

                public override int _index { get { return 3; } }
                public override string _skillName { get { return "Demon Skill 3"; } }

                public override void Use()
                {
                    Debug.Log("Used : " + _skillName);
                }
            }

            // BLOOD SKILLS **********************
            public class VampiricForce : Skill, ISkill
            {
                public Resource _resource { get { return Resource.Magic; } }
                public Spec _spec { get { return Spec.Blood; } }

                public override int _index { get { return 4; } }
                public override string _skillName { get { return "Vampiric Force"; } }

                public override void Use()
                {
                    Debug.Log("Used : " + _skillName);
                }

            }

            public class AortalConduit : Skill, ISkill
            {
                public Resource _resource { get { return Resource.Health; } }
                public Spec _spec { get { return Spec.Blood; } }

                public override int _index { get { return 5; } }
                public override string _skillName { get { return "Aortal Conduit"; } }

                public override void Use()
                {
                    Debug.Log("Used : " + _skillName);
                }
            }

            public class BloodSkill3 : Skill, ISkill
            {
                public Resource _resource { get { return Resource.Health; } }
                public Spec _spec { get { return Spec.Blood; } }

                public override int _index { get { return 6; } }
                public override string _skillName { get { return "Blood Skill 3"; } }

                public override void Use()
                {
                    Debug.Log("Used : " + _skillName);
                }
            }



            // GHOST SKILLS ***************************
            public class ChillingOrb : Skill, ISkill
            {
                public Resource _resource { get { return Resource.Magic; } }
                public Spec _spec { get { return Spec.Ghost; } }

                public override int _index { get { return 7; } }
                public override string _skillName { get { return "Chilling Orb"; } }

                public override void Use()
                {
                    Debug.Log("Used : " + _skillName);
                }

            }

            public class Moonflight : Skill, ISkill
            {
                public Resource _resource { get { return Resource.Magic; } }
                public Spec _spec { get { return Spec.Ghost; } }

                public override int _index { get { return 8; } }
                public override string _skillName { get { return "Moonflight"; } }

                public override void Use()
                {
                    Debug.Log("Used : " + _skillName);
                }

            }

            public class GhostSkill3 : Skill, ISkill
            {
                public Resource _resource { get { return Resource.Magic; } }
                public Spec _spec { get { return Spec.Ghost; } }

                public override int _index { get { return 9; } }
                public override string _skillName { get { return "Ghost Skill 3"; } }

                public override void Use()
                {
                    Debug.Log("Used : " + _skillName);
                }

            }
        */

    }



}