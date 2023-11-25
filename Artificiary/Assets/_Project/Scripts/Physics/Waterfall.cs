using Mystie.ChemEngine;
using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class Waterfall : MonoBehaviour
    {
        [SerializeField] protected FieldController controller;
        [SerializeField] protected FieldController source;
        [SerializeField] protected float flowVolume = 1.0f;
        [SerializeField] protected LayerMask mask = -1;

        protected FieldController field;

        private void FixedUpdate()
        {
            float volumeDelta = flowVolume * Time.deltaTime;

            if (source != null)
            {
                volumeDelta = -source.ChangeVolume(-volumeDelta);
            }

            if (field != null)
            {
                field.ChangeVolume(volumeDelta);
            }
        }

        private void Reset()
        {
            controller = GetComponent<FieldController>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if ((source == null || col.transform != source.transform) 
                && col.gameObject.IsInLayerMask(mask))
            {
                FieldController f = col.gameObject.GetComponent<FieldController>();
                if (f != null) field = f;
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (field != null && col.transform == field.transform)
            {
                field = null;
            }
        }
    }
}
