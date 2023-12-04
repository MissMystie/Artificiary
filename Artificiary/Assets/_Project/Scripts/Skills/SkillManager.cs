using Mystie.Core;
using Mystie.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie.Gameplay
{
    public class SkillManager : MonoBehaviour
    {
        #region Events

        public event Action<Gear, int> onEquip;

        #endregion

        #region Components

        public Entity entity { get; private set; }
        public InputController controller;
        //public Animator anim { get; private set; }

        #endregion

        public Gear weapon;
        public Gear[] abilities = new Gear[2];

        private bool isUsingSkill;
        private SkillState skillState;
        private Skill nextSkill = null;

        protected List<ActionInput> inputs = new List<ActionInput>();

        // properties

        [SerializeField] private float atkMult = 1;
        public float AtkMult { get { return atkMult; } }

        #region Init

        protected virtual void Awake()
        {
            // cache components
            entity = Entity.Get(gameObject);
            controller = entity.Controller;
            //anim = entity.Anim;

            inputs.Add(controller.skill1);
            inputs.Add(controller.skill2);
        }

        public void OnEnable()
        {
            if (controller != null)
            {
                controller.attack.performed += OnAttack;
                controller.attack.released += OnAttackRelease;
            }

            for (int i = 0; i < inputs.Count(); i++)
            {
                int index = i;
                inputs[i].performed += () => OnSkill(index);
                inputs[i].released += () => OnSkillRelease(index);
            }
        }

        public void OnDisable()
        {
            if (controller != null)
            {
                controller.attack.performed -= OnAttack;
                controller.attack.released -= OnAttackRelease;
            }

            for (int i = 0; i < inputs.Count(); i++)
            {
                int index = i;
                inputs[i].performed -= () => OnSkill(index);
                inputs[i].released -= () => OnSkillRelease(index);
            }
        }

        public void OnDestroy()
        {
            if (skillState != null) skillState.onSkillEnd -= OnSkillEnd;
        }

        #endregion

        private void Update()
        {
            // if there are skills in queue, start the next one
            if (!isUsingSkill && nextSkill != null)
            {
                UseNextSkill();
            }
        }

        #region Abilities

        public void Equip(Gear ability, int index = 0)
        {
            abilities[index] = ability;
            onEquip?.Invoke(ability, index);
        }

        #endregion

        #region Skills

        public void OnAttack()
        {
            //Debug.Log("On attack");

            weapon.Use(entity);
        }

        public void OnAttackRelease()
        {
            //Debug.Log("On attack release");

            weapon.Release(entity);
        }

        public void OnSkill(int index)
        {
            //Debug.Log("On skill (" + index + ") used.");

            if (index < 0 || index >= abilities.Length 
                || abilities[index] == null) return;

            abilities[index].Use(entity);
        }

        public void OnSkillRelease(int index)
        {
            //Debug.Log("On skill (" + index + ") released.");

            if (index < 0 || index >= abilities.Length
                || abilities[index] == null) return;

            abilities[index].Release(entity);
        }

        #endregion

        #region Skill queue

        // use the next skill in queue
        public void UseNextSkill()
        {
            UseSkill(nextSkill);
            ClearSkillQueue();
        }

        public void UseSkill(Skill skill)
        {
            if (skill == null) return;

            skillState = skill.Initiate(entity);
            if (skillState != null)
            {
                isUsingSkill = true;
                skillState.onSkillEnd += OnSkillEnd;
            }  
        }

        public void OnSkillEnd()
        {
            if (skillState != null)
            {
                skillState.onSkillEnd -= OnSkillEnd;
            }
                
            skillState = null;
            isUsingSkill = false;
        }

        // add an attack to the queue
        public void QueueSkill(Skill skill)
        {
            if (skill != null) nextSkill = skill;
        }

        // clear the attack queue
        public void ClearSkillQueue()
        {
            nextSkill = null;
        }

        #endregion
    }
}
