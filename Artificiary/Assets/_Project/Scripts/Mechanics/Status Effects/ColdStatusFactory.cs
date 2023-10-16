using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Cold Status", menuName = "CustomData/Status Effects/Cold Status", order = 4)]
    public class ColdStatusFactory : StatusEffectFactory<ColdStatusData, ColdStatus> { }

    [System.Serializable]
    public class ColdStatusData
    {
        public float duration = 12f;

        public Stat.Mod accMod = new Stat.Mod(0.15f);
        public Stat.Mod frictionMod = new Stat.Mod(0.1f);

        [Header("VFX")]

        public ParticleSystemController coldPFX;
    }

    public class ColdStatus : StatusEffect<ColdStatusData>
    {
        public ParticleSystemController coldPFX;

        public override bool Apply()
        {
            if (!React()) return false;

            duration = data.duration;
            Debug.Log(target.gameObject.name + " is <color=cyan>cold</color>");

            /*
             if (target.status.HasStatus(StatusType.Burn))
            { 
                obj.SetStatus(StatusType.Wet);
                //ChemEngine.SetStatus(obj, Type.Damp);
                return false;
            }
            else
                obj.RemoveStatus(StatusType.Wet);
            //ChemEngine.RemoveStatus(obj, Type.Damp);
             
             */

            try
            {
                //TODO use unity object pooling system
                coldPFX = GameObject.Instantiate(data.coldPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();
                coldPFX.SetShape(target.entity.Sprite);
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("ChemEngine: Wet status pfx unassigned.");
            }

            if (coldPFX != null) coldPFX.Play();

            target.entity.Actor.acc.AddMod(data.accMod);
            target.entity.Actor.friction.AddMod(data.frictionMod);

            return true;
        }

        public override void Expire()
        {
            Debug.Log(target.gameObject.name + " has <color=cyan>thawed</color>");

            target.entity.Actor.acc.RemoveMod(data.accMod);
            target.entity.Actor.friction.RemoveMod(data.frictionMod);

            if (coldPFX != null)
            {
                coldPFX.DetachParticles();
                coldPFX.gameObject.SetActive(false);
            }

            target.entity.Status.ApplyStatus(StatusType.Wet);
        }

        public override bool React()
        {
            bool blocked = false;

            if (target.HasStatus(StatusType.Burn))
            {
                Debug.Log("Melt!");
                blocked = true;
            }

            if (target.HasStatus(StatusType.Wet))
            {
                Debug.Log("Freeze!");
                target.ApplyStatus(StatusType.Frozen);
                blocked = true;
            }

            return !blocked;
        }

        public override StatusType GetStatusType()
        {
            return StatusType.Cold;
        }
    }
}
