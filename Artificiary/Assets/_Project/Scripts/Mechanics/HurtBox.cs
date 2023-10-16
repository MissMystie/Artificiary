using MoreMountains.Feedbacks;
using Mystie.Physics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie.Core
{
    public class HurtBox : MonoBehaviour
    {
        [SerializeField] protected Collider2D col;
        public Collider2D Col { get { return col; } }

        public bool active = true;

        [SerializeField] private Vulnerabilities vulnerabilities = new Vulnerabilities();

        public Color inactiveColor = Color.gray;
        public Color activeColor = Color.green;

        private List<IHittable> hittables = new List<IHittable>();
        private List<IDamageable> damageables = new List<IDamageable>();

        [Header("Feedback")]

        [SerializeField] private MMFeedbacks hitFX;

        protected void Start()
        {
            if (col == null) col = GetComponent<Collider2D>();
            hittables = GetComponentsInChildren<IHittable>().ToList();
            damageables = GetComponentsInChildren<IDamageable>().ToList();
        }

        public void TakeHit(Damage dmg, Vector2 kbVelocity)
        {
            if (!active) return;

            TakeDamage(dmg);

            foreach (IHittable hittable in hittables)
                hittable.TakeHit(kbVelocity);

            hitFX?.PlayFeedbacks();
        }

        public void TakeDamage(Damage dmg)
        {
            if (!active) return;

            Debug.Log("Damage taken!");

            dmg.value = vulnerabilities.ApplyDamageRate(dmg);

            foreach (IDamageable damageable in damageables)
            {
                Debug.Log("Take damage");
                damageable.TakeDamage(dmg);
            }
        }

        protected void CheckGismosColor()
        {
            Gizmos.color = active ? activeColor : inactiveColor;
        }

        protected void Reset()
        {
            if (col == null) col = GetComponent<Collider2D>();
        }
    }
}
