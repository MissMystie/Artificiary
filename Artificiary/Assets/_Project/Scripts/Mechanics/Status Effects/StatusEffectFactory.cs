using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    public abstract class StatusEffectFactory : ScriptableObject
    {
        public abstract StatusEffect GetStatus(StatusManager target);
    }

    public class StatusEffectFactory<DataType, StatusEffectType> : StatusEffectFactory
        where StatusEffectType : StatusEffect<DataType>, new()
    {
        public DataType data;
        public override StatusEffect GetStatus(StatusManager target)
        {
            return new StatusEffectType { data = this.data, target = target };
        }
    }

    public abstract class StatusEffect
    {
        protected bool expires = true;
        protected float duration;

        public abstract bool Apply();
        public virtual void Reapply() { }
        public abstract void Expire();
        public virtual bool React() { return true; }
        public virtual void Update(float t) { }
        public virtual void OnUpdate(float t) { }
        public virtual void OnDeath() { }
        public abstract StatusType GetStatusType();
        public virtual void OnDrawGizmos() { }
    }

    public abstract class StatusEffect<DataType> : StatusEffect
    {
        public DataType data;
        public StatusManager target;

        public override void Update(float t)
        {
            if (expires)
            {
                duration -= t;
                if (duration <= 0)
                {
                    target.RemoveStatus(GetStatusType());
                    return;
                }
            }

            OnUpdate(t);
        }
    }
}
