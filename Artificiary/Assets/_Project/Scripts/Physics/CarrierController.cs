using FMODUnity;
using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie.Physics
{
    [RequireComponent(typeof(PhysicsBody))]
    public class CarrierController : MonoBehaviour
    {
        [SerializeField] protected PhysicsBody body;
        [SerializeField] protected List<Collider2D> colliders;

        [SerializeField] private LayerMask passengerMask = -1;
        [SerializeField] private float detectRange = 1 / 16;

        private List<Collider2D> passengers;

        protected virtual void Start()
        {
            if (body == null) body = GetComponent<PhysicsBody>();
            passengers = new List<Collider2D>();
        }

        private void OnEnable()
        {
            body.onMoveBefore += (Vector2 moveAmount) => {
                //CalculatePassengerMovement(moveAmount);
                MovePassengers(moveAmount, true); //Move passengers before
            };

            body.onMove += (Vector2 moveAmount) => MovePassengers(moveAmount, false); //Move passengers after
        }

        private void OnDisable()
        {
            body.onMoveBefore -= (Vector2 moveAmount) => {
                //CalculatePassengerMovement(moveAmount);
                MovePassengers(moveAmount, true); //Move passengers before
            };

            body.onMove -= (Vector2 moveAmount) => MovePassengers(moveAmount, false); //Move passengers after
        }

        protected void Reset()
        {
            body = GetComponent<PhysicsBody>();
            colliders = GetComponentsInChildren<Collider2D>().ToList();
        }

        protected void FixedUpdate()
        {
            passengers.Clear();

            foreach (Collider2D collider in colliders)
            {
                Vector2 passengerCheckPos = collider.bounds.center + transform.up * (collider.bounds.extents.y + detectRange);
                Collider2D[] collidersDetected = Physics2D.OverlapBoxAll(passengerCheckPos, new Vector2(collider.bounds.size.x, detectRange), transform.rotation.eulerAngles.z, passengerMask);
                foreach (Collider2D detectedCollider in collidersDetected)
                {
                    if (!passengers.Contains(detectedCollider)) passengers.Add(detectedCollider);
                }
            }
        }

        protected void MovePassengers(Vector2 moveAmount, bool moveBeforeCarrier)
        {
            

            if (moveBeforeCarrier)
            {

            }
            else
            {
                foreach (Collider2D passenger in passengers)
                {
                    PhysicsBody phys = passenger.GetComponent<PhysicsBody>();
                    if (phys != null) phys.Move(moveAmount);
                }
            }
        }

#if UNITY_EDITOR

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            //if (platform && velocity != Vector2.zero)
            //GizmosExtensions.DrawArrowRay(platform.transform.position, velocity.normalized, 0.5f);

            foreach (Collider2D c in colliders)
            {
                if (c == null) continue;

                Vector2 passengerCheckPos = c.bounds.center + transform.up * (c.bounds.extents.y + (detectRange / 2));
                Gizmos.color = !passengers.IsNullOrEmpty() ? Color.green : Color.blue;
                Gizmos.DrawWireCube(passengerCheckPos, new Vector2(c.bounds.size.x, detectRange));
            }
        }

#endif
    }
}
