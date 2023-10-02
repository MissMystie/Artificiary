using Mystie.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Core
{
    public interface IDamageable
    {
        void TakeDamage(Damage dmg);
    }

    public interface IHittable
    {
        void TakeHit(Vector2 kb);
    }

    public interface IEmittable
    {
        void SetEmitter(GameObject emitter);
    }

    public interface IRespawnable
    {
        void OnRespawn();
    }

    public interface IEffector
    {
        Vector2 GetForce(PhysicsObject target);
    }

    public interface IFrameCheckHandler
    {
        void OnHitFrameStart();
        void OnHitFrameEnd();
        void OnLastFrameStart();
        void OnLastFrameEnd();
        void OnClipStopped();
    }
}
