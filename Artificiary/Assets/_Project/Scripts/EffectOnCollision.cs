using Mystie.Core;
using Mystie.Gameplay;
using Mystie.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mystie.Physics
{
    public class EffectOnCollision : MonoBehaviour
    {
        protected IEmitter emitter;

        public Collider2D col;
        public PhysicsObject phys;
        public HealthManager health;

        [Space]

        public bool affectEmitter = false;

        public bool hasMinVelocity = false;
        [ShowIf("hasMinVelocity")] float minVelocity = 20;

        [Space]

        public LayerMask mask;
        public List<string> neededTags = new List<string>();
        public List<string> requiredTags = new List<string>();
        public List<string> ignoreTags = new List<string>();

        public ParticleSystem impactPFX;

        public UnityEvent onCollision;

        protected virtual void Awake()
        {
            if (!phys) phys = GetComponent<PhysicsObject>();
            if (!col) col = GetComponent<Collider2D>();
            if (!health) health = GetComponent<HealthManager>();
        }

        protected void Reset()
        {
            phys = GetComponent<PhysicsObject>();
            col = GetComponent<Collider2D>();
            health = GetComponent<HealthManager>();
        }

        protected virtual void OnImpact(GameObject target) 
        {
            if (impactPFX)
                Instantiate(impactPFX, col.bounds.center, Quaternion.identity);

            onCollision?.Invoke();
        }

        protected void OnTriggerEnter2D(Collider2D c)
        {
            if (!affectEmitter && emitter != null && c.gameObject == emitter.gameObj())
                return;
            
            if (c.gameObject.IsInLayerMask(mask) && c.gameObject.FilterTags(neededTags, ignoreTags, requiredTags))
            {
                if (phys && hasMinVelocity && phys.velocity.magnitude < minVelocity) return;

                OnImpact(c.gameObject);
            }
        }
    }
}
