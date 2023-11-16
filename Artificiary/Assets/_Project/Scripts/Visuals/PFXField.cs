using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class PFXField : MonoBehaviour
    {
        public BoxCollider2D col;
        public ParticleSystem pfx;

        [Space]

        public float emissionRate = 10f;

        private void OnEnable()
        {
            UpdatePFX();
        }

        void Update()
        {
            UpdatePFX();
        }

        private void UpdatePFX()
        {
            if (pfx != null && col != null)
            {
                var main = pfx.main;
                main.startLifetimeMultiplier = col.size.y / main.startSpeedMultiplier;

                var shape = pfx.shape;
                shape.radius = col.size.x / 2;

                var emission = pfx.emission;
                emission.rateOverTimeMultiplier = emissionRate * shape.radius;
            }
        }
    }
}
