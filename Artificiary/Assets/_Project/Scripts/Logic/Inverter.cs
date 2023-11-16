using LDtkUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Logic
{
    public class Inverter : LogicBehavior
    {
        [SerializeField] protected LogicBehavior _input;
        [SerializeField] protected LogicBehavior _inverter;

        protected override void OnEnable()
        {
            if (_input && _inverter)
            {
                _on = _input.On ^ _inverter.On;
                _input.onSwitch += (on) => { SetOnValue(on ^ _inverter.On); };
                _inverter.onSwitch += (on) => { SetOnValue(_on ^ on); };
            }

            base.OnEnable();
        }

        protected virtual void OnDisable()
        {
            if (_input && _inverter)
            {
                _input.onSwitch -= (on) => { SetOnValue(on ^ _inverter.On); };
                _inverter.onSwitch -= (on) => { SetOnValue(_on ^ on); };
            }
        }

        public override void OnLDtkImportFields(LDtkFields fields)
        {
            base.OnLDtkImportFields(fields);

            LDtkReferenceToAnEntityInstance inputEntity;
            LDtkReferenceToAnEntityInstance inverterEntity;

            if (fields.TryGetEntityReference("input", out inputEntity) && inputEntity != null)
            {
                _input = inputEntity.FindEntity()?.gameObject?.GetComponent<LogicBehavior>();
            }

            if (fields.TryGetEntityReference("inverter", out inverterEntity) && inverterEntity != null)
            {
                _inverter = inverterEntity.FindEntity()?.gameObject?.GetComponent<LogicBehavior>();
            }
        }
    }
}
