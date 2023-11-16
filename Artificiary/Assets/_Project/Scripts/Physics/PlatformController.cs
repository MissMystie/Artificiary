using Mystie.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie.Physics
{
    public abstract class PlatformController : MonoBehaviour
    {
        [SerializeField] protected PhysicsBody body;
        [SerializeField] protected List<Collider2D> colliders;

        protected void Awake()
        {
            body = GetComponentInChildren<PhysicsBody>();
            colliders = GetComponentsInChildren<Collider2D>().ToList();
        }

        protected void FixedUpdate()
        {
            Vector2 moveAmount = GetMoveAmount(Time.deltaTime);
            //transform.Translate(moveAmount, Space.World); //Move the object
            body.Move(moveAmount);
        }

        protected abstract Vector2 GetMoveAmount(float deltaTime);

        protected void Reset()
        {
            body = GetComponent<PhysicsBody>();
            if (body != null) colliders = body.GetComponents<Collider2D>().ToList();
            else colliders = GetComponents<Collider2D>().ToList();
        }
    }
}
