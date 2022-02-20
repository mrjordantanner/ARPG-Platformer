using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for defining player skills
/// </summary>
namespace Assets.Scripts
{
    public class Skill : MonoBehaviour
    {
        [HideInInspector]
        public PlayerCharacter player;

        public virtual void Use() { }

        [HideInInspector]
        public List<EnemyCharacter> EnemyHits;

        // Properties to be set in inspector or initialization of individual skill
        public bool CanCrit;
        public float BaseDamageMax;
        public float BaseAccuracy;
        public float BaseResourceCost;
        public float BaseCooldown;
        public float hitStun;

        // public bool ready;

        public virtual int Index { get; set; }
        public virtual string SkillName { get; set; }
        public virtual Item.Spec SkillSpec { get; set; }
        public virtual Resource SkillResource { get; set; }
        public virtual float Range { get; set; }  


        void Start()
        {
            player = PlayerRef.Player;
            EnemyHits = new List<EnemyCharacter>();
        }

    }



    /*
    public static class SkillFactory
    {
        public static T GetSkill<T>()
            where T : Skill, new()
        {
            return new T();
        }
    }
    */
}







