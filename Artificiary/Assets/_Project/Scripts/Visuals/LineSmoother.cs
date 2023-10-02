using Mystie.Utils;
using Mystie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineSmoother : MonoBehaviour
    {
        [SerializeField] public LineRenderer line;

        [Space]

        public float smoothingLength = 2f;
        public int smoothingLevel = 10;

        public Vector3[] positions;

        private PathSmoother smoother = new PathSmoother();
        private Vector3[] smoothPath;

        private void Awake()
        {
            if (line == null) line.GetComponent<LineRenderer>();

            positions = new Vector3[line.positionCount];
            line.GetPositions(positions);

            UpdateLine();
        }

        private void Update()
        {
            UpdateLine();
        }

        public void UpdateLine()
        {
            smoothPath = smoother.GetSmoothPath(positions, smoothingLength, smoothingLevel);
            if (line) DrawLine(smoothPath);
        }

        public void DrawLine(Vector3[] positions)
        {
            if (line == null) return;

            line.positionCount = smoothPath.Length;
            line.SetPositions(smoothPath);
        }

        private void OnValidate()
        {
            smoothingLevel = Math.Max(smoothingLevel, 0);
        }

        private void Reset()
        {
            line = GetComponent<LineRenderer>();
        }
    }

    [Serializable]
    public class PathSmoother
    {
        [SerializeField, HideInInspector]
        private BezierCurve[] curves;

        public Vector3[] GetSmoothPath(Vector3[] path, float smoothingLength, int smoothingLevel)
        {
            if (path.Length < 3) return path;

            CalculatePath(path, smoothingLength);
            return GetPath(smoothingLevel);
        }

        public void UpdateCurves(Vector3[] path)
        {
            if (curves.IsNullOrEmpty() || curves.Length != path.Length - 1)
            {
                curves = new BezierCurve[path.Length - 1];
                for (int i = 0; i < curves.Length; i++)
                {
                    curves[i] = new BezierCurve();
                }
            }
        }

        private void CalculatePath(Vector3[] path, float smoothingLength)
        {
            if (path.Length < 3) return;

            UpdateCurves(path);

            for (int i = 0; i < curves.Length; i++)
            {
                CalculateCurve(path, i, smoothingLength);
            }

            // apply look-ahead for first curve and retroactively apply the end tangent
            Vector3 nextDirection = (curves[1].points.p3 - curves[1].points.p0).normalized;
            Vector3 lastDirection = (curves[0].points.p3 - curves[0].points.p0).normalized;

            curves[0].points.p2 = curves[0].points.p3 - (lastDirection + nextDirection) * smoothingLength;
        }

        private void CalculateCurve(Vector3[] path, int i, float smoothingLength)
        {
            Vector3 pos = path[i];
            Vector3 lastPos = i == 0 ? path[0] : path[i - 1];
            Vector3 nextPos = path[i + 1];

            Vector3 lastDirection = (pos - lastPos).normalized;
            Vector3 nextDirection = (nextPos - pos).normalized;

            Vector3 startTangeant = (lastDirection + nextDirection) * smoothingLength;
            Vector3 endTangeant = -startTangeant;

            // todo align the end tangeant of each curve with the start tangent of the next curve

            curves[i].points.p0 = pos;                      // start position
            curves[i].points.p1 = pos + startTangeant;      // start tangent
            curves[i].points.p2 = nextPos + endTangeant;    // end tangent
            curves[i].points.p3 = nextPos;                  // end position
        }

        private Vector3[] GetPath(int smoothingLevel)
        {
            Vector3[] smoothPath = new Vector3[curves.Length * smoothingLevel];

            int index = 0;

            for (int i = 0; i < curves.Length; i++)
            {
                Vector3[] segments = curves[i].GetSegments(smoothingLevel);
                for (int j = 0; j < segments.Length; j++)
                {
                    smoothPath[index] = segments[j];
                    index++;
                }
            }

            return smoothPath;
        }

    }
}
