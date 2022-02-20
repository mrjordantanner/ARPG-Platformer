using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class AreaOfEffect : MonoBehaviour
    {
        // Generic trigger script to be attached to effect objects that are briefly instantiated by skills
        // Populates parent skill's enemyHits
        // May also have Animator added to it
        [HideInInspector]
        public Skill parentSkill;
        // public IMagicAttack parentSkill;
        [HideInInspector]
        public Collider2D collider2D;
        [HideInInspector]
        public float lifeSpan;

        private void Awake()
        {
            collider2D = GetComponent<Collider2D>();
            if (lifeSpan > 0)
                Destroy(gameObject, lifeSpan);
        }

        private void Update()
        {
            // TODO: Detect when animation is done playing and 
            // Destroy(gameObject);

        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<EnemyCharacter>();
                if (enemy.isColliding) return;
                enemy.isColliding = true;
                parentSkill.EnemyHits.Add(enemy);

                //foreach (var e in parentSkill.EnemyHits)
                // print("AoE hit: " + e.name);
            }

        }
    }
}