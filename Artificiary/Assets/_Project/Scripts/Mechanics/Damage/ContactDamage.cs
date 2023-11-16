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

        [SerializeField] protected bool disableOnContact = true;
        [SerializeField] protected LayerMask targetMask = -1;
        [SerializeField] protected bool trigger = true;

        protected virtual void OnCollision(Collider2D targetCollider, float magnitude)
        {
            HurtBox target = targetCollider.GetComponent<HurtBox>();
            if (target == null) return;

            Damage(target);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (!trigger) return;

            if (collider.gameObject.IsInLayerMask(targetMask))
            {
                float magnitude = phys ? phys.velocity.magnitude : 0f;
                OnCollision(collider, magnitude);
                if (disableOnContact) gameObject.SetActive(false);
            }

            
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (trigger) return;

            if (collision.collider.gameObject.IsInLayerMask(targetMask))
            {
                OnCollision(collision.collider, collision.relativeVelocity.magnitude);
                if (disableOnContact) gameObject.SetActive(false);
            }
        }
    }
}
