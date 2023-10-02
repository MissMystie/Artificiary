using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Animation
{
    internal static class AnimatorExtension
    {
        //Check if the animator is playing a certain animation
        public static bool IsPlayingOnLayer(this Animator anim, int fullPathHash, int layer)
        {
            return anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == fullPathHash;
        }

        //Check the progress of the current animation
        public static double NormalizedTime(this Animator anim, System.Int32 layer)
        {
            double time = anim.GetCurrentAnimatorStateInfo(layer).normalizedTime;
            return time > 1 ? 1 : time;
        }
    }
}
