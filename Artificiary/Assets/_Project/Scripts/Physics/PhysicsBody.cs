using Mystie.Logic;
using Mystie.UI;
using Mystie.Utils;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.Physics
{
    public class PhysicsBody : RaycastController
    {
        #region Variables

        public Action<Vector2> onMoveBefore;
        public Action<Vector2> onMove;

        #endregion

        public static string oneWayTag = "OneWay";
        public static string throughTag = "PassThrough";

        public LayerMask colMask;
        public float wallAngle = 80f;

        [Space]

        public int faceDir = 1;
        public bool passThrough;
        public float fallThroughTime = 0.5f;

        public CollisionInfo collisions;
        protected List<Collider2D> ignoredColliders = new List<Collider2D>();
        public List<IConstrainer> constraints = new List<IConstrainer>();

        [Header("Debug")]

        public bool debug = false;
        [ShowIf("debug")] public bool debugLines = false;

        public Vector2 Move(Vector2 moveAmount)
        {
            UpdateRaycastOrigins();

            collisions.Reset(); //Reset collision information every frame
            collisions.moveAmountOld = moveAmount;

            ignoredColliders.RemoveAll(item => item == null);

            //If the collision mask is not empty, apply collisions
            if (colMask.value != 0)
            {
                if (moveAmount.y < 0) DescendSlope(ref moveAmount);
                if (moveAmount.x != 0) faceDir = Math.Sign(moveAmount.x);

                HCollisions(ref moveAmount);
                VCollisions(ref moveAmount);

                ApplyConstraints(ref moveAmount);
            }

            onMoveBefore?.Invoke(moveAmount);
            transform.Translate(moveAmount, Space.World); //Move the object

            onMove?.Invoke(moveAmount);

            return moveAmount;
        }

        #region Collisions

        private void HCollisions(ref Vector2 moveAmount)
        {
            //Get the direction in which the ray is being casted
            int dirX = faceDir;
            //Get the length of the ray to cast based on the velocity
            float rayLength = Mathf.Abs(moveAmount.x) + SKIN_WIDTH; 

            //If the move amount is lower than skin width (since we can check horziontal collisions without movement in x)
            if (Mathf.Abs(moveAmount.x) < SKIN_WIDTH)
                rayLength = 2 * SKIN_WIDTH;

            for (int i = 0; i < hRayCount; i++)
            {
                //If moving left, start raycasting from the bottom left, if going right from the bottom right
                Vector2 rayOrigin = (dirX == -1) ? rcOrigins.botL : rcOrigins.botR; 
                rayOrigin += transform.up.xy() * (hRaySpacing * i); //Add offset for the current ray

                RaycastHit2D hit = Physics2D.RaycastAll(rayOrigin, transform.right * dirX, rayLength, colMask).GetHit(rayLength, col);

                if (debug)
                    Debug.DrawRay(rayOrigin, transform.right * dirX * (debugLines ? 1 : SKIN_WIDTH), hit ? Color.green : Color.red); //rayLength

                if (hit)
                {
                    //If hit is inside the collider, ignore to prevent clipping, and check for one-way platforms
                    if (hit.distance == 0 || ignoredColliders.Contains(hit.collider))
                        continue;

                    //Check for one-way platforms
                    if (hit.collider.gameObject.HasTag(oneWayTag))
                    {
                        continue; //Ignore the current collider
                    }

                    float groundAngle = Vector2.Angle(hit.normal, Vector2.up); //Get slope angle of the ground

                    if (i == 0 && groundAngle < wallAngle) //gravitySign <= 0 //If bottom ray and the ground slope is climbable
                        HandleSlope(ref moveAmount, hit, dirX, groundAngle);

                    //Handle horizontal collisions if not climbing a slope
                    if (!collisions.climbingSlope || groundAngle >= wallAngle)
                    {
                        moveAmount.x = (hit.distance - SKIN_WIDTH) * dirX;
                        rayLength = hit.distance; //The ray length is shortened in order to check for closer collisions than the current one

                        //Prevent keeping the y velocity on a slope when colliding with an obstacle
                        if (collisions.climbingSlope)   
                            moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);

                        collisions.slopeNormal = hit.normal;

                        collisions.left = dirX == -1; //If hitting something while going left
                        collisions.right = dirX == 1; //If hitting something while going left

                        //Register current collider
                        if (collisions.left) collisions.colLeft = hit.collider;
                        if (collisions.right) collisions.colRight = hit.collider;
                    }
                }
            }

            for (int i = 0; i < hRayCount; i++)
            {
                //If moving left, start raycasting from the bottom left, if going right from the bottom right
                Vector2 rayOrigin = (-dirX == -1) ? rcOrigins.botL : rcOrigins.botR; 
                rayOrigin += transform.up.xy() * (hRaySpacing * i); //Add offset for the current ray

                RaycastHit2D hit = Physics2D.RaycastAll(rayOrigin, transform.right * -dirX, 2 * SKIN_WIDTH, colMask).GetHit(2 * SKIN_WIDTH, col);

                if (debug)
                    Debug.DrawRay(rayOrigin, transform.right * -dirX * (debugLines ? 1 : SKIN_WIDTH), hit ? Color.green : Color.yellow); //rayLength
            }
        }

        private void VCollisions(ref Vector2 moveAmount)
        {
            float dirY = moveAmount.y > 0? 1 : -1; //Get the direction in which the ray is being casted
            float rayLength = Mathf.Abs(moveAmount.y) + SKIN_WIDTH; //Get the length of the ray to cast based on the velocity

            //If the move amount is lower than skin width (since we can check vertical collisions without movement in y)
            if (Mathf.Abs(moveAmount.y) < SKIN_WIDTH)
                rayLength = 2 * SKIN_WIDTH;

            for (int i = 0; i < vRayCount; i++)
            {
                //If moving down, start raycasting from the bottom left, if going up from the top left
                Vector2 rayOrigin = (dirY == -1) ? rcOrigins.botL : rcOrigins.topL; 
                rayOrigin += transform.right.xy() * (vRaySpacing * i + moveAmount.x); //Add offset for the current ray

                //transform.up * directionY
                RaycastHit2D hit = Physics2D.RaycastAll(rayOrigin, Vector2.up * dirY, rayLength, colMask).GetHit(rayLength, col);

                if (debug)
                    Debug.DrawRay(rayOrigin, Vector2.up * dirY * (debugLines ? 1 : SKIN_WIDTH), hit ? Color.green : Color.red);
                
                if (hit)
                {
                    //Check for one-way platforms
                    if (hit.collider.gameObject.HasTag(oneWayTag))
                    {
                        //If going in the opposite direction to gravity or inside collider
                        if (Mathf.Sign(dirY) > 0 || hit.distance == 0) 
                        {
                            continue; //Ignore the current collider
                        }
                    }

                    //Check for pass-through platforms
                    if (hit.collider.gameObject.HasTag(throughTag))
                    {
                        if (hit.distance == 0 || collisions.fallingThrough) //If falling through and / or inside collider
                        {
                            continue; //Ignore the current collider
                        }
                        if (passThrough)
                        {
                            collisions.fallingThrough = true;
                            Invoke("ResetFallingThrough", fallThroughTime);
                            continue;
                        }
                    }

                    if (ignoredColliders.Contains(hit.collider)) continue; //Ignore the current collider

                    moveAmount.y = (hit.distance - SKIN_WIDTH) * dirY;
                    rayLength = hit.distance; //The ray length is shortened in order to check for closer collisions than the current one

                    //URGENT
                    if (collisions.climbingSlope) //Prevent keeping the x velocity on a slope when colliding with an obstacle upward
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                    }

                    collisions.below = dirY == -1; //If hitting something while going down
                    collisions.above = dirY == 1; //If hitting something while going up
                    
                    //Debug.Log("VCollision hit " + dirY);

                    //Register current collider
                    if (collisions.below) collisions.colBelow = hit.collider;
                    if (collisions.above) collisions.colAbove = hit.collider;
                }
            }

            //Fix clipping when transitionning between slopes with different angles
            if (collisions.climbingSlope)
            {
                //Fire a horizontal ray on the y axis at the expected position after moving to check for a slope at that height
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + SKIN_WIDTH;
                Vector2 rayOrigin = ((directionX == -1) ? rcOrigins.botL : rcOrigins.botR) + Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, colMask);

                if (hit)
                {
                    float groundSlopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    //If the angle is different at the next position, it means it's a new slope
                    if (groundSlopeAngle != collisions.slopeAngle)
                    {
                        moveAmount.x = (hit.distance - SKIN_WIDTH) * directionX;
                        collisions.slopeAngle = groundSlopeAngle;
                    }
                }
            }
        }

        #endregion

        void ApplyConstraints(ref Vector2 moveAmount)
        {
            foreach (IConstrainer constraint in constraints)
                constraint.ApplyConstraint(ref moveAmount);
        }

        #region Slopes

        void HandleSlope(ref Vector2 moveAmount, RaycastHit2D hit, int directionX, float slopeAngle)
        {
            //If climbing slope, cancel the change in y move amount from descending slope
            if (collisions.descendingSlope)
            {
                collisions.descendingSlope = false;
                moveAmount = collisions.moveAmountOld;
            }

            float distanceToSlopeStart = 0;
            if (slopeAngle != collisions.slopeAngleOld) //If we're starting to climb a new slope
            {
                distanceToSlopeStart = hit.distance - SKIN_WIDTH;
                //We substract the distance to the slope so that when we call the ClimbSlope it only uses the velocity x once it reaches the slope
                moveAmount.x -= distanceToSlopeStart * directionX; 
            }
            ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
            collisions.colBelow = hit.collider; //If climbing a slope, set the slope as the ground collider
            moveAmount.x += distanceToSlopeStart * directionX;
        }

        void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
        {
            float moveDistance = Mathf.Abs(moveAmount.x);
            float climbMoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (moveAmount.y <= climbMoveAmountY)
            {
                moveAmount.y = climbMoveAmountY;
                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                collisions.below = true; //Set collion below to true since a slope is being climbed
                collisions.climbingSlope = true;
                collisions.slopeAngle = slopeAngle;
                collisions.slopeNormal = slopeNormal;
            }
        }

        void DescendSlope(ref Vector2 moveAmount)
        {
            //if (!collisions.slidingDownMaxSlope) {
            float directionX = Mathf.Sign(moveAmount.x);

            //If moving to the left, contact point is bottom right and vice versa
            Vector2 rayOrigin = (directionX == -1) ? rcOrigins.botR : rcOrigins.botL;

            //Cast a ray down to check for slopes
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -transform.up, Mathf.Infinity, colMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, transform.up);
                //If not on flat terrain and slope is not too steep
                if (slopeAngle != 0 && slopeAngle <= wallAngle)
                {
                    //Hit normal is perpendicular to the slope, so its x component tells us the direction of the slope
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    { //If moving down the slope
                      //Check if the distance to slope is shorter to the necessary move amount on the y axis to descend it based on its angle
                        if (hit.distance - SKIN_WIDTH <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendMoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendMoveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true; //Set collion below to true since a slope is being descended
                            collisions.colBelow = hit.collider;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }

        void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
        {
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, transform.up);
                if (slopeAngle > wallAngle)
                {
                    moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                    collisions.slopeAngle = slopeAngle;
                    collisions.slidingDownMaxSlope = true;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }

        #endregion

        void ResetFallingThrough()
        {
            collisions.fallingThrough = false;
        }

        //Holds information about current collisions
        public struct CollisionInfo
        {
            public bool below, above; //Is colliding down / up
            public bool left, right; //Is colliding left / right

            public bool climbingSlope, descendingSlope, slidingDownMaxSlope;
            public float slopeAngle, slopeAngleOld; //Previous and current ground slope
            public Vector2 slopeNormal;

            public Vector2 moveAmountOld;
            public bool fallingThrough;

            //Keep track of collided objects
            public Collider2D colAbove, colBelow;
            public Collider2D colLeft, colRight;

            public void Reset()
            {
                above = below = false;
                left = right = false;

                climbingSlope = false;
                descendingSlope = false;
                slidingDownMaxSlope = false;
                slopeAngleOld = slopeAngle;
                slopeAngle = 0f;
                slopeNormal = Vector2.zero;

                colAbove = colBelow = null;
                colLeft = colRight = null;
            }
        }

        private void Reset()
        {
            col = GetComponent<Collider2D>();
        }

#if UNITY_EDITOR

        private void OnGUI()
        {
            if (debug && Selection.Contains(gameObject))
            {
                GUIStyle style = new GUIStyle();
                float ratio = UIManager.ScreenScale;
                //float pixelRatio = UI.CanvasScaleAdjuster.pixelRatio;
                style.fontSize = (int)Math.Ceiling(10 * ratio); 
                style.normal.textColor = Color.white;

                Vector2 size = new Vector2(100f, 20f) * ratio;
                Vector2 anchor = new Vector2(150f, 10f) * ratio;
                Vector2 gap = new Vector2(0f, 25f) * ratio;

                //RectData rect = new RectData(200f, 40f, Screen.width - (300f), 20f, 0f, 50f);
                RectData rect = new RectData(size.x, size.y, Screen.width - anchor.x, anchor.y, gap.x, gap.y);

                GUI.Label(rect.GetRect(0), "Below: " + collisions.below, style);
                GUI.Label(rect.GetRect(1), "Above: " + collisions.above, style);
                GUI.Label(rect.GetRect(2), "Climbing Slope: " + collisions.climbingSlope, style);
                GUI.Label(rect.GetRect(3), "Descending Slope: " + collisions.descendingSlope, style);
                GUI.Label(rect.GetRect(4), "Slope Angle: " + collisions.slopeAngle, style);
                GUI.Label(rect.GetRect(5), "At wall: " + (collisions.left | collisions.right), style);
            }
        }

#endif
    }
}
