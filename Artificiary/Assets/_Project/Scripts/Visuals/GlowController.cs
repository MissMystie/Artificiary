using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Visuals
{
    public class GlowController : MonoBehaviour
    {
        public new Renderer renderer;
        public string colorParam = "_EmissionColor";

        [Space]

        [SerializeField] private float minIntensity = 1f;
        [SerializeField] private float maxIntensity = 4f;

        [Header("Flash")]

        [SerializeField] private float flashDuration = 1f;
        [SerializeField] private float flashIntensity = 8f;

        private bool flashing;

        [Space]

        [SerializeField] private float cycleDuration = 1f;
        [SerializeField] private bool percentUp = true;
        [Range(0, 2)] public float easeAmount;

        private Material emissiveMaterial;
        private Color color;
        private float intensity;
        private float timer = 0;

        private const byte k_MaxByteForOverexposedColor = 191; //internal Unity const

        private void Reset()
        {
            if (renderer == null) renderer = GetComponent<Renderer>();
        }

        private void Start()
        {
            if (renderer == null) renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                emissiveMaterial = renderer.material;
                color = emissiveMaterial.GetColor(colorParam);

                float intensity = GetIntensity(color);
                color /= intensity;

                //if (intensity != 0) 
            }

            intensity = percentUp ? minIntensity : maxIntensity;
            timer = 0;
        }

        private void OnValidate()
        {
            intensity = Mathf.Clamp(intensity, minIntensity, maxIntensity);
        }

        private void Update()
        {
            if (flashing) UpdateFlash(Time.deltaTime);
            else UpdateGlow(Time.deltaTime);
        }

        private void UpdateGlow(float deltaTime)
        {
            float percent = (percentUp ? timer : (cycleDuration / 2) - timer) / (cycleDuration / 2);
            float easedPercent = percent.Ease(easeAmount + 1);

            intensity = Mathf.Lerp(minIntensity, maxIntensity, easedPercent);
            SetIntensity(intensity);

            timer += deltaTime;
            if (timer >= (cycleDuration / 2))
            {
                timer = 0;
                percentUp = !percentUp;
            }
        }

        private void UpdateFlash(float deltaTime)
        {
            float percent = timer / flashDuration;
            float easedPercent = percent.Ease(easeAmount + 1);

            intensity = Mathf.Lerp(minIntensity, flashIntensity, easedPercent);
            SetIntensity(intensity);

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                percentUp = true;
                flashing = false;
            }
        }

        public void Flash()
        {
            timer = flashDuration;
            SetIntensity(flashIntensity);
            flashing = true;
        }

        public float GetIntensity(Color color)
        {
            if (color == null) return 0f;

            float maxColorComponent = color.maxColorComponent;
            float scaleFactor = k_MaxByteForOverexposedColor / maxColorComponent;
            float intensity = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);
            return intensity;
        }

        public void SetIntensity(float intensity)
        {
            if (emissiveMaterial != null)
                emissiveMaterial.SetColor(colorParam, color * intensity);
        }
    }
}
