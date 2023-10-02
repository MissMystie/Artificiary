using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.Audio
{
    public class VolumeSlider : MonoBehaviour
    {
        public AudioBusType busType;
        public Slider slider;

        private void OnEnable()
        {
            if (slider != null)
            {
                slider.normalizedValue = GameSettings.GetVolume(busType);
                slider.onValueChanged.AddListener(delegate { SetVolume(); });
            }
        }

        private void OnDisable()
        {
            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(delegate { SetVolume(); });
            }
        }

        public void SetVolume()
        {
            float newVolume = slider.value / slider.maxValue;
            GameSettings.SetVolume(busType, newVolume);
        }
    }
}
