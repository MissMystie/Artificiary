using Mystie.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Core
{
    public static class GameSettings
    {
        public static Dictionary<AudioBusType, RuntimeAudioBus> audioBuses;

        public static void LoadSettings(SystemDataScriptable data)
        {
            LoadAudioBuses(data);
        }

        public static void LoadAudioBuses(SystemDataScriptable data)
        {
            audioBuses = new Dictionary<AudioBusType, RuntimeAudioBus>();

            foreach (AudioBus b in data.audioBuses)
            {
                if (!audioBuses.ContainsKey(b.type))
                {
                    RuntimeAudioBus audioBus = new RuntimeAudioBus(b);
                    if (!audioBus.IsValid) continue;

                    audioBuses.Add(b.type, audioBus);
                }
            }
        }

        public static float GetVolume(AudioBusType busType)
        {
            float volume = 0f;

            if (!audioBuses.ContainsKey(busType))
            {
                Debug.LogWarning("GameSettings: No audio bus of type " + busType);
                return volume;
            }

            return audioBuses[busType].Volume;
        }

        public static void SetVolume(AudioBusType busType, float volume)
        {
            if (!audioBuses.ContainsKey(busType))
            {
                Debug.LogWarning("GameSettings: No audio bus of type " + busType);
                return;
            }

            audioBuses[busType].Volume = volume;
        }
    }
}
