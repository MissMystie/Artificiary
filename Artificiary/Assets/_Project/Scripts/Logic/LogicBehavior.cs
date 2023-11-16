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
        protected bool _on = false;
        [SerializeField]
        protected bool _locked = false;
        [SerializeField]
        protected bool _invertOutput = false;
        public bool On { get => _on ^ _invertOutput; } 

        [Space]

        [Foldout("Feedback")]
        [SerializeField] protected string onAnimParam = "on";

        [Space]

        [Foldout("Feedback")]
        [SerializeField] protected string onAnimState = "on";
        [Foldout("Feedback")]
        [SerializeField] protected EventReference onSFX;
        [Foldout("Feedback")]
        [SerializeField] protected MMFeedbacks onFX;

        [Space]

        [Foldout("Feedback")]
        [SerializeField] protected string offAnimState = "off";
        [Foldout("Feedback")]
        [SerializeField] protected EventReference offSFX;
        [Foldout("Feedback")]
        [SerializeField] protected MMFeedbacks offFX;

        [Space]

        [Header("Debug")]

        [SerializeField] protected bool showDebug = true;

        protected virtual void Awake()
        {
            anim = GetComponentInChildren<Animator>();
        }

        protected virtual void OnEnable()
        {
            SetOnValue(_on);

            if (anim)
            {
                anim.logWarnings = false;
                string animState = _on ? onAnimState : offAnimState;
                if (!animState.IsNullOrEmpty()) anim.Play(animState);
                anim.Update(0);
            }
        }

        [Button()]
        public virtual void Toggle()
        {
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

            OnSwitchEvent();
        }

        [Button()]
        public virtual void SetOff()
        {
            _on = false;
            anim?.SetBool(onAnimParam, false);
            onFX?.StopFeedbacks();
            offFX?.PlayFeedbacks();

            OnSwitchEvent();
        }

        public virtual void OnLDtkImportFields(LDtkFields fields)
        {
            fields.TryGetBool("on", out _on);
            fields.TryGetBool("invert_output", out _invertOutput);
        }
    }
}
