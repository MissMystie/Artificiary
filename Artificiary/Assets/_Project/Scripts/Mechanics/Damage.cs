using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    [System.Serializable]
    public class Damage
    {
        public int value = 1;
        public DamageType type = DamageType.TRUE;

        public Damage(int value = 1, DamageType type = DamageType.TRUE)
        {
            this.value = value;
            this.type = type;
        }

        public Damage(Damage dmg)
        {
            value = dmg.value;
            type = dmg.type;
        }
    }


    public enum DamageType
    {
        TRUE = 0,
        SLASH = 1,
        BLUNT = 2,
        PIERCE = 3,
        BLAST = 4,

        HEAT = 10,
        COLD = 11,
        SHOCK = 12,
        WATER = 13,
        ENERGY = 14,
        POISON = 15,
        CORROSION = 16,
        
        RADIATION = 20,
        MALICE = 21
    }
}
