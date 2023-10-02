using FMOD.Studio;
using FMODUnity;
using Mystie.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.UI
{
    public class UIState : MonoBehaviour
    {
        protected UIManager manager;

        [SerializeField] protected Button submitBtn;
        [SerializeField] protected Button closeBtn;
        [SerializeField] protected bool closeStateOnCancel = true;
        [SerializeField] public UIState pauseState;

        [Space]

        [SerializeField] private GameObject panel;
        [SerializeField] private List<GameObject> uiElements = new List<GameObject>();
        [SerializeField] protected List<NavButton> navButtons = new List<NavButton>();
        [SerializeField] protected bool showCursor = true;

        [Space]

        [SerializeField] private EventReference displaySFX;
        [SerializeField] private EventReference closeSFX;

        protected virtual void Awake()
        {
            manager = UIManager.Instance;
            CloseState();
        }

        protected virtual void OnEnable()
        {
            if (submitBtn != null) submitBtn.onClick.AddListener(Submit);
            if (closeBtn != null) closeBtn.onClick.AddListener(Close);

            foreach (NavButton navButton in navButtons)
            {
                if (navButton.btn != null) navButton.btn.onClick.AddListener(
                    () => { manager.SetState(navButton.state); });
            }
        }

        protected virtual void OnDisable()
        {
            if (submitBtn != null) submitBtn.onClick.RemoveListener(Submit);
            if (closeBtn != null) closeBtn.onClick.RemoveListener(Close);

            foreach (NavButton navButton in navButtons)
            {
                if (navButton.btn != null) navButton.btn.onClick.RemoveListener(
                    () => { manager.SetState(navButton.state); });
            }
        }

        public virtual void DisplayState()
        {
            if (panel != null) panel.SetActive(true);

            if (!uiElements.IsNullOrEmpty())
                foreach (GameObject element in uiElements)
                    element.SetActive(true);

            Cursor.visible = showCursor;

            RuntimeManager.PlayOneShot(displaySFX);
        }

        public virtual void PauseState()
        {
            if (!uiElements.IsNullOrEmpty())
                foreach (GameObject element in uiElements)
                    element.SetActive(false);
        }

        public virtual void CloseState()
        {
            if (panel != null) panel.SetActive(false);

            if (!uiElements.IsNullOrEmpty())
                foreach (GameObject element in uiElements)
                    if (element != null) element.SetActive(false);

            Cursor.visible = !showCursor;
        }

        public virtual void Close()
        {
            if (manager.CurrentState == this)
                manager.CloseState();
        }

        public void SetManager(UIManager ctx)
        {
            manager = ctx;
        }

        public virtual void Escape()
        {
            
        }

        public virtual void Submit() { }

        public virtual void Cancel()
        {
            if (closeStateOnCancel) manager.CloseState();
        }

        public virtual void Pause()
        {
            if (pauseState != null) manager.SetState(pauseState);
        }
    }
}
