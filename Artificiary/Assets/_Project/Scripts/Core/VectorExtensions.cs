using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie.Core
{
    public static class VectorExtension
    {
        public static Vector2 xy(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static float Angle(this Vector2 v)
        {
            return Vector2.SignedAngle(Vector2.up, v);
        }

        public static float Angle(this Vector2 v, Vector2 w)
        {
            return Vector2.SignedAngle(w, v);
        }

        public static Vector2 Mirror(this Vector2 v)
        {
            return new Vector2(-v.x, -v.y);
        }

        public static Vector2 MirrorX(this Vector2 v)
        {
            return new Vector2(-v.x, v.y);
        }

        public static Vector2 mirrorY(this Vector2 v)
        {
            return new Vector2(v.x, -v.y);
        }

        public static Vector2 SetLength(this Vector2 v, float l)
        {
            return v.normalized * l;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;

            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);

            return v;
        }

        public static List<Vector2> OffsetPos(this IList<Vector2> v2, Vector2 offset)
        {
            List<Vector2> v = new List<Vector2>(v2);
            for (int i = 0; i < v.Count(); i++)
                v[i] += offset;

            return v;
        }

        public static List<Vector3> OffsetPos(this IList<Vector3> v3, Vector3 offset)
        {
            List<Vector3> v = new List<Vector3>(v3);
            for (int i = 0; i < v.Count(); i++)
                v[i] += offset;

            return v;
        }

        public static List<Vector2> ToV2(this IList<Vector3> v3)
        {
            List<Vector2> v2 = new List<Vector2>();
            for (int i = 0; i < v3.Count(); i++)
                v2.Add(v3[i]);

            return v2;
        }

        public static List<Vector2> GetArc(this Vector2 dir, float arcLength, float radius, float maxSteps = 20)
        {
            List<Vector2> arcPoints = new List<Vector2>();

            float angle = -dir.Angle() - arcLength / 2;
            float stepAngle = arcLength / maxSteps;

            for (int i = 0; i <= maxSteps; i++)
            {
                float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                arcPoints.Add(new Vector2(x, y));

                angle += stepAngle;
            }

            return arcPoints;
        }
    }
}
