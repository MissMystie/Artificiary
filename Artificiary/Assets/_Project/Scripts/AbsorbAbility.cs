using Mystie.Core;
using Mystie.Gameplay;
using Mystie.Logic;
using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mystie.ChemEngine
{
    public class AbsorbAbility : MonoBehaviour
    {
        #region Events

        public Action<Element> onElementSet;

        #endregion

        protected Entity _entity;
        protected InputController _controller;
        protected SkillManager _skillManager;
        protected Animator _anim;

        [SerializeField] protected LayerMask _absorbMask = -1;

        protected List<Collider2D> _interactibles;

        protected bool _isTransmuting;
        protected Timer _absorbTimer;

        public int maxCharges = 1;

        public ElementGauge gauge = new ElementGauge();
        public ElementGauge reserve = new ElementGauge();

        public List<ElementalAbility> abilities;
        public Dictionary<Element, Gear> abilitiesDict;

        [Header("Absorb")]

        public float absorbRadius = 4f;
        public float absorbTime = 0.75f;
        public bool secondGauge;

        [Header("Cooldown")]

        public float cooldownTime = 0.1f;
        protected Timer _cooldownTimer;

        protected void Awake()
        {
            _entity = Entity.Get(gameObject);
            _controller = _entity.Controller;
            _skillManager = _entity.SkillManager;
            _anim = _entity.Anim;

            _absorbTimer = new Timer();
            _interactibles = new List<Collider2D>();

            abilitiesDict = new Dictionary<Element, Gear>();
            foreach(ElementalAbility ability in abilities)
            {
                abilitiesDict.Add(ability.element, ability.ability);
            }
        }

        protected void OnEnable()
        {
            _controller.interact.performed += OnTransmute;
            _controller.interact.released += OnTransmuteRelease;
            _absorbTimer.onTimerEnd += Absorb;
        }

        protected void OnDisable()
        {
            _controller.interact.performed -= OnTransmute;
            _controller.interact.released -= OnTransmuteRelease;
            _absorbTimer.onTimerEnd -= Absorb;
        }

        private void Update()
        {
            _absorbTimer.Tick(Time.deltaTime);
        }

        #region Transmute

        private void OnTransmute()
        {
            _isTransmuting = true;
            _absorbTimer.SetTime(absorbTime);
        }

        private void OnTransmuteRelease()
        {
            if (!_isTransmuting) return;

            _isTransmuting = false;
            _absorbTimer.SetTime(0f);

            Switch();
        }

        private void Absorb()
        {
            _isTransmuting = false;
            _absorbTimer.SetTime(0f);

            Debug.Log("On absorb");

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, absorbRadius);

            foreach (Collider2D col in cols)
            {
                IAbsorbable absorbable = col.GetComponent<IAbsorbable>();
                if (absorbable != null)
                {
                    Debug.Log("Gain charges");
                    GainCharges(absorbable.Consume());
                    break;
                }
            }
        }

        public void Switch()
        {
            if (secondGauge)
                SetElement(reserve.element);
        }

        #endregion

        #region Charges

        public bool UseCharges(int amount = 1)
        { 
            if (amount <= 0) return true;
            else if (amount <= gauge.charges)
            {
                gauge.charges = Math.Clamp(gauge.charges - amount, 0, maxCharges);
                _skillManager.Equip(null);
                Debug.Log("Charges used: " + amount + ", " + gauge.element);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GainCharges(ElementalCharge charge)
        {
            if (charge.value <= 0) return false;

            // if switching to a different element of 
            bool isNewElement = gauge.element != charge.element || gauge.charges <= 0;
            SetElement(charge.element);

            if (charge.value > 0 && gauge.charges < maxCharges)
            {
                gauge.charges = Math.Clamp(gauge.charges + charge.value, 0, maxCharges);
                if (isNewElement) _skillManager.Equip(abilitiesDict[gauge.element]);
                return true;
            }
            else return false;
        }

        public void Fill()
        {
            if (gauge.charges <= 0) _skillManager.Equip(abilitiesDict[gauge.element]);
            gauge.charges = maxCharges;
        }

        #endregion

        public void SetElement(Element element)
        {
            if (gauge.element == element)
                return;

            //If the second gauge is empty or contains the element, swap
            if (secondGauge && (reserve.element == element || reserve.charges == 0))
            {
                gauge.Swap(reserve);
            }
            else
            {
                gauge.charges = 0;
            }
                
            gauge.element = element;

            onElementSet?.Invoke(element);
        }

        [Serializable]
        public class ElementGauge
        {
            public Element element;
            public int charges;

            public ElementGauge(Element element = Element.Fire, int charges = 0)
            {
                this.element = element;
                this.charges = charges;
            }

            public ElementGauge(ElementGauge gauge)
            {
                element = gauge.element;
                charges = gauge.charges;
            }

            public ElementGauge Swap(ElementGauge gauge)
            {
                Element oldElement = element;
                int oldCharges = charges;

                element = gauge.element;
                charges = gauge.charges;

                gauge.element = oldElement;
                gauge.charges = oldCharges;

                return this;
            }
        }

        [Serializable]
        public class ElementalAbility
        {
            public Element element;
            public Gear ability;
        }
    }
}
