using Mystie.Core;
using Mystie.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    public class ChemObject : MonoBehaviour, IDamageable
    {
        public Entity entity { get; private set; }
        public StatusManager status { get; private set; }

        void Awake()
        {
            entity = Entity.Get(gameObject);
            status = entity.Status;
        }

        public virtual void TakeDamage(Damage dmg)
        {
            switch (dmg.type)
            {
                case DamageType.HEAT:
                    if (status) status.ApplyStatus(StatusType.Burn);
                    break;
                case DamageType.SHOCK:
                    if (status) status.ApplyStatus(StatusType.Shock);
                    break;
                case DamageType.WATER:
                    if (status) status.ApplyStatus(StatusType.Wet);
                    break;
                case DamageType.COLD:
                    if (status) status.ApplyStatus(StatusType.Frozen);
                    break;
            }
        }
    }
}
