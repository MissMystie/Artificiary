using Mystie.Core;
using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.Physics
{
    public class MovingPlatform : PlatformController
    {
        [SerializeField] protected bool local = true;
        [SerializeField] protected Vector2[] waypoints;

        [SerializeField, HideInInspector] protected Vector2[] _waypoints;

        [Space]

        public float speed = 3;
        public int direction = 1;
        public bool loop = true;
        //[HideIf("cyclic")] public bool fullLoop = true;
        public float waitTime = 0.5f;

        private Timer waitTimer;

        [Space]

        [Range(0, 1)] public float movePercent;
        [Range(0, 2)] public float easeAmount;

        [Space]

        [SerializeField] protected int startIndex;
        protected int lastIndex;
        protected int nextIndex;
        //protected float nextMoveTime;

        [Header("Debug")]

        public bool showPlatforms = true;

        protected void Start()
        {
            waitTimer = new Timer();

            if (local && !_waypoints.IsNullOrEmpty())
            {
                //waypoints = waypoints.Offset(startIndex);
                UpdateWaypoints();
            }

            //if (speed < 0) lastIndex--;
            lastIndex = startIndex;
            nextIndex = _waypoints.LoopBack(lastIndex + Math.Sign(speed));
        }

        protected void Update()
        {
            waitTimer.Tick(Time.deltaTime);
        }

        protected override Vector2 GetMoveAmount(float deltaTime)
        {
            if (_waypoints.IsNullOrEmpty() || speed == 0 || waitTimer.IsRunning() || IsWaypointReached())
                return Vector2.zero;

            float distance = Vector2.Distance(_waypoints[lastIndex], _waypoints[nextIndex]);

            //Calculate new move percent
            movePercent += Time.deltaTime * speed / distance;
            movePercent = Mathf.Clamp01(movePercent);
            float easedPercent = movePercent.Ease(easeAmount + 1);

            //Calculate velocity
            Vector2 newPos = Vector2.Lerp(_waypoints[lastIndex], _waypoints[nextIndex], easedPercent);
            Vector2 moveAmount = newPos - body.transform.position.xy();

            return moveAmount;
        }

        public bool IsWaypointReached()
        {
            //If waypoint is reached
            //if ((dir == 1 && movePercent >= 1) || (dir == -1 && movePercent <= 0))
            if (movePercent >= 1)
            {
                movePercent = 0f;

                if (loop)
                {
                    lastIndex = _waypoints.LoopBack(lastIndex + direction);
                }
                else
                {
                    //int lastWaypoint = (!fullLoop && waypoints.Length > 2) ? waypoints.Length - 2 : waypoints.Length - 1;

                    lastIndex = Math.Clamp(lastIndex + direction, 0, _waypoints.Length - 1);

                    if ((direction == 1 && lastIndex >= (_waypoints.Length - 1)) || (direction == -1 && lastIndex <= 0))
                        direction *= -1;
                }

                //Reset move percent
                //if (Mathf.Sign(speed) == 1 && movePercent >= 1) movePercent = 0f;
                //else if (Mathf.Sign(speed) == -1 && movePercent <= 0) movePercent = 1f;

                //Select next waypoint
                nextIndex = _waypoints.LoopBack(lastIndex + direction);

                //if (dynamic)
                //followTarget = dynamicWaypoints[fromWaypointIndex];

                waitTimer.SetTime(waitTime);

                return true;
            }

            return false;
        }

        protected virtual void UpdateWaypoints()
        {
            if (waypoints.IsNullOrEmpty())
            {
                _waypoints = new Vector2[0];
                return;
            }

            _waypoints = new Vector2[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++)
            {
                _waypoints[i] = waypoints[i];
                if (local) _waypoints[i] += (Vector2)transform.position;
            }
        }

        protected void OnValidate()
        {
            UpdateWaypoints();
            movePercent = Mathf.Clamp01(movePercent);
            direction = Math.Sign(direction);

            if (!_waypoints.IsNullOrEmpty())
            {
                startIndex = Mathf.Clamp(startIndex, 0, _waypoints.Length - 1);

                /*
                if (!EditorApplication.isPlayingOrWillChangePlaymode && platform != null)
                {
                    if (_waypoints.Length >= 2)
                    {
                        float easedPercent = movePercent.Ease(easeAmount + 1);
                        int nextIndex = loop ? (startIndex + 1) % _waypoints.Length : Mathf.Clamp(startIndex + 1, 0, _waypoints.Length - 1);
                        platform.transform.position = Vector2.Lerp(_waypoints[startIndex], _waypoints[nextIndex], easedPercent);
                    }
                    else
                    {
                        platform.transform.position = _waypoints[startIndex];
                    }
                }*/
            }
            else
            {
                startIndex = 0;
            }
        }


#if UNITY_EDITOR

        protected virtual void OnDrawGizmos()
        {
            if (body != null && !_waypoints.IsNullOrEmpty() && _waypoints.Length >= 2)
            {
                Vector2 offset = col != null? col.bounds.center - body.transform.position : Vector2.zero;

                for (int i = 0; i < _waypoints.Length; i++)
                {
                    Handles.Label(_waypoints[i] + offset, i.ToString());

                    int j = (i > 0) ? i - 1 : _waypoints.Length - 1;
                    if ((i > 0) || loop)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(_waypoints[j] + offset, _waypoints[i] + offset);
                    }
                }

                foreach (Vector2 waypoint in _waypoints)
                {
                    Gizmos.color = Color.red;
                    GizmosExtensions.DrawCross(waypoint + offset, .3f);

                    if (showPlatforms && body && col != null)
                    {
                        Gizmos.DrawWireCube(waypoint + offset, col.bounds.size);
                    }
                }
            }
        }

#endif
    }
}
