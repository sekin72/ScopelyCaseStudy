using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CerberusFramework.Core.Managers.Animation
{
    public class AnimationManager : Manager
    {
        public override bool IsCore => false;

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            SetReady();
            return UniTask.CompletedTask;
        }

        public static void PlayAnimationNamed(string animationName, Animator animator, Action onAnimationFinished = null,
            float animationSpeed = 1, float animationPercentage = .5f, Action onPercentageReached = null)
        {
            if (string.IsNullOrEmpty(animationName))
            {
                onAnimationFinished?.Invoke();
                onPercentageReached?.Invoke();
                return;
            }

            var stateHash = Animator.StringToHash(animationName);
            if (!animator.HasState(0, stateHash))
            {
                onAnimationFinished?.Invoke();
                onPercentageReached?.Invoke();
                return;
            }

            foreach (var parameter in animator.parameters)
            {
                if (parameter.name.Equals("AnimationSpeed"))
                {
                    animator.SetFloat("AnimationSpeed", animationSpeed);
                }
            }

            animator.Play(animationName, 0, 0);
            WaitUntilPercentageReached(animationName, animator, animationPercentage, onPercentageReached).Forget();
            WaitUntilAnimFinish(animationName, animator, onAnimationFinished).Forget();
        }

        public static float GetAnimationLength(string animationName, Animator animator)
        {
            return Array.Find(animator.runtimeAnimatorController.animationClips, x => x.name.Equals(animationName)).length;
        }

        private static async UniTask WaitUntilAnimFinish(string name, Animator animator, Action action = null)
        {
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(name));
            await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorStateInfo(0).length));
            action?.Invoke();
        }

        private static async UniTask WaitUntilPercentageReached(string name, Animator animator, float percentage,
            Action action = null)
        {
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(name));
            await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorStateInfo(0).length * percentage));

            action?.Invoke();
        }
    }
}