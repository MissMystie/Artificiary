using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class TrackTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [Header("Tracker")]

        [SerializeField] private Transform socket;
        [SerializeField] private Transform tracker;
        [SerializeField] private float maxOffset = 1f;

        [Header("Tracking")]

        [SerializeField] private float maxSeeRange = 10f;
        [SerializeField] private float minTrackRange = 0.5f;
        [SerializeField] private float maxTrackRange = 8f;
        [SerializeField] private float minTrackDelta = 0.001f;
        [SerializeField] private float trackSpeed = 1.0f;

        private void Awake()
        {

        }

        private void Update()
        {
            if (socket != null && tracker != null && target != null)
            {
                Vector3 targetDist = target.position - socket.position;
                float offset = Mathf.Min(targetDist.magnitude / maxTrackRange, 1) * maxOffset;
                Vector3 lookDir = targetDist.normalized * offset;
                Vector2 newTrackerPos = socket.position + lookDir;
                
                if (Vector2.Distance(tracker.position, newTrackerPos) > minTrackDelta)
                {
                    float step = trackSpeed * Time.deltaTime;
                    tracker.position = Vector2.MoveTowards(tracker.position, newTrackerPos, step);
                }
            }
        }
    }
}
