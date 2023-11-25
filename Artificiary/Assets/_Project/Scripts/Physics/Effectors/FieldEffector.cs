using Mystie.Core;
using Mystie.Physics;
using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public abstract class FieldEffector : MonoBehaviour, IEffector
    {
        [SerializeField] protected BoxCollider2D collider;
        [SerializeField] protected LayerMask mask = -1;

        [Space]

        protected HashSet<IEffectable> affectedObjects;

        public abstract Vector2 GetForce(PhysicsObject target);

        public Vector2 GetAnchorPos(Vector2 anchorPos)
        {
            return transform.position.xy() + anchorPos.Rotate(transform.rotation.eulerAngles.z);
        }

        protected virtual void Awake()
        {
            if (collider == null) collider = GetComponent<BoxCollider2D>();
            affectedObjects = new HashSet<IEffectable>();
        }

        protected virtual bool EnterEffector(PhysicsObject physObj)
        {
            if (physObj == null) return false;

            physObj.AddEffector(this);
            affectedObjects.Add(physObj);

            return true;
        }

        protected virtual bool LeaveEffector(PhysicsObject physObj)
        {
            if (physObj == null || !affectedObjects.Contains(physObj)) return false;

            physObj.RemoveEffector(this);
            affectedObjects.Remove(physObj);

            return true;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.IsInLayerMask(mask))
            {
                EnterEffector(col.GetComponent<PhysicsObject>());
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.gameObject.IsInLayerMask(mask))
            {
                LeaveEffector(col.GetComponent<PhysicsObject>());
            }
        }
    }
}
