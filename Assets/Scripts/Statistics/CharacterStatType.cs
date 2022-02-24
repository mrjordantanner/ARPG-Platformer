
namespace Assets.Scripts
{
    public enum CharacterStatType
    {
        // Common between Player and Enemies
        MaxHealth, Defense, MoveSpeed,

        // Player only
        Strength, Constitution, Agility, Intelligence, Luck,
        AttackSpeed,
        MaxMagic,
        WeaponDamage,
        MagicDamage,
        WeaponDamageBonus,
        MagicDamageBonus,
        CritChance,
        CritDamage,
        DodgeChance,
        //CooldownReduction,
        LifeRegen,
        MagicRegen,
        LifeOnHit,
        MagicOnHit,
        TreasureBonus,
        GoldBonus,
        XPBonus,
        PickupRadius,

        // Enemies only
        XP,
        // MagicDefense,
        ContactDamage
    }

}
