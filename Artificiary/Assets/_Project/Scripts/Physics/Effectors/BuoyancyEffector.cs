using LDtkUnity;
using Mystie.Core;
using Mystie.Physics;
using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie
{
    public class BuoyancyEffector : FieldEffector, ILDtkImportedFields
    {
        [SerializeField] private Vector2 gravity = new Vector2(0f, -10f);
        public float density = 4f;
        public float flowAngle = 0f;
        public float flowStrength = 0f;
        [SerializeField] protected StatV2.Mod drag = new StatV2.Mod(1.5f);
        [SerializeField] protected float margin = 0.25f;

        public Vector2 AddedVelocity { get => Vector2.right.Rotate(flowAngle) * flowStrength; }

        protected override void Awake()
        {
            base.Awake();
        }

        public override Vector2 GetForce(PhysicsObject target)
        {
            float posY = target.YLevel;
            bool atSurface = posY >= (collider.bounds.max.y - margin);

            if (atSurface)
            {
                return Vector2.zero;
            }

            Vector2 force = -gravity * density * target.volume;
            force += Vector2.right.Rotate(flowAngle) * flowStrength;
            //
            return force;
        }

        public override Vector2 GetAddedVelocity(PhysicsObject target)
        {
            Vector2 addedVelocity = Vector2.right.Rotate(flowAngle + transform.eulerAngles.z) * flowStrength; 
            //flowStrength.normalized.Rotate() * buoyancyStrength;
            return addedVelocity;
        }

        protected override bool EnterEffector(PhysicsObject physObj)
        {
            if (!base.EnterEffector(physObj)) return false;

            physObj.drag.AddMod(drag);
            return true;
        }

        protected override bool LeaveEffector(PhysicsObject physObj)
        {
            if (!base.LeaveEffector(physObj)) return false;

            physObj.drag.RemoveMod(drag);
            return true;
        }

        public void OnLDtkImportFields(LDtkFields fields)
        {
            fields.TryGetFloat("flow_angle", out flowAngle);
            fields.TryGetFloat("flow_strength", out flowStrength);
        }

        protected void Reset()
        {
            collider = GetComponent<BoxCollider2D>();
        }

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Handles.Label(transform.position, "Density: " + density);
            if (collider != null) 
                Gizmos.DrawRay(collider.bounds.min + new Vector3(0, collider.bounds.size.y - margin), new Vector2(collider.bounds.size.x, 0));
            if (flowStrength != 0) 
                GizmosExtensions.DrawArrowRay(transform.position, Vector2.right.Rotate(flowAngle));
        }
#endif
    }
}
