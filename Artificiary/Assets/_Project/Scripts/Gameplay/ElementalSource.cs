using LDtkUnity;
using Mystie.ChemEngine;
using Mystie.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Logic
{
    public class ElementalSource : LogicBehavior, IDamageable, IAbsorbable, ILDtkImportedFields
    {
        public DamageType damageType;
        public ElementalCharge charge = new ElementalCharge();

        [Space]

        public int value = 0;
        public int maxValue = 1;
        public bool unlimitedCharges;

        protected override void Awake()
        {
            base.Awake();
            _on = value > 0 || unlimitedCharges;
        }

        public void Feed(int amount = 1)
        {
            if (_locked || value >= maxValue || amount <= 0) return;

            if (value <= 0 && !_on)
            {
                SetOn();
            }

            value = Math.Min(value + amount, maxValue);
        }

        public ElementalCharge Consume(int amount = 1)
        {
            if (!unlimitedCharges)
            {
                value = Math.Max(value - amount, 0);
                if (value <= 0) SetOff();
            }

            return charge;
        }

        public void TakeDamage(Damage dmg)
        {
            if (damageType == dmg.type)
            {
                Feed();
            }
        }

        public override void OnLDtkImportFields(LDtkFields fields)
        {
            base.OnLDtkImportFields(fields);

            fields.TryGetInt("charges", out value);
            fields.TryGetBool("unlimited", out unlimitedCharges);
        }
    }

    
}
