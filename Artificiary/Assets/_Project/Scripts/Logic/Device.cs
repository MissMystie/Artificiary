using LDtkUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Logic
{
    public class Device : LogicBehavior, ILDtkImportedFields
    {
        [SerializeField] protected LogicBehavior _input;
        [SerializeField] protected LogicBehavior _inputLock;
        [SerializeField] protected LogicBehavior _inputInverter;
        [SerializeField] protected bool _invertInput = false;

        protected override void OnEnable()
        {
            if (_input)
            {
                _on = _input.On ^ _invertInput;
                _input.onSwitch += (on) => { SetOnValue(on ^ _invertInput); };
            }

            if (_inputInverter)
            {
                _invertInput = _inputInverter.On;
                _inputInverter.onSwitch += (on) => { SetOnValue(on); };
            }
            
            base.OnEnable();
        }

        protected virtual void OnDisable()
        {
            if (_input) _input.onSwitch -= (on) => { SetOnValue(on ^ _invertInput); };

            if (_inputInverter) _inputInverter.onSwitch -= (on) => { SetOnValue(on); };
        }

        protected virtual void SetInvert(bool on = true)
        {
            _invertInput = on;
        }

        public override void OnLDtkImportFields(LDtkFields fields)
        {
            base.OnLDtkImportFields(fields);

            LDtkReferenceToAnEntityInstance inputEntity;
            if (fields.TryGetEntityReference("input", out inputEntity) && inputEntity != null)
            {
                _input = inputEntity.FindEntity()?.gameObject?.GetComponent<LogicBehavior>();
            }
 
            fields.TryGetBool("invert_input", out _invertInput);
        }
    }
}
