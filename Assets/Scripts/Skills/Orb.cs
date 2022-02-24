using UnityEngine;

namespace Assets.Scripts
{
    public class Orb : MonoBehaviour
    {
        public enum OrbType { GainDamageOverTime, RandomDamageRange, Freeze }

        [Header("Type")]
        public OrbType orbType;

        [Header("Level")]
        public float velocityIncrease = 1f;     
        public float damageIncrease = 1f;

        [Header("Velocity")]
        public StatRange velocity = new StatRange(20f, 20f);
        public Vector2 offset = new Vector2(0.4f, 0.1f);

        [HideInInspector]
        public float damage;
        float currentDamage;

        [Header("Damage Range")]
        public StatRange damageRange = new StatRange(20, 100);
        public StatRange baseDamageRange = new StatRange(20, 100);

        [Header("Gain Damage")]
        public StatRange baseDamageGainRange = new StatRange(20, 100);
        public float powerLevel;                           // % indicating max power orb is currently at, can be used to affect the damage
        public bool affectWeaponDamage;                    // affect sword damage too? maybe check for certain sword equipped e.g. "Blood" etc
        public float affectWeaponDamageAmount = 1.01f;     // gain 1% wpn damg per powerLevel %
        public float timeToMaxDamage = 5f;
        StatRange damageGainRange;

        float damageTimer;
        float curDamage;
        float damagePerFrame;

        [Header("Behavior")]
        public int cost = 5;
        public bool multiBall = false;
        public bool canFreeze;
        public float chanceToFreeze = 0.10f;

        [Header("VFX")]
        public GameObject Particles;
        public bool trails = true;
        public float trailDuration;
        public float trailRepeatRate;
        public float trailOpacity;

        PlayerCharacter player;
        EnergyBall energyBall;
        EnemyCharacter enemy;
        GameObject CurrentTarget, PreviousTarget;

        void Start()
        {
            InitializeStats();
            player = PlayerRef.Player;
            energyBall = SkillBank.Instance.GetComponent<EnergyBall>();
        }


        void InitializeStats()
        {
            switch (orbType)
            {
                case OrbType.GainDamageOverTime:
                    damageTimer = 0f;
                    damageGainRange.min = baseDamageGainRange.min;
                    damageGainRange.max = baseDamageGainRange.max;
                    currentDamage = damageGainRange.min;
                    curDamage = currentDamage;
                    break;

                case OrbType.RandomDamageRange:
                    damageRange.min = baseDamageRange.min;
                    damageRange.min = baseDamageRange.max;
                    currentDamage = damageRange.min;
                    break;

                case OrbType.Freeze:
                    // damage = 0;
                    damageRange.min = baseDamageRange.min;
                    damageRange.max = baseDamageRange.max;
                    currentDamage = Random.Range(damageRange.min, damageRange.max);
                    break;

            }


        }

        void Update()
        {
            PreviousTarget = null;

            if (orbType == OrbType.GainDamageOverTime)
                GainDamageOverTime();

            damage = currentDamage * (1 + Stats.Instance.MagicDamageBonus.Value);

        }

        void GainDamageOverTime()
        {
            damagePerFrame = (damageGainRange.max - damageGainRange.min) / timeToMaxDamage * Time.deltaTime;

            if (damageTimer < timeToMaxDamage)
            {
                damageTimer += Time.deltaTime;
                curDamage = curDamage + damagePerFrame;
                currentDamage = Mathf.RoundToInt(curDamage);

                // TODO: do something with color of ball or effect, to indicate gradually increasing power
            }

            if (currentDamage >= damageGainRange.max)
                currentDamage = damageGainRange.min;

            powerLevel = damageGainRange.max / currentDamage;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            // collision with enemy triggers
            if (other.gameObject.CompareTag("Enemy"))
            {
                CurrentTarget = other.gameObject;

                // Prevents hitting multiple times on same target
                if (CurrentTarget == PreviousTarget)
                    return;

                enemy = CurrentTarget.GetComponent<EnemyCharacter>();
                if (!enemy.dead)
                    Combat.Instance.EnemyHitByOrb(this, CurrentTarget);

                PreviousTarget = CurrentTarget;

            }

            // Collect projectile
            if (other.gameObject.tag == "Player")
            {
                energyBall.Collect(gameObject);
            }

        }
    }

    /*
// V A R I A N T    I D E A S  


BLOOD BALL:  gains attack damage with every enemy hit

CHARGE UP: gains damage over time exposed

TIME BOMB: does flat rate damage until explosion after x seconds

LONG SHOT: does more damage based on distance from player

SPEED KILLER:  does more damage based on its speed, could encourage long, fast passages
                   and movements to achieve big damage

VARIABLE GRAVITY:  hold/release Circle button while ball is exposed to control its level of 
                         gravity and/or looseness

CONTROL:  control which direction the ball fires with D-pad -- test just like dash

MULTI or DUAL BALL:    combine multiple balls with the d-pad ability

JUGGLE:  combine multiple balls with the d-pad shoot ability

MOLECULE:  multiple balls, different sizes/speeds, effects


*/

}
