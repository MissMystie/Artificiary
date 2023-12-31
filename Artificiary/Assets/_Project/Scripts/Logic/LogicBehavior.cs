using FMOD.Studio;
using FMODUnity;
using LDtkUnity;
using MoreMountains.Feedbacks;
using Mystie.Utils;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Logic
{
    public class LogicBehavior : MonoBehaviour, ILDtkImportedFields
    {
        #region events

        public event Action<bool> onSwitch;
        public event Action onSwitchOn;
        public event Action onSwitchOff;

        public void OnSwitchEvent()
        {
            onSwitch?.Invoke(On);
            
            if (On) onSwitchOn?.Invoke();
            else onSwitchOff?.Invoke();
        }

        #endregion

        protected Animator anim;

        [SerializeField]
        protected bool _on = true;
        [SerializeField]
        protected bool _locked = false;
        [SerializeField]
        protected bool _invertOutput = false;
        public bool On { get => _on ^ _invertOutput; } 

        [Space]

        [Foldout("Feedback")]
        [SerializeField] protected string onAnimParam = "On";
        [Foldout("Feedback")]
        [SerializeField] protected string toggleAnimParam = "Toggle";

        [Space]

        [Foldout("Feedback")]
        [SerializeField] protected string onAnimState = "On";
        [Foldout("Feedback")]
        [SerializeField] protected EventReference onSFX;
        [Foldout("Feedback")]
        [SerializeField] protected MMFeedbacks onFX;

        [Space]

        [Foldout("Feedback")]
        [SerializeField] protected string offAnimState = "Off";
        [Foldout("Feedback")]
        [SerializeField] protected EventReference offSFX;
        [Foldout("Feedback")]
        [SerializeField] protected MMFeedbacks offFX;

        [Space]

        [Foldout("Feedback")]
        [SerializeField] protected EventReference toggleSFX;
        [Foldout("Feedback")]
        [SerializeField] protected MMFeedbacks toggleFX;

        [Space]

        [Foldout("Feedback")]
        [SerializeField] protected EventReference onLoop;
        protected EventInstance onLoopInstance;

        [Space]

        [Header("Debug")]

        [SerializeField] protected bool showDebug = true;

        protected virtual void Awake()
        {
            anim = GetComponentInChildren<Animator>();

            if (!onLoop.IsNull)
                onLoopInstance = RuntimeManager.CreateInstance(onLoop);
            RuntimeManager.AttachInstanceToGameObject(onLoopInstance, transform);
        }

        protected virtual void OnEnable()
        {
            SetOnValue(_on);

            if (anim)
            {
                anim.logWarnings = false;
                string animState = _on ? onAnimState : offAnimState;
                if (!animState.IsNullOrEmpty()) 
                    anim.Play(animState);
                anim.Update(0);
            }
        }

        [Button()]
        public virtual void Toggle()
        {
            anim?.SetTrigger(toggleAnimParam);
            toggleFX?.PlayFeedbacks();
            SetOnValue(!_on);
        }

        public virtual void SetOnValue(bool on = true)
        {
            if (on) SetOn();
            else SetOff();
        }

        [Button()]
        public virtual void SetOn()
        {
            _on = true;
            anim?.SetBool(onAnimParam, true);
            onFX?.PlayFeedbacks();
            onLoopInstance.start();

            OnSwitchEvent();
        }

        [Button()]
        public virtual void SetOff()
        {
            _on = false;
            anim?.SetBool(onAnimParam, false);
            onFX?.StopFeedbacks();
            offFX?.PlayFeedbacks();
            onLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            OnSwitchEvent();
        }

        public virtual void SetLocked(bool locked)
        {
            _locked = locked;
        }

        public virtual void OnLDtkImportFields(LDtkFields fields)
        {
            fields.TryGetBool("on", out _on);
            fields.TryGetBool("invert_output", out _invertOutput);
        }
    }
}
