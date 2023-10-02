using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Utils
{
    public class BezierCurve
    {
        // p0 - start position
        // p1 - start tangent
        // p2 - end tangent
        // p3 - end position
        public (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) points;

        public BezierCurve()
        {
            Vector3 v0 = Vector3.zero;
            points = (v0, v0, v0, v0);
        }

        public BezierCurve((Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) points)
        {
            this.points = points;
        }

        public Vector3[] GetSegments(int subdivisions)
        {
            Vector3[] segments = new Vector3[subdivisions];

            float t;
            for (int i = 0; i < subdivisions; i++)
            {
                t = (float)i / subdivisions;
                segments[i] = GetSegment(t);
            }

            return segments;
        }

        // equation from: https://en.wikipedia.org/wiki/B%C3%A9zier_curve
        public Vector3 GetSegment(float t0)
        {
            t0 = Mathf.Clamp01(t0);
            float t1 = 1 - t0;

            Vector3 segment = (t1 * t1 * t1 * points.p0)
                + (3 * t1 * t1 * t0 * points.p1)
                + (3 * t1 * t0 * t0 * points.p2)
                + (t0 * t0 * t0 * points.p3);

            return segment;
        }
    }
}
