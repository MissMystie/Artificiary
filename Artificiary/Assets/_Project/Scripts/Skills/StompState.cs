using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public class StompState : BaseState
    {
        private Skill skill;

        public StompState(StateManager ctx, Skill skill) : base(ctx) 
        {
            this.skill = skill;
        }

        public override void EnterState()
        {
        }

        public override void ExitState()
        {
        }

        public override void UpdateState(float deltaTime)
        {
        }

        public override void UpdatePhysics(float deltaTime)
        {
        }

        public override bool CheckStateTransitions()
        {
            return false;
        }

        public override void Animate(float deltaTime)
        {
        }

        public override string ToString() { return "Skill: " + skill.name; }
    }
}
