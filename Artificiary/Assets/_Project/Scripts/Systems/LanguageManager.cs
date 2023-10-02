using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using System;
using System.Reflection;

namespace Mystie.Systems
{
    public class LanguageManager
    {
        public const string LOCALE_CODE_KEY = "locale";

        public static int LoadLocale()
        {
            //Get locale code
            string localeCode = PlayerPrefs.GetString(LOCALE_CODE_KEY);

            //Find locale index
            int localeIndex = 0;
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
            {
                Locale locale = LocalizationSettings.AvailableLocales.Locales[i];
                if (locale.Identifier.Code == localeCode) {
                    localeIndex = i;
                    LocalizationSettings.SelectedLocale = locale;
                    return localeIndex;
                }
            }

            //If locale was not found, reset locale to default
            LocaleSelected(localeIndex);
            return localeIndex;
        }

        public static void LocaleSelected(int index)
        {
            if (index < 0 || index >= LocalizationSettings.AvailableLocales.Locales.Count)
            {
                Debug.LogWarning("Locale at index " + index + " not found.");
                return;
            }

            Locale locale = LocalizationSettings.AvailableLocales.Locales[index];
            string localeCode = locale.Identifier.Code;
            PlayerPrefs.SetString(LOCALE_CODE_KEY, localeCode);

            LocalizationSettings.SelectedLocale = locale;
        }
    }

}
