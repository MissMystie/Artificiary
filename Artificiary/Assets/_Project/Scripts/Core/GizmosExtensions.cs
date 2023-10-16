using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Utils
{
    public static class GizmosExtensions
    {
        /// <summary>
        /// Draws a cross.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="length"></param>
        public static void DrawCross(Vector3 pos, float length)
        {
            Gizmos.DrawLine(new Vector2(pos.x - length / 2, pos.y), new Vector2(pos.x + length / 2, pos.y));
            Gizmos.DrawLine(new Vector2(pos.x, pos.y - length / 2), new Vector2(pos.x, pos.y + length / 2));
        }

        /// <summary>
        /// Draws an arrow.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="arrowHeadLength"></param>
        /// <param name="arrowHeadAngle"></param>
        /// <param name="arrowPosition"></param>
        public static void DrawArrow(Vector3 from, Vector3 to, float arrowPosition = 1f, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawLine(from, to);
            var dir = to - from;
            DrawArrowEnd(from, dir, arrowPosition, arrowHeadLength, arrowHeadAngle);
        }

        /// <summary>
        /// Draws an arrow.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="dir"></param>
        /// <param name="arrowHeadLength"></param>
        /// <param name="arrowHeadAngle"></param>
        /// <param name="arrowPosition"></param>
        public static void DrawArrowRay(Vector3 from, Vector3 dir, float arrowPosition = 1f, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(from, dir);
            DrawArrowEnd(from, dir, arrowPosition, arrowHeadLength, arrowHeadAngle);
        }

        private static void DrawArrowEnd(Vector3 from, Vector3 dir, float arrowPosition = 1f, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Vector3 right = Quaternion.LookRotation(dir) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation(dir) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 up = Quaternion.LookRotation(dir) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
            Vector3 down = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

            Vector3 arrowTip = from + (dir * arrowPosition);
            Gizmos.DrawRay(arrowTip, right * arrowHeadLength);
            Gizmos.DrawRay(arrowTip, left * arrowHeadLength);
            Gizmos.DrawRay(arrowTip, up * arrowHeadLength);
            Gizmos.DrawRay(arrowTip, down * arrowHeadLength);
        }

        public static void DrawPolygon(Vector2[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                Gizmos.DrawLine(points[i], points[j]);
            }
        }

        public static void DrawPolygon(PolygonCollider2D polygonCol)
        {
            DrawPolygon(polygonCol.points);
        }

        /// <summary>
        /// Draws a wire arc.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir">The direction from which the arc length is taken into account.</param>
        /// <param name="arcLength">The arc length, in degrees.</param>
        /// <param name="radius"></param>
        /// <param name="maxSteps">How many steps to use to draw the arc.</param>
        public static void DrawWireArc(Vector3 pos, Vector2 dir, float arcLength, float radius, float maxSteps = 20)
        {
            List<Vector2> arcPoints = dir.GetArc(arcLength, radius, maxSteps);

            for (int i = 0; i < arcPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(pos.xy() + arcPoints[i], pos.xy() + arcPoints[i + 1]);
            }
        }
    }
}
