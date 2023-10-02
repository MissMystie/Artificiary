using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace Mystie
{
    public class HitBox : ColBox
    {
        public event Action<Collider2D[]> onCollision;

        public LayerMask mask;
        public HitboxState state;

        protected ColInfo colInfo = new ColInfo();

        [Space]

        [SerializeField] private Color inactiveColor = Color.gray;
        [SerializeField] private Color openColor = Color.yellow;
        [SerializeField] private Color collidingColor = Color.red;

        [Space]

        public bool showDebug = true;

        public void FixedUpdate()
        {
            HitboxUpdate();
        }

        public void HitboxUpdate()
        {
            if (state == HitboxState.Closed)
                return;

            Collider2D[] colliders = new Collider2D[0];

            if (colInfo.isCircle)
                colliders = Physics2D.OverlapCircleAll(transform.position + colInfo.offset, colInfo.radius, mask);
            else
                colliders = Physics2D.OverlapBoxAll(transform.position + colInfo.offset, colInfo.size, colInfo.rotation, mask);

            state = colliders.Length > 0 ? HitboxState.Colliding : HitboxState.Open;

            if (state == HitboxState.Colliding) onCollision?.Invoke(colliders);
        }

        public void SetColInfo(ColInfo info)
        {
            colInfo = info;
        }

        public void StartCheckingCol()
        {
            state = HitboxState.Open;
        }

        public void StopCheckingCol()
        {
            state = HitboxState.Closed;
        }

        protected void SetGismosColor()
        {
            switch (state)
            {
                case HitboxState.Closed:
                    Gizmos.color = inactiveColor;
                    break;
                case HitboxState.Open:
                    Gizmos.color = openColor;
                    break;
                case HitboxState.Colliding:
                    Gizmos.color = collidingColor;
                    break;
            }
        }

        protected void OnDrawGizmos()
        {
            if (!showDebug) return;

            SetGismosColor();

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

            if (colInfo.isCircle)
                Gizmos.DrawWireSphere(colInfo.offset, colInfo.radius);
            else
            {
                Vector2 colBox = new Vector2(colInfo.size.x, colInfo.size.y); //Because size is halfExtents
                Gizmos.DrawWireCube(colInfo.offset, colBox);
            }
        }

        [System.Serializable]
        public class ColInfo
        {
            public bool isCircle = false;
            public Vector3 offset = Vector3.zero;
            public Vector2 size = Vector2.one;
            public float rotation;
            public float radius = 1f;

            public ColInfo(ColInfo colInfo = null)
            {
                if (colInfo == null) return;

                isCircle = colInfo.isCircle;
                offset = colInfo.offset;
                size = colInfo.size;
                rotation = colInfo.rotation;
                radius = colInfo.radius;
            }
        }

        public enum HitboxState { Closed, Open, Colliding }
    }
}
