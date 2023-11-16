using Mystie.Core;
using Mystie.Gameplay;
using Mystie.Physics;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class DamageBehavior : MonoBehaviour, IEmittable
    {
        public event Action<HurtBox> onDmgDealt;

        [Header("Components")]

        [SerializeField] protected Collider2D col;
        protected PhysicsObject phys;

        [Header("Properties")]

        [SerializeField] protected bool dealsDmg = true;
        [SerializeField] protected Damage dmg = new Damage(1);
        [SerializeField] protected bool dealsKB = true;
        [ShowIf("dealsKB")][SerializeField] protected Force knockback;
        
        [Space]

        [SerializeField] protected bool disableOnDmgDealt = true;
        [SerializeField] private GameObject damageFX;

        [Space]

        [SerializeField] protected bool showDebug = true;

        private Entity emitter;

        protected virtual void Awake()
        {
            if (col == null) col = GetComponent<Collider2D>();
            if (phys == null) phys = col?.GetComponent<PhysicsObject>();
        }

        protected void OnDisable()
        {
            emitter = null;
        }

        protected virtual bool Damage(HurtBox target)
        {
            if (target == null) return false;

            if (dealsDmg) target.TakeDamage(new Damage(dmg));
            if (dealsKB) target.TakeHit(dmg, knockback.GetVelocity(col, target.Col));

            OnDmgDealtEvent(target);

            if (disableOnDmgDealt)
            {
                if (damageFX != null) Instantiate(damageFX, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }

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

        public void Emit(Entity emitter, Vector2 velocity)
        {
            this.emitter = emitter;
        }

        protected virtual void Reset()
        {
            col = GetComponent<Collider2D>();
            phys = GetComponent<PhysicsObject>();
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
