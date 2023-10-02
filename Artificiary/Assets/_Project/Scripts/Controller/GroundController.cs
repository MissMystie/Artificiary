using FMODUnity;
using Mystie.Core;
using Mystie.Core;
using System;
using UnityEngine;

namespace Mystie
{
    public class GroundController : MoveController
    {
        [SerializeField] private GroundState groundState;

        public override BaseState GetState()
        {
            groundState.SetContext(ctx);
            return groundState;
        }
    }

    [System.Serializable]
    public class GroundState : BaseState
    {
        [SerializeField] private Vector2 input;

        [Header("States")]

        [SerializeField] private MoveController wallState;
        [SerializeField] private MoveController dashState;
        [SerializeField] private MoveController swimState;

        [Header("Movement")]

        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float acc = 0.1f;
        [SerializeField] private float accAir = 0.08f;
        [SerializeField] private float friction = 0.175f;
        [SerializeField] private Vector2 drag = new Vector2(.005f, 0);

        [Header("Jump")]

        [SerializeField] private bool canJump = true;
        
        [SerializeField] private float jumpTime = 0.2f;

        private int jumpCount;
        [SerializeField] private int jumpCountMax = 2;
        [SerializeField] private float maxJumpVelocity = 12f;
        [SerializeField] private float minJumpVelocity = 8f;

        [Space]

        public Timer coyoteTimer;
        public float coyoteTime = 1f;

        [Space]

        public string jumpAnim = "jump";
        public string doubleJumpAnim = "doubleJump";
        [SerializeField] private EventReference jumpSFX;

        [Header("Dash")]

        [SerializeField] private bool canDash = true;
        [SerializeField] private float dashCD = 0.1f;
        
        private int dashCount;
        [SerializeField] private int dashCountMax = 1;

        protected Timer dashCDTimer;

        [Header("Feedback")]

        [SerializeField] private ParticleSystem walkingPFX;

        [Header("Debug")]

        [SerializeField] private bool showDebug = true;

        public GroundState(StateManager ctx) : base(ctx)
        {
        }

        public override void EnterState()
        {
            if (dashCDTimer == null) dashCDTimer = new Timer();

            input = GetInput();
        }

        public override void ExitState()
        {
        }

        public override void UpdateState(float deltaTime)
        {
            input = GetInput();
            if (input.x != 0)
                phys.faceDir = Math.Sign(input.x);

            TickTimers(deltaTime);
            if (CheckStateTransitions()) return;
        }

        public override void TickTimers(float deltaTime)
        {
            dashCDTimer.Tick(deltaTime);
        }

        public override void UpdatePhysics(float deltaTime)
        {
            Vector2 v = phys.rb.velocity;
            
            // move
            if (input.x != 0)
            {
                // if moving in a different direction or going lower than the movespeed, accelerate
                if (((Math.Sign(input.x) == Math.Sign(v.x)) && (Mathf.Abs(v.x) < moveSpeed))
                    || (Math.Sign(input.x) != Math.Sign(v.x)))
                {
                    v.x += input.x * moveSpeed * (phys.state.grounded ? acc : accAir);
                    v.x = Mathf.Clamp(v.x, -moveSpeed, moveSpeed);
                }
            }
            // apply friction
            else
            {
                v.x *= (1 - (phys.state.grounded ? friction : drag.x));
            }

            phys.rb.velocity = v;
        }

        public override bool CheckStateTransitions()
        {
            if (swimState != null && phys.state.immersed)
            {
                ctx.SetState(swimState.GetState());
                return true;
            }
            else if (!phys.state.grounded && phys.state.atWall 
                 && !phys.state.leavingWall && wallState != null)
            {
                ctx.SetState(wallState.GetState());
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
            if (phys.state.grounded && input.y < 0)
            {
                Collider2D colBelow = phys.groundCol;

                /*
                if (colBelow.gameObject.HasTag(Tags.LEDGE_TAG))
                {
                    return;
                }
                if (colBelow.gameObject.HasTag(Tags.THROUGH_TAG))
                {
                    return;
                }*/
            }

            if (canJump && (phys.state.grounded || jumpCount > 0))
            {
                Vector2 v = phys.rb.velocity;
                v.y = maxJumpVelocity;

                // If midair
                if (phys.state.grounded)
                {
                    anim?.SetTrigger(jumpAnim);
                }
                else
                {
                    // Override horizontal velocity when jumping for more accuracy if the current velocity is
                    // slower than the move speed or inputting in the opposite direction
                    v.x = input.x * moveSpeed;
                    jumpCount--;
                    anim?.SetTrigger(doubleJumpAnim);
                }

                phys.rb.velocity = v;

                coyoteTimer.SetTime(0f);

                RuntimeManager.PlayOneShot(jumpSFX, ctx.transform.position);
            }
        }

        public override void JumpRelease()
        {
            if ((phys.rb.velocity.y > minJumpVelocity)) 
            {
                Vector2 v = phys.rb.velocity;
                v.y = minJumpVelocity;
                phys.rb.velocity = v;
            }
        }

        public override void Dash()
        {
            if (canDash && (phys.state.grounded || dashCount > 0) && !dashCDTimer.IsRunning())
            {
                if (!phys.state.grounded)
                    dashCount--;
                else
                    coyoteTimer.SetTime(0f);

                dashCDTimer.SetTime(dashCD);

                ctx.SetState(dashState.GetState());
            }
        }

        public void Crouch()
        {

        }

        public void UnCrouch()
        {

        }

        public override void OnGrounded()
        {
            base.OnGrounded();
            ResetMoveCounts();
            coyoteTimer.SetTime(coyoteTime);
        }

        public void ResetMoveCounts()
        {
            jumpCount = jumpCountMax;
            dashCount = dashCountMax;
        }

        public void CalculateJumpVelocity(float maxJumpHeight, float timeToJumpApex)
        {

        }

        public override void Animate(float deltaTime)
        {
            
        }

        public override string ToString() { return "Grounded"; }
    }
}
