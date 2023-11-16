using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class Spawner : MonoBehaviour
    {
        public GameObject spawnable;
        public Transform spawnPoint;

        public Transform SpawnPoint
        {
            get => spawnPoint ? spawnPoint : null;
        }

        [Button()]
        protected void Spawn()
        {
            if (spawnable != null)
            {
                GameObject.Instantiate(spawnable, transform.position, Quaternion.identity);
            }
        }
    }
}
