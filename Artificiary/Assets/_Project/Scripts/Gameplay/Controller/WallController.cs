using FMODUnity;
using Mystie.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public class WallController : MoveController
    {
        [SerializeField] private WallState wallState;

        public override BaseState GetState()
        {
            wallState.SetContext(ctx);
            return wallState;
        }
    }

    [System.Serializable]
    public class WallState : BaseState
    {
        [Header("States")]

        [SerializeField] private MoveController groundState;
        [SerializeField] private MoveController dashState;

        [Header("Wall")]

        [SerializeField] private bool canClimb;
        [SerializeField] private float climbSpeed = 4f;
        [SerializeField] private float wallSlideSpeedMax = 6f;
        [SerializeField] private float minRunUpWallSpeed = 5f;

        private bool climbing;
        private bool runningUpWall;

        public float wallStickTime = 0.15f;
        protected Timer wallStickTimer;

        [Header("Wall Jump")]

        public Vector2 wallJumpUp = new Vector2(7f, 15f);
        public Vector2 wallJumpOff = new Vector2(6f, 1f);
        public Vector2 wallJump = new Vector2(16f, 12f);

        [Space]

        [SerializeField] private string wallJumpUpAnim = "Jump";
        [SerializeField] private string wallJumpOffAnim = "Jump";
        [SerializeField] private string wallJumpAnim = "Jump";

        [Space]

        [SerializeField] private EventReference wallJumpUpSFX;
        [SerializeField] private EventReference wallJumpOffSFX;
        [SerializeField] private EventReference wallJumpSFX;

        [Header("Feedback")]

        [SerializeField] public ParticleSystem wallSlideFX;
        [SerializeField] public ParticleSystem wallJumpFX;
        [SerializeField] private string atWallAnim = "AtWall";
        [SerializeField] private string wallClimbAnim = "Climbing";
        [SerializeField] private string wallRunAnim = "Wallrun";
        [SerializeField] private string wallSlideAnim = "Wallslide";

        public WallState(StateManager ctx) : base(ctx)
        {
        }

        public override void EnterState()
        {
            if (wallStickTimer == null) wallStickTimer = new Timer();
            wallStickTimer.SetTime(wallStickTime);

            runningUpWall = phys.velocity.y >= minRunUpWallSpeed;
            phys.applyGravity = runningUpWall;

            anim?.SetBool(atWallAnim, true);
            if (wallSlideFX) wallSlideFX.Play();
        }

        public override void ExitState()
        {
            runningUpWall = false;
            phys.applyGravity = true;

            if (anim != null)
            {
                anim?.SetBool(atWallAnim, false);
                anim.SetBool(wallRunAnim, false);
                anim.SetBool(wallSlideAnim, false);

            }

            if (wallSlideFX) wallSlideFX.Stop();
        }

        public override void UpdateState(float deltaTime)
        {
            if (CheckStateTransitions()) return;

            if (runningUpWall && Math.Sign(phys.velocity.y) == -1)
                runningUpWall = false;

            climbing = canClimb && ctx.controller.move.y >= 0 && !runningUpWall;

            phys.applyGravity = runningUpWall;
            entity.faceDir = (climbing ? 1 : -1) * phys.state.wallDir;

            // handle wall unstick time

            if (Math.Sign(ctx.controller.move.x) == -phys.state.wallDir)
                wallStickTimer.Tick(deltaTime);
            else
                wallStickTimer.SetTime(wallStickTime);
        }

        public override void UpdatePhysics(float deltaTime)
        {
            Vector2 velocity = Vector2.zero;

            if (climbing)
            {
                velocity = new Vector2(0, ctx.controller.move.y * climbSpeed);
            }
            else if (runningUpWall)
            {
                velocity = phys.velocity;
            }
            else
            {
                velocity = new Vector2(0, -wallSlideSpeedMax);
            }

            if (!wallStickTimer.IsRunning())
            {
                velocity.x = ctx.controller.move.x;
            }

            phys.velocity = velocity;
        }

        public override void Jump()
        {
            phys.state.leavingWall = true;
            ctx.SetState(groundState.GetState());

            Vector2 velocity = phys.velocity;
            int wallDir = phys.state.wallDir;

            // jump off
            if (ctx.controller.move.x == 0)
            {
                velocity.x = -wallDir * wallJumpOff.x;
                if (phys.velocity.y < wallJumpOff.y)
                    velocity.y = wallJumpOff.y;

                RuntimeManager.PlayOneShot(wallJumpOffSFX, ctx.transform.position);
            }
            // jump up
            else if (wallDir == Math.Sign(ctx.controller.move.x))
            {
                velocity.x = -wallDir * wallJumpUp.x;
                if (phys.velocity.y < wallJumpUp.y)
                    velocity.y = wallJumpUp.y;

                RuntimeManager.PlayOneShot(wallJumpUpSFX, ctx.transform.position);
            }
            // leap
            else
            {
                velocity.x = -wallDir * wallJump.x;
                if (phys.velocity.y < wallJump.y)
                    velocity.y = wallJump.y;

                RuntimeManager.PlayOneShot(wallJumpSFX, ctx.transform.position);
            }

            phys.velocity = velocity;
        }

        public override void Dash()
        {
            phys.state.leavingWall = true;
            entity.faceDir = -1 * phys.state.wallDir;
            ctx.SetState(dashState.GetState());
        }

        public override bool CheckStateTransitions()
        {
            if ((phys.state.grounded || !phys.state.atWall))
            {
                ctx.SetState(groundState.GetState());
                return true;
            }

            return false;
        }

        #region Events

        public override void OnGroundedEvent()
        {
            phys.state.leavingWall = false;
        }

        public override void OnWallLeftEvent()
        {
            phys.state.leavingWall = false;
        }

        #endregion

        public override void Animate(float deltaTime)
        {
            anim.SetBool(wallClimbAnim, climbing);
            anim.SetBool(wallRunAnim, runningUpWall);
        }

        public override string ToString() { return "Wall"; }
    }
}
