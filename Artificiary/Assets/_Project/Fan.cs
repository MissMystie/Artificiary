using MoreMountains.Feedbacks;
using Mystie.ChemEngine;
using Mystie.Logic;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class Fan : Device
    {
        [SerializeField] private StatusManager statusMngr;
        [SerializeField] private FieldController field;
        [SerializeField] private BuoyancyEffector effector;

        [SerializeField] private float width = 1f;
        [SerializeField] private FieldProperties normalField;
        [SerializeField] private FieldProperties chargedField;

        [SerializeField] private bool _charged;

        [Space]

        public StatusType chargedStatus;

        [Foldout("Feedback")]
        [SerializeField] protected string chargedAnimState = "Charged";
        [Foldout("Feedback")]
        [SerializeField] protected MMFeedbacks chargedFX;

        protected override void Awake()
        {
            if (statusMngr == null) statusMngr = GetComponent<StatusManager>();
            if (field == null) field = GetComponentInChildren<FieldController>();
            if (effector == null) effector = GetComponentInChildren<BuoyancyEffector>();

            base.Awake();
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

        public void SetCharged(bool charged)
        {
            _charged = charged;

            if (field != null)
            {
                Vector2 size = new Vector2(width, _charged? chargedField.range: normalField.range);
                field.SetSize(size);
            }

            if (effector != null)
            {
                effector.density = _charged ? chargedField.strength : normalField.strength;
            }
        }

        public override void SetOn() 
        {
            Vector2 size = new Vector2(width, normalField.range);
            field.SetSize(size);
        }

        public override void SetOff()
        {
            field.SetSize(Vector2.zero);
        }

        [Serializable]
        public class FieldProperties
        {
            public float strength = 4f;
            public float range = 4f;
        }
    }
}
