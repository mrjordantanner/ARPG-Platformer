using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StatsGUI : MonoBehaviour
    {
        // Stores references to and updates Character Stats Sheet GUI

        [Header("Stat Value Fields")]
        public Text Level_text;
        public Text XP_text;
        public Text nextXPRemaining_text;
        public Text STR_text;
        public Text CON_text;
        public Text AGL_text;
        public Text INT_text;
        public Text LCK_text;
        public Text weaponDamage_text;
        public Text damage_text;
        public Text maxHP_text;
        public Text maxMP_text;
        public Text defense_text;
        public Text critChance_text;
        public Text critDamage_text;
        public Text lifePerSecond_text;
        public Text magicPerSecond_text;
        public Text lifeOnHit_text;
        public Text magicOnHit_text;
        public Text movementSpeed_text;
        public Text weaponDamageBonus_text;
        public Text magicDamageBonus_text;
        public Text dodgeChance_text;
        public Text goldBonus_text;
        public Text defenseBonus_text;
        public Text treasureBonus_text;
        public Text xpBonus_text;
        public Text goldAmount_text;
        public Text statPoints_text;
        public Text difficulty_text;


        [Header("Comparison Value Fields")]
        public Text weaponDamage_comparison_text;
        public Text damage_comparison_text;
        public Text maxHP_comparison_text;
        public Text maxMP_comparison_text;
        public Text defense_comparison_text;
        public Text critChance_comparison_text;
        public Text critDamage_comparison_text;
        public Text lifePerSecond_comparison_text;
        public Text magicPerSecond_comparison_text;
        public Text lifeOnHit_comparison_text;
        public Text magicOnHit_comparison_text;
        public Text movementSpeed_comparison_text;
        public Text weaponDamageBonus_comparison_text;
        public Text magicDamageBonus_comparison_text;
        public Text dodgeChance_comparison_text;
        public Text goldBonus_comparison_text;
        public Text defenseBonus_comparison_text;
        public Text treasureBonus_comparison_text;
        public Text xpBonus_comparison_text;


        public void UpdateStatsGUI()
        {
            statPoints_text.text = Stats.Instance.statPoints.ToString();
            Level_text.text = Stats.Instance.playerLevel.ToString();
            XP_text.text = Stats.Instance.XP.ToString();
            nextXPRemaining_text.text = Stats.Instance.nextXPRemaining.ToString();
            STR_text.text = Stats.Instance.Strength.Value.ToString();
            CON_text.text = Stats.Instance.Constitution.Value.ToString();
            AGL_text.text = Stats.Instance.Agility.Value.ToString();
            INT_text.text = Stats.Instance.Intelligence.Value.ToString();
            LCK_text.text = Stats.Instance.Luck.Value.ToString();
            weaponDamage_text.text = Stats.Instance.WeaponDamage.Value.ToString();
            damage_text.text = Stats.Instance.EstimateWeaponDamage().ToString();
            maxHP_text.text = Stats.Instance.MaxHealth.Value.ToString();
            maxMP_text.text = Stats.Instance.MaxMagic.Value.ToString();
            defense_text.text = FormatPercentage(Stats.Instance.Defense.Value);
            critChance_text.text = FormatPercentage(Stats.Instance.CritChance.Value);
            critDamage_text.text = (100 + (100 * Stats.Instance.CritDamage.Value)).ToString() + "%";
            lifePerSecond_text.text = FormatPercentage(Stats.Instance.LifeRegen.Value);
            magicPerSecond_text.text = FormatPercentage(Stats.Instance.MagicRegen.Value);
            lifeOnHit_text.text = FormatPercentage(Stats.Instance.LifeOnHit.Value);
            magicOnHit_text.text = FormatPercentage(Stats.Instance.MagicOnHit.Value);
            movementSpeed_text.text = Stats.Instance.MoveSpeed.Value.ToString();
            weaponDamageBonus_text.text = FormatPercentage(Stats.Instance.WeaponDamageBonus.Value);
            magicDamageBonus_text.text = FormatPercentage(Stats.Instance.MagicDamageBonus.Value);
            goldBonus_text.text = FormatPercentage(Stats.Instance.GoldBonus.Value);
            treasureBonus_text.text = FormatPercentage(Stats.Instance.TreasureBonus.Value);
            xpBonus_text.text = FormatPercentage(Stats.Instance.XPBonus.Value);
            dodgeChance_text.text = FormatPercentage(Stats.Instance.DodgeChance.Value);
            goldAmount_text.text = Stats.Instance.gold.ToString();
            difficulty_text.text = Difficulty.Instance.difficultyLevel.ToString();
        }


        public string FormatPercentage(float value)
        {
            string s = (100 * value).ToString() + "%";
            return s;
        }

        public void UpdateStatsComparisonGUI()
        {

            // TODO

        }



    }


}