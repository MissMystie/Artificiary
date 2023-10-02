using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public interface IEmitter
    {
        public GameObject gameObj();
    }

    public interface IEmittable
    {
        public void Emit(Vector2 velocity, Entity emitter);
    }
}
