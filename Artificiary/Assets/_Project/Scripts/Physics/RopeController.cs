using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class RopeController : MonoBehaviour
    {
        public LineController line;

        private List<RopeSegment> segments = new List<RopeSegment>();

        public Transform anchorA;
        public Transform anchorB;
        [SerializeField] protected float ropeSegLen = 0.25f;
        [SerializeField] protected float maxLength = 20f;
        [SerializeField] protected Vector2 gravity = new Vector2(0f, -1.5f);

        private void Awake()
        {
            if (line == null) line = GetComponent<LineController>();

            Vector3 ropeStartPoint = anchorA.position;

            int segmentLength = (int)Math.Ceiling(maxLength / ropeSegLen);

            for (int i = 0; i < segmentLength; i++)
            {
                segments.Add(new RopeSegment(ropeStartPoint));
                ropeStartPoint.y -= ropeSegLen;
            }

            if (line != null)
            {
                line.coordinateMode = LineController.CoordinateMode.GLOBAL;
                UpdateLine();
            }
        }

        private void FixedUpdate()
        {
            if (anchorA != null && anchorB != null)
            {
                SimulateRope();
                UpdateLine();
            }
        }

        public void UpdateLine()
        {
            if (line == null) return;

            List<Vector3> positions = new List<Vector3>();

            for (int i = 0; i < segments.Count; i++)
            {
                positions.Add(segments[i].posNow);
            }

            line.positions = positions;
        }

        private void SimulateRope()
        {
            //SIMULATION
            for (int i = 0; i < segments.Count; i++)
            {
                RopeSegment firstSegment = segments[i];
                Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
                firstSegment.posOld = firstSegment.posNow;
                firstSegment.posNow += velocity;
                firstSegment.posNow += gravity * Time.fixedDeltaTime;
                segments[i] = firstSegment;
            }

            //CONSTRAINTS
            for (int i = 0; i < 50; i++)
            {
                ApplyConstraint();
            }
        }

        private void ApplyConstraint()
        {
            RopeSegment firstSegment = segments[0];
            firstSegment.posNow = anchorA.position;
            segments[0] = firstSegment;

            RopeSegment lastSegment = segments[segments.Count - 1];
            lastSegment.posNow = anchorB.position;
            segments[segments.Count - 1] = lastSegment;

            for (int i = 0; i < segments.Count - 1; i++)
            {
                RopeSegment firstSeg = segments[i];
                RopeSegment secondSeg = segments[i + 1];

                float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
                float error = Mathf.Abs(dist - ropeSegLen);
                Vector2 changeDir = Vector2.zero;

                if (dist > ropeSegLen)
                {
                    changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
                }
                else if (dist < ropeSegLen)
                {
                    changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
                }

                Vector2 changeAmount = changeDir * error;
                if (i != 0)
                {
                    firstSeg.posNow -= changeAmount * 0.5f;
                    segments[i] = firstSeg;
                    secondSeg.posNow += changeAmount * 0.5f;
                    segments[i + 1] = secondSeg;
                }
                else
                {
                    secondSeg.posNow += changeAmount;
                    segments[i + 1] = secondSeg;
                }
            }
        }

        public struct RopeSegment
        {
            public Vector2 posNow;
            public Vector2 posOld;

            public RopeSegment(Vector2 pos)
            {
                posNow = pos;
                posOld = pos;
            }
        }
    }
}
