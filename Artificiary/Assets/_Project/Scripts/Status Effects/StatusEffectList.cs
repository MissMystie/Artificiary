using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Status Effects List", menuName = "CustomData/Status Effects/Status Effects List", order = 0)]
    public class StatusEffectList : ScriptableObject
    {
        public List<StatusEffectPair> statusEffects = new List<StatusEffectPair>();
      
        public Dictionary<StatusType, StatusEffectFactory> Get()
        {
            Dictionary<StatusType, StatusEffectFactory> dict = new Dictionary<StatusType, StatusEffectFactory>();
            foreach (var statusEffect in statusEffects)
                dict.Add(statusEffect.type, statusEffect.factory);

            return dict;
        }
    }

    [Serializable]
    public class StatusEffectPair
    {
        public string name;
        public StatusType type;
        public StatusEffectFactory factory;
    }
}
