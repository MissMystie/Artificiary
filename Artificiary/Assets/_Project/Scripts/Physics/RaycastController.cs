using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Physics
{
    public class RaycastController : MonoBehaviour
    {
        public Collider2D col;

        //Calculated in the CalculateRaySpacing method
        protected int hRayCount;
        protected int vRayCount;
        protected float hRaySpacing;
        protected float vRaySpacing;
        protected RaycastOrigins rcOrigins;

        public const float SKIN_WIDTH = 1/16f;
        public const float RAY_DST = 1/4f;

        protected virtual void Awake()
        {
            if (col == null) col = GetComponent<Collider2D>();
            CalculateRaySpacing();
            UpdateRaycastOrigins();
        }

        public void CalculateRaySpacing()
        {
            Bounds bounds = col.bounds;
            bounds.Expand(SKIN_WIDTH * -2);

            float width = bounds.size.x; 
            float height = bounds.size.y; 

            vRayCount = Math.Max(2, Mathf.RoundToInt(width / RAY_DST));
            hRayCount = Math.Max(2, Mathf.RoundToInt(height / RAY_DST));

            //Get the space between rays
            vRaySpacing = width / (vRayCount - 1);
            hRaySpacing = height / (hRayCount - 1);
        }

        protected void UpdateRaycastOrigins()
        {
            Bounds bounds = col.bounds;
            bounds.Expand(SKIN_WIDTH * -2);

            rcOrigins.botL = new Vector2(bounds.min.x, bounds.min.y);
            rcOrigins.botR = new Vector2(bounds.max.x, bounds.min.y); 
            rcOrigins.topL = new Vector2(bounds.min.x, bounds.max.y);
            rcOrigins.topR = new Vector2(bounds.max.x, bounds.max.y);
        }

        //Contains the coordinates of the corners of the collider
        public struct RaycastOrigins
        {
            public Vector2 topL, topR;
            public Vector2 botL, botR;
        }
    }
}
