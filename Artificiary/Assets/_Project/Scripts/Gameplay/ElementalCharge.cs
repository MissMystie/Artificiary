using Mystie.ChemEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    [Serializable]
    public class ElementalCharge
    {
        public Element element;
        public int value;

        public ElementalCharge(Element element = Element.Fire, int value = 1)
        {
            this.element = element;
            this.value = value;
        }
    }
}
