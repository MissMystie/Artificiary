using FMOD.Studio;
using FMODUnity;
using LDtkUnity;
using Mystie.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class Drain : Device
    {
        [SerializeField] protected FieldController controller;
        [SerializeField] protected float fillRate = 1.0f;
        [SerializeField] protected float drainRate = 1.0f;

        [SerializeField] protected EventReference waterLevelUp;
        [SerializeField] protected EventReference waterLevelDown;

        protected EventInstance waterLevelUpInstance;
        protected EventInstance waterLevelDownInstance;

        protected override void Awake()
        {
            if (!waterLevelUp.IsNull)
                waterLevelUpInstance = RuntimeManager.CreateInstance(waterLevelUp);
            if (!waterLevelDown.IsNull)
                waterLevelDownInstance = RuntimeManager.CreateInstance(waterLevelDown);
            
            RuntimeManager.AttachInstanceToGameObject(waterLevelUpInstance, transform);
            RuntimeManager.AttachInstanceToGameObject(waterLevelDownInstance, transform);
        }

        private void FixedUpdate()
        {
            float volumeDelta = (_on ? fillRate : -drainRate) * Time.deltaTime;

            if (controller != null)
            {
                controller.ChangeVolume(volumeDelta);
            }

            if (volumeDelta > 0 && controller.IsFull)
            {
                waterLevelUpInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else if (volumeDelta < 0 && controller.IsEmpty)
            {
                waterLevelDownInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        public override void SetOn()
        {
            waterLevelUpInstance.start();
            waterLevelDownInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            base.SetOn();
        }

        public override void SetOff()
        {
            waterLevelUpInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            waterLevelDownInstance.start();
            base.SetOff();
        }

        public override void OnLDtkImportFields(LDtkFields fields)
        {
            base.OnLDtkImportFields(fields);

            LDtkReferenceToAnEntityInstance controllerEntity;

            if (fields.TryGetEntityReference("controller", out controllerEntity) && controllerEntity != null)
            {
                controller = controllerEntity.FindEntity()?.gameObject?.GetComponent<FieldController>();
            }

            fields.TryGetFloat("fill_rate", out fillRate);
            fields.TryGetFloat("drain_rate", out drainRate);
        }
    }
}
