using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Physics
{
    internal static class Physics2DExtension
    {
        //Return the first valid collision in a list of hits
        public static RaycastHit2D GetHit(this RaycastHit2D[] hits, float rayLength, Collider2D col)
        {
            RaycastHit2D validHit = new RaycastHit2D();
            float hitDistance = rayLength;

            foreach (RaycastHit2D hit in hits)
                if (hit.collider != col && hit.distance < hitDistance)
                {
                    validHit = hit;
                    hitDistance = hit.distance;
                }

            return validHit;
        }
    }
}
