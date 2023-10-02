using FMODUnity;
using Mystie.Core;
using Mystie.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mystie.HitBox;

namespace Mystie.Gameplay
{
    [CreateAssetMenu(fileName = "Attack", menuName = "CustomData/Skill/Melee Attack", order = 0)]
    public class Attack : Skill
    {
        [Header("Damage")]

        public Damage dmg = new Damage();
        public ColInfo hitbox;
        public Force knocback;

        [Header("Recoil")]
        public Vector2 recoil;
        public string[] recoilIgnoreTags;
        public bool recoilOverrideX = true, recoilOverrideY = true;

        [Header("Feedback")]

        public EventReference hitSfx;

        public override SkillState Initiate(Entity entity)
        {
            if (entity == null) return null;

            AttackState attackState = new AttackState(entity.StateManager, this);
            entity.StateManager.SetState(attackState);
            return attackState;
        }

        public override void OnStart(Entity entity)
        {
            ColInfo colInfo = new ColInfo(hitbox);
            colInfo.offset.x *= Math.Sign(entity.Phys.faceDir);
            entity.HitBox?.SetColInfo(colInfo);
        }

        public override void OnHitFrameStart(Entity entity)
        {

        }

        public override void OnHitFrameEnd(Entity entity)
        {

        }

        public override void OnEnd(Entity entity)
        {

        }
    }
}
