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

            if (data.coldPFX != null)
            {
                coldPFX = GameObject.Instantiate(data.coldPFX.gameObject, target.entity.Sprite.transform).GetComponent<ParticleSystemController>();
                if (target.entity.Sprite != null)
                    coldPFX.SetShape(target.entity.Sprite);
                coldPFX.Play();
            }

            if (target.entity.StateManager)
            {
                target.entity.StateManager.accMod.AddMod(data.accMod);
            }

            if (target.phys)
            {
                target.phys.friction.AddMod(data.frictionMod);
            }

            return true;
        }

        public override void Expire()
        {
            Debug.Log(target.gameObject.name + " has <color=cyan>thawed</color>");

            if (target.entity.StateManager)
            {
                target.entity.StateManager.accMod.RemoveMod(data.accMod);
            }

            if (target.phys)
            {
                target.phys.friction.RemoveMod(data.frictionMod);
            }

            if (coldPFX != null)
            {
                coldPFX.DetachParticles();
                coldPFX.gameObject.SetActive(false);
            }

            target.entity.StatusMngr.ApplyStatus(StatusType.Wet);
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
