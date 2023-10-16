using Mystie.Core;
using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.Logic
{
    public class HitTrigger : LogicBehavior, IDamageable
    {
        public List<DamageType> vulnerabilities = new List<DamageType>();

        public void TakeDamage(Damage dmg)
        {
            if (vulnerabilities.IsNullOrEmpty() || vulnerabilities.Contains(dmg.type))
            {
                Toggle();
            }
            else
            {
                Debug.Log("Damage type " + dmg.type + " is unable to trigger this switch.");
            }
        }
    }
}
