using Mystie.Physics;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class SpawnOnCollision : EffectOnCollision
    {
        public GameObject spawnable;
        public Transform spawnPoint;

        public Vector2 SpawnPos
        {
            get => spawnPoint ? spawnPoint.position : transform.position;
        }

        protected override void OnImpact(GameObject target)
        {
            Spawn();
            base.OnImpact(target);
        }

        [Button()]
        protected void Spawn()
        {
            if (spawnable != null)
            {
                Debug.Log("Spawn!");
                GameObject.Instantiate(spawnable, SpawnPos, Quaternion.identity);
            }
        }
    }
}
