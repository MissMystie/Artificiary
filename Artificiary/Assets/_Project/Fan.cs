using FMOD.Studio;
using FMODUnity;
using MoreMountains.Feedbacks;
using Mystie.ChemEngine;
using Mystie.Core;
using Mystie.Gameplay;
using Mystie.Logic;
using Mystie.Physics;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class Fan : Device, IDamageable, ICarryable
    {
        [SerializeField] private StatusManager statusMngr;
        [SerializeField] private FieldController field;
        [SerializeField] private BuoyancyEffector effector;

        [SerializeField] private float width = 1f;
        [SerializeField] private FieldProperties normalField;
        [SerializeField] private FieldProperties chargedField;
        [SerializeField] private DamageType activateDmgType;
        [SerializeField] private bool _charged;

        [Space]

        public StatusType chargedStatus;

        [Foldout("Feedback")]
        [SerializeField] protected string chargedAnimParam = "Charged";
        [Foldout("Feedback")]
        [SerializeField] protected string chargedAnimState = "Charged";
        [Foldout("Feedback")]
        [SerializeField] protected MMFeedbacks chargedFX;

        [Foldout("Feedback")]
        [SerializeField] protected EventReference chargedLoop;
        protected EventInstance chargedLoopInstance;

        protected override void Awake()
        {
            if (statusMngr == null) statusMngr = GetComponent<StatusManager>();
            if (field == null) field = GetComponentInChildren<FieldController>();
            if (effector == null) effector = GetComponentInChildren<BuoyancyEffector>();

            base.Awake();

            if (!chargedLoop.IsNull)
                chargedLoopInstance = RuntimeManager.CreateInstance(chargedLoop);
            RuntimeManager.AttachInstanceToGameObject(chargedLoopInstance, transform);
        }

        protected override void OnEnable()
        {
            statusMngr.onStatusInflicted += (statusMngr, statusInflicted) => OnStatusInflicted(statusInflicted);
            statusMngr.onStatusExpired += (statusMngr, statusInflicted) => OnStatusExpired(statusInflicted);

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            statusMngr.onStatusInflicted -= (statusMngr, statusInflicted) => OnStatusInflicted(statusInflicted);
            statusMngr.onStatusExpired -= (statusMngr, statusInflicted) => OnStatusExpired(statusInflicted);

            base.OnDisable();
        }

        protected void OnStatusInflicted(StatusType statusInflicted)
        {
            if (statusInflicted == chargedStatus)
            {
                if (!_charged) SetCharged(true);
            }
        }

        protected void OnStatusExpired(StatusType statusInflicted)
        {
            if (statusInflicted == chargedStatus)
            {
                if (_charged) SetCharged(false);
            }
        }

        public override void SetOn() 
        {
            base.SetOn();
            Vector2 size = new Vector2(width, normalField.range);
            field.SetSize(size);
            field.gameObject.SetActive(true);

            if (_charged) SetChargedOn();
        }

        public override void SetOff()
        {
            base.SetOff();
            //field.SetSize(Vector2.zero);
            field.gameObject.SetActive(false);

            if (_charged) SetChargedOff();
        }

        public void TakeDamage(Damage dmg)
        {
            if (dmg.type == activateDmgType) 
            {
                Toggle();
            }
        }

        public void SetCharged(bool charged)
        {
            _charged = charged;

            anim?.SetBool(chargedAnimParam, _charged);

            if (field != null)
            {
                Vector2 size = new Vector2(width, _charged ? chargedField.range : normalField.range);
                field.SetSize(size);
            }

            if (effector != null)
            {
                effector.density = _charged ? chargedField.strength : normalField.strength;
            }

            if (_charged) SetChargedOn();
            else SetChargedOff();
        }

        protected void SetChargedOn()
        {
            chargedLoopInstance.start();
            chargedFX?.PlayFeedbacks();
        }

        protected void SetChargedOff()
        {
            chargedLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            chargedFX?.StopFeedbacks();
        }

        public void OnCarry(InteractBehavior interactor)
        {
            SetOff();
            SetLocked(true);
        }

        public void OnDrop(InteractBehavior interactor)
        {
            SetLocked(false);
        }

        [Serializable]
        public class FieldProperties
        {
            public float strength = 4f;
            public float range = 4f;
        }
    }
}
