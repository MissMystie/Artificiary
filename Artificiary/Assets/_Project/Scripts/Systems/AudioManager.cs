using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Systems
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        public AudioBus[] buses = new AudioBus[1];

        private Dictionary<string, AudioBus> busDict;

        private void Awake()
        {
            //If an instance already exists
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Init()
        {
            busDict = new Dictionary<string, AudioBus>();
            foreach (AudioBus bus in buses)
            {
                busDict.Add(bus.key, bus);
                bus.LoadBus();
            }
        }

        public float SetVolume(string key, float volume)
        {
            if (busDict.ContainsKey(key))
                return busDict[key].SetVolume(volume);
            else
                Debug.LogWarning("Volume data with key " + key + " not found.", this);

            return 1f;
        }

        public float GetVolume(string key)
        {
            if (busDict.ContainsKey(key))
                return busDict[key].volume;
            else
                Debug.LogWarning("Volume data with key " + key + " not found.", this);

            return 1f;
        }
    
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);

        }

        public EventInstance CreateEventInstance(EventReference eventReference) 
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            return eventInstance;
        }
    }

    [System.Serializable]
    public class AudioBus
    {
        public string name;
        public string key;
        public string busName;
        public float volume = 1f;

        protected Bus bus;

        public AudioBus(string _key, string _busName) { key = _key; busName = _busName; }

        public void LoadBus()
        {
            bus = RuntimeManager.GetBus("bus:/" + busName);
            if (PlayerPrefs.HasKey(key))
                volume = PlayerPrefs.GetFloat(key);

            bus.setVolume(volume);
        }

        public float SetVolume(float newVolume)
        {
            volume = newVolume;
            bus.setVolume(volume);
            PlayerPrefs.SetFloat(key, volume);

            return volume;
        }
    }
}
