
namespace Assets.Scripts
{
        public enum StatModifierSource
        {
            CoreStat,   // one of the 5 Core Stats
            StatPoints, // a note to be able to reference the actual StatPoints modifier on a Core Stat
            EquippedWeapon, EquippedMail, EquippedCloak, EquippedBracers, EquippedHelm, EquippedBoots,
            StatusEffect,
            Difficulty // for enemies only
        }

}
