using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    [CreateAssetMenu(fileName = "Melee Weapon", menuName = "CustomData/Weapon/Melee Weapon", order = 0)]
    public class MeleeWeapon : Gear
    {
        [Header("Moveset")]

        public Moveset standard;
        public Moveset charged;

        public override void Use(Entity entity)
        {
            Skill skill = SelectSkill(entity, entity.Controller.move);
            if (skill) skill.Use(entity);
        }

        public override void Release(Entity entity)
        {
        }

        public Skill SelectSkill(Entity entity, Vector2 input)
        {
            Moveset moveset = standard;

            if (moveset == null) return null;

            return moveset.GetSkill(moveset.GetMoveset(), input);
        }

        public override void Reset()
        {
            name = "Melee Weapon";
        }

        [System.Serializable]
        public class Moveset
        {
            public Skills regular;

            public Skill[] GetMoveset()
            {
                Skill[] skills = new Skill[3];

                skills[(int)Dir.FRONT] = regular.frontSkill;
                skills[(int)Dir.UP] = regular.upSkill;
                skills[(int)Dir.DOWN] = regular.downSkill;

                return skills;
            }

            public Skill GetSkill(Skill[] skills, Vector2 input)
            {
                // if there is no skill in the skill pool, return null
                if (skills.IsNullOrEmpty()) return null;

                Skill skill = null;

                int dir = (int)Dir.FRONT;
                if (input.y >= 0.5) dir = (int)Dir.UP;
                else if (input.y <= -0.5) dir = (int)Dir.DOWN;
                // TODO: use dominant input

                int index = dir;

                // if there is no skill, use the default
                if (skills[dir] == null)
                    index = 0;

                if (index < skills.Length)
                {
                    if (skills[index] != null)
                        skill = skills[index];
                }

                return skill;
            }

            [System.Serializable]
            public class Skills
            {
                public Skill frontSkill;
                public Skill upSkill;
                public Skill downSkill;
            }
        }

        public enum Dir { FRONT, UP, DOWN }
    }
}
