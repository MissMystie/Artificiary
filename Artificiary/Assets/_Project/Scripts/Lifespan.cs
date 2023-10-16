using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mystie.Core
{
    public class Lifespan : MonoBehaviour
    {
        private Timer timer;

        public float lifetime = 2;
        public bool on = true;

        [Foldout("Feedback")][SerializeField] private GameObject expireFX;

        [Header("Events")]

        public UnityEvent onExpire = new UnityEvent();

        public void Awake()
        {
            timer = new Timer();
            
        }

        public void OnEnable()
        {
            timer.onTimerEnd += OnLifespanEnd;
            if (on) StartLifetime();
        }

        public void OnDisable()
        {
            StopLifetime();
            timer.onTimerEnd -= OnLifespanEnd;
        }

        public virtual void SetOn()
        {
            StartLifetime();
        }

        public virtual void SetOff()
        {
            StopLifetime();
        }

        public void StartLifetime()
        {
            StartLifetime(lifetime);
        }

        public void StartLifetime(float lifetime)
        {
            on = true;
            timer.SetTime(lifetime);
        }

        public void StopLifetime()
        {
            timer.SetTime(0f);
        }

        public void Update()
        {
            if (on) timer.Tick(Time.deltaTime);
        }

        public void OnLifespanEnd()
        {
            if (expireFX != null) Instantiate(expireFX, transform.position, Quaternion.identity);

            foreach (ParticleSystemController particleSystem in GetComponentsInChildren<ParticleSystemController>()) {
                particleSystem.DetachParticles();
            }

            gameObject.SetActive(false);

            onExpire.Invoke();
        }
    }

    
}
