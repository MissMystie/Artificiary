using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Physics
{
    [RequireComponent(typeof(PhysicsObject))]
    public class BuoyantObject : MonoBehaviour, IConstrainer
    {
        public PhysicsObject phys;
        public PhysicsObject connectedObject;

        public float buoyancy = 10f;
        public float distance = 1f;

        private void Awake()
        {
            if (phys == null) phys = GetComponent<PhysicsObject>();
            connectedObject.AddConstraint(this);
        }

        private void FixedUpdate()
        {
            Vector2 v = Vector2.up * buoyancy;
            
            phys.velocity = v;
        }

        public void ApplyConstraint(Transform target, ref Vector2 moveAmount)
        {
            Vector2 predictedPos = target.position.xy() + moveAmount;
            Vector2 predictedRope = phys.transform.position.xy() - predictedPos;

            if (predictedRope.magnitude >= distance)
                moveAmount += (predictedRope.normalized * (predictedRope.magnitude - distance));
        }
    }
}
