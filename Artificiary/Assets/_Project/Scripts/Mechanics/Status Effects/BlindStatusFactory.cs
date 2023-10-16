using Mystie.Core;
using Mystie.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Blind Status", menuName = "CustomData/Status Effects/Blind Status")]
    public class BlindStatusFactory : StatusEffectFactory<BlindStatusData, BlindStatus> { }

    [System.Serializable]
    public class BlindStatusData
    {
        public float duration = 12f;
    }

    public class BlindStatus : StatusEffect<BlindStatusData>
    {
        public Sight sight;

        public override bool Apply()
        {
            duration = data.duration;
            sight = target.gameObject.GetComponent<Sight>();
            if(sight != null) sight.enabled = false;
            if (target.entity.Anim != null) target.entity.Anim.SetBool("blind", true);

            Debug.Log(target.gameObject.name + " is <color=cyan>blinded</color>");

            return true;
        }

        public override void Reapply()
        {
            duration = data.duration;
        }

        public override void Expire()
        {
            if (sight != null) sight.enabled = true;
            if (target.entity.Anim != null) target.entity.Anim.SetBool("blind", false);
            Debug.Log(target.gameObject.name + " is no longer <color=cyan>blinded</color>");
        }

        public override StatusType GetStatusType()
        {
            return StatusType.Blind;
        }
    }
}
