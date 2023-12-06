using FMODUnity;
using Mystie.Animation;
using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public abstract class Skill : ScriptableObject
    {
        [Header("Animation")]

        //public AnimClip.Info animInfo;
        public AnimationClip clip;
        public string animStateName;
        public int layerNumber = 0;

        public int hitFrameStart = 0;
        public int hitFrameEnd = 1;

        [Header("Feedback")]

        public EventReference sfx;

        public virtual void Use(Entity entity)
        {
            if (entity && entity.SkillManager)
                entity.SkillManager.QueueSkill(this);
        }

        public AnimClip GetAnimClip(Animator anim)
        {
            return new AnimClip(anim, clip, animStateName, layerNumber);
        }

        public virtual SkillState Initiate(Entity entity)
        {
            if (entity == null) return null;

            RuntimeManager.PlayOneShot(sfx, entity.transform.position);

            SkillState skillState = new SkillState(entity.StateManager, this);
            entity.StateManager.SetState(skillState);
            return skillState;
        }

        public abstract void OnStart(Entity entity);

        public abstract void OnEnd(Entity entity);

        public abstract void OnHitFrameStart(Entity entity);

        public abstract void OnHitFrameEnd(Entity entity);
    }
}
