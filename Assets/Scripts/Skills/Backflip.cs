using UnityEngine;

namespace Assets.Scripts
{
    class Backflip : Skill, ISkill
    {
        public override int Index { get { return 5; } }
        public override string SkillName { get { return "Backflip"; } }
        //public override Item.Spec SkillSpec { get { return Item.Spec.Bone; } }
        public override Resource SkillResource { get { return Resource.Stamina; } }
        public override float Range { get { return 3f; } }

        public float attackDelay, attackSpeed, attackReset;
        public float effectDuration;
        public float offsetX, offsetY, maxVerticalDistance;

        public LayerMask ObstacleLayer;

        int faceDir;
        RaycastHit2D hit;
        AreaOfEffect aoe;

        public GameObject BackflipSlash;
        public bool isBackflipping;
        public float backflipSpeed = 30f;
        public float backflipMaxVelocity = 5f;

        // Can only backflip near the apex of the jump, dictated by this value
        int backflipFrameCount;
        int backflipFrames;

        public override void Use()
        {
            if (Stats.Instance.currentMP >= BaseResourceCost || Stats.Instance.resourceCostsRemoved)
                HandleBackflipMechanics();
            else
                HUD.Instance.NotEnoughMP();

        }

        void Update()
        {
            if (isBackflipping)
                HandleBackflipMechanics();

        }

        void HandleBackflipMechanics()
        {
            // Input: Attack and Up direction 
            if (
                (Input.GetKeyDown(InputManager.Instance.meleeAttack_keyboard) ||
                Input.GetKeyDown(InputManager.Instance.meleeAttack_gamepad))
                && player.vert > 0 && !player.grounded && !player.inputSuspended)
                StartBackflip();

            if (isBackflipping)
                BackFlipRotation();
        }

        void StartBackflip()
        {
            BackflipSlash.SetActive(true);
            isBackflipping = true;
            player.canDash = false;
            player.canAttack = false;
            player.anim.SetBool("Backflip", true);
            backflipFrameCount = 0;
            backflipFrames = Mathf.RoundToInt(360 / backflipSpeed);
        }

        void BackFlipRotation()
        {
            if (player.facingRight)    // Rotate CCW
                player.PlayerGraphics.transform.Rotate(0, 0, backflipSpeed);

            else                // Rotate CW
                player.PlayerGraphics.transform.Rotate(0, 0, backflipSpeed);

            backflipFrameCount++;
            if (backflipFrameCount >= backflipFrames)
                ResetBackflip();
        }

        void ResetBackflip()
        {
            BackflipSlash.SetActive(false);
            player.PlayerGraphics.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(Vector3.zero));
            player.anim.SetBool("Backflip", false);
            player.canDash = true;
            player.canAttack = true;
            isBackflipping = false;
        }

    }
}
