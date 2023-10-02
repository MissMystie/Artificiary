using Mystie.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Mystie.Systems
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private AudioManager audioSettings;

        private void Awake()
        {
            if(audioSettings != null) audioSettings.Init();
        }

        IEnumerator Start()
        {
            // Wait for the localization system to initialize, loading Locales, preloading etc.
            yield return LocalizationSettings.InitializationOperation;
            LanguageManager.LoadLocale();
        }
            
    }
}
