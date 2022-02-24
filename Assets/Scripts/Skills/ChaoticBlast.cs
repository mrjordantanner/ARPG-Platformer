using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChaoticBlast : Skill, ISkill
    {
        // public int baseDamageMax;
        //public float baseAccuracy;
        //public bool canCrit;

        public override int Index { get { return 1; } }
        public override string SkillName { get { return "Chaotic Blast"; } }
        //public override Item.Spec SkillSpec { get { return Item.Spec.Ghost; } }
        public override Resource SkillResource { get { return Resource.Magic; } }
        public override float Range { get { return 4f; } }

        // public override List<EnemyCharacter> EnemyHits { get; set; }
        //public override bool CanCrit { get { return true; } }
        //public override float BaseDamageMax { get; set; }
        //public override float BaseAccuracy { get; set; }
        //public override float BaseResourceCost { get; set; }

        public bool uniqueEffects;
        public float blastForce;

        public bool dotCanStack = false;
        public float baseDot = 50f;
        public float baseDotDuration = 5f;
        public float repeatTime = 0.5f;

        bool criticalHit;
        AreaOfEffect aoe;
        CircleCollider2D circleCollider;
        Animation animationClip;
        public GameObject ParticleEffects;
        PointEffector2D pointEffector;


        public override void Use()
        {
            if (Stats.Instance.currentMP >= BaseResourceCost || Stats.Instance.resourceCostsRemoved)
                StartCoroutine(Attack());
            else
                HUD.Instance.NotEnoughMP();

        }


        public IEnumerator Attack()
        {
            if (!Stats.Instance.resourceCostsRemoved)
                Stats.Instance.LoseMP(BaseResourceCost);
            CameraShaker.Shake(0.12f, 0.08f);

            var newEffectObject = new GameObject("Effect Object");
            newEffectObject.transform.position = player.transform.position;
            newEffectObject.layer = 13;

            circleCollider = newEffectObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = Range / 2;

            aoe = newEffectObject.AddComponent<AreaOfEffect>();
            aoe.parentSkill = this;

            var vfx = Instantiate(ParticleEffects, player.transform.position, Quaternion.identity);
            Destroy(vfx, 0.5f);
            AudioManager.Instance.Play("Soul Blast");

            yield return new WaitForSeconds(0.05f);

            //print("CB Use() Enemies Hit: " + EnemyHits.Count);

            if (EnemyHits != null && EnemyHits.Count > 0)
            {
                foreach (var enemy in EnemyHits)
                {
                    if (!enemy.dead)
                    {
                        Combat.Instance.EnemyHitByMagic(this, enemy);
                        Combat.Instance.ApplyStatusEffect(StatusEffect.EffectType.Curse, enemy);
                        Combat.Instance.ApplyStatusEffect(StatusEffect.EffectType.Poison, enemy);
                    }
                }
            }

            yield return new WaitForSeconds(0.05f);
            aoe.collider2D.enabled = false;
            // TODO: Destroy gameobject after animation has played
            Destroy(aoe.gameObject, 0.5f);
            EnemyHits.Clear();
        }







    }

}