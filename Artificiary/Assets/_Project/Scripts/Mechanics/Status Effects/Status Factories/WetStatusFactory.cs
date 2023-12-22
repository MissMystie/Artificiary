using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Wet Status", menuName = "CustomData/Status Effects/Wet Status", order = 3)]
    public class WetStatusFactory : StatusEffectFactory<WetStatusData, WetStatus> { }

    [System.Serializable]
    public class WetStatusData
    {
        public float duration = 12f;

        public GameObject steamCloud;

        [Header("VFX")]

        public ParticleSystemController wetPFX;

        [Header("Sound effects")]

        public FMODUnity.EventReference vaporizeSFX;
    }

    public class WetStatus : StatusEffect<WetStatusData>
    {
        public ParticleSystemController wetPFX;

        public override bool Apply()
        {
            if (!React()) return false;

            duration = data.duration;
            Debug.Log(target.gameObject.name + " is <color=cyan>damp</color>");

            try
            {
                wetPFX = GameObject.Instantiate(data.wetPFX.gameObject, target.entity.Sprite.transform).GetComponent<ParticleSystemController>();
                wetPFX.SetShape(target.entity.Sprite);
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("ChemEngine: Wet status pfx unassigned.");
            }

            if (wetPFX != null) wetPFX.Play();

            return true;
        }

        public override void Reapply()
        {
            duration = data.duration;
        }

        public override void Expire()
        {
            Debug.Log(target.gameObject.name + " is no longer <color=cyan>wet</color>");

            if (wetPFX != null)
            {
                wetPFX.DetachParticles();
                wetPFX.gameObject.SetActive(false);
            }
        }

        public override bool React()
        {
            bool blocked = false;

            if (target.HasStatus(StatusType.Burn))
            {
                Debug.Log("Vaporize!");
                
                if (data.steamCloud != null)
                    GameObject.Instantiate(data.steamCloud, target.transform);

                if (!data.vaporizeSFX.IsNull)
                    FMODUnity.RuntimeManager.PlayOneShot(data.vaporizeSFX.Path, target.transform.position);

                target.RemoveStatus(StatusType.Burn);
                blocked = true;
            }

            if (target.HasStatus(StatusType.Cold))
            {
                Debug.Log("Freeze!");
                target.ApplyStatus(StatusType.Frozen);
                blocked = true;
            }
                
            if (target.HasStatus(StatusType.Shock))
            {
                Debug.Log("Electrocharge!");
            }

            if (target.HasStatus(StatusType.Oily))
            {
                Debug.Log("Wash!");
            }

            return !blocked;
        }

        public override StatusType GetStatusType()
        {
            return StatusType.Wet;
        }
    }
}
