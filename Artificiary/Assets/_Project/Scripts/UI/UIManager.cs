using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        private Canvas canvas;
        private CanvasScaler scaler;
        private Vector2 refResolution = new Vector2(1920, 1080);

        public static Vector2 RefResolution { get { return Instance.refResolution; } }
        public static float ScreenScale
        {
            get { return Instance.canvas != null ? Screen.width / RefResolution.x : 1f; }
        }

        private Controls controls;

        [SerializeField] public UIState startState;
        protected Stack<UIState> stateStack = new Stack<UIState>();

        public UIState CurrentState { get { return (stateStack.Count > 0) ? stateStack.Peek() : null; } }

        void Awake()
        {
            //If an instance already exists
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            canvas = GetComponent<Canvas>();
            scaler = canvas.GetComponent<CanvasScaler>();
            refResolution = scaler.referenceResolution;

            controls = GameManager.controls;
        }

        protected void OnEnable()
        {
            controls.UI.Submit.performed += ctx => { CurrentState?.Submit(); };
            controls.UI.Cancel.performed += ctx => { CurrentState?.Cancel(); };
            controls.UI.Pause.performed += ctx => { CurrentState?.Pause(); };
        }

        protected void OnDisable()
        {
            controls.UI.Submit.performed -= ctx => { CurrentState?.Submit(); };
            controls.UI.Cancel.performed -= ctx => { CurrentState?.Cancel(); };
            controls.UI.Pause.performed -= ctx => { CurrentState?.Pause(); };

            GameManager.Unpause();
        }

        protected void Start()
        {
            SetState(startState);
        }

        public void SetState(UIState newState)
        {
            if (newState == null) return;

            if (CurrentState != null) CurrentState.PauseState(); // pause the current state

            newState.SetManager(this);
            newState.DisplayState();

            stateStack.Push(newState); // we push the new state on top of the stack
        }

        public void CloseState()
        {
            if (CurrentState == null) return;

            stateStack.Pop().CloseState(); // we close the current state
            CurrentState?.DisplayState();
        }

        public void ClearStates()
        {
            while (CurrentState != null) CloseState();
        }
    }
}
