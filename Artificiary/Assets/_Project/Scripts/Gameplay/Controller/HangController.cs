using Mystie.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.Gameplay
{
    public class HangController : MoveController
    {
        [SerializeField] private HangState hangState;

        public override BaseState GetState()
        {
            hangState.SetContext(ctx);
            return hangState;
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.HasTag(Tags.HANGABLE_TAG))
            {
                hangState.hangableCol = collider;
            }
        }

        public void OnTriggerExit2D(Collider2D collider)
        {
            if (collider == hangState.hangableCol)
            {
                hangState.hangableCol = null;
                hangState.stoppedHanging = false;
            }
        }
    }

    [System.Serializable]
    public class HangState : BaseState
    {
        [SerializeField] private Vector2 input;

        [HideInInspector] public Collider2D hangableCol;
        [HideInInspector] public bool stoppedHanging;

        [Header("States")]

        [SerializeField] private MoveController groundState;

        [Header("Movement")]

        [SerializeField] private float moveSpeed = 6f;

        [SerializeField] private string hangingAnim = "hanging";

        public HangState(StateManager ctx, Collider2D hangableCol) : base(ctx)
        {
            this.hangableCol = hangableCol;
        }

        public override void EnterState() 
        {
            phys.applyGravity = false;

            float posY = hangableCol.bounds.center.y - (2 * phys.col.bounds.extents.y);
            phys.transform.position = new Vector2(phys.transform.position.x, posY);

            anim?.SetBool(hangingAnim, true);
        }

        public override void ExitState() 
        {
            phys.applyGravity = true;

            anim?.SetBool(hangingAnim, false);
        }

        public override void UpdateState(float deltaTime)
        {
            if (CheckStateTransitions()) return;
            input = ctx.controller.move;
            if (input.x != 0)
                ctx.entity.faceDir = Math.Sign(input.x);
        }

        public override void UpdatePhysics(float deltaTime)
        {
            Vector2 velocity = new Vector2(input.x * moveSpeed, 0);
            phys.SetVelocity(velocity);
        }

        public override void Jump()
        {
            ctx.SetState(groundState.GetState());

            stoppedHanging = true;
        }

        public override bool CheckStateTransitions()
        {
            if ((hangableCol == null || stoppedHanging) && groundState)
            {
                ctx.SetState(groundState.GetState());
                return true;
            }

            return false;
        }

        public override void Animate(float deltaTime)
        {
            
        }

        public override string ToString() { return "Hanging"; }
    }
}
