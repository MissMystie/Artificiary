using Mystie.Core;
using Mystie.Physics;
using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;

namespace Mystie
{
    public class BuoyancyEffector : FieldEffector
    {
        [SerializeField] private Vector2 gravity = new Vector2(0f, -10f);
        [SerializeField] protected float density = 4f;
        [SerializeField] protected float flowAngle = 0f;
        [SerializeField] protected float flowStrength = 0f;
        [SerializeField] protected StatV2.Mod drag = new StatV2.Mod(1.5f);
        [SerializeField] protected float margin = 0.25f;

        protected override void Awake()
        {
            base.Awake();
        }

        public override Vector2 GetForce(PhysicsObject target)
        {
            float posY = target.col.bounds.center.y;
            bool atSurface = posY >= (col.bounds.max.y - margin);

            if (atSurface)
            {
                return Vector2.zero;
            }

            Vector2 force = -gravity * density * target.volume;
            force += Vector2.right.Rotate(flowAngle) * flowStrength;
            //force = flowStrength.normalized.Rotate(transform.eulerAngles.z) * buoyancyStrength;
            return force;
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

        protected void Reset()
        {
            col = GetComponent<BoxCollider2D>();
        }

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Handles.Label(transform.position, "Density: " + density);
            if (col != null) Gizmos.DrawRay(col.bounds.min + new Vector3(0, col.bounds.size.y - margin), new Vector2(col.bounds.size.x, 0));
            if (flowStrength != 0) 
                GizmosExtensions.DrawArrowRay(transform.position, Vector2.right.Rotate(flowAngle));
        }
#endif
    }
}
