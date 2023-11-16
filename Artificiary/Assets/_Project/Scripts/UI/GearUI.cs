using Mystie.Core;
using Mystie.Gameplay;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class AbilityUI : MonoBehaviour
    {
        [SerializeField] private SkillManager target;
        [SerializeField] private List<Animator> gearIcons = new List<Animator>();

        void Awake()
        {
            Entity player = LevelManager.Instance.player;
            if (target == null && player != null)
            {
                target = player.GetComponent<SkillManager>();
                if (target == null)
                {
                    Debug.LogWarning("No skill manager assigned.", this);
                    return;
                }
            }
        }

        private void OnEnable()
        {
            if (target != null)
            {
                target.onEquip += UpdateUI;
                UpdateUI();
            }
        }

        private void OnDisable()
        {
            if (target != null)
            {
                target.onEquip -= UpdateUI;
            }
        }

        [Button()]
        void UpdateUI()
        {
            for (int i = 0; i < gearIcons.Count; i++) 
            {
                if (i < target.abilities.Length && target.abilities[i] != null) 
                    UpdateUI(target.abilities[i], i);
                else UpdateUI(null, i);
            }
        }

        void UpdateUI(Gear gear, int index)
        {
            if (index >= gearIcons.Count) return;

            if (gear != null)
            {
                gearIcons[index].runtimeAnimatorController = gear.animController;
            }
            else
            {
                gearIcons[index].runtimeAnimatorController = null;
            }
        }
    }
}
