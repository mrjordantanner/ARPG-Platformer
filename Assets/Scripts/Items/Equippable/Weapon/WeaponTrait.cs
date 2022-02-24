
namespace Assets.Scripts
{
    /// <summary>
    /// Immutable traits that dictate weapon behavior.  
    /// Specific to each WeaponType.
    /// </summary>
    public enum WeaponTrait
    {
        // Animation configs
        AttackDelay,
        AttackDuration,
        AttackReset,

        // OnHit Motion Effects
        AirHover,
        PlayerHitStop,
        EnemyHitStop,

        // Max enemies hittable
        MaxHits,

        // AttackObject Offsets
        GroundAttackX,
        GroundAttackY,
        AirAttackX,
        AirAttackY,
        AirAttackDownX,
        AirAttackDownY,
        CrouchAttackX,
        CrouchAttackY,
        CrouchAttackDownX,
        CrouchAttackDownY

    }
}
