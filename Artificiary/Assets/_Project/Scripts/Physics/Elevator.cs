using LDtkUnity;
using Mystie.Utils;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.Physics
{
    public class Elevator : PlatformController, ILDtkImportedFields
    {
        [SerializeField] private LayerMask targetMask;
        public float speed = 3f;
        [Range(0, 1)] public float movePercent;

        [SerializeField] protected Vector2 posA;
        [SerializeField] protected Vector2 posB;

        [SerializeField, HideInInspector] protected Vector2 anchorA;
        [SerializeField, HideInInspector] protected Vector2 anchorB;

        [SerializeField] protected bool moving;
        [SerializeField] protected int direction = 1;
        public bool resting = true;

        [Header("Debug")]

        public bool showPlatforms = true;

        protected void Start()
        {
            UpdateWaypoints();
        }

        protected override Vector2 GetMoveAmount(float deltaTime) 
        {
            if (moving)
            {
                Vector2 anchorPos = direction == 1 ? anchorA : anchorB;
                Vector2 distance = (anchorB - anchorA) * direction;

                movePercent += (speed / distance.magnitude) * Time.deltaTime;
                movePercent = Mathf.Clamp01(movePercent);

                Vector2 targetPos = anchorPos + (distance * movePercent);

                if (movePercent >= 1)
                {
                    StopMoving();
                    direction *= -1;
                    movePercent = 0;
                }

                return targetPos - body.transform.position.xy();
            }

            return Vector2.zero;
        }

        [Button()]
        public void StartMoving()
        {
            resting = false;
            moving = true;

            //startFX?.PlayFeedbacks();
        }

        [Button()]
        public void StopMoving()
        {
            //anim.SetBool(onAnimParam, false);
            moving = false;
            //if (barrier != null) barrier.SetActive(false);

            //if (anim != null) { anim.SetBool(onAnimParam, false); }

            //stopFX?.PlayFeedbacks();
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (resting && collider.gameObject.IsInLayerMask(targetMask)) 
                StartMoving();
        }

        protected virtual void UpdateWaypoints()
        {
            anchorA = posA + (Vector2)transform.position;
            anchorB = posB + (Vector2)transform.position;
        }

        protected void OnValidate()
        {
            UpdateWaypoints();
            movePercent = Mathf.Clamp01(movePercent);
            direction = Math.Sign(direction);
        }

        public void OnLDtkImportFields(LDtkFields fields)
        {
        }

#if UNITY_EDITOR

        protected void OnDrawGizmos()
        {
            if (body != null && !colliders.IsNullOrEmpty())
            {
                Collider2D col = colliders[0];

                Vector2 offset = col.bounds.center - body.transform.position;

                Handles.Label(anchorA + offset, "A");
                Handles.Label(anchorB + offset, "B");
                Gizmos.color = Color.white;
                Gizmos.DrawLine(anchorA + offset, anchorB + offset);

                Gizmos.color = Color.red;
                GizmosExtensions.DrawCross(anchorA + offset, .3f);
                GizmosExtensions.DrawCross(anchorB + offset, .3f);

                if (showPlatforms && body && col)
                {
                    Gizmos.DrawWireCube(anchorA + offset, col.bounds.size);
                    Gizmos.DrawWireCube(anchorB + offset, col.bounds.size);
                }
            }
        }

#endif
    }
}
