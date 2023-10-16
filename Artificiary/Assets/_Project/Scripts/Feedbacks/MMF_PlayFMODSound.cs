using FMOD.Studio;
using FMODUnity;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mista.Feedbacks
{
    /// <summary>
	/// A class used to play an FMOD sound
	/// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("You can add a description for your feedback here.")]
    [FeedbackPath("Audio/FMOD Sound")]
    public class MMF_PlayFMODSound : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        /// use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        public override float FeedbackDuration { get { return 0f; } }

        /// pick a color here for your feedback's inspector
        #if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }
        #endif

        [MMFInspectorGroup("Audiosource", true, 28, true)]
        [Tooltip("the sound origin")]
        public Transform soundOrigin;
        /// the target audio source to play
        [Tooltip("the target sound event to play")]
        public EventReference soundEvent;

        //protected float _duration;

        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);
            // your init code goes here
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            if (soundOrigin != null) RuntimeManager.PlayOneShot(soundEvent, soundOrigin.position);
            else RuntimeManager.PlayOneShot(soundEvent);

            //RuntimeManager.PlayOneShotAttached(soundEvent, soundOrigin);

            //_duration = ms * 0.001f;
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!FeedbackTypeAuthorized) return;

            // your stop code goes here
        }
    }
}
