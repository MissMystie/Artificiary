using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Physics
{
    public interface IHittable
    {
        void TakeHit(Vector2 kb);
    }

    public interface IEffector
    {
        Vector2 GetForce(PhysicsObject target);
    }

    public interface IConstrainer
    {
        void ApplyConstraint(ref Vector2 moveAmount);
    }
}
