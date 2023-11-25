using Mystie.Core;
using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class CollisionDamage : DamageBehavior
    {
        [SerializeField] protected HurtBox hurtbox;
        [SerializeField] protected float minVelocity = 10f;

        [SerializeField] protected bool disableOnContact = true;
        [SerializeField] protected LayerMask obstacleMask = 0;
        [SerializeField] protected LayerMask targetMask = -1;
        [SerializeField] private bool dmgSelf = false;

        protected override void Awake()
        {
            base.Awake();
            if (hurtbox == null) hurtbox = GetComponent<HurtBox>();
        }

        protected virtual void OnCollision(Collider2D targetCollider, float magnitude)
        {
            if (magnitude < minVelocity) return;

            HurtBox target = targetCollider.GetComponent<HurtBox>();
            if (target != null) Damage(target);

            if (dmgSelf && hurtbox != null)
            {
                if (dealsDmg && dealsKB)
                    hurtbox.TakeHit(new Damage(dmg), knockback.GetVelocity(target.Col, col));
                else if (dealsDmg) hurtbox.TakeDamage(new Damage(dmg));
                else if (dealsKB) hurtbox.TakeHit(null, knockback.GetVelocity(col, target.Col));
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.IsInLayerMask(targetMask))
            {
                //OnCollision(collision.collider, collision.relativeVelocity.magnitude);

                float magnitude = phys ? phys.velocity.magnitude : 0f;
                OnCollision(collider, magnitude);
                if (disableOnContact) gameObject.SetActive(false);
            }
            else if (collider.gameObject.IsInLayerMask(obstacleMask))
            {
                if (disableOnContact) gameObject.SetActive(false);
            }
        }
    }
}
