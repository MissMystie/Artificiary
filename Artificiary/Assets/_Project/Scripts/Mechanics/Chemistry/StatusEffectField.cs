using Mystie.ChemEngine;
using Mystie.Core;
using Mystie.Physics;
using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    public class StatusEffectField : MonoBehaviour
    {
        public StatusType status = StatusType.Wet;
        public float reapplyRate = 1.0f;

        protected Timer reapplyTimer;

        [SerializeField] protected LayerMask mask = -1;

        protected Dictionary<Transform, StatusManager> objDict;

        protected virtual void Awake()
        {
            objDict = new Dictionary<Transform, StatusManager>();
            reapplyTimer = new Timer(reapplyRate);
            reapplyTimer.onTimerEnd += ApplyStatus;
        }

        protected void OnDestroy()
        {
            reapplyTimer.onTimerEnd -= ApplyStatus;
        }

        protected virtual void Update()
        {
            reapplyTimer.Tick(Time.deltaTime);
        }

        protected void ApplyStatus()
        {
            foreach(StatusManager statusManager in objDict.Values)
            {
                statusManager.ApplyStatus(status);
            }

            reapplyTimer.SetTime(reapplyRate);
        }

        protected virtual bool EnterEffector(Transform obj)
        {
            if (obj == null || objDict.ContainsKey(obj)) return false;

            StatusManager statusManager = obj.GetComponent<StatusManager>();
            if (statusManager != null)
            {
                objDict.Add(obj, statusManager);
                objDict[obj].ApplyStatus(status);
            }

            return true;
        }

        protected virtual bool LeaveEffector(Transform obj)
        {
            if (obj == null || !objDict.ContainsKey(obj)) return false;

            objDict[obj].ApplyStatus(status);
            objDict.Remove(obj);

            return true;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.IsInLayerMask(mask))
            {
                EnterEffector(col.transform);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (objDict.ContainsKey(col.transform))
            {
                LeaveEffector(col.transform);
            }
        }
    }
}
