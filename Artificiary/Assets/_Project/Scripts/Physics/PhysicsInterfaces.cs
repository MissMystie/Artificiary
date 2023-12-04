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
        Vector2 GetAddedVelocity(PhysicsObject target);
    }

    public interface IEffectable
    {
        void AddEffector(IEffector effector);
        void RemoveEffector(IEffector effector);
    }

    public interface IConstrainer
    {
        void ApplyConstraint(Transform target, ref Vector2 moveAmount);
    }

    public interface ICarryable
    {
        void Carry();
    }
}
