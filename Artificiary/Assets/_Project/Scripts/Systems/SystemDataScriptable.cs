using Mystie.Audio;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    [CreateAssetMenu(fileName = "System Data", menuName = "CustomData/System Data", order = 0)]
    public class SystemDataScriptable : ScriptableObject
    {
        public new string name = "SystemData";

        [Scene] public string mainMenuScene = "MainMenu";
        [Scene] public string transitionScene = "Transition";
        [Scene] public string gameoverScene = "Gameover";

        [Header("Audio")]

        public List<AudioBus> audioBuses = new List<AudioBus>();
    }
}
