using Mystie.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    public interface IAbsorbable
    {
        ElementalCharge Consume(int amount = 1);
    }
}
