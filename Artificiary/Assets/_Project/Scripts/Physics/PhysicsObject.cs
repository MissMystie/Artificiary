using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Mystie.Core;
using System;
using Mystie.Core;

namespace Mystie.Physics
{
    public class PhysicsObject : MonoBehaviour, IHittable
    {
        #region Events

        public event Action onGrounded;
        public event Action onWallLeft;

        #endregion

        #region Variables

        public Entity entity;
        public Collider2D col { get; private set; }
        public PhysicsState state;

        #endregion

        public Rigidbody2D rb { get; private set; }
        [SerializeField] private GameObject sprite;
        public Animator anim { get; protected set; }

        [Header("State")]

        public int faceDir = 1;
        public Collider2D groundCol;
        public Collider2D wallCol;

        [Header("Physics")]

        public bool applyGravity = true;
        [SerializeField] private float mass = 1f;
        [SerializeField] private Vector2 gravity = new Vector2(0f, -10f);

        public Vector2 weight
        {
            get { return mass * gravity; }
        }

        [SerializeField] private float maxVelocity = 40f;
        [SerializeField] private bool applyVelocityCap = true;

        [Header("Ground Check")]

        public Transform groundCheck;
        public float groundCheckDist = 0.1f;
        public LayerMask groundMask;
        public float groundAngle { get; private set; }

        public Collider2D waterCol;
        public LayerMask waterLayer;

        [Header("Wall Check")]

        public Transform wallCheck;
        public float wallCheckDist = 0.1f;

        [Header("Feedbacks")]

        [SerializeField] private MMFeedbacks impactFX;
        [SerializeField] private string groundedAnimParam = "Grounded";
        [SerializeField] private string speedXAnimParam = "SpeedX";
        [SerializeField] private string speedYAnimParam = "SpeedY";
        [SerializeField] private MMFeedbacks landFX;

        [Header("Debug")]

        public bool showDebug = true;

        private void Awake()
        {
            // cache components

            if (col == null) col = GetComponent<Collider2D>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (anim == null) anim = GetComponentInChildren<Animator>();

            rb.gravityScale = 0f;

            Physics2D.queriesStartInColliders = false;              // Raycast will not start within the ant's collider
            Physics2D.queriesHitTriggers = false;                   // Raycast will not collide with triggers

            if (anim != null) anim.logWarnings = false;
        }

        private void Update()
        {
            // ground check

            bool wasGrounded = state.grounded;
            state.grounded = IsGrounded();
            if (!wasGrounded && state.grounded) OnGrounded();

            // wall check

            bool wasAtWall = state.atWall;
            
            RaycastHit2D hitLeft = Physics2D.Raycast(wallCheck.position, -transform.right, wallCheckDist, groundMask);
            RaycastHit2D hitRight = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDist, groundMask);

            if (showDebug)
            {
                Debug.DrawRay(wallCheck.position, -transform.right * groundCheckDist, hitLeft ? Color.green : Color.red);
                Debug.DrawRay(wallCheck.position, transform.right * groundCheckDist, hitRight ? Color.green : Color.red);
            }

            state.atWall = hitLeft || hitRight;

            if (state.atWall)
            {
                if (hitLeft)
                {
                    state.wallDir = -1;
                    wallCol = hitLeft.collider;
                }
                else if (hitRight)
                {
                    state.wallDir = 1;
                    wallCol = hitRight.collider;
                }
            }
            else
            {
                groundCol = null;
            }

            if (state.atWall && !wasAtWall)
            {

            }
            else if (!state.atWall && wasAtWall)
            {
                onWallLeft?.Invoke();
            }

            // water check

            bool wasImmersed = state.immersed;
            bool isImmersed = state.inWater;

            state.immersed = isImmersed;

            Animate();
        }

        private void FixedUpdate()
        {
            ApplyForces(Time.fixedDeltaTime);
            if (applyVelocityCap) ApplyVelocityCap();
        }

        public bool IsGrounded()
        {
            if (groundCheck == null) return false;

            RaycastHit2D hitDown = Physics2D.Raycast(groundCheck.position, -transform.up, groundCheckDist, groundMask);

            if (showDebug)
            {
                Debug.DrawRay(groundCheck.position, -transform.up * groundCheckDist, hitDown ? Color.green : Color.red);
            }

            state.grounded = hitDown;

            if (state.grounded)
            {
                groundCol = hitDown.collider;
                groundAngle = hitDown.normal.Angle();
                return true;
            }
            else
            {
                groundCol = null;
                groundAngle = 0f;
                return false;
            }
        }

        public void OnGrounded()
        {
            onGrounded?.Invoke();
        }

        #region Velocity

        public void SetVelocity(Vector2 velocity)
        {
            rb.velocity = velocity;
        }

        public void AddVelocity(Vector2 velocity)
        {
            rb.velocity += velocity;
        }

        public void ApplyVelocityCap()
        {
            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }

        #endregion

        #region Effectors

        protected void ApplyForces(float deltaTime)
        {
            Vector2 forceApplied = Vector2.zero;

            if (applyGravity) forceApplied += weight;

            rb.velocity += forceApplied * deltaTime;
        }

        #endregion

        private void Animate()
        {
            if (sprite != null)
            {
                Vector2 scale = sprite.transform.localScale;
                scale.x = faceDir * Mathf.Abs(scale.x);
                sprite.transform.localScale = scale;
            }

            if (anim != null)
            {
                anim.logWarnings = false;
                anim.SetBool(groundedAnimParam, state.grounded);
                anim.SetFloat(speedXAnimParam, Math.Abs(rb.velocity.x));
                anim.SetFloat(speedYAnimParam, rb.velocity.y);
                anim.logWarnings = true;
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
            state.inWater = false;
            waterCol = null;
        }

        public void TakeHit(Vector2 kb)
        {
            SetVelocity(kb);
        }

        [System.Serializable]
        public class PhysicsState
        {
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
