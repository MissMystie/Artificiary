using Mystie.Logic;
using Mystie.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class PressurePlate : LogicBehavior
    {
        [SerializeField] protected LayerMask triggerMask = -1;

        [Space]

        [Foldout("Tags")]
        [SerializeField] protected string[] tagsNeeded; //Collider needs to posses any of these tags to be elligible
        [Foldout("Tags")]
        [SerializeField] protected string[] tagsRequired; //Collider needs to possess all of these tags to be elligible
        [Foldout("Tags")]
        [SerializeField] protected string[] tagsIgnored; //Collider needs to possess none of these tags to be elligible

        protected HashSet<Collider2D> _targets = new HashSet<Collider2D>();

        void Update()
        {
            // if it's on and there are no collisions (or vice versa), toggle
            if (On == _targets.IsNullOrEmpty()) Toggle();
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.IsInLayerMask(triggerMask) 
                && col.gameObject.FilterTags(tagsNeeded, tagsRequired, tagsIgnored))
            {
                _targets.Add(col);
            }

        }

        void OnTriggerExit2D(Collider2D col)
        {
            // removes the collider if it's present in the list of colliders
            _targets.Remove(col); 
        }
    }
}
