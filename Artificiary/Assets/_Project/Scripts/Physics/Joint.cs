using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Physics
{
    public class Joint : MonoBehaviour, IConstrainer
    {
        public PhysicsObject anchorA;
        public PhysicsObject anchorB;

        public float distance = 1f;

        public void Awake()
        {
            anchorB.AddConstraint(this);
        }

        public void ApplyConstraint(Transform target, ref Vector2 moveAmount)
        {
            Vector2 predictedPos = target.position.xy() + moveAmount;
            Vector2 predictedRope = anchorA.transform.position.xy() - predictedPos;

            if (predictedRope.magnitude >= distance)
                moveAmount += (predictedRope.normalized * (predictedRope.magnitude - distance));
        }
    }
}
