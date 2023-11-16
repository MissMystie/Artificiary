using Mystie.Core;
using Mystie.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Physics
{
    [System.Serializable]
    public class Force
    {
        public enum Type { FIXED, RADIAL, VELOCITY, NORMAL, REFLECT }
        public Type type = Type.FIXED;

        public float strength = 10f;

        [AllowNesting]
        [EnableIf("type", Type.RADIAL)]
        public Vector2 anchor = Vector2.zero;
        [AllowNesting]
        [EnableIf("type", Type.FIXED), Range(0f, 360f)]
        public float angle = 90f;

        public Vector2 GetVelocity(Collider2D sourceCol, Collider2D targetCol)
        {
            if (sourceCol == null || targetCol == null) return Vector2.zero;

            Vector2 velocity = Vector2.zero;

            switch (type)
            {
                
                case Type.FIXED:
                    velocity = Vector2.right.Rotate(sourceCol.transform.eulerAngles.z + angle);
                    velocity *= strength;

                    // flip the x axis based on the source's position
                    if (sourceCol.transform.lossyScale.x < 0) velocity *= -1;

                    break;
                case Type.RADIAL:
                    velocity = (targetCol.bounds.center.xy() - GetAnchorPos(sourceCol.transform)).normalized * strength;
                    break;
                case Type.VELOCITY:
                    PhysicsObject phys = sourceCol.GetComponent<PhysicsObject>();
                    if (phys != null) velocity = phys.velocity.normalized * strength;
                    break;
                case Type.NORMAL:
                    velocity = (targetCol.bounds.center.xy() - sourceCol.ClosestPoint(targetCol.bounds.center)).normalized * strength;
                    break;
                case Type.REFLECT:
                    Vector2 normal = (targetCol.bounds.center.xy() - sourceCol.ClosestPoint(targetCol.bounds.center)).normalized;
                    velocity = Vector2.Reflect(targetCol.attachedRigidbody.velocity, normal) + normal * strength;
                    break;
            }

            return velocity;
        }

        public Vector2 GetAnchorPos(Transform t)
        {
            return t.position.xy() + anchor.Rotate(t.rotation.eulerAngles.z);
        }

        public void DrawGizmos(Collider2D col)
        {
            if (col == null) return;

            switch (type)
            {
                case Force.Type.FIXED:
                    Vector2 ray = Vector2.right.normalized.Rotate(col.transform.eulerAngles.z + angle);
                    GizmosExtensions.DrawArrowRay(col.transform.position, ray);

                    break;
                case Force.Type.RADIAL:
                    Vector2 anchorPos = GetAnchorPos(col.transform);
                    GizmosExtensions.DrawCross(anchorPos, 0.5f);

                    break;
            }
        }
    }
}
