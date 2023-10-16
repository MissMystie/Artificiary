using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.Logic
{
    public class LogicGate : LogicBehavior
    {
        public enum OP { AND, OR, NAND, NOR, XOR, XNOR };
        public OP logicOp = OP.AND;

        [SerializeField] protected List<LogicBehavior> inputs = new List<LogicBehavior>();

        /*
        protected override void OnEnable()
        {
            if (input)
            {
                _on = input.IsOn() ^ _invertInput;
                input.onSwitch += (on) => { SetOnValue(on ^ _invertInput); };
            }
            
            base.OnEnable();
        }

        protected virtual void OnDisable()
        {
            if (input) input.onSwitch -= (on) => { SetOnValue(on ^ _invertInput); };
        }*/

        protected override void Awake()
        {
            _on = Logic.Check(GetInputs(inputs), logicOp);
            base.Awake();
        }

        protected virtual void Update()
        {
            if (_on != Logic.Check(GetInputs(inputs), logicOp)) Toggle();
        }

        public static List<bool> GetInputs(List<LogicBehavior> inputs)
        {

            if (inputs.IsNullOrEmpty()) return null;

            List<bool> values = new List<bool>();

            foreach (LogicBehavior input in inputs)
                values.Add(input.On);

            return values;
        }



#if UNITY_EDITOR

        protected virtual void OnDrawGizmos()
        {
            if (!showDebug) return;

            //base.OnDrawGizmos();

            Handles.Label(transform.position + new Vector3(0, 0.2f), logicOp.ToString());

            if (!inputs.IsNullOrEmpty())
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    if (inputs[i] != null)
                    {
                        if (inputs[i].On) Gizmos.color = Color.green;
                        else Gizmos.color = Color.red;

                        Gizmos.DrawLine(transform.position, inputs[i].transform.position);
                    }
                }
            }
        }

#endif

        public class Logic
        {
            public static bool Check(List<bool> values, OP logicOp)
            {
                if (values.IsNullOrEmpty()) return false;

                switch (logicOp)
                {
                    case OP.AND:
                        return AND(values);
                    case OP.OR:
                        return OR(values);
                    case OP.NAND:
                        return NAND(values);
                    case OP.NOR:
                        return NOR(values);
                    case OP.XOR:
                        return XOR(values);
                    case OP.XNOR:
                        return XNOR(values);
                    default:
                        return false;
                }
            }

            private static bool AND(List<bool> values)
            {
                foreach (bool value in values)
                {
                    if (value == false)
                        return false;
                }

                return true; //If no input returned false, return true
            }

            private static bool OR(List<bool> values)
            {
                foreach (bool value in values)
                {
                    if (value == true)
                        return true;
                }

                return false; //If no input returned true, return false
            }

            private static bool NAND(List<bool> values)
            {
                foreach (bool value in values)
                {
                    if (value == false)
                        return true;
                }

                return false; //If no input returned false, return false
            }

            private static bool NOR(List<bool> values)
            {
                foreach (bool value in values)
                {
                    if (value == true)
                        return false;
                }

                return true; //If no input returned true, return true
            }

            private static bool XOR(List<bool> values)
            {
                int trueChecks = 0;

                foreach (bool value in values)
                {
                    if (value == true)
                    {
                        trueChecks++;
                        if (trueChecks > 1)
                            return false;
                    }
                }

                if (trueChecks == 1)
                    return true; //If only one input returned true, return true
                else
                    return false;
            }

            private static bool XNOR(List<bool> values)
            {
                int trueChecks = 0;

                foreach (bool value in values)
                {
                    if (value == true)
                        trueChecks++;
                }

                if (trueChecks == 1)
                    return false; //If only one input returned true, return true
                else
                    return true;
            }
        }

    }
}
