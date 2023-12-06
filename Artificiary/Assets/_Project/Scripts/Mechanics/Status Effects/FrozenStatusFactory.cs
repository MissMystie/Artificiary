using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Frozen Status", menuName = "CustomData/Status Effects/Frozen Status", order = 5)]
    public class FrozenStatusFactory : StatusEffectFactory<FrozenStatusData, FrozenStatus> { }

    [System.Serializable]
    public class FrozenStatusData
    {
        public float duration = 12f;

        public Stat.Mod accMod = new Stat.Mod(0.15f);
        public Stat.Mod frictionMod = new Stat.Mod(0.1f);

        [Header("VFX")]

        public ParticleSystemController frozenPFX;
        public Material frozenMat;

        [Header("Sound effects")]

        public FMODUnity.EventReference freezeSFX;
        public FMODUnity.EventReference unfreezeSFX;
        public FMODUnity.EventReference thawSFX;
    }

    public class FrozenStatus : StatusEffect<FrozenStatusData>
    {
        public ParticleSystemController frozenPFX;

        public override bool Apply()
        {
            if (!React()) return false;

            duration = data.duration;
            Debug.Log(target.gameObject.name + " is <color=cyan>frozen</color>");

            if (data.frozenPFX != null)
            {
                frozenPFX = GameObject.Instantiate(data.frozenPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();
                if (target.entity.Sprite != null) 
                    frozenPFX.SetShape(target.entity.Sprite);
            }

            if (frozenPFX != null) frozenPFX.Play();

            if (target.phys)
            {
                target.phys.friction.AddMod(data.frictionMod);
            }

            if (target.entity.StateManager)
            {
                target.entity.StateManager.accMod.AddMod(data.accMod);
            }
                
            if (data.frozenMat && target.entity.SpriteManager)
                target.entity.SpriteManager.SetMaterial(data.frozenMat);

            if (!data.freezeSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.freezeSFX.Path, target.transform.position);

            return true;
        }

        public override void Expire()
        {
            Debug.Log(target.gameObject.name + " is no longer <color=cyan>frozen</color>");

            if (target.phys)
            {
                target.phys.friction.RemoveMod(data.frictionMod);
            }

            if (target.entity.StateManager)
            {
                target.entity.StateManager.accMod.RemoveMod(data.accMod);
            }

            if (frozenPFX != null)
            {
                frozenPFX.DetachParticles();
                frozenPFX.gameObject.SetActive(false);
            }

            if (data.frozenMat && target.entity.SpriteManager)
                target.entity.SpriteManager.ResetMaterial();

            if (!data.unfreezeSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.unfreezeSFX.Path, target.transform.position);

            target.entity.StatusMngr.ApplyStatus(StatusType.Wet);
        }

        public override bool React()
        {
            bool blocked = false;

            if (target.HasStatus(StatusType.Burn))
            {
                Debug.Log("Melt!");
                target.entity.StatusMngr.ApplyStatus(StatusType.Wet);

                if (!data.thawSFX.IsNull)
                    FMODUnity.RuntimeManager.PlayOneShot(data.thawSFX.Path, target.transform.position);
                
                blocked = true;
            }

            if (target.HasStatus(StatusType.Cold))
            {
                blocked = true;
            }

            if (target.HasStatus(StatusType.Frozen))
            {
            }

            if (target.HasStatus(StatusType.Wet))
            {
                blocked = true;
            }

            if (target.HasStatus(StatusType.Oily))
            {
            }

            return !blocked;
        }

        public override StatusType GetStatusType()
        {
            return StatusType.Frozen;
        }
    }
}
