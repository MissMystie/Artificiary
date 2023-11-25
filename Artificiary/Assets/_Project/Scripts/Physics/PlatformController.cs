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
        [SerializeField] protected Collider2D col;

        protected void Awake()
        {
            body = GetComponentInChildren<PhysicsBody>();
            col = GetComponentInChildren<Collider2D>();
        }

        protected void FixedUpdate()
        {
            Vector2 moveAmount = GetMoveAmount(Time.deltaTime);
            body.Move(moveAmount);
        }

        protected abstract Vector2 GetMoveAmount(float deltaTime);

        protected void Reset()
        {
            body = GetComponent<PhysicsBody>();
            if (body != null) col = body.GetComponentInChildren<Collider2D>();
            else col = GetComponentInChildren<Collider2D>();
        }
    }
}
