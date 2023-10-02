using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public abstract class Gear : ScriptableObject
    {
        public new string name = "New Item";
        public Sprite icon = null;

        public abstract void Use(Entity entity);

        public abstract void Release(Entity entity);

        public virtual void Reset()
        {

        }
    }
}
