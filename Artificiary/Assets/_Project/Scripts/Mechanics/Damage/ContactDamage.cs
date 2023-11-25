using Mystie.Core;
using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class ContactDamage : DamageBehavior
    {
        [Header("Properties")]

        [SerializeField] protected LayerMask targetMask = -1;
        
        protected virtual void OnCollision(Collider2D targetCollider)
        {
            HurtBox target = targetCollider.GetComponent<HurtBox>();
            if (target != null) Damage(target);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.IsInLayerMask(targetMask))
            {
                OnCollision(collider);
            }
        }
    }
}
