using FMODUnity;
using Mystie.Core;
using NodeCanvas.BehaviourTrees;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public class AttackState : SkillState, IFrameCheckHandler
    {
        protected HitBox hitbox;
        private Attack attack;

        protected List<Collider2D> targetsHit = new List<Collider2D>();

        public AttackState(StateManager ctx, Attack skill) : base (ctx, skill) 
        {
            hitbox = ctx.entity.HitBox;
            attack = skill;
        }

        public override void EnterState()
        {
            if (skill == null)
            {
                ctx.SetDefault();
                return;
            }

            if (hitbox != null)
                hitbox.onCollision += OnCollision;

            base.EnterState();
        }

        public override void ExitState()
        {
            if (skill == null) return;

            targetsHit.Clear();

            hitbox.StopCheckingCol();

            if (hitbox != null)
                hitbox.onCollision -= OnCollision;

            base.ExitState();
        }

        public override void UpdateState(float deltaTime)
        {
            frameChecker.CheckFrames(skill.hitFrameStart, skill.hitFrameEnd);
        }

        public override void UpdatePhysics(float deltaTime)
        {
        }

        public override bool CheckStateTransitions()
        {
            return false;
        }

        #region Frame Checker

        public override void OnHitFrameStart()
        {
            base.OnHitFrameStart();
            hitbox.StartCheckingCol();
        }

        public override void OnHitFrameEnd()
        {
            base.OnHitFrameEnd();
            hitbox.StopCheckingCol();
        }

        #endregion

        #region Damage

        // handle collision with the target
        public void OnCollision(Collider2D[] colliders) 
        {
            bool triggerRecoil = false;

            foreach (Collider2D collider in colliders)
            {
                // if the target was already hit during this attack or the target is the attacker, skip
                if (collider.gameObject == ctx.gameObject 
                    || targetsHit.Contains(collider)) continue;

                targetsHit.Add(collider);
                DealDamage(collider.gameObject);

                triggerRecoil = true;
            }

            if (triggerRecoil)
            {
                Vector2 recoilV = ctx.phys.rb.velocity;
                if (attack.recoilOverrideX 
                    && (Mathf.Sign(recoilV.x) != Mathf.Sign(attack.recoil.x)
                    || Mathf.Abs(attack.recoil.x) > Mathf.Abs(recoilV.x)))
                    recoilV.x = attack.recoil.x * inputX;
                if (attack.recoilOverrideY 
                    && (Mathf.Sign(recoilV.y) == Mathf.Sign(attack.recoil.y)
                    || Mathf.Abs(attack.recoil.y) > Mathf.Abs(recoilV.y))) 
                    recoilV.y = attack.recoil.y;
                phys.SetVelocity(recoilV);
            }

            RuntimeManager.PlayOneShotAttached(attack.hitSfx, ctx.gameObject);
        }

        public bool DealDamage(GameObject target)
        {
            Damage dmg = attack.dmg;
            dmg.value = (int)(attack.dmg.value * SkillManager.AtkMult);

            HurtBox hurtbox = target.GetComponent<HurtBox>();
            hurtbox?.TakeHit(dmg, attack.knocback.GetVelocity(ctx.entity.Collider, hurtbox.Col));

            return hurtbox != null;
        }

        #endregion

        public override string ToString() { return "Attack: " + skill.name; }
    }
}
