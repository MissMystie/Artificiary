using Mystie.Core;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class LineController : MonoBehaviour
    {
        [SerializeField] protected LineRenderer lineRenderer;
        [SerializeField] protected EdgeCollider2D col;

        [Space]

        [SerializeField] protected float width = 0.05f;
        
        public enum CoordinateMode { GLOBAL, LOCAL, ANCHORS }
        [SerializeField] public CoordinateMode coordinateMode = CoordinateMode.LOCAL;

        [ShowIf("coordinateMode", CoordinateMode.GLOBAL)] 
        public List<Vector3> positions = new List<Vector3>();

        [ShowIf("coordinateMode", CoordinateMode.LOCAL)] 
        public List<Vector3> localPositions = new List<Vector3>();

        [ShowIf("coordinateMode", CoordinateMode.ANCHORS)]
        public List<Transform> anchors = new List<Transform>();

        [Header("Smoothing")]

        [SerializeField] private bool applySmoothing = false;
        [SerializeField, ShowIf("applySmoothing")] 
        private float smoothingLength = 2f;
        [SerializeField, ShowIf("applySmoothing")] 
        private int smoothingLevel = 10;
        
        private PathSmoother smoother = new PathSmoother();

        protected Material mat;

        private void Awake()
        {
            if (lineRenderer == null) lineRenderer = GetComponentInChildren<LineRenderer>();
            if (col == null) col = GetComponent<EdgeCollider2D>();

            if (lineRenderer != null) mat = lineRenderer.material;
        }

        void Update()
        {
            // disable if one of the anchors is null

            bool enabled = true;

            /*
            foreach (Transform a in anchors)
            {
                if (a == null || !a.gameObject.activeInHierarchy)
                {
                    enabled = false;
                    break;
                }
            }*/

            if (lineRenderer != null) lineRenderer.enabled = enabled;
            if (col != null) col.enabled = enabled;

            UpdateLine();
        }

        public void UpdateLine()
        {
            Vector3[] positions = UpdatePositions();
            if (applySmoothing)
            {
                positions = smoother.GetSmoothPath(positions, smoothingLength, smoothingLevel);
            }

            if (lineRenderer) DrawLine(positions);
            if (col) UpdateCollider(positions);
        }

        public virtual Vector3[] UpdatePositions()
        {
            switch (coordinateMode)
            {
                case CoordinateMode.LOCAL:

                    positions = new List<Vector3>();

                    foreach (Vector3 pos in localPositions)
                    {
                        positions.Add(transform.position + pos);
                    }

                    break;
                case CoordinateMode.ANCHORS:

                    positions = new List<Vector3>();

                    if (!anchors.IsNullOrEmpty() && anchors.Count >= 2)
                    {
                        foreach (Transform point in anchors)
                        {
                            if (point) positions.Add(point.position);
                        }
                    }

                    break;
            }

            return positions.ToArray();
        }

        public virtual void DrawLine(Vector3[] positions)
        {
            if (lineRenderer == null) return;

            //lineRenderer.startWidth = width;
            //lineRenderer.endWidth = width;

            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }

        public void UpdateCollider(Vector3[] positions)
        {
            if (col == null) return;

            List<Vector2> points = new List<Vector2>();
            foreach (Vector3 pos in positions)
            {
                Vector2 point = pos - transform.position;
                point = point.Rotate(-col.transform.eulerAngles.z);
                points.Add(point);
            }

            col.SetPoints(points);
            //col.SetPoints(positions.OffsetPos(-transform.position).ToV2());

            //col.edgeRadius = width / 2;

            // todo - use local positions
        }

        public void AddAnchor(Transform newAnchor)
        {
            anchors.Add(newAnchor);
        }

        public void SetMat(Material newMat)
        {
            if (lineRenderer != null) lineRenderer.material = newMat;
        }

        public void ResetMat()
        {
            if (lineRenderer != null) lineRenderer.material = mat;
        }

        private void OnValidate()
        {
            smoothingLevel = Math.Max(smoothingLevel, 0);
        }

        protected virtual void Reset()
        {
            positions = new List<Vector3>();
            localPositions = new List<Vector3>();
            anchors = new List<Transform>();

            lineRenderer = GetComponent<LineRenderer>();
            col = GetComponent<EdgeCollider2D>();
        }
    }
}
