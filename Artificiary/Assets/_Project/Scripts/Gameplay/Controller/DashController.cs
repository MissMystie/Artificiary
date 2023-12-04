using FMODUnity;
using Mystie.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public class DashController : MoveController
    {
        [SerializeField] private DashState dashState;

        public override BaseState GetState()
        {
            dashState.SetContext(ctx);
            return dashState;
        }
    }

    [System.Serializable]
    public class DashState : BaseState
    {
        [Header("States")]

        [SerializeField] private MoveController groundState;

        [Space]

        [SerializeField] private float length = 7f;
        [SerializeField] private float duration = 0.2f;
        [Range(0, 1)] public float damp = 0.8f;

        private Vector2 dashV;
        protected Timer timer;

        [Space]

        public float neutralJump = 12f;
        public float longJump = 18f;
        [Range(0, 1)] public float longJumpDamp = 0.2f;

        [Space]

        public string dashAnim = "dash";
        [SerializeField] private EventReference dashSFX;

        public DashState(StateManager ctx) : base(ctx)
        {
        }

        public override void EnterState()
        {
            if (timer == null) timer = new Timer();
            timer.SetTime(duration);
            timer.onTimerEnd += EndDash;

            dashV = GetDashV();

            if (anim) anim.SetBool(dashAnim, true);

            RuntimeManager.PlayOneShot(dashSFX, ctx.transform.position);
        }

        public override void ExitState()
        {
            timer.onTimerEnd -= EndDash;
            if (anim) anim.SetBool(dashAnim, false);
        }

        public override void UpdateState(float deltaTime)
        {
            timer.Tick(deltaTime);
        }

        public override void UpdatePhysics(float deltaTime)
        {
            ctx.phys.SetVelocity(dashV * (length / duration));
        }

        public void EndDash()
        {
            ctx.SetState(groundState.GetState());
            phys.SetVelocity(phys.localVelocity * (1 - damp));
            if (anim) anim.SetBool(dashAnim, false);
        }

        public Vector2 GetDashV()
        {
            int direction = Math.Sign(ctx.controller.move.x);
            if (direction == 0) direction = ctx.entity.faceDir;

            Vector2 dashV = new Vector2(direction, 0);
            dashV.Normalize();
            return dashV;
        }

        public override void Jump()
        {
            Vector2 v = phys.localVelocity;

            if (phys.state.grounded)
            {
                v = new Vector2(v.x * (1 - longJumpDamp), longJump);
            }
            else
            {
                v = new Vector2(v.x * (1 - damp), neutralJump);
            }

            phys.SetVelocity(v);

            ctx.SetState(groundState.GetState());
        }

        public override bool CheckStateTransitions()
        {
            return false;
        }

        public override void Animate(float deltaTime)
        {

        }

        public override string ToString() { return "Dashing"; }
    }
}
