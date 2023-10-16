using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Logic
{
    public class Device : LogicBehavior
    {
        [SerializeField] protected LogicBehavior input;
        [SerializeField] protected LogicBehavior inputLock;
        [SerializeField] protected LogicBehavior inputInverter;
        [SerializeField] protected bool _invertInput = false;

        protected override void OnEnable()
        {
            if (input)
            {
                _on = input.On ^ _invertInput;
                input.onSwitch += (on) => { SetOnValue(on ^ _invertInput); };
            }

            if (inputInverter)
            {
                _invertInput = inputInverter.On;
                inputInverter.onSwitch += (on) => { SetOnValue(on); };
            }
            
            base.OnEnable();
        }

        protected virtual void OnDisable()
        {
            if (input) input.onSwitch -= (on) => { SetOnValue(on ^ _invertInput); };

            if (inputInverter) inputInverter.onSwitch -= (on) => { SetOnValue(on); };
        }

        protected virtual void SetInvert(bool on = true)
        {
            _invertInput = on;
        }
    }
}
