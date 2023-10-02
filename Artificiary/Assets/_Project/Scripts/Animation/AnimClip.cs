using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Animation
{
    [System.Serializable]
    public class AnimClip
    {
        public Animator anim;
        public Info info;

        public int totalFrames { get; private set; }
        private int animationFullNameHash;

        public AnimClip(Animator anim, Info _info)
        {
            info = _info;
            Init(anim);
        }

        public AnimClip(Animator anim, AnimationClip clip, string animStateName, int layerNumber = 0)
        {
            info = new Info(clip, animStateName, layerNumber);
            Init(anim);
        }

        public void Init(Animator _anim)
        {
            anim = _anim;

            //We get the number of frames in the animation to convert it for use with the extension method in animator
            totalFrames = GetFrameCount(info.clip);

            if (anim.isActiveAndEnabled)
            {
                //Get the name hash of the animation
                string name = anim.GetLayerName(info.layerNumber) + "." + info.animStateName;
                animationFullNameHash = Animator.StringToHash(name);
            }
        }

        //Check if the animation is currently playing
        public bool IsActive()
        {
            return anim.IsPlayingOnLayer(animationFullNameHash, info.layerNumber); //0
        }

        //Method to get the frame count of a clip
        public static int GetFrameCount(AnimationClip _clip)
        {
            if (_clip == null)
                return 0; //If the clip is null, return a frame count of 0
            else
                return Mathf.RoundToInt(_clip.length * _clip.frameRate);
        }

        double PercentOnFrame(int frameNumber)
        {
            return (double)frameNumber / (double)totalFrames;
        }

        //Check if the animation passed a specific frame
        public bool BiggerOrEqualThanFrame(int frameNumber)
        {
            double percentage = anim.NormalizedTime(info.layerNumber);
            return (percentage >= PercentOnFrame(frameNumber));
        }

        //Check if the animation is on the last frame to avoid issues with transitions
        public bool IsOnLastFrame()
        {
            double percentage = anim.NormalizedTime(info.layerNumber);
            return (percentage > PercentOnFrame(totalFrames - 1));
        }

        [System.Serializable]
        public class Info
        {

            public AnimationClip clip;
            public string animStateName;
            public int layerNumber;

            public Info(AnimationClip _clip, string _animStateName, int _layerNumber = 0)
            {
                clip = _clip;
                animStateName = _animStateName;
                layerNumber = _layerNumber;
            }
        }
    }
}
