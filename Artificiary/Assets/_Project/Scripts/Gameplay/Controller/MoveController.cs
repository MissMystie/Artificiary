using Mystie.Core;
using Mystie.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public abstract class MoveController : MonoBehaviour
    {
        protected StateManager ctx;

        private void Awake()
        {
            ctx = GetComponent<StateManager>();
        }

        public abstract BaseState GetState();
    }

    [System.Serializable]
    public abstract class BaseState
    {
        protected StateManager ctx;
        protected Entity entity;
        protected Actor actor;
        protected PhysicsObject phys;
        protected Animator anim;

        public BaseState(StateManager ctx)
        {
            SetContext(ctx);
        }

        public void SetContext(StateManager ctx)
        {
            this.ctx = ctx;
            entity = ctx.entity;
            actor = ctx.actor;
            phys = ctx.phys;
            anim = ctx.anim;

            phys.onGrounded += () => OnGroundedEvent();
            phys.onWallLeft += () => OnWallLeftEvent();
        }

        public abstract void EnterState();

        public abstract void ExitState();

        public abstract void UpdateState(float deltaTime);

        public abstract void UpdatePhysics(float deltaTime);
        
        public virtual void TickTimers(float deltaTime) { }

        public abstract bool CheckStateTransitions();

        #region Actions

        public virtual void Jump() { }

        public virtual void JumpRelease() { }

        public virtual void Dash() { }

        #endregion

        #region Events

        public virtual void OnGrounded() { }

        public virtual void OnGroundedEvent() { }

        public virtual void OnWallLeftEvent() { }

        #endregion

        public virtual void Animate(float deltaTime) { }
    }
}
