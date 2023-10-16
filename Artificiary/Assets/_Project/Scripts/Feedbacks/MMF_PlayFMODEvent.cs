using FMODUnity;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mista.Feedbacks
{
    /// <summary>
	/// A class used to play an FMOD event
	/// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("You can add a description for your feedback here.")]
    [FeedbackPath("Audio/FMOD Event Emitter")]
    public class MMF_PlayFMODEvent : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        /// use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        public override float FeedbackDuration { get { return 0f; } }

        /// pick a color here for your feedback's inspector
        #if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }
        public override bool EvaluateRequiresSetup() { return (audioSource == null); }
        public override string RequiredTargetText { get { return audioSource != null ? audioSource.name : ""; } }
        public override string RequiresSetupText { get { return "This feedback requires that an AudioSource be set to be able to work properly. You can set one below."; } }
        #endif

        /// the possible ways to interact with the audiosource
        public enum Mode { Play, Stop, None }

        [MMFInspectorGroup("Audiosource", true, 28, true)]
        /// the target audio source to play
        [Tooltip("the target audio source to play")]
        public StudioEventEmitter audioSource;
        /// whether we should play the audio source or stop it or pause it
		[Tooltip("whether to play the audio source or stop it or pause it when feedback is played")]
        public Mode playMode = Mode.Play;
        [Tooltip("whether to play the audio source or stop it or pause it when feedback is stopped")]
        public Mode stopMode = Mode.Stop;

        [Space]

        public bool setParameterOnPlay = false;
        [MMFCondition("setParameterOnPlay", true)]
        public string parameterNamePlay;
        [MMFCondition("setParameterOnPlay", true)]
        public float parameterValuePlay;
        [MMFCondition("setParameterOnPlay", true)]
        public bool parameterIsGlobalPlay;

        [Space]

        public bool setParameterOnStop = false;
        [MMFCondition("setParameterOnStop", true)]
        public string parameterNameStop;
        [MMFCondition("setParameterOnStop", true)]
        public float parameterValueStop;
        [MMFCondition("setParameterOnStop", true)]
        public bool parameterIsGlobalStop;

        protected float _duration;

        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);
            // your init code goes here
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            PlayFeedback(playMode);
            if (setParameterOnPlay)
            {
                if (parameterIsGlobalPlay)
                {
                    RuntimeManager.StudioSystem.setParameterByName(parameterNamePlay, parameterValuePlay);
                }
                else
                {
                    audioSource.SetParameter(parameterNamePlay, parameterValuePlay);
                }
            }
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!FeedbackTypeAuthorized) return;

            PlayFeedback(stopMode);
            if (setParameterOnStop)
            {
                if (parameterIsGlobalStop)
                {
                    RuntimeManager.StudioSystem.setParameterByName(parameterNameStop, parameterValueStop);
                }
                else
                {
                    audioSource.SetParameter(parameterNameStop, parameterValueStop);
                }
            }
        }

        protected void PlayFeedback(Mode mode)
        {
            switch (mode)
            {
                case Mode.Play:
                    int ms = 0;
                    audioSource.EventDescription.getLength(out ms);
                    _duration = ms * 0.001f;
                    audioSource.Play();
                    break;
                case Mode.Stop:
                    _duration = 0f;
                    audioSource.Stop();
                    break;
            }
        }
    }
}
