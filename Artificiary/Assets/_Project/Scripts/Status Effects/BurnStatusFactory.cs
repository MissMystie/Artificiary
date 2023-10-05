using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Burn Status", menuName = "CustomData/Status Effects/Burn Status", order = 1)]
    public class BurnStatusFactory : StatusEffectFactory<BurnStatusData, BurnStatus> { }

    [System.Serializable]
    public class BurnStatusData
    {
        public Damage dmgPerTick;
        public float tickTime = 2f;
        public float duration = 12f;

        public GameObject steamCloud;

        [Header("Fire spread")]

        public float spreadRadius = 2f;
        public float spreadChance = 1f;

        [Header("VFX")]

        public ParticleSystemController burnPFX;
        public ParticleSystemController burnProcPFX;

        [Header("Sound effects")]

        public FMODUnity.EventReference igniteSFX;
        public FMODUnity.EventReference burnSFX;
        public FMODUnity.EventReference extinguishSFX;
    }
    
    public class BurnStatus: StatusEffect<BurnStatusData>
    {
        protected float tickTime;
        protected ParticleSystemController burnPFX;
        protected ParticleSystemController burnProcPFX;

        public override bool Apply()
        {
            if (!React()) return false;

            //obj.gameObject.AddTag(type.ToString());
            Debug.Log(target.gameObject.name + " is <color=magenta>burning</color>");

            try {
                //TODO use unity object pooling system
                burnPFX = GameObject.Instantiate(data.burnPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();
                burnProcPFX = GameObject.Instantiate(data.burnProcPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();

                burnPFX.SetShape(target.entity.Sprite);
                burnProcPFX.SetShape(target.entity.Sprite);
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("ChemEngine: Burn status pfx unassigned.");
            }
            catch (System.NullReferenceException)
            {
                Debug.LogWarning("ChemEngine: No entity or sprite renderer assigned to " + target.gameObject.name + ".");
            }

            if (burnPFX != null) burnPFX.Play();
            if (!data.igniteSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.igniteSFX.Path, target.transform.position);

            tickTime = data.tickTime;
            duration = data.duration;

            return true;
        }

        public override void Reapply()
        {
            duration = data.duration;
        }
        public override void Expire()
        {
            //obj.gameObject.RemoveTag(type.ToString());

            Debug.Log(target.gameObject.name + " is no longer <color=magenta>burning</color>");
            
            if (burnPFX != null) burnPFX.DetachParticles();
            if (burnProcPFX != null) burnProcPFX.DetachParticles();

            if (!data.extinguishSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.extinguishSFX.Path, target.transform.position);
        }

        public override bool React()
        {
            bool blocked = false;

            if (target.HasStatus(StatusType.Shock))
            {
                Debug.Log("Overcharged!");
                //foreach (IHittable h in obj.hittables) { //h.TakeHit(dmg, obj.transform, kb); }
            }

            if (target.HasStatus(StatusType.Cold))
            {
                Debug.Log("Melt!");
            }

            if (target.HasStatus(StatusType.Frozen))
            {
                Debug.Log("Melt!");
            }

            if (target.HasStatus(StatusType.Wet))
            {
                Debug.Log("Vaporize!");
                if (data.steamCloud != null)
                    GameObject.Instantiate(data.steamCloud, target.transform.position, Quaternion.identity);
                blocked = true;
            }

            if (target.HasStatus(StatusType.Oily))
            {
                Debug.Log("Blast!");
            }

            return !blocked;
        }

        public override void OnUpdate(float t)
        {
            tickTime -= t;
            if (tickTime <= 0) {
                Proc();
                tickTime = data.tickTime;
            }
        }

        protected void Proc() {
            if (burnProcPFX != null) burnProcPFX.Play();

            if (!data.burnSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.burnSFX.Path, target.transform.position);

            target.entity.Health.TakeDamage(data.dmgPerTick);
            Spread();
        }

        protected bool Spread()
        {
            bool spread = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, data.spreadRadius);
            foreach (Collider2D col in colliders)
            {
                StatusManager entity;
                if (col.gameObject != target.gameObject && (entity = col.GetComponent<StatusManager>()) != null)
                {
                    if (UnityEngine.Random.Range(0, 1) <= data.spreadChance)
                    {
                        spread = entity.ApplyStatus(GetStatusType()) || spread;
                        //Debug.Log(obj.gameObject.name + " spread <color=magenta>fire</color> to " + target.gameObject.name);
                    }
                }
            }

            return spread;
        }

        public override StatusType GetStatusType()
        {
            return StatusType.Burn;
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.transform.position, data.spreadRadius);
        }
    }
}
