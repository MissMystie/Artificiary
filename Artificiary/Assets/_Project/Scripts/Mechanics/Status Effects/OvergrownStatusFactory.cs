using Mystie.Core;
using Mystie.Physics;
using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.ChemEngine
{
    [CreateAssetMenu(fileName = "Overgrown Status", menuName = "CustomData/Status Effects/Overgrown Status")]
    public class OvergrownStatusFactory : StatusEffectFactory<OvergrownStatusData, OvergrownStatus> { }

    [System.Serializable]
    public class OvergrownStatusData
    {
        public LayerMask bloomMask;
        public GameObject smokeCloud;

        [Header("VFX")]

        public ParticleSystemController overgrownPFX;
        public ParticleSystemController bloomPFX;

        [Header("Sound effects")]

        public FMODUnity.EventReference applySFX;
        public FMODUnity.EventReference bloomSFX;
        public FMODUnity.EventReference burnSFX;
    }
    
    public class OvergrownStatus : StatusEffect<OvergrownStatusData>
    {
        protected PhysicsObject phys;
        protected ParticleSystemController overgrownPFX;
        protected ParticleSystemController bloomPFX;
        protected bool hasBloomed;

        public override bool Apply()
        {
            if (!React()) return false;

            //obj.gameObject.AddTag(type.ToString());
            Debug.Log(target.gameObject.name + " is <color=green>overgrown</color>");

            Debug.Log(target.entity);
            Debug.Log(target.entity.Sprite);

            phys = target.entity.Phys;
            if (phys != null) phys.onCollision += OnCollision;

            // TODO use unity object pooling system
            if (data.overgrownPFX != null)
            {
                overgrownPFX = GameObject.Instantiate(data.overgrownPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();
                if (target.entity.Sprite != null)
                    overgrownPFX.SetShape(target.entity.Sprite);

                overgrownPFX.Play();
            }

            if (!data.applySFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(data.applySFX.Path, target.transform.position);

            return true;
        }

        public void Bloom(Collider2D col)
        {
            hasBloomed = true;

            if (phys != null)
            {
                phys.SetVelocity(Vector2.zero);
                phys.simulatePhysics = false;
                phys.onCollision -= OnCollision;
            }

            target.transform.SetParent(col.transform);

            if (overgrownPFX != null) overgrownPFX.Stop();

            if (data.bloomPFX != null)
            {
                bloomPFX = GameObject.Instantiate(data.bloomPFX.gameObject, target.transform).GetComponent<ParticleSystemController>();
                if (target.entity.Sprite != null)
                    bloomPFX.SetShape(target.entity.Sprite);
            }

            FMODUnity.RuntimeManager.PlayOneShot(data.bloomSFX.Path, target.transform.position);
            Debug.Log(target.gameObject.name + " has <color=green>bloomed</color>");
        }

        public override void Expire()
        {
            if (hasBloomed)
            {
                if (bloomPFX != null) overgrownPFX.Stop();
            }
            else
            {
                if (overgrownPFX != null) overgrownPFX.Stop();
            }
        }

        public void OnCollision(Collider2D col)
        {
            if(!hasBloomed && col.gameObject.IsInLayerMask(data.bloomMask)) 
                Bloom(col);
        }

        public override bool React()
        {
            bool blocked = false;

            if (target.HasStatus(StatusType.Burn))
            {
                Debug.Log("Burn!");
                FMODUnity.RuntimeManager.PlayOneShot(data.burnSFX.Path, target.transform.position);
                GameObject.Instantiate(data.smokeCloud, target.transform);
                target.RemoveStatus(StatusType.Overgrown);
                blocked = true;
            }

            /*
            if (target.HasStatus(StatusType.Shock))
            {
                Debug.Log("Overcharged!");
                if (data.electroBlast != null)
                {
                    GameObject blast = GameObject.Instantiate(data.electroBlast, target.transform.position, Quaternion.identity);
                }
                    
                target.RemoveStatus(StatusType.Shock);
                blocked = true;
            }*/

            return !blocked;
        }

        public override StatusType GetStatusType()
        {
            return StatusType.Overgrown;
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(target.transform.position, 0.1f);
        }
    }
}
