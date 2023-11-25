using Mystie.ChemEngine;
using Mystie.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    [RequireComponent(typeof(StatusManager))]
    public class StatusSwitch : LogicBehavior
    {
        public StatusManager statusMngr;
        public StatusType status;

        protected override void Awake()
        {
            base.Awake();
            if (statusMngr == null) statusMngr = GetComponent<StatusManager>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            statusMngr.onStatusInflicted += (statusMngr, statusInflicted) => OnStatusInflicted(statusInflicted);
            statusMngr.onStatusExpired += (statusMngr, statusInflicted) => OnStatusExpired(statusInflicted);
        }

        protected virtual void OnDisable()
        {
            statusMngr.onStatusInflicted -= (statusMngr, statusInflicted) => OnStatusInflicted(statusInflicted);
            statusMngr.onStatusExpired -= (statusMngr, statusInflicted) => OnStatusExpired(statusInflicted);
        }

        protected void OnStatusInflicted(StatusType statusInflicted)
        {
            if (!_on && statusInflicted == status) SetOn();
        }

        protected void OnStatusExpired(StatusType statusInflicted)
        {
            if (_on && statusInflicted == status) SetOff();
        }
    }
}
