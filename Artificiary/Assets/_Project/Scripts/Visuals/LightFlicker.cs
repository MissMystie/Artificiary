using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Mystie
{
    public class LightFlicker : MonoBehaviour
    {
        [SerializeField] private new Light2D light;

        [Header("Flicker")]

        public bool flickers;
        public float minIntensity = 0f;
        public float maxIntensity = 1f;
        [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
        [Range(1, 50)] public int smoothing = 5;

        // Continuous average calculation via FIFO queue
        // Saves us iterating every time we update, we just change by the delta
        Queue<float> smoothQueue;
        float lastSum = 0;

        void Awake()
        {
            if (light == null) light = GetComponent<Light2D>();
            smoothQueue = new Queue<float>(smoothing);
        }

        void Update()
        {
            if (light == null) return;
            if (flickers) Flicker();
        }

        public void Flicker()
        {
            // pop off an item if too big
            while (smoothQueue.Count >= smoothing)
                lastSum -= smoothQueue.Dequeue();

            // Generate random new item, calculate new average
            float newVal = Random.Range(minIntensity, maxIntensity);
            smoothQueue.Enqueue(newVal);
            lastSum += newVal;

            // Calculate new smoothed average
            light.intensity = lastSum / (float)smoothQueue.Count;
        }

        /// <summary>
        /// Reset the randomness and start again. You usually don't need to call
        /// this, deactivating/reactivating is usually fine but if you want a strict
        /// restart you can do.
        /// </summary>
        public void Reset()
        {
            light = GetComponent<Light2D>();
            if (!smoothQueue.IsNullOrEmpty()) smoothQueue.Clear();
            lastSum = 0;
        }
    }
}
