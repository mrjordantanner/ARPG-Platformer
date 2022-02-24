using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class ItemGraphics : MonoBehaviour
    {
        // Loads and holds references to all Item UI and World icons

        #region Singleton
        public static ItemGraphics Instance;
        private void Awake()
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
        #endregion

        //Common Armor - 1
        //Rare Armor - 3 / 4
        //Magical Armor - 2 / 5

        //Weapons
        //Common and Rare 1a - 4a
        //Magical 1b - 4b

        public Backgrounds backgrounds = new Backgrounds();
        public Weapons weapons = new Weapons();
        public Armor armor = new Armor();

        public Sprite[] allBackgrounds;

        private void Start()
        {
            LoadIcons();
        }

        void LoadIcons()
        {
            backgrounds.common = Resources.Load<Sprite>("Icons/Backgrounds/white");
            backgrounds.rare = Resources.Load<Sprite>("Icons/Backgrounds/gold");
            backgrounds.magical = Resources.Load<Sprite>("Icons/Backgrounds/purple");

            allBackgrounds = Resources.LoadAll<Sprite>("Icons/Backgrounds");

            //armor.allArmor = Resources.LoadAll<Sprite>("Icons/Items/Armor");
            armor.helms = Resources.LoadAll<Sprite>("Icons/Items/Armor/Helms");
            armor.mail = Resources.LoadAll<Sprite>("Icons/Items/Armor/Mail");
            armor.cloaks = Resources.LoadAll<Sprite>("Icons/Items/Armor/Gloves");
            armor.bracers = Resources.LoadAll<Sprite>("Icons/Items/Armor/Bracers");
            armor.boots = Resources.LoadAll<Sprite>("Icons/Items/Armor/Boots");

            //weapons.allWeapons = Resources.LoadAll<Sprite>("Icons/Items/Weapons");
            weapons.swords = Resources.LoadAll<Sprite>("Icons/Items/Weapons/Swords");
            weapons.axes = Resources.LoadAll<Sprite>("Icons/Items/Weapons/Axes");
            weapons.maces = Resources.LoadAll<Sprite>("Icons/Items/Weapons/Maces");
            weapons.staves = Resources.LoadAll<Sprite>("Icons/Items/Weapons/Staves");
            weapons.daggers = Resources.LoadAll<Sprite>("Icons/Items/Weapons/Daggers");
        }

        public class Backgrounds
        {
            public Sprite
                common,
                rare,
                magical;
        }

        public class Armor
        {
            public Sprite[]
                helms,
                mail,
                cloaks,
                bracers,
                boots;
        }

        public class Weapons
        {
            public Sprite[]
                swords,
                axes,
                maces,
                staves,
                daggers;
        }



        public Sprite GetRandomIcon(Sprite[] iconArray)
        {
            var randomIndex = Random.Range(0, iconArray.Length - 1);
            return iconArray[randomIndex]; 
        }

        #region Icon Bank
        // Armor
        // Helms
        //Sprite helm_1 = Resources.Load<Sprite>("Items/Armor/helm-1");
        //Sprite helm_2 = Resources.Load<Sprite>("Items/Armor/helm-2");
        //Sprite helm_3 = Resources.Load<Sprite>("Items/Armor/helm-3");
        //Sprite helm_4 = Resources.Load<Sprite>("Items/Armor/helm-4");
        //Sprite helm_5 = Resources.Load<Sprite>("Items/Armor/helm-5");
        //// Mail
        //Sprite mail_1 = Resources.Load<Sprite>("Items/Armor/mail-1");
        //Sprite mail_2 = Resources.Load<Sprite>("Items/Armor/mail-2");
        //Sprite mail_3 = Resources.Load<Sprite>("Items/Armor/mail-3");
        //Sprite mail_4 = Resources.Load<Sprite>("Items/Armor/mail-4");
        //Sprite mail_5 = Resources.Load<Sprite>("Items/Armor/mail-5");
        //// Cloak
        //Sprite cloak_1 = Resources.Load<Sprite>("Items/Armor/cloak-1");
        //Sprite cloak_2 = Resources.Load<Sprite>("Items/Armor/cloak-2");
        //Sprite cloak_3 = Resources.Load<Sprite>("Items/Armor/cloak-3");
        //Sprite cloak_4 = Resources.Load<Sprite>("Items/Armor/cloak-4");
        //Sprite cloak_5 = Resources.Load<Sprite>("Items/Armor/cloak-5");
        //// Bracers
        //Sprite bracers_1 = Resources.Load<Sprite>("Items/Armor/bracers-1");
        //Sprite bracers_2 = Resources.Load<Sprite>("Items/Armor/bracers-2");
        //Sprite bracers_3 = Resources.Load<Sprite>("Items/Armor/bracers-3");
        //Sprite bracers_4 = Resources.Load<Sprite>("Items/Armor/bracers-4");
        //Sprite bracers_5 = Resources.Load<Sprite>("Items/Armor/bracers-5");
        //// Boots
        //Sprite boots_1 = Resources.Load<Sprite>("Items/Armor/boots-1");
        //Sprite boots_2 = Resources.Load<Sprite>("Items/Armor/boots-2");
        //Sprite boots_3 = Resources.Load<Sprite>("Items/Armor/boots-3");
        //Sprite boots_4 = Resources.Load<Sprite>("Items/Armor/boots-4");
        //Sprite boots_5 = Resources.Load<Sprite>("Items/Armor/boots-5");

        //// Weapons
        //// Swords
        //Sprite sword_1a = Resources.Load<Sprite>("Items/Weapons/sword-1a");
        //Sprite sword_2a = Resources.Load<Sprite>("Items/Weapons/sword-2a");
        //Sprite sword_3a = Resources.Load<Sprite>("Items/Weapons/sword-3a");
        //Sprite sword_4a = Resources.Load<Sprite>("Items/Weapons/sword-4a");

        //Sprite sword_1b = Resources.Load<Sprite>("Items/Weapons/sword-1b");
        //Sprite sword_2b = Resources.Load<Sprite>("Items/Weapons/sword-2b");
        //Sprite sword_3b = Resources.Load<Sprite>("Items/Weapons/sword-3b");
        //Sprite sword_4b = Resources.Load<Sprite>("Items/Weapons/sword-4b");

        //// Axes
        //Sprite axe_1a = Resources.Load<Sprite>("Items/Weapons/axe-1a");
        //Sprite axe_2a = Resources.Load<Sprite>("Items/Weapons/axe-2a");
        //Sprite axe_3a = Resources.Load<Sprite>("Items/Weapons/axe-3a");
        //Sprite axe_4a = Resources.Load<Sprite>("Items/Weapons/axe-4a");

        //Sprite axe_1b = Resources.Load<Sprite>("Items/Weapons/axe-1b");
        //Sprite axe_2b = Resources.Load<Sprite>("Items/Weapons/axe-2b");
        //Sprite axe_3b = Resources.Load<Sprite>("Items/Weapons/axe-3b");
        //Sprite axe_4b = Resources.Load<Sprite>("Items/Weapons/axe-4b");

        //// Maces
        //Sprite mace_1a = Resources.Load<Sprite>("Items/Weapons/mace-1a");
        //Sprite mace_2a = Resources.Load<Sprite>("Items/Weapons/mace-2a");
        //Sprite mace_3a = Resources.Load<Sprite>("Items/Weapons/mace-3a");
        //Sprite mace_4a = Resources.Load<Sprite>("Items/Weapons/mace-4a");

        //Sprite mace_1b = Resources.Load<Sprite>("Items/Weapons/mace-1b");
        //Sprite mace_2b = Resources.Load<Sprite>("Items/Weapons/mace-2b");
        //Sprite mace_3b = Resources.Load<Sprite>("Items/Weapons/mace-3b");
        //Sprite mace_4b = Resources.Load<Sprite>("Items/Weapons/mace-4b");

        //// Staves
        //Sprite staff_1a = Resources.Load<Sprite>("Items/Weapons/mace-1a");
        //Sprite staff_2a = Resources.Load<Sprite>("Items/Weapons/mace-2a");
        //Sprite staff_3a = Resources.Load<Sprite>("Items/Weapons/mace-3a");
        //Sprite staff_4a = Resources.Load<Sprite>("Items/Weapons/mace-4a");

        //Sprite staff_1b = Resources.Load<Sprite>("Items/Weapons/mace-1b");
        //Sprite staff_2b = Resources.Load<Sprite>("Items/Weapons/mace-2b");
        //Sprite staff_3b = Resources.Load<Sprite>("Items/Weapons/mace-3b");
        //Sprite staff_4b = Resources.Load<Sprite>("Items/Weapons/mace-4b");

        //// Daggers
        //Sprite dagger_1a = Resources.Load<Sprite>("Items/Weapons/dagger-1a");
        //Sprite dagger_2a = Resources.Load<Sprite>("Items/Weapons/dagger-2a");
        //Sprite dagger_3a = Resources.Load<Sprite>("Items/Weapons/dagger-3a");
        //Sprite dagger_4a = Resources.Load<Sprite>("Items/Weapons/dagger-4a");

        //Sprite dagger_1b = Resources.Load<Sprite>("Items/Weapons/dagger-1b");
        //Sprite dagger_2b = Resources.Load<Sprite>("Items/Weapons/dagger-2b");
        //Sprite dagger_3b = Resources.Load<Sprite>("Items/Weapons/dagger-3b");
        //Sprite dagger_4b = Resources.Load<Sprite>("Items/Weapons/dagger-4b");
        #endregion

        }
    }
