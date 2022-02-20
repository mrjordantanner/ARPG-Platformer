using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class RangedColumnAttack : Skill, ISkill
    {
        public override int Index { get { return 2; } }
        public override string SkillName { get { return "Moonstrike"; } }
        public override Item.Spec SkillSpec { get { return Item.Spec.Ghost; } }
        public override Resource SkillResource { get { return Resource.Magic; } }
        public override float Range { get { return 7f; } }

        public float attackDelay, attackSpeed, attackReset;
        public float effectDuration;
        public float offsetX, offsetY, maxVerticalDistance;

        public GameObject RangedColumnObject;
        public LayerMask ObstacleLayer;

        int faceDir;
        RaycastHit2D hit;
        AreaOfEffect aoe;

        public override void Use()
        {
            if (Stats.Instance.currentMP >= BaseResourceCost || Stats.Instance.resourceCostsRemoved)
                StartCoroutine(Attack());
            else
                HUD.Instance.NotEnoughMP();

        }


        IEnumerator Attack()
        {

            if (player.facingRight) faceDir = 1;
            else faceDir = -1;

            yield return new WaitForSeconds(attackDelay);

            // TODO: ATTACK SPEED - set animation speed to attack speed

            Vector2 offset = new Vector2(offsetX * faceDir, offsetY);
            Vector2 firePosition = (Vector2)(player.transform.position - player.transform.localScale / 2) + offset * transform.localScale.x;
            hit = Physics2D.Raycast(firePosition, Vector2.down, maxVerticalDistance, ObstacleLayer);

            if (hit.collider != null && hit.fraction != 0)
            {
                if (!Stats.Instance.resourceCostsRemoved)
                    Stats.Instance.LoseMP(BaseResourceCost);
                Vector2 effectPosition = new Vector2(offset.x, hit.point.y + RangedColumnObject.transform.localScale.y / 2);
                var newEffectObject = Instantiate(RangedColumnObject, hit.point, Quaternion.identity);
                CameraShaker.Shake(0.08f, 0.08f);
                // TODO: Play sound and animation
                aoe = newEffectObject.AddComponent<AreaOfEffect>();
                aoe.parentSkill = this;

                Destroy(newEffectObject, effectDuration);

                StartCoroutine(CalculateHits());

                player.anim.SetBool("Attacking", true);
                player.anim.SetTrigger("SwingSword");
                player.canAttack = false;
                player.canTurnAround = false;
                if (player.grounded) player.canMove = false;

                yield return new WaitForSeconds(attackReset);

                player.canAttack = true;
                player.canMove = true;
                player.canTurnAround = true;
                player.anim.SetBool("Attacking", false);
                player.anim.ResetTrigger("SwingSword");
            }
            else
                HUD.Instance.ShowMessage("No ground surface within range.", Color.red, 8, 2);


        }

        public IEnumerator CalculateHits()
        {
            yield return new WaitForSeconds(0.05f);
            //print("CB Use() Enemies Hit: " + EnemyHits.Count);
            if (EnemyHits != null && EnemyHits.Count > 0)
            {
                foreach (var enemy in EnemyHits)
                {
                    if (!enemy.dead)
                    {
                        Combat.Instance.EnemyHitByMagic(this, enemy);
                        //print(SkillName + " hit: " + enemy.name);
                    }
                }
            }

            yield return new WaitForSeconds(0.05f);
            aoe.collider2D.enabled = false;
            // TODO: Destroy gameobject after animation has played
            Destroy(aoe.gameObject, effectDuration);
            EnemyHits.Clear();
        }




    }
}