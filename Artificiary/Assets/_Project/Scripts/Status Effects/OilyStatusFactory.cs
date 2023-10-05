using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Oily Status", menuName = "CustomData/Status Effects/Oily Status", order = 2)]
    public class OilyStatusFactory : StatusEffectFactory<OilyStatusData, OilyStatus> { }

    [System.Serializable]
    public class OilyStatusData
    {
        public float duration = 12f;

        public Stat.Mod accMod = new Stat.Mod(0.15f);
        public Stat.Mod frictionMod = new Stat.Mod(0.1f);

        [Header("VFX")]

        public ParticleSystemController oilPFX;
    }

    public class OilyStatus : StatusEffect<OilyStatusData>
    {
        public ParticleSystemController oilPFX;

        public override bool Apply()
        {
            duration = data.duration;
            Debug.Log(target.gameObject.name + " is <color=yellow>oily</color>");

            try
            {
                oilPFX = GameObject.Instantiate(data.oilPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();
                oilPFX.SetShape(target.entity.Sprite);
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("ChemEngine: Wet status pfx unassigned.");
            }

            if (oilPFX != null) oilPFX.Play();

            target.entity.Phys.acc.AddMod(data.accMod);
            target.entity.Phys.friction.AddMod(data.frictionMod);

            return true;
        }

        public override void Expire()
        {
            Debug.Log(target.gameObject.name + " is no longer <color=yellow>oily</color>");

            target.entity.Phys.acc.RemoveMod(data.accMod);
            target.entity.Phys.friction.RemoveMod(data.frictionMod);

            if (oilPFX != null)
            {
                oilPFX.DetachParticles();
                oilPFX.gameObject.SetActive(false);
            }
        }

        public override bool React()
        {
            bool blocked = false;

            if (target.HasStatus(StatusType.Burn))
            {
                Debug.Log("Blast!");
            }

            if (target.HasStatus(StatusType.Shock))
            {
                Debug.Log("Blast!");
                blocked = true;
            }

            if (target.HasStatus(StatusType.Wet))
            {
                Debug.Log("Wash!");
                blocked = true;
            }

            return !blocked;
        }

        public override StatusType GetStatusType()
        {
            return StatusType.Oily;
        }
    }
}
