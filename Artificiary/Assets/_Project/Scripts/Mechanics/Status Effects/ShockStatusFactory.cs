using Mystie.Core;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Shock Status", menuName = "CustomData/Status Effects/Shock Status", order = 2)]
    public class ShockStatusFactory : StatusEffectFactory<ShockStatusData, ShockStatus> { }

    [System.Serializable]
    public class ShockStatusData
    {
        public Damage dmgPerTick;
        public float tickTime = 2f;
        public bool expires = true;

        [ShowIf("expires")]
        public float duration = 12f;

        public GameObject electroBlast;

        [Header("Shock spread")]

        public float spreadRadius = 2f;
        public float spreadChance = 1f;

        [Header("VFX")]

        public ParticleSystemController shockPFX;
        public ParticleSystemController shockProcPFX;

        [Header("Sound effects")]

        public FMODUnity.EventReference shockSFX;
        public FMODUnity.EventReference shockProcSFX;
        public FMODUnity.EventReference shockStopSFX;
    }

    public class ShockStatus: StatusEffect<ShockStatusData>
    {
        protected float tickTime;
        protected ParticleSystemController shockPFX;
        protected ParticleSystemController shockProcPFX;

        public override bool Apply()
        {
            if (!React()) return false;

            duration = data.duration;
            tickTime = data.tickTime;

            //obj.gameObject.AddTag(type.ToString());
            Debug.Log(target.gameObject.name + " is <color=magenta>shocked</color>");

            try {
                shockPFX = GameObject.Instantiate(data.shockPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();
                shockProcPFX = GameObject.Instantiate(data.shockProcPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();

                shockPFX.SetShape(target.entity.Sprite);
                shockProcPFX.SetShape(target.entity.Sprite);
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("ChemEngine: Shock status pfx unassigned.");
            }
            catch (System.NullReferenceException)
            {
                Debug.LogWarning("ChemEngine: No entity or sprite renderer assigned to " + target.gameObject.name + ".");
            }

            if (shockPFX != null) shockPFX.Play();
            if (!data.shockSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.shockSFX.Path, target.transform.position);

            return true;
        }

        public override void Reapply()
        {
            duration = data.duration;
        }

        public override void Expire()
        {
            //obj.gameObject.RemoveTag(type.ToString());

            Debug.Log(target.gameObject.name + " is no longer <color=magenta>shocked</color>");
            
            if (shockPFX != null) 
            {
                shockPFX.DetachParticles();
                shockPFX.gameObject.SetActive(false);
            }
            if (shockProcPFX != null)
            {
                shockProcPFX.DetachParticles();
                shockProcPFX.gameObject.SetActive(false);
            }

            if (!data.shockStopSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.shockStopSFX.Path, target.transform.position);
        }

        public override bool React()
        {
            bool blocked = false;

            if (target.HasStatus(StatusType.Burn)) {
                Debug.Log("Overcharged!");
                if (data.electroBlast != null)
                    GameObject.Instantiate(data.electroBlast, target.transform.position, Quaternion.identity);
                target.RemoveStatus(StatusType.Burn);
                blocked = true;
            }

            if (target.HasStatus(StatusType.Cold)) {
                Debug.Log("Superconduct!");
            }

            if (target.HasStatus(StatusType.Frozen)) {
                Debug.Log("Superconduct!");
            }

            if (target.HasStatus(StatusType.Wet)) {
                Debug.Log("Electrocharge!");
            }

            if (target.HasStatus(StatusType.Oily)) {
                Debug.Log("Blast!");
                blocked = true;
            }

            return !blocked;
        }

        public override void OnUpdate(float t)
        {
            tickTime -= t;
            if (tickTime <= 0)
            {
                Proc();
                tickTime = data.tickTime;
            }
        }

        protected void Proc()
        {
            if (shockProcPFX != null) shockProcPFX.Play();

            if (!data.shockProcSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.shockProcSFX.Path, target.transform.position);

            if (target.entity.Health)
                target.entity.Health.TakeDamage(data.dmgPerTick);
            Spread();

            //if (target.HasStatus(StatusType.Wet))
        }

        bool Spread()
        {
            bool spread = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, data.spreadRadius);
            foreach (Collider2D col in colliders)
            {
                if (col.gameObject == target.gameObject)
                    continue;

                ChemObject chem = col.GetComponent<ChemObject>();

                if (chem != null && chem.properties.Conductive)
                {
                    if (Random.Range(0, 1) <= data.spreadChance)
                    {
                        spread = chem.statusMngr.ApplyStatus(GetStatusType()) || spread;
                        //Debug.Log(obj.gameObject.name + " spread <color=magenta>shock</color> to " + target.gameObject.name);
                    }
                }
            }

            return spread;
        }

        public override StatusType GetStatusType()
        {
            return StatusType.Shock;
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(target.transform.position, data.spreadRadius);
        }
    }
}
