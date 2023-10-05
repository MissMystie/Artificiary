using FMODUnity;
using Mystie.Core;
using Mystie.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "CustomData/Skill/Projectile", order = 1)]
    public class Projectile : Skill
    {
        public PhysicsObject projectile;
        public float strength = 20;

        public AimType aimType = AimType.FREEAIM;
        public enum AimType { FREEAIM, MOVE, FIXED }

        public float fixedAngle = 0f;

        [Header("Recoil")]

        public float recoilStrength = 5f;
        public bool recoilOverridesVelocity = false;

        public override void OnStart(Entity entity) { }

        public override void OnEnd(Entity entity) { }

        public override void OnHitFrameStart(Entity entity)
        {
            Vector2 velocity = Vector2.zero;

            switch (aimType)
            {
                case AimType.FREEAIM: 
                    velocity = entity.Controller.aim; 
                    break;
                case AimType.MOVE: 
                    velocity = entity.Controller.move;
                    break;
                default: 
                    velocity = Vector2.right.Rotate(fixedAngle);
                    if (entity.Phys != null) 
                        velocity.x *= entity.Phys.faceDir;
                    break;
            }

            velocity = velocity.normalized * strength;
            Emit(projectile, entity.transform.position, velocity, entity);
        }

        public override void OnHitFrameEnd(Entity entity) { }

        public void Emit(PhysicsObject projectile, Vector2 throwPoint, Vector2 velocity, Entity emitter = null)
        {
            PhysicsObject instance = EmitProjectile(projectile, throwPoint, velocity, emitter);

            if (instance != null)
            {
                IEmittable[] emittables = instance.GetComponents<IEmittable>();
                foreach (IEmittable emittable in emittables)
                    emittable.Emit(velocity, emitter);
            }

            Recoil(emitter, velocity);

            RuntimeManager.PlayOneShot(sfx, throwPoint);
        }

        public PhysicsObject EmitProjectile(PhysicsObject projectile, Vector2 pos, Vector2 shootV, Entity emitter)
        {
            if (projectile != null)
            {
                PhysicsObject physObj = Instantiate(projectile.gameObject, pos, Quaternion.identity).GetComponent<PhysicsObject>();
                physObj.transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, shootV.normalized));
                physObj.SetVelocity(shootV);
                return physObj;
            }

            return null;
        }

        public void Recoil(Entity entity, Vector2 aim)
        {
            Vector2 recoilVelocity = -aim.normalized * recoilStrength;
            Vector2 velocity = entity.Phys.rb.velocity;

            if (recoilOverridesVelocity)
            {
                if (recoilVelocity.x == 0 || 
                    (Mathf.Sign(velocity.x) == Mathf.Sign(recoilVelocity.x) 
                    && Math.Abs(velocity.x) > Math.Abs(recoilVelocity.x)))
                    recoilVelocity.x = velocity.x;

                if (recoilVelocity.y == 0 || 
                    (Mathf.Sign(velocity.y) == Mathf.Sign(recoilVelocity.y) 
                    && Math.Abs(velocity.y) > Math.Abs(recoilVelocity.y)))
                    recoilVelocity.y = velocity.y;

                entity.Phys.SetVelocity(recoilVelocity);
            }
            else
            {
                entity.Phys.AddVelocity(recoilVelocity);
            }
        }
    }
}
