using MoreMountains.Feedbacks;
using Mystie.Core;
using Mystie.Gameplay;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mystie.Core
{
    [RequireComponent(typeof(HurtBox))]
    public class HealthManager : MonoBehaviour, IDamageable
    {
        public event Action<HealthManager> onHealthChanged;
        public event Action<HealthManager, int> onDamaged;  // dmgValue
        public event Action<HealthManager, int> onHealed;   // healValue
        public event Action<HealthManager> onDeath;

        [SerializeField] private Entity entity;
        [SerializeField] private StateManager stateManager;

        public int currentHealth { get; private set; }
        public int currentShield { get; private set; }

        [SerializeField] private int maxHealth = 1;
        [SerializeField] private float iFramesDuration = (1f/8);
        [SerializeField] public float stunDuration = (1f/8);

        private Timer iFramesTimer;

        public int MaxHealth { get { return maxHealth; } }
        public bool IFramesOn { get { return iFramesTimer.IsRunning(); } }
        public bool HasShield { get { return currentShield > 0; } }
        public bool IsDead { get { return currentHealth <= 0; } }

        [Header("Death")]

        public bool disableOnDeath = true;
        public float deathDelay = 0f;
        [SerializeField] private string deathAnimParam = "death";
        [SerializeField] private string stunAnimParam = "stunned";
        public GameObject deathFXPrefab;
        public UnityEvent onDeathEvent = new UnityEvent();

        [Header("Feedback")]

        [SerializeField] private MMFeedbacks hurtFX;
        [SerializeField] private MMFeedbacks deathFX;
        [SerializeField] private MMBlink iFramesFX;
        [SerializeField] private MMFeedbacks stunFX;

        [Space]

        [SerializeField] private MMFeedbacks shieldFX;
        [SerializeField] private MMFeedbacks shieldHurtFX;
        [SerializeField] private MMBlink iFramesShieldFX;
        [SerializeField] private MMFeedbacks shieldGainFX;
        [SerializeField] private MMFeedbacks shieldExpireFX;

        private void Start()
        {
            if (entity == null) entity = Entity.Get(gameObject);
            if (stateManager == null) stateManager = entity.StateManager;

            currentHealth = maxHealth;
            onHealthChanged?.Invoke(this);

            iFramesTimer = new Timer();
            iFramesTimer.onTimerEnd += OnIFramesEnd;
        }

        private void Update()
        {
            iFramesTimer.Tick(Time.deltaTime);
        }

        public virtual void TakeDamage(Damage dmg)
        {
            if (IsDead || IFramesOn || dmg.value == 0) return; // no damage
            else if (dmg.value < 0) // heal if negative damage
            {
                Heal(dmg.value);
                return;
            }

            if (HasShield)
            {
                TakeShieldDamage(dmg);
                return;
            }

            // TODO: add stun state
            stateManager?.SetState(new StunState(stateManager, stunDuration, stunAnimParam, stunFX)); ;

            int dmgValue = dmg.value;

            if (dmgValue > 0)
            {
                currentHealth -= dmgValue;
                onDamaged?.Invoke(this, dmgValue);

                if (IsDead)
                {
                    StartCoroutine(DeathCoroutine());
                    return;
                }

                if (iFramesDuration > 0) StartIFrames();

                onHealthChanged?.Invoke(this);

                hurtFX?.PlayFeedbacks();
            }
        }

        [Button()]
        public virtual void Heal(int healValue = 1)
        {
            if (currentHealth >= MaxHealth || healValue <= 0) return; // no healing

            currentHealth += healValue;
            currentHealth = Math.Clamp(currentHealth, 0, MaxHealth);

            onHealed?.Invoke(this, healValue);
            onHealthChanged?.Invoke(this);
        }

        [Button()]
        public void FullHeal()
        {
            currentHealth = maxHealth;
            onHealthChanged?.Invoke(this);
        }

        #region Death

        public void Die()
        {
            StartCoroutine(DeathCoroutine());
        }

        public IEnumerator DeathCoroutine()
        {
            DeactivateShield();
            if (IFramesOn) OnIFramesEnd();

            currentHealth = 0;
            onHealthChanged?.Invoke(this);

            // TODO: add death state
            stateManager?.SetState(new GroundState(stateManager));

            entity.Anim?.SetTrigger(deathAnimParam);

            deathFX?.PlayFeedbacks();
            if (deathFXPrefab != null)
                Instantiate(deathFXPrefab, transform.position, Quaternion.identity);

            onDeathEvent?.Invoke();
            onDeath?.Invoke(this);

            yield return new WaitForSeconds(deathDelay);

            if (disableOnDeath) gameObject.SetActive(false);
        }

        private void OnRespawn()
        {
            FullHeal();
        }

        #endregion

        #region IFrames

        [Button()]
        private void StartIFrames()
        {
            if (iFramesDuration == 0) return;

            iFramesTimer.SetTime(iFramesDuration);
            if (HasShield) iFramesShieldFX?.StartBlinking();
            else iFramesFX?.StartBlinking();
        }

        private void OnIFramesEnd()
        {
            iFramesTimer.SetTime(0f);
            iFramesFX?.StopBlinking();
            iFramesShieldFX?.StopBlinking();
        }

        #endregion

        #region Shield

        [Button()]
        public void GainShield(int shieldValue = 1)
        {
            if (shieldValue <= 0) return;

            if (!HasShield) shieldFX?.PlayFeedbacks();
            shieldGainFX?.PlayFeedbacks();

            currentShield += shieldValue;
        }

        public void TakeShieldDamage(Damage dmg)
        {
            if (dmg.value <= 0) return;

            currentShield--;

            StartIFrames();

            if (currentShield <= 0) DeactivateShield();
            else shieldHurtFX?.PlayFeedbacks();

            return;
        }

        [Button()]
        public void DeactivateShield()
        {
            currentShield = 0;
            shieldFX?.StopFeedbacks();
            shieldExpireFX?.PlayFeedbacks();
        }

        #endregion

        private void OnValidate()
        {
            maxHealth = Math.Max(maxHealth, 0);
            iFramesDuration = Math.Max(iFramesDuration, 0);
        }
    }
}
