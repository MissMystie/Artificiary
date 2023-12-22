using FMOD.Studio;
using FMODUnity;
using LDtkUnity;
using Mystie.ChemEngine;
using Mystie.Systems;
using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Logic
{
    [RequireComponent(typeof(Collider2D))]
    public class Waterfall : Device, ILDtkImportedFields
    {
        [SerializeField] protected bool _open;
        [SerializeField] protected Collider2D col;
        [SerializeField] protected SpriteRenderer sprite;
        [SerializeField] protected FieldController controller;
        [SerializeField] protected FieldController source;
        [SerializeField] protected float flowVolume = 1.0f;
        [SerializeField] protected LayerMask mask = -1;

        protected FieldController field;

        [SerializeField] protected EventReference waterfallLoop;

        protected EventInstance waterfallInstance;

        protected override void Awake()
        {
            base.Awake();
            if (col == null) col = GetComponent<Collider2D>();

            if (!waterfallLoop.IsNull)
                waterfallInstance = RuntimeManager.CreateInstance(waterfallLoop);
            RuntimeManager.AttachInstanceToGameObject(waterfallInstance, transform);
        }

        private void FixedUpdate()
        {
            if (!_on || (_open && source != null && source.IsEmpty))
            {
                Close();
            }
            else if (_on && !_open && (source == null || !source.IsEmpty))
            {
                Open();
            }

            if (_open)
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
        }

        public void Open()
        {
            _open = true;
            controller.enabled = true;
            sprite.gameObject.SetActive(true);
            waterfallInstance.start();
            col.enabled = true;
        }

        public void Close()
        {
            _open = false;
            controller.enabled = false;
            sprite.gameObject.SetActive(false);
            waterfallInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            col.enabled = false;
        }

        private void Reset()
        {
            col = GetComponent<Collider2D>();
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

        public override void OnLDtkImportFields(LDtkFields fields)
        {
            base.OnLDtkImportFields(fields);

            LDtkReferenceToAnEntityInstance sourceEntity;
            
            if (fields.TryGetEntityReference("source", out sourceEntity) && sourceEntity != null)
            {
                source = sourceEntity.FindEntity()?.gameObject?.GetComponent<FieldController>();
            }
            
            fields.TryGetFloat("rate", out flowVolume);
        }
    }
}
