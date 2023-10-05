using Mystie.Core;
using Mystie.Physics;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie.ChemEngine
{
    public class StatusManager : MonoBehaviour
    {
        #region Events

        [HideInInspector] public Action<StatusManager, StatusType> onStatusInflicted;
        [HideInInspector] public Action<StatusManager, StatusType> onStatusExpired;

        #endregion

        public Entity entity { get; private set; }
        public PhysicsObject phys { get; private set; }
        public HealthManager health { get; private set; }
        public ChemObject chem { get; private set; }

        public IDamageable[] damageables { get; private set; }

        public StatusEffectList statusList;
        private Dictionary<StatusType, StatusEffect> statusEffects = new Dictionary<StatusType, StatusEffect>();
        private Dictionary<StatusType, StatusEffectFactory> statusEffectFactories = new Dictionary<StatusType, StatusEffectFactory>();
        
        public const string STATUS_EFFECTS_LIST_FILENAME = "StatusEffects";

        void Awake()
        {
            entity = Entity.Get(gameObject);
            phys = entity.Phys; 
            health = entity.Health;
            chem = entity.Chem;

            damageables = GetComponents<IDamageable>();
            
            if (statusList == null) statusList = Resources.Load<StatusEffectList>(STATUS_EFFECTS_LIST_FILENAME);
            statusEffectFactories = statusList.Get();
        }

        private void OnEnable()
        {
            if (health != null) health.onDeath += OnDeath;
        }

        private void OnDisable()
        {
            if (health != null) health.onDeath -= OnDeath;
        }

        public void Update()
        {
            foreach (StatusEffect status in statusEffects.Values.ToList())
            {
                status.Update(Time.deltaTime);
            }
        }

        public StatusEffect GetStatus(StatusType type)
        {
            return statusEffectFactories[type].GetStatus(this);
        }

        public virtual bool HasStatus(StatusType status)
        {
            return statusEffects.ContainsKey(status);
        }

        public virtual bool ApplyStatus(StatusType statusToApply)
        {
            if (!statusEffects.ContainsKey(statusToApply))
            {
                StatusEffect newStatus = GetStatus(statusToApply);
                if (!newStatus.Apply()) return false;

                statusEffects.Add(statusToApply, newStatus);
                onStatusInflicted?.Invoke(this, statusToApply);
            }
            else
            {
                statusEffects[statusToApply].Reapply();
            }

            return true;
        }

        public virtual void RemoveStatus(StatusType statusToRemove)
        {
            if (statusEffects.ContainsKey(statusToRemove))
            {
                statusEffects[statusToRemove].Expire();
                onStatusExpired?.Invoke(this, statusToRemove);
                statusEffects.Remove(statusToRemove);
            }
        }

        [Button("Remove All Status")]
        public virtual void RemoveAllStatus()
        {
            foreach (StatusType status in statusEffects.Keys.ToList())
            {
                RemoveStatus(status);
            }
        }

        public virtual void OnDeath(HealthManager health)
        {
            foreach (StatusEffect status in statusEffects.Values.ToList())
            {
                status.OnDeath();
            }

            RemoveAllStatus();
        }

        private void OnDrawGizmosSelected()
        {
            foreach (StatusEffect status in statusEffects.Values.ToList())
                status.OnDrawGizmos();
        }
    }
}
