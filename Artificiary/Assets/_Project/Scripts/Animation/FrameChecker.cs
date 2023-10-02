using Mystie.Animation;
using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Animation
{
    // this class allows to check frames of animation within a certain range

    public class FrameChecker
    {
        private IFrameCheckHandler frameCheckHandler;
        private AnimClip animClip;

        private bool checkedHitFrameStart;
        private bool checkedHitFrameEnd;
        private bool lastFrame;

        public FrameChecker(IFrameCheckHandler frameCheckHandler, AnimClip animClip)
        {
            Init(frameCheckHandler, animClip);
        }

        public void Init(IFrameCheckHandler frameCheckHandler, AnimClip animClip)
        {
            this.frameCheckHandler = frameCheckHandler;
            this.animClip = animClip;

            checkedHitFrameStart = false;
            checkedHitFrameEnd = false;
            lastFrame = false;
        }

        public void CheckFrames(int hitFrameStart, int hitFrameEnd) 
        {
            if (lastFrame)
            {
                lastFrame = false;
                frameCheckHandler.OnLastFrameEnd();
            }

            // if the animation isn't playing anymore, return
            if (!animClip.IsActive()) 
            {
                frameCheckHandler.OnClipStopped();
                return;
            }

            // if we haven't checked that the animation passed the start flag and it did
            if (!checkedHitFrameStart && animClip.BiggerOrEqualThanFrame(hitFrameStart))
            {
                frameCheckHandler.OnHitFrameStart();
                checkedHitFrameStart = true;
            }
            else if (!checkedHitFrameEnd && animClip.BiggerOrEqualThanFrame(hitFrameEnd))
            {
                frameCheckHandler.OnHitFrameEnd();
                checkedHitFrameEnd = true;
            }

            if (!lastFrame && animClip.IsOnLastFrame())
            {
                frameCheckHandler.OnLastFrameStart();
                lastFrame = true; // this is here so we don't skip the last frame
            }
        }
    }
}
