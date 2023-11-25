using FMODUnity;
using Mystie.Gameplay;
using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.Physics
{
    [RequireComponent(typeof(PhysicsBody))]
    public class CarrierController : MonoBehaviour
    {
        [SerializeField] protected PhysicsBody body;
        [SerializeField] protected List<RaycastController> colliders;

        [SerializeField] private LayerMask passengerMask = -1;
        [SerializeField] private float detectRange = 1 / 16;

        private List<Passenger> passengers;
        Dictionary<Transform, PhysicsBody> passengerDictionary;

        public bool showDebug = false;

        protected virtual void Awake()
        {
            if (body == null) body = GetComponent<PhysicsBody>();
            passengerDictionary = new Dictionary<Transform, PhysicsBody>();
        }

        private void OnEnable()
        {
            body.onMoveBefore += (Vector2 moveAmount) => {
                CalculatePassengerMovement(moveAmount);
                MovePassengers(moveAmount, true); //Move passengers before
            };

            body.onMove += (Vector2 moveAmount) => MovePassengers(moveAmount, false); //Move passengers after
        }

        private void OnDisable()
        {
            body.onMoveBefore -= (Vector2 moveAmount) => {
                CalculatePassengerMovement(moveAmount);
                MovePassengers(moveAmount, true); //Move passengers before
            };

            body.onMove -= (Vector2 moveAmount) => MovePassengers(moveAmount, false); //Move passengers after
        }

        protected void MovePassengers(Vector2 moveAmount, bool moveBeforeCarrier)
        {
            foreach (Passenger passenger in passengers)
            {
                if (!passengerDictionary.ContainsKey(passenger._transform))
                {
                    PhysicsBody body = passenger._transform.GetComponent<PhysicsBody>(); //TODO avoid using GetComponent this much
                    if (body != null) passengerDictionary.Add(passenger._transform, body);
                }

                if (passengerDictionary.ContainsKey(passenger._transform) 
                    && passenger._moveBeforeCarrier == moveBeforeCarrier)
                {
                    passengerDictionary[passenger._transform].Move(passenger._moveAmount, passenger._info); 
                }
            }
        }

        //Calculate the movement for passengers
        public Vector2 CalculatePassengerMovement(Vector2 moveAmount)
        {
            //Keep track of moved passengers
            HashSet<Transform> movedPassengers = new HashSet<Transform>(); //Hash set to record the passengers that have already been moved
            passengers = new List<Passenger>(); //List of passengers to move

            //moveAmount = CalculateCarriedPassengers(moveAmount);

            //If the passenger mask is not empty, move passengers
            if (passengerMask.value == 0) return moveAmount;

            float directionX = Math.Sign(moveAmount.x); //Direction of the movement on the x axis
            float directionY = Math.Sign(moveAmount.y); //Direction of the movement on the y axis

            // Vertically moving, push up or down and allow for horizontal movement
            if (directionY != 0) MoveVertical(movedPassengers, moveAmount, directionY);

            // Horizontally moving
            if (directionX != 0) MoveHorizontal(movedPassengers, moveAmount, directionX);

            // Passenger on top while moving horizontally or downward
            if (directionY == -1 || (directionY == 0 && directionX != 0)) 
                MoveHorizontalOrDown(movedPassengers, moveAmount);

            return moveAmount;
        }

        public void MoveVertical(HashSet<Transform> movedPassengers, Vector2 moveAmount, float directionY)
        {
            float rayLength = Mathf.Abs(moveAmount.y) + RaycastController.SKIN_WIDTH; //Get the length of the ray to cast based on the velocity
            rayLength = Mathf.Max(rayLength, 2 * RaycastController.SKIN_WIDTH);

            foreach (RaycastController c in colliders)
            {
                for(int i = 0; i < c.vRayCount; i++)
                {
                    //Set the origin of the ray to be bottom left for movement down, and top left for up
                    Vector2 rayOrigin = (directionY == -1) ? c.RcOrigins.botL : c.RcOrigins.topL; //If moving down, start raycasting from the bottom left, if going up from the top left
                    rayOrigin += (Vector2)transform.right * (c.vRaySpacing * i); //Add offset for the current ray

                    RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, transform.up * directionY, rayLength, passengerMask);

                    if (showDebug)
                        Debug.DrawRay(rayOrigin, transform.up * directionY * rayLength, !hits.IsNullOrEmpty() ? Color.blue : Color.yellow);

                    //If there is a detected collider 
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit && hit.distance != 0 && !movedPassengers.Contains(hit.transform)) //If passenger found and is not inside the collider
                        {
                            movedPassengers.Add(hit.transform); //Add the collider

                            //Calculate the push on the passenger
                            float pushX = (directionY == 1) ? moveAmount.x : 0; //If the carrier is moving up, transfer the x move amount
                            float pushY = moveAmount.y - (hit.distance - RaycastController.SKIN_WIDTH) * directionY;
                            Vector2 pushAmount = new Vector2(pushX, pushY);

                            //If moving up, the passenger is above the platform, if moving down, they're below
                            PassengerInfo info = new PassengerInfo(c.col);
                            info.below = directionY == 1;
                            info.above = directionY == -1;

                            passengers.Add(new Passenger(hit.transform, pushAmount, true, info));
                        }
                    }
                }
            }
        }

        public void MoveHorizontal(HashSet<Transform> movedPassengers, Vector2 moveAmount, float directionX)
        {
            float rayLength = Mathf.Abs(moveAmount.x) + RaycastController.SKIN_WIDTH; //Get the length of the ray to cast based on the velocity
            rayLength = Mathf.Max(rayLength, 2 * RaycastController.SKIN_WIDTH);

            foreach (RaycastController c in colliders)
            {
                for (int i = 0; i < c.hRayCount; i++)
                {
                    //Check push collision

                    //Set the origin of the ray to be bottom left for movement to the left, and bottom left for the right
                    Vector2 rayOrigin = (directionX == -1) ? c.RcOrigins.botL : c.RcOrigins.botR; //If moving left, start raycasting from the bottom left, if going up from the bottom right
                    rayOrigin += (Vector2)transform.up * (c.hRaySpacing * i); //Add offset for the current ray

                    RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, transform.right * directionX, rayLength, passengerMask);

                    if (showDebug)
                        Debug.DrawRay(rayOrigin, transform.right * directionX * rayLength, !hits.IsNullOrEmpty() ? Color.blue : Color.yellow);

                    //If there is a detected collider 
                    foreach (RaycastHit2D hit in hits)
                    {
                        //If passenger found and is not inside the collider
                        if (hit && hit.distance != 0 && !movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);

                            float pushX = moveAmount.x - (hit.distance - RaycastController.SKIN_WIDTH) * directionX;
                            float pushY = 0f; 

                            Vector2 pushAmount = new Vector2(pushX, pushY);

                            PassengerInfo info = new PassengerInfo(c.col);
                            info.right = directionX == -1;
                            info.left = directionX == 1;

                            passengers.Add(new Passenger(hit.transform, pushAmount, true, info));
                        }
                    }
                }
            }
        }

        public void MoveHorizontalOrDown(HashSet<Transform> movedPassengers, Vector2 moveAmount) 
        {
            //Get the length of the ray to cast based on the velocity
            float rayLength = 2 * RaycastController.SKIN_WIDTH;

            foreach (RaycastController c in colliders)
            {
                for (int i = 0; i < c.vRayCount; i++) 
                {
                    Vector2 rayOrigin = c.RcOrigins.topL + transform.right.xy() * (c.vRaySpacing * i); //Add offset for the current ray
                    RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, transform.up, rayLength, passengerMask);

                    if (showDebug)
                        Debug.DrawRay(rayOrigin, transform.up * rayLength, !hits.IsNullOrEmpty() ? Color.blue : Color.yellow);

                    foreach (RaycastHit2D hit in hits)
                    {
                        //If passenger found and is not inside the collider
                        if (hit && hit.distance != 0 && !movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);

                            float pushX = moveAmount.x;
                            float pushY = moveAmount.y;
                            Vector2 pushAmount = new Vector2(pushX, pushY);

                            PassengerInfo info = new PassengerInfo(c.col);
                            info.below = true;

                            passengers.Add(new Passenger(hit.transform, pushAmount, false, info));
                        }
                    }
                }
            }
        }

        protected void Reset()
        {
            body = GetComponent<PhysicsBody>();
            colliders = GetComponentsInChildren<RaycastController>().ToList();
        }


        public struct Passenger
        {
            public Transform _transform;
            public Vector2 _moveAmount;
            public bool _moveBeforeCarrier;
            public PassengerInfo _info;

            public Passenger(Transform transform, Vector2 moveAmount, bool moveBeforeCarrier, PassengerInfo info)
            {
                _transform = transform;
                _moveAmount = moveAmount;
                _moveBeforeCarrier = moveBeforeCarrier;
                _info = info;
            }
        }
    }

    public class PassengerInfo
    {
        public Collider2D col;
        public bool below, above;
        public bool left, right;
        //public bool sliding;
        //public bool hanging;

        public PassengerInfo(Collider2D _col, bool _below = false, bool _above = false, bool _left = false, bool _right = false, bool _sliding = false, bool _hanging = false)
        {
            col = _col;
            below = _below;
            above = _above;
            left = _left;
            right = _right;
            //sliding = _sliding;
            //hanging = _hanging;
        }
    }
}
