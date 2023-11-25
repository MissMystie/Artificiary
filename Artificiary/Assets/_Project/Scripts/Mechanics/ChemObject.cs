using Mystie.Core;
using Mystie.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    public class ChemObject : MonoBehaviour, IDamageable
    {
        public Entity entity { get; private set; }
        public StatusManager statusMngr { get; private set; }

        public Properties properties;

        void Awake()
        {
            entity = Entity.Get(gameObject);
            statusMngr = entity.StatusMngr;
            properties.ctx = this;
        }

        public virtual void TakeDamage(Damage dmg)
        {
            switch (dmg.type)
            {
                case DamageType.HEAT:
                    if (statusMngr && properties.Flammable) statusMngr.ApplyStatus(StatusType.Burn);
                    break;
                case DamageType.SHOCK:
                    if (statusMngr && properties.Conductive) statusMngr.ApplyStatus(StatusType.Shock);
                    break;
                case DamageType.WATER:
                    if (statusMngr) statusMngr.ApplyStatus(StatusType.Wet);
                    break;
                case DamageType.COLD:
                    if (statusMngr) statusMngr.ApplyStatus(StatusType.Frozen);
                    break;
            }
        }

        [Serializable]
        public class Properties
        {
            [HideInInspector] public ChemObject ctx;

            [SerializeField] private bool flammable;
            [SerializeField] private bool conductive;
            
            public bool Flammable
            {
                get
                {
                    return flammable && !ctx.statusMngr.HasStatus(StatusType.Wet);
                }
            }

            public bool Conductive
            {
                get
                {
                    return conductive || ctx.statusMngr.HasStatus(StatusType.Wet);
                }
            }
        }
    }
}
