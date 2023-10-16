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

    public interface IRespawnable
    {
        void OnRespawn();
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
