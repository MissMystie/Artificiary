using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    [System.Serializable]
    public class Vulnerabilities
    {
        public List<Vulnerability> vulnerabilities = new List<Vulnerability>();
        public bool resistAll = false;

        private Dictionary<DamageType, Vulnerability> vulnerabilitiesDict;
        private Dictionary<DamageType, Vulnerability> VulnerabilitiesDict
        {
            get
            {
                if (vulnerabilitiesDict.IsNullOrEmpty())
                {
                    vulnerabilitiesDict = new Dictionary<DamageType, Vulnerability>();
                    foreach (Vulnerability res in vulnerabilities)
                    {
                        if (!vulnerabilitiesDict.ContainsKey(res.type))
                            vulnerabilitiesDict.Add(res.type, res);
                    }
                }

                return vulnerabilitiesDict;
            }
        }

        public int ApplyDamageRate(Damage dmg)
        {
            int dmgValue = (int)Math.Floor(dmg.value * GetDamageRate(dmg.type));
            return dmgValue;
        }

        public float GetDamageRate(DamageType type)
        {
            if (VulnerabilitiesDict.ContainsKey(type)) 
                return VulnerabilitiesDict[type].percent;
            else return resistAll ? 0f : 1f;
        }

        [System.Serializable]
        public class Vulnerability
        {
            public DamageType type;
            public float percent;
            public bool resistKnockback;

            public Vulnerability(DamageType type, float percent = 1f)
            {
                this.type = DamageType.TRUE;
                this.percent = percent;
            }
        }
    }
}
