using MoreMountains.Feedbacks;
using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public class StunState : BaseState
    {
        private Timer timer;
        private string animParam;
        private MMFeedbacks fx;
        
        public StunState(StateManager ctx, float duration, string animParam, MMFeedbacks fx) : base(ctx)
        {
            timer = new Timer(duration);
            this.animParam = animParam;
            this.fx = fx;
        }

        public override void EnterState()
        {
            timer.onTimerEnd += () => CheckStateTransitions();

            ctx.anim?.SetBool(animParam, true);
            fx?.PlayFeedbacks();
        }

        public override void ExitState()
        {
            timer.onTimerEnd -= () => CheckStateTransitions();

            ctx.anim?.SetBool(animParam, false);
            fx?.StopFeedbacks();
        }

        public override void UpdateState(float deltaTime)
        {
            timer.Tick(deltaTime);
        }

        public override void UpdatePhysics(float deltaTime)
        {
        }

        public override bool CheckStateTransitions()
        {
            if (ctx.phys.state.grounded)
            {
                ctx.SetDefault();
                return true;
            }

            return false;
        }

        public override string ToString() { return "Stunned"; }
    }
}
