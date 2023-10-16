using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Mista.Feedbacks
{
    /// <summary>
	/// This feedback will let you control the color and intensity of a 2D Light when played
	/// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you control the color and intensity of a 2D Light in your scene for a certain duration (or instantly).")]
    [FeedbackPath("Light/Light 2D")]
    public class MMF_Light2D : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
        /// sets the inspector color for this feedback
        #if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.LightColor; } }
        public override bool EvaluateRequiresSetup() { return (BoundLight == null); }
        public override string RequiredTargetText { get { return BoundLight != null ? BoundLight.name : ""; } }
        public override string RequiresSetupText { get { return "This feedback requires that a BoundLight be set to be able to work properly. You can set one below."; } }
#endif

        /// the duration of this feedback is the duration of the light, or 0 if instant
        public override float FeedbackDuration { get { return (Mode == Modes.Instant) ? 0f : ApplyTimeMultiplier(Duration); } set { Duration = value; } }
        public override bool HasChannel => true;
        public override bool HasRandomness => true;

        /// the possible modes for this feedback
        public enum Modes { OverTime, Instant, ShakerEvent, ToDestination }

        [MMFInspectorGroup("Light", true, 37, true)]
        /// the light to affect when playing the feedback
        [Tooltip("the light to affect when playing the feedback")]
        public Light2D BoundLight;
        /// whether the feedback should affect the light instantly or over a period of time
        [Tooltip("whether the feedback should affect the light instantly or over a period of time")]
        public Modes Mode = Modes.OverTime;
        /// how long the light should change over time
        [Tooltip("how long the light should change over time")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent, (int)Modes.ToDestination)]
        public float Duration = 0.2f;
        /// whether or not that light should be turned off on start
        [Tooltip("whether or not that light should be turned off on start")]
        public bool StartsOff = true;
        /// if this is true, the light will be disabled when this feedbacks is stopped
        [Tooltip("if this is true, the light will be disabled when this feedbacks is stopped")]
        public bool DisableOnStop = true;
        /// whether or not the values should be relative or not
        [Tooltip("whether or not the values should be relative or not")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent, (int)Modes.Instant)]
        public bool RelativeValues = true;
        /// whether or not to reset shaker values after shake
        [Tooltip("whether or not to reset shaker values after shake")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        [Tooltip("whether or not to reset the target's values after shake")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public bool ResetTargetValuesAfterShake = true;
        /// whether or not to broadcast a radius to only affect certain shakers
        [Tooltip("whether or not to broadcast a radius to only affect certain shakers")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public bool UseRadius = false;
        /// the radius of the event, in units
        [Tooltip("the radius of the event, in units")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public float EventRadius = 100f;
        /// the transform to use to broadcast the event as origin point
        [Tooltip("the transform to use to broadcast the event as origin point")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public Transform EventOriginTransform;
        /// if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over
        [Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
        public bool AllowAdditivePlays = false;

        [MMFInspectorGroup("Color", true, 38, true)]
        /// whether or not to modify the color of the light
        [Tooltip("whether or not to modify the color of the light")]
        public bool ModifyColor = true;
        /// the colors to apply to the light over time
        [Tooltip("the colors to apply to the light over time")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public Gradient ColorOverTime;
        /// the color to move to in instant mode
        [Tooltip("the color to move to in instant mode")]
        [MMFEnumCondition("Mode", (int)Modes.Instant, (int)Modes.ShakerEvent)]
        public Color InstantColor = Color.red;
        /// the color to move to in destination mode
        [Tooltip("the color to move to in destination mode")]
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public Color ToDestinationColor = Color.red;

        [MMFInspectorGroup("Intensity", true, 39, true)]
        /// whether or not to modify the intensity of the light
        [Tooltip("whether or not to modify the intensity of the light")]
        public bool ModifyIntensity = true;
        /// the curve to tween the intensity on
        [Tooltip("the curve to tween the intensity on")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent, (int)Modes.ToDestination)]
        public AnimationCurve IntensityCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        /// the value to remap the intensity curve's 0 to
        [Tooltip("the value to remap the intensity curve's 0 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapIntensityZero = 0f;
        /// the value to remap the intensity curve's 1 to
        [Tooltip("the value to remap the intensity curve's 1 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapIntensityOne = 1f;
        /// the value to move the intensity to in instant mode
        [Tooltip("the value to move the intensity to in instant mode")]
        [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantIntensity = 1f;
        /// the value to move the intensity to in ToDestination mode
        [Tooltip("the value to move the intensity to in ToDestination mode")]
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public float ToDestinationIntensity = 1f;

        [MMFInspectorGroup("Radius", true, 40, true)]
        /// whether or not to modify the radius of the light
        [Tooltip("whether or not to modify the radius of the light")]
        public bool ModifyRadius = true;
        /// the radius to apply to the light over time
        [Tooltip("the radius to apply to the light over time")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent, (int)Modes.ToDestination)]
        public AnimationCurve RadiusCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        /// the value to remap the radius curve's 0 to
        [Tooltip("the value to remap the radius curve's 0 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapRadiusZero = 0f;
        /// the value to remap the radius curve's 0 to
        [Tooltip("the value to remap the radius curve's 0 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapRadiusOne = 10f;
        /// the value to move the intensity to in instant mode
        [Tooltip("the value to move the intensity to in instant mode")]
        [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantRadius = 10f;
        /// the value to move the intensity to in ToDestination mode
        [Tooltip("the value to move the intensity to in ToDestination mode")]
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public float ToDestinationRadius = 10f;

        [MMFInspectorGroup("Shadow Strength", true, 41, true)]
        /// whether or not to modify the shadow strength of the light
        [Tooltip("whether or not to modify the shadow intensity of the light")]
        public bool ModifyShadowIntensity = true;
        /// the radius to apply to the light over time
        [Tooltip("the radius to apply to the light over time")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent, (int)Modes.ToDestination)]
        public AnimationCurve ShadowIntensityCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        /// the value to remap the shadow strength's curve's 0 to
        [Tooltip("the value to remap the shadow intensity's curve's 0 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapShadowIntensityZero = 0f;
        /// the value to remap the shadow strength's curve's 1 to
        [Tooltip("the value to remap the shadow intensity's curve's 1 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapShadowIntensityOne = 1f;
        /// the value to move the shadow strength to in instant mode
        [Tooltip("the value to move the shadow intensity to in instant mode")]
        [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantShadowIntensity = 1f;
        /// the value to move the shadow strength to in ToDestination mode
        [Tooltip("the value to move the shadow intensity to in ToDestination mode")]
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public float ToDestinationShadowIntensity = 1f;

        protected float _initialRadius;
        protected float _initialShadowIntensity;
        protected float _initialIntensity;
        protected Coroutine _coroutine;
        protected Color _initialColor;
        protected Color _targetColor;

        /// <summary>
        /// On init we turn the light off if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);

            if (BoundLight == null)
            {
                return;
            }

            _initialRadius = BoundLight.pointLightOuterRadius;
            _initialShadowIntensity = BoundLight.shadowIntensity;
            _initialIntensity = BoundLight.intensity;
            _initialColor = BoundLight.color;

            if (EventOriginTransform == null)
            {
                EventOriginTransform = owner.transform;
            }

            if (Active)
            {
                if (StartsOff)
                {
                    Turn(false);
                }
            }
        }

        /// <summary>
        /// On Play we turn our light on and start an over time coroutine if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            if (Mode == Modes.ToDestination)
            {
                _initialRadius = BoundLight.pointLightOuterRadius;
                _initialShadowIntensity = BoundLight.shadowIntensity;
                _initialIntensity = BoundLight.intensity;
                _initialColor = BoundLight.color;
            }

            float intensityMultiplier = ComputeIntensity(feedbacksIntensity, position);
            Turn(true);
            switch (Mode)
            {
                case Modes.Instant:
                    BoundLight.intensity = InstantIntensity * intensityMultiplier;
                    BoundLight.shadowIntensity = InstantShadowIntensity;
                    BoundLight.pointLightOuterRadius = InstantRadius;
                    if (ModifyColor)
                    {
                        BoundLight.color = InstantColor;
                    }
                    break;
                case Modes.OverTime:
                case Modes.ToDestination:
                    if (!AllowAdditivePlays && (_coroutine != null))
                    {
                        return;
                    }
                    _coroutine = Owner.StartCoroutine(LightSequence(intensityMultiplier));
                    break;
                case Modes.ShakerEvent:
                    MMLightShakeEvent.Trigger(FeedbackDuration, RelativeValues, ModifyColor, ColorOverTime, IntensityCurve,
                        RemapIntensityZero, RemapIntensityOne, RadiusCurve, RemapRadiusZero * intensityMultiplier, RemapRadiusOne * intensityMultiplier,
                        ShadowIntensityCurve, RemapShadowIntensityZero, RemapShadowIntensityOne, feedbacksIntensity,
                        ChannelData, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake,
                        UseRadius, EventRadius, EventOriginTransform.position);
                    break;
            }
        }

        /// <summary>
        /// This coroutine will modify the intensity and color of the light over time
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator LightSequence(float intensityMultiplier)
        {
            IsPlaying = true;
            float journey = NormalPlayDirection ? 0f : FeedbackDuration;
            while ((journey >= 0) && (journey <= FeedbackDuration) && (FeedbackDuration > 0))
            {
                float remappedTime = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);

                SetLightValues(remappedTime, intensityMultiplier);

                journey += NormalPlayDirection ? FeedbackDeltaTime : -FeedbackDeltaTime;
                yield return null;
            }
            SetLightValues(FinalNormalizedTime, intensityMultiplier);
            if (DisableOnStop)
            {
                Turn(false);
            }
            IsPlaying = false;
            _coroutine = null;
            yield return null;
        }


        /// <summary>
        /// Sets the various values on the light on a specified time (between 0 and 1)
        /// </summary>
        /// <param name="time"></param>
        protected virtual void SetLightValues(float time, float intensityMultiplier)
        {
            float intensity = 0f;
            float radius = 0f;
            float shadowStrength = 0f;

            switch (Mode)
            {
                case Modes.OverTime:
                    intensity = MMFeedbacksHelpers.Remap(IntensityCurve.Evaluate(time), 0f, 1f, RemapIntensityZero, RemapIntensityOne);
                    radius = MMFeedbacksHelpers.Remap(RadiusCurve.Evaluate(time), 0f, 1f, RemapRadiusZero, RemapRadiusOne);
                    shadowStrength = MMFeedbacksHelpers.Remap(ShadowIntensityCurve.Evaluate(time), 0f, 1f, RemapShadowIntensityZero, RemapShadowIntensityOne);
                    _targetColor = ColorOverTime.Evaluate(time);
                    break;
                case Modes.ToDestination:
                    intensity = Mathf.Lerp(_initialIntensity, ToDestinationIntensity, IntensityCurve.Evaluate(time));
                    radius = Mathf.Lerp(_initialRadius, ToDestinationRadius, RadiusCurve.Evaluate(time));
                    shadowStrength = Mathf.Lerp(_initialShadowIntensity, ToDestinationShadowIntensity, ShadowIntensityCurve.Evaluate(time));
                    _targetColor = Color.Lerp(_initialColor, ToDestinationColor, time);
                    break;
            }

            if (RelativeValues && (Mode != Modes.ToDestination))
            {
                intensity += _initialIntensity;
                shadowStrength += _initialShadowIntensity;
                radius += _initialRadius;
            }

            if (ModifyIntensity)
            {
                BoundLight.intensity = intensity * intensityMultiplier;
            }
            if (ModifyRadius)
            {
                BoundLight.pointLightOuterRadius = radius;
            }
            if (ModifyShadowIntensity)
            {
                BoundLight.shadowIntensity = Mathf.Clamp01(shadowStrength);
            }
            if (ModifyColor)
            {
                BoundLight.color = _targetColor;
            }
        }

        /// <summary>
        /// Turns the light off on stop
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!FeedbackTypeAuthorized)
            {
                return;
            }

            base.CustomStopFeedback(position, feedbacksIntensity);
            IsPlaying = false;
            if (Active && (_coroutine != null))
            {
                Owner.StopCoroutine(_coroutine);
                _coroutine = null;
            }
            if (Active && DisableOnStop)
            {
                Turn(false);
            }
        }

        /// <summary>
        /// Turns the light on or off
        /// </summary>
        /// <param name="status"></param>
        protected virtual void Turn(bool status)
        {
            BoundLight.gameObject.SetActive(status);
            BoundLight.enabled = status;
        }
    }
}
