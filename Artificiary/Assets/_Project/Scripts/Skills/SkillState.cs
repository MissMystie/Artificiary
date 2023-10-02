using FMODUnity;
using Mystie.Animation;
using Mystie.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public class SkillState : BaseState, IFrameCheckHandler
    {
        // events

        public event Action onSkillEnd;

        // components

        protected SkillManager SkillManager;
        protected FrameChecker frameChecker;
        protected Skill skill;

        // state

        protected int inputX;

        public SkillState(StateManager ctx, Skill skill) : base(ctx) 
        {
            SkillManager = ctx.entity.SkillManager;
            this.skill = skill;
        }

        public override void EnterState()
        {
            if (skill == null)
            {
                ctx.SetDefault();
                return;
            }

            inputX = ctx.phys.faceDir;

            skill.OnStart(ctx.entity);

            AnimClip animClip = skill.GetAnimClip(anim);
            frameChecker = new FrameChecker(this, animClip);
            anim.Play(animClip.info.animStateName);

            RuntimeManager.PlayOneShotAttached(skill.sfx, ctx.gameObject);
        }

        public override void ExitState()
        {
            if (skill == null) return;

            skill.OnEnd(ctx.entity);
            skill = null;

            ctx.phys.applyGravity = true;
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

        public void InterruptSkill()
        {
            anim.Rebind();
        }

        #region Frame Checker

        public virtual void OnHitFrameStart()
        {
            skill.OnHitFrameStart(ctx.entity);
        }

        public virtual void OnHitFrameEnd()
        {
            skill.OnHitFrameEnd(ctx.entity);
        }

        public void OnLastFrameStart()
        {
        }

        public void OnLastFrameEnd()
        {
        }

        public void OnClipStopped()
        {
            // if the animation stopped playing but the current attack is still ongoing, stop it
            ctx.SetDefault();
            onSkillEnd?.Invoke();
        }

        #endregion

        public override string ToString() { return "Skill: " + skill.name; }
    }
}
