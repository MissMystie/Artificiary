using Mystie.Core;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mystie.Utils
{
    public class GizmosDrawer : MonoBehaviour
    {
        public enum Shape { CIRCLE, RECTANGLE, SPHERE, BOX, CROSS, LINE, ARC, COLLIDER }
        public Shape shape = Shape.CIRCLE;

        public bool scaleToTransform = true;
        public Vector2 offset = Vector2.zero;

        [ShowIf("shape", Shape.RECTANGLE)] public Vector2 dimensions = Vector2.one;
        [ShowIf("shape", Shape.CIRCLE)] public float radius = 5f;
        [ShowIf("shape", Shape.ARC)] public Vector2 arcDir = Vector2.right;
        [ShowIf("shape", Shape.ARC)] [MinMaxSlider(0,360)] public float arcLength = 90f;
        [ShowIf("shape", Shape.COLLIDER)] public new Collider2D collider;

        [SerializeField] protected Color color = new Color(1, 0, 1, 5f / 255);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = color;

            Vector2 scale = scaleToTransform? transform.lossyScale : Vector2.one;

            switch (shape)
            {
                case Shape.CIRCLE:
                    Gizmos.DrawWireSphere(transform.position.xy() + offset, radius * scale.x);
                    break;
                case Shape.RECTANGLE:
                    Gizmos.DrawWireCube(transform.position.xy() + offset, dimensions * scale);
                    break;
                case Shape.SPHERE:
                    Gizmos.DrawSphere(transform.position.xy() + offset, radius * scale.x);
                    break;
                case Shape.BOX:
                    Gizmos.DrawCube(transform.position.xy() + offset, dimensions * scale);
                    break;
                case Shape.LINE:
                    break;
                case Shape.ARC:
                    GizmosExtensions.DrawWireArc(transform.position.xy() + offset, arcDir, arcLength, radius * scale.x);
                    break;
                case Shape.COLLIDER:
                    DrawColliderGizmos(collider);
                    break;
            }
        }
#endif

        private void DrawColliderGizmos(Collider2D collider)
        {
            if (collider == null) return;
            System.Type type = collider.GetType();

            Vector2 scale = collider.transform.lossyScale;

            if (type == typeof(BoxCollider2D))
            {
                BoxCollider2D boxCol = collider as BoxCollider2D;
                Vector2 size = new Vector2(scale.x * boxCol.size.x, scale.y * boxCol.size.y);
                if (collider.enabled) Gizmos.DrawCube(boxCol.bounds.center, size);
                else Gizmos.DrawWireCube(boxCol.bounds.center, size);
            }
            else if (type == typeof(CircleCollider2D))
            {
                CircleCollider2D circleCol = collider as CircleCollider2D;
                if (circleCol.enabled) Gizmos.DrawSphere(circleCol.bounds.center, circleCol.radius * scale.x);
                else Gizmos.DrawWireSphere(circleCol.bounds.center, circleCol.radius * scale.x);
                Gizmos.DrawWireSphere(transform.position.xy() + offset, radius * scale.x);
            }
            else if (type == typeof(PolygonCollider2D))
            {
                PolygonCollider2D polygonCol = collider as PolygonCollider2D;
                if (polygonCol != null) DrawPolygonColliderGizmos(polygonCol);
            }
            else
            {
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }

        private void DrawPolygonColliderGizmos(PolygonCollider2D polygonCol)
        {
            List<Vector2[]> pointsList = new List<Vector2[]>();
            List<Vector3> _tList = new List<Vector3>();

            Vector2[] pointWithOffset = new Vector2[polygonCol.points.Length];

            for (int i = 0; i < polygonCol.points.Length; i++)
            {
                pointWithOffset[i] = polygonCol.points[i] + polygonCol.offset;
            }

            pointsList.Add(pointWithOffset);
            _tList.Add(polygonCol.transform.position);


            for (int j = 0; j < pointsList.Count; j++)
            {

                Vector2[] points = pointsList[j];
                Vector3 _t = _tList[j];

                for (int i = 0; i < points.Length - 1; i++)
                {
                    Gizmos.DrawLine(new Vector3(points[i].x + _t.x, points[i].y + _t.y), new Vector3(points[i + 1].x + _t.x, points[i + 1].y + _t.y));
                }

                Gizmos.DrawLine(new Vector3(points[points.Length - 1].x + _t.x, points[points.Length - 1].y + _t.y), new Vector3(points[0].x + _t.x, points[0].y + _t.y));

            }
        }
    }

}
