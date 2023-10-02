using Mystie.Core;
using Mystie.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class DamageBehavior : MonoBehaviour
    {
        public event Action<HurtBox> onDmgDealt;

        [Header("Components")]

        [SerializeField] protected Collider2D col;
        [SerializeField] protected Rigidbody2D rb;

        [Header("Properties")]

        [SerializeField] protected bool dealsDmg = true;
        [SerializeField] protected Damage dmg = new Damage(1);

        [SerializeField] protected bool dealsKB = true;
        [SerializeField] protected Force knockback;

        [Space]

        [SerializeField] protected bool showDebug = true;

        protected virtual void Awake()
        {
            if (col == null) col = GetComponent<Collider2D>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();
        }

        protected virtual bool Damage(HurtBox target)
        {
            if (target == null) return false;

            if (dealsDmg) target.TakeDamage(new Damage(dmg));
            if (dealsKB) target.TakeHit(dmg, knockback.GetVelocity(col, target.Col));

            OnDmgDealtEvent(target);
            return true;
        }

        protected bool Damage(GameObject target)
        {
            HurtBox hurtbox = target.GetComponent<HurtBox>();
            if (hurtbox == null) return false;
            else return Damage(hurtbox);
        }

        protected void OnDmgDealtEvent(HurtBox target)
        {
            onDmgDealt?.Invoke(target);
        }

        protected virtual void Reset()
        {
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (showDebug)
            {
                Gizmos.color = Color.cyan;
                knockback.DrawGizmos(col);
            }
        }
#endif
    }
}
