using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Mystie.Core;
using System;
using Mystie.Utils;
using NaughtyAttributes;
using Unity.VisualScripting;

namespace Mystie.Physics
{
    [RequireComponent(typeof(PhysicsBody))]
    public class PhysicsObject : MonoBehaviour, IEffectable, IHittable
    {
        #region Events

        public event Action<Collider2D> onCollision;
        public event Action onGrounded;
        public event Action onWall;
        public event Action onWallLeft;

        #endregion

        #region Variables

        public Entity entity;
        public Collider2D col { get; private set; }
        public PhysicsState state;

        #endregion

        public PhysicsBody body { get; private set; }
        [SerializeField] private GameObject sprite;
        public Animator anim { get; protected set; }

        [Header("State")]

        protected Collider2D _groundCol;
        protected Collider2D _wallCol;

        public Collider2D GroundCol { get => _groundCol; }
        public Collider2D WallCol { get => _wallCol; }

        [Header("Physics")]

        [SerializeField] public PhysicsData data;
        public bool simulatePhysics = true;
        public bool applyGravity = true;
        public bool applyDrag = true;
        [SerializeField] private Stat mass = new Stat(1f);
        public float volume = 1f;
        [SerializeField] private Vector2 gravity = new Vector2(0f, -10f);
        public Vector2 Gravity { get => gravity; }
        public Vector2 localVelocity;
        public Stat friction = new Stat(0.4f);
        public StatV2 drag = new StatV2(.005f, 0);

        public Vector2 Weight
        {
            get { return mass * gravity; }
        }

        private Vector3 lastPos;
        public Vector2 velocity { get; protected set; }

        [Space]

        public const float minVelocity = 0.001f;
        public const float maxVelocity = 40f;

        private HashSet<IEffector> effectors = new HashSet<IEffector>();
        private List<IConstrainer> constraints = new List<IConstrainer>();

        public Collider2D waterCol;
        public LayerMask waterLayer;

        [Header("Wall Check")]

        [Foldout("Feedbacks")][SerializeField] private MMFeedbacks impactFX;
        [Foldout("Feedbacks")][SerializeField] private MMFeedbacks landFX;
        [Foldout("Feedbacks")][SerializeField] private string groundedAnimParam = "Grounded";
        [Foldout("Feedbacks")][SerializeField] private string speedXAnimParam = "SpeedX";
        [Foldout("Feedbacks")][SerializeField] private string speedYAnimParam = "SpeedY";
        
        [Header("Debug")]

        public bool showDebug = true;

        private void Awake()
        {
            // cache components

            if (col == null) col = GetComponent<Collider2D>();
            if (body == null) body = GetComponent<PhysicsBody>();
            if (anim == null) anim = GetComponentInChildren<Animator>();

            //Physics2D.queriesStartInColliders = false;              // Raycast will not start within the ant's collider
            //Physics2D.queriesHitTriggers = false;                   // Raycast will not collide with triggers

            if (anim != null) anim.logWarnings = false;

            lastPos = transform.position;
        }

        private void Update()
        {
            Animate();
        }

        protected void FixedUpdate()
        {
            UpdateState();

            if (simulatePhysics)
            {
                ApplyForces(Time.fixedDeltaTime);
                Vector2 addedVelocity = GetAddedVelocity();
                //If colliding upward or downward, reset vertical velocity to avoid accumulation
                //If bouncing, reset vertical velocity upon reaching minimum threshold
                if ((body.collisions.below && localVelocity.y < 0) 
                    || (body.collisions.above && localVelocity.y > 0))
                {
                    localVelocity.y *= (1 - data.impactDamp);
                }

                if (applyDrag) ApplyDrag(Time.fixedDeltaTime);
                ApplyVelocityBounds();

                Vector2 moveAmount = (localVelocity + addedVelocity) * Time.fixedDeltaTime;
                Move(moveAmount);
            }

            velocity = (transform.position - lastPos) / Time.fixedDeltaTime;
            lastPos = transform.position;
        }

        public void Move(Vector2 moveAmount, PassengerInfo passengerInfo = null)
        {
            ApplyConstraints(ref moveAmount);

            body.Move(moveAmount, passengerInfo);
        }

        #region Status

        protected void UpdateState()
        {
            CheckGrounded();
            CheckAtWall();
            CheckImmersed();
        }

        public bool CheckGrounded()
        {
            bool wasGrounded = state.grounded;
            bool isGrounded = body.collisions.below;
            
            if (isGrounded && !wasGrounded)
            {
                OnGrounded();
                onGrounded?.Invoke();
            }

            state.grounded = isGrounded;
            return state.grounded;
        }

