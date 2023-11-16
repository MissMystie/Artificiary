using LDtkUnity;
using MoreMountains.Feedbacks;
using Mystie.Core;
using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Physics
{
    [RequireComponent(typeof(Collider2D))]
    public class BouncePad : MonoBehaviour, ILDtkImportedFields
    {
        public event Action onBounce;

        #region Components

        [Header("Components")]

        [SerializeField] private Collider2D col;
        [SerializeField] private PhysicsObject phys;
        [SerializeField] private Animator anim;

        #endregion

        [SerializeField] protected private LayerMask mask;
        [SerializeField] protected Force force = new Force();
        [SerializeField] protected bool isKinematic = true;

        [Header("Feedback")]

        [SerializeField] protected string bounceAnim = "bounce";
        [SerializeField] protected string bounceTargetAnim = "bounce";
        [SerializeField] protected MMFeedbacks bounceFX;

        [Header("Debug")]

        public bool showDebug = true;

        private void Awake()
        {
            if (col == null) col = GetComponent<Collider2D>();
            if (phys == null) phys = GetComponent<PhysicsObject>();
            if (anim == null) anim = GetComponentInChildren<Animator>();
        }

        private void Bounce(Collider2D targetCol) {
            if (targetCol == null) return;

            Entity target = Entity.Get(targetCol.gameObject);
                targetCol.gameObject.GetComponent<PhysicsObject>();
            if (target.Phys != null)
            {
                Vector2 velocity = force.GetVelocity(col, targetCol);
                target.Phys?.SetVelocity(velocity);

                if (!isKinematic) phys?.SetVelocity(-velocity);

                target.Anim?.SetTrigger(bounceTargetAnim);

                if (anim != null) anim.SetTrigger(bounceAnim);
                if (bounceFX != null) bounceFX?.PlayFeedbacks();

                onBounce?.Invoke();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.gameObject.IsInLayerMask(mask))
            {
                //PhysicsObject targetPhys = col.gameObject.GetComponent<PhysicsObject>();
                //if (targetPhys != null) 
                Bounce(collision.collider);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.IsInLayerMask(mask))
            {
                //PhysicsObject targetPhys = col.gameObject.GetComponent<PhysicsObject>();
                //if (targetPhys != null) 
                Bounce(collider);
            }
        }

        public void OnLDtkImportFields(LDtkFields fields)
        {
            fields.TryGetFloat("strength", out force.strength);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (showDebug)
            {
                Gizmos.color = Color.cyan;
                force.DrawGizmos(col);
            }
        }
#endif
    }
}
