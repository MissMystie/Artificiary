using FMOD.Studio;
using FMODUnity;
using Mystie.ChemEngine;
using Mystie.Logic;
using Mystie.Utils;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie.Physics
{
    public class WheelController : LogicBehavior, IConstrainer
    {
        public StatusManager statusMngr;
        public List<PhysicsObject> platforms;

        [Space]

        public float radius = 5f;
        public float maxRotSpeed = 20f;
        public float rotSpeed = 0f;
        public float rotPercent = 0f;
        public float minSpeed = 1 / 16f;
        public float friction = 0.1f;

        [Space]

        public float minPowerSpeed = 5f;

        [Space]

        public StatusType frictionlessStatus;
        public StatusType lockedStatus;

        protected bool noFriction = false;

        public float Circumference { get => Mathf.PI * radius; }

        [Foldout("Feedback")]
        [SerializeField] protected EventReference wheelLoop;
        [Foldout("Feedback")]
        [SerializeField] protected EventReference fastWheelLoop;

        protected EventInstance wheelLoopInstance;
        protected EventInstance fastWheelLoopInstance;

        protected override void Awake()
        {
            if (statusMngr == null) statusMngr = GetComponent<StatusManager>();
            foreach (PhysicsObject platform in platforms)
            {
                platform.AddConstraint(this);
            }

            base.Awake();

            if (!wheelLoop.IsNull)
                wheelLoopInstance = RuntimeManager.CreateInstance(wheelLoop);
            RuntimeManager.AttachInstanceToGameObject(wheelLoopInstance, transform);

            if (!fastWheelLoop.IsNull)
                fastWheelLoopInstance = RuntimeManager.CreateInstance(fastWheelLoop);
            RuntimeManager.AttachInstanceToGameObject(fastWheelLoopInstance, transform);
        }

        protected override void OnEnable()
        {
            statusMngr.onStatusInflicted += (statusMngr, statusInflicted) => OnStatusInflicted(statusInflicted);
            statusMngr.onStatusExpired += (statusMngr, statusInflicted) => OnStatusExpired(statusInflicted);
        
            base.OnEnable();
        }

        protected void OnDisable()
        {
            statusMngr.onStatusInflicted -= (statusMngr, statusInflicted) => OnStatusInflicted(statusInflicted);
            statusMngr.onStatusExpired -= (statusMngr, statusInflicted) => OnStatusExpired(statusInflicted);
        }

        protected void FixedUpdate()
        {
            bool powerOn = Mathf.Abs(rotSpeed) >= minPowerSpeed;
            if (_on != powerOn) SetOnValue(powerOn);

            if (!_locked)
            {
                float rotDelta = (rotSpeed / Circumference) * 360f * Time.deltaTime;

                foreach (PhysicsObject platform in platforms)
                {
                    Vector2 moveAmount = CalculateMovement(platform, -rotDelta);
                    platform.body.Move(moveAmount);
                }

                if (!noFriction) rotSpeed *= (1f - friction);
                if (Mathf.Abs(rotSpeed) < minSpeed) rotSpeed = 0f;
                rotSpeed = Mathf.Clamp(rotSpeed, -maxRotSpeed, maxRotSpeed);
            }
        }

        protected Vector2 CalculateMovement(PhysicsObject platform, float rotDelta) 
        {
            Vector2 pos = platform.transform.position;
            Vector2 newPos = transform.position.xy() + (pos - transform.position.xy()).Rotate(rotDelta);
            Vector2 moveAmount = newPos - pos;

            return moveAmount;
        }

        public void ApplyConstraint(Transform target, ref Vector2 moveAmount)
        {
            if (!_locked)
            {
                Vector2 perpendicular = -Vector2.Perpendicular(target.position - transform.position).normalized;
                Debug.DrawRay(target.position, perpendicular);

                float force = moveAmount.magnitude * Vector2.Dot(perpendicular, moveAmount.normalized);
                rotSpeed += force;
                rotSpeed = Mathf.Clamp(rotSpeed, -maxRotSpeed, maxRotSpeed);
            }

            moveAmount = Vector2.zero;
        }

        protected void OnStatusInflicted(StatusType statusInflicted)
        {
            if (statusInflicted == lockedStatus)
            {
                if (!_locked) SetLocked(true);
            }

            if (statusInflicted == frictionlessStatus)
            {
                noFriction = true;
            }
        }

        protected void OnStatusExpired(StatusType statusInflicted)
        {
            if (statusInflicted == lockedStatus)
            {
                if (_locked) SetLocked(false);
            }

            if (statusInflicted == frictionlessStatus)
            {
                noFriction = false;
            }
        }

        public override void SetLocked(bool locked)
        {
            if (locked)
            {
                rotSpeed = 0f;
                if (_on) SetOff();
            }
            
            base.SetLocked(locked);
        }

        protected void Reset()
        {
            platforms = GetComponentsInChildren<PhysicsObject>().ToList();
        }

        protected void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
