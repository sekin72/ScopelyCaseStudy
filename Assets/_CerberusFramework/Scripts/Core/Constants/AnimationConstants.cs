using UnityEngine;

namespace CerberusFramework.Core.Constants
{
    public static class AnimationConstants
    {
        public static readonly int Appear = Animator.StringToHash("Appear");
        public static readonly int Disappear = Animator.StringToHash("Disappear");
        public static readonly int Open = Animator.StringToHash("Open");
        public static readonly int Close = Animator.StringToHash("Close");
        public static readonly int Create = Animator.StringToHash("Create");
        public static readonly int Disable = Animator.StringToHash("Disable");
        public static readonly int Destroy = Animator.StringToHash("Destroy");
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Invisible = Animator.StringToHash("Invisible");
        public static readonly int Play = Animator.StringToHash("Play");
        public static readonly int Reset = Animator.StringToHash("Reset");
    }
}