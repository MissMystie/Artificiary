using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mystie.Core
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private PlayerInput input;
        public PlayerInput Input { get { return input; } }

        [SerializeField] private Transform aimPoint;
        public Vector2 AimPosition
        {
            get { return aimPoint ? aimPoint.position : transform.position; }
        }

        public InputAction moveAction { get; private set; }
        public InputAction aimAction { get; private set; }

        public Vector2 move { get; private set; }
        public Vector2 aim { get; private set; }

        private List<ActionInput> actions = new List<ActionInput>();

        #region Inputs

        public const string MOVE_ACTION = "move";
        public const string AIM_ACTION = "aim";
        public const string JUMP_ACTION = "jump";
        public const string DASH_ACTION = "dash";
        public const string INTERACT_ACTION = "interact";

        public const string ATTACK_ACTION = "attack";
        public const string SHOOT_ACTION = "shoot";

        public const string SKILL1_ACTION = "skill1";
        public const string SKILL2_ACTION = "skill2";

        public const string KEYBOARD_CTRL = "Keyboard";
        public const string GAMEPAD_CTRL = "Gamepad";

        public ActionInput jump;
        public ActionInput dash;
        public ActionInput interact;
        public ActionInput attack;
        public ActionInput shoot;
        public ActionInput skill1;
        public ActionInput skill2;

        #endregion

        private void Awake()
        {
            if (input == null) input = GetComponent<PlayerInput>();

            actions = new List<ActionInput> {
                (jump = new ActionInput(JUMP_ACTION)),
                (dash = new ActionInput(DASH_ACTION)),
                (interact = new ActionInput(INTERACT_ACTION)),
                (attack = new ActionInput(ATTACK_ACTION)),
                (shoot = new ActionInput(SHOOT_ACTION)),
                (skill1 = new ActionInput(SKILL1_ACTION)),
                (skill2 = new ActionInput(SKILL2_ACTION))
            };

            if (input != null) SetInput(input);
        }

        private void OnEnable() 
        {
            GameManager.onPause += DisableInput;
            GameManager.onUnpause += EnableInput;
            if (input != null) input.onControlsChanged += OnControlsChanged;
        }

        private void OnDisable() 
        {
            GameManager.onPause -= DisableInput;
            GameManager.onUnpause -= EnableInput;
            if (input != null) input.onControlsChanged += OnControlsChanged;
        }

        private void Update()
        {
            if (input != null)
            {
                move = moveAction.ReadValue<Vector2>();
                aim = GetAim(AimPosition);
            }
        }

        private void EnableInput()
        {
            input?.ActivateInput();
        }

        private void DisableInput()
        {
            input?.DeactivateInput();
        }

        private void OnControlsChanged(PlayerInput input) 
        {
            Debug.Log("Controls changed");
        }

        private void SetInput(PlayerInput newInput)
        {
            input = newInput;

            moveAction = input.actions[MOVE_ACTION];
            aimAction = input.actions[AIM_ACTION];

            foreach (ActionInput action in actions)
            {
                action.Disable();
                action.Set(input.actions[action.name]);
                action.Enable();
            }
        }

        public Vector2 GetAim(Vector2 aimPosition)
        {
            //Debug.Log("Control scheme: " + input.currentControlScheme);

            if (input.currentControlScheme == KEYBOARD_CTRL)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(aimAction.ReadValue<Vector2>());
                return (mousePos - aimPosition).normalized;
            }
            else
            {
                return aimAction.ReadValue<Vector2>();
            }

        }

        private void Reset()
        {
            input = GetComponent<PlayerInput>();
        }

        
    }

    public class ActionInput
    {
        public event Action performed;
        public event Action released;

        public string name;
        public InputAction input { get; protected set; }

        public bool pressed;
        public float time;

        public ActionInput(string _name)
        {
            name = _name;
        }

        public ActionInput Set(InputAction newInput)
        {
            input = newInput;
            name = input.name;

            return this;
        }

        public void Enable()
        {
            if (input != null)
            {
                input.performed += ctx => { Performed(); };
                input.canceled += ctx => { Released(); };
            }
        }

        public void Disable()
        {
            if (pressed) Released();

            if (input != null)
            {
                input.performed -= ctx => { Performed(); };
                input.canceled -= ctx => { Released(); };
            }
        }

        private void Performed()
        {
            pressed = true;
            performed?.Invoke();
        }

        private void Released()
        {
            pressed = false;
            released?.Invoke();
            time = 0f;
        }
    }
}
