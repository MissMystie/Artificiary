using FMODUnity;
using Mystie.Core;
using Mystie.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    [CreateAssetMenu(fileName = "Ranged Weapon", menuName = "CustomData/Weapon/Ranged Weapon", order = 1)]
    public class RangedWeapon : Gear
    {
        public ShootMode mode = ShootMode.SEMIAUTO;
        public enum ShootMode { SEMIAUTO, AUTO, RELEASE, TRIGGER, RELEASE_TRIGGER };

        public float cdTime = 0.5f;
        public float chargeTime = 0.5f;

        [Header("Recoil")]

        public float recoilStrength = 5f;
        public bool recoilOverridesVelocity = false;

        [Header("Projectiles")]

        public Projectile projectile;
        public Projectile chargedProjectile;

        public override void Use(Entity entity)
        {
            if (entity == null) return;

            //if (shot != null) shot.Use(entity);
        }

        public override void Release(Entity entity)
        {
            if (entity == null) return;

            if (projectile != null) projectile.Use(entity);
        }

        public override void Reset()
        {
            base.Reset();
            name = "Ranged Weapon";
        }
    }
}