        public bool CheckAtWall()
        {
            bool wasAtWall = state.atWall;
            bool isAtWall = (body.collisions.left || body.collisions.right);

            state.atWall = isAtWall;
            state.wallDir = body.collisions.left ? -1 : 1;

            if (isAtWall && !wasAtWall)
            {
                OnWall();
                onWall?.Invoke();
            }
            else if (!isAtWall && wasAtWall) onWallLeft?.Invoke();
            return state.atWall;
        }

        public bool CheckImmersed()
        {
            bool wasImmersed = state.immersed;
            bool isImmersed = state.inWater && col.bounds.center.y <= waterCol.bounds.max.y;

            state.immersed = isImmersed;

            return state.immersed;
        }

        public void OnGrounded()
        {
            landFX?.PlayFeedbacks();
            onGrounded?.Invoke();
        }

        public void OnWall() { }

        #endregion

        #region Velocity

        public void SetVelocity(Vector2 newVelocity)
        {
            localVelocity = newVelocity;
        }

        public void AddVelocity(Vector2 newVelocity)
        {
            localVelocity += newVelocity;
        }

        public void ApplyVelocityBounds()
        {
            if (localVelocity.magnitude <= minVelocity)
                localVelocity.x = 0f;
            if (localVelocity.magnitude <= minVelocity)
                localVelocity.x = 0f;
            if (localVelocity.magnitude > maxVelocity)
                localVelocity = localVelocity.normalized * maxVelocity;
        }

        public void TakeHit(Vector2 kb)
        {
            SetVelocity(kb);
        }

        #endregion

        #region Effectors

        protected void ApplyForces(float deltaTime)
        {
            Vector2 forceApplied = Vector2.zero;

            if (applyGravity)
            {
                forceApplied += Weight;
            }

            foreach (IEffector effector in effectors)
            {
                forceApplied += effector.GetForce(this);
            }

            localVelocity += forceApplied * deltaTime;
        }

        protected Vector2 GetAddedVelocity()
        {
            Vector2 addedVelocity = Vector2.zero;

            foreach (IEffector effector in effectors)
            {
                addedVelocity += effector.GetAddedVelocity(this);
            }

            return addedVelocity;
        }

        protected void ApplyDrag(float deltaTime)
        {
            Vector2 v = localVelocity;

            if (state.grounded) v.x *= (1 - friction);
            else
            {
                v.x *= 1 - drag.x;
                v.y *= 1 - drag.y;
            }

            localVelocity = v;
        }

        public void AddEffector(IEffector effector)
        {
            effectors.Add(effector);
        }

        public void RemoveEffector(IEffector effector)
        {
            if (effectors.Contains(effector))
                effectors.Remove(effector);
        }

        #endregion

        #region Constraints

        public void AddConstraint(IConstrainer constraint)
        {
            constraints.Add(constraint);
        }

        public void RemoveConstraint(IConstrainer constraint)
        {
            if (constraints.Contains(constraint))
                constraints.Remove(constraint);
        }

        void ApplyConstraints(ref Vector2 moveAmount)
        {
            foreach (IConstrainer constraint in constraints)
                constraint.ApplyConstraint(transform, ref moveAmount);
        }

        #endregion
        private void Animate()
        {
            if (anim != null)
            {
                anim.logWarnings = false;
                anim.SetBool(groundedAnimParam, state.grounded);
                anim.SetFloat(speedXAnimParam, Math.Abs(localVelocity.x));
                anim.SetFloat(speedYAnimParam, localVelocity.y);
                anim.logWarnings = true;
            }
        }

        /*
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.IsInLayerMask(body.colMask))
            {
                onCollision?.Invoke(collision.collider);
                impactFX?.PlayFeedbacks();
            }
        }*/

        protected void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.IsInLayerMask(body.colMask))
            {
                onCollision?.Invoke(collider);
                impactFX?.PlayFeedbacks();
            }

            if (collider.gameObject.IsInLayerMask(waterLayer))
            {
                waterCol = collider;
                state.inWater = true;
            }
        }

        protected void OnTriggerExit2D(Collider2D collider)
        {
            if (collider == waterCol)
            {
                state.inWater = false;
                waterCol = null;
            }
        }

        [Serializable]
        public class PhysicsData
        {
            [Range(0, 1)] public float impactDamp = 0.8f;
        }

        [System.Serializable]
        public class PhysicsState
        {
            public Vector2 velocity = Vector2.zero;

            public bool grounded;
            public bool falling;
            public bool atWall;
            public bool leavingWall;
            public int wallDir;

            public bool inWater;
            public bool immersed;
            public bool atSurface;
        }
    }
}
