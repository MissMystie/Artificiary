using Mystie.Core;
using Mystie.Physics;
using Mystie.UI;
using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie
{
    public class StateManager : MonoBehaviour, IRespawnable
    {
        #region Components

        public Entity entity { get; private set; }
        public InputController controller;
        public PhysicsObject phys { get; private set; }
        public HealthManager health { get; private set; }
        public Animator anim { get; private set; }

        #endregion

        [SerializeField] private MoveController startState;
        public BaseState _currentState { get; protected set; }

        [Header("Debug")]

        public bool showDebug = true;

        protected virtual void Awake()
        {
            // cache components
            entity = Entity.Get(gameObject);
            controller = entity.Controller;
            phys = entity.Phys;
            health = entity.Health;
            anim = entity.Anim;
        }

        private void OnEnable()
        {
            SubInputs(controller);
            SubPhysics(phys);
        }

        private void OnDisable()
        {
            UnsubInputs(controller);
            UnsubPhysics(phys);
        }

        private void Start()
        {
            // setup states
            if (startState != null) SetState(startState.GetState());
        }

        private void Update()
        {
            if (_currentState != null)
            {
                _currentState.UpdateState(Time.deltaTime);
                _currentState.Animate(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (_currentState != null)
            {
                _currentState.UpdatePhysics(Time.deltaTime);
            }
        }

        public void SetState(BaseState state)
        {
            if (_currentState != null)
                _currentState.ExitState();

            _currentState = state;

            if (_currentState != null)
                _currentState.EnterState();
        }

        public void SetDefault()
        {
            SetState(startState.GetState());
        }

        public void OnRespawn()
        {
            if (startState != null)
                SetState(startState.GetState());
        }

        #region Inputs

        protected virtual void SubInputs(InputController controller)
        {
            if (controller == null) return;

            controller.jump.performed += OnJump;
            controller.jump.released += OnJumpRelease;
            controller.dash.performed += OnDash;
        }

        protected virtual void UnsubInputs(InputController controller)
        {
            if (controller == null) return;

            controller.jump.performed -= OnJump;
            controller.jump.released -= OnJumpRelease;
            controller.dash.performed -= OnDash;
        }

        protected virtual void SubPhysics(PhysicsObject physics)
        {
            if (phys == null) return;

            phys.onGrounded += () => _currentState?.OnGrounded();
        }

        protected virtual void UnsubPhysics(PhysicsObject physics)
        {
            if (phys == null) return;

            phys.onGrounded -= () => _currentState?.OnGrounded();
        }

        protected virtual void OnJump()
        {
            _currentState?.Jump();
        }

        protected virtual void OnJumpRelease()
        {
            _currentState?.JumpRelease();
        }

        protected virtual void OnDash()
        {
            _currentState?.Dash();
        }

        #endregion

#if UNITY_EDITOR
        protected virtual void OnGUI()
        {
            if (showDebug && Selection.Contains(gameObject))
            {
                GUIStyle style = new GUIStyle();
                float ratio = UIManager.ScreenScale;
                style.fontSize = (int)Math.Ceiling(24 * ratio);
                style.normal.textColor = Color.white;

                Vector2 size = new Vector2(200f, 48f) * ratio;
                Vector2 offset = new Vector2(48f, 48f) * ratio;
                Vector2 gap = new Vector2(0f, 24f) * ratio;

                //RectData rect = new RectData(200f, 40f, Screen.width - (300f), Screen.height - 90f, 0f, 50f);
                RectData rect = new RectData(size.x, size.y, Screen.width - size.x - offset.x, Screen.height - size.y - offset.y, gap.x, gap.y);

                string displayText = "State: " + (_currentState != null ? _currentState.ToString() : "null");
                GUI.Label(rect.GetRect(0), displayText, style);
            }
        }
#endif
    }
}
