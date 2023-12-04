using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.Gameplay
{
    public class SwimController : MoveController
    {
        [SerializeField] private SwimState swimState;

        public override BaseState GetState()
        {
            swimState.SetContext(ctx);
            return swimState;
        }
    }

    [System.Serializable]
    public class SwimState : BaseState
    {
        [SerializeField] private Vector2 input;

        [Header("States")]

        [SerializeField] private MoveController groundState;

        [Header("Movement")]

        [SerializeField] private Vector2 moveSpeed = new Vector2(8f, 8f);
        [SerializeField] private Vector2 acc = new Vector2(0.1f, 0.1f);
        [SerializeField] private Vector2 friction = new Vector2(0.175f, 0.175f);

        private int direction = 0;

        [Header("Jump")]

        [SerializeField] private bool canJump = true;
        [SerializeField] private float jumpVelocity = 8f;
        [SerializeField] private float jumpTime = 0.2f;
        [SerializeField] private EventReference jumpSFX;

        [Header("Feedback")]

        [SerializeField] private string swimmingAnim = "Swimming";
        //[SerializeField] private ParticleSystem walkingPFX;

        [Header("Debug")]

        [SerializeField] private bool showDebug = true;

        public SwimState(StateManager ctx) : base(ctx)
        {
        }

        public override void EnterState()
        {
            input = GetInput();
            direction = 0;
            anim?.SetBool(swimmingAnim, true);
        }

        public override void ExitState()
        {
            direction = 0;
            anim?.SetBool(swimmingAnim, false);
        }

        public override void UpdateState(float deltaTime)
        {
            if (CheckStateTransitions()) return;
            input = GetInput();

            if (input.x != 0)
                ctx.entity.faceDir = Math.Sign(input.x);
        }

        public override void UpdatePhysics(float deltaTime)
        {
            Vector2 v = ctx.phys.localVelocity;

            // move
            if (input.x != 0)
            {
                v.x = input.x * moveSpeed.x;
            }
            // apply friction
            else
            {
                v.x *= (1 - friction.x);
            }

            // move
            if (input.y != 0)
            {
                v.y = input.y * moveSpeed.y;
            }
            // apply friction
            else
            {
                v.y *= (1 - friction.y);
            }

            if (v.y > 0 && ctx.phys.state.atSurface)
                v.y = 0f;

            ctx.phys.SetVelocity(v);
        }

        public override bool CheckStateTransitions()
        {
            if (groundState && !ctx.phys.state.immersed)
            {
                ctx.SetState(groundState.GetState());
                return true;
            }

            return false;
        }

        public Vector2 GetInput()
        {
            Vector2 input = ctx.controller.move;

            return input;
        }

        public override void Jump()
        {
            if (ctx.phys.state.atSurface)
            {
                ctx.phys.state.immersed = false;
                ctx.SetState(groundState.GetState());

                Vector2 v = ctx.phys.localVelocity;
                v.y = jumpVelocity;
                ctx.phys.SetVelocity(v);

                RuntimeManager.PlayOneShot(jumpSFX, ctx.transform.position);
            }
        }

        public override void Animate(float deltaTime)
        {
            
        }

        public override string ToString() { return "Swimming"; }
    }
}
