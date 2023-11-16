using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Mystie.Core;
using System;
using Mystie.Utils;
using NaughtyAttributes;

namespace Mystie.Physics
{
    [RequireComponent(typeof(PhysicsBody))]
    public class PhysicsObject : MonoBehaviour, IHittable
    {
        #region Events

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
        public Vector2 velocity;
        public Vector2 addedVelocity;
        public Stat friction = new Stat(0.4f);
        public StatV2 drag = new StatV2(.005f, 0);

        public Vector2 Weight
        {
            get { return mass * gravity; }
        }

        [Space]

        public const float minVelocity = 0.001f;
        public const float maxVelocity = 40f;

        private HashSet<IEffector> effectors = new HashSet<IEffector>();

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

                //If colliding upward or downward, reset vertical velocity to avoid accumulation
                //If bouncing, reset vertical velocity upon reaching minimum threshold
                if ((body.collisions.below && velocity.y < 0) 
                    || (body.collisions.above && velocity.y > 0))
                {
                    velocity.y *= (1 - data.impactDamp);
                }

                if (applyDrag) ApplyDrag(Time.fixedDeltaTime);
                ApplyVelocityBounds();

                body.Move((velocity + addedVelocity) * Time.fixedDeltaTime);
            }
        }

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

        #region Velocity

        public void SetVelocity(Vector2 newVelocity)
        {
            velocity = newVelocity;
        }

        public void AddVelocity(Vector2 newVelocity)
        {
            velocity += newVelocity;
        }

        public void ApplyVelocityBounds()
        {
            if (velocity.magnitude <= minVelocity)
                velocity.x = 0f;
            if (velocity.magnitude <= minVelocity)
                velocity.x = 0f;
            if (velocity.magnitude > maxVelocity)
                velocity = velocity.normalized * maxVelocity;
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

            velocity += forceApplied * deltaTime;
        }

        protected void ApplyDrag(float deltaTime)
        {
            Vector2 v = velocity;

            if (state.grounded) v.x *= (1 - friction);
            else
            {
                v.x *= 1 - drag.x;
                v.y *= 1 - drag.y;
            }

            velocity = v;
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

        private void Animate()
        {
            if (anim != null)
            {
                anim.logWarnings = false;
                anim.SetBool(groundedAnimParam, state.grounded);
                anim.SetFloat(speedXAnimParam, Math.Abs(velocity.x));
                anim.SetFloat(speedYAnimParam, velocity.y);
                anim.logWarnings = true;
            }
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.IsInLayerMask(body.colMask))
            {
                impactFX?.PlayFeedbacks();
            }
        }

        protected void OnTriggerEnter2D(Collider2D collider)
        {
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
