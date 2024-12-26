using UnityEditor;
using UnityEngine;

namespace IAToolkit
{
    /// <summary>
    /// 编辑器下播放各种效果
    /// </summary>
    public static class EditorPlayEffectHelper
    {
        /// <summary>
        /// 播放动画
        /// </summary>
        public static void PlayAnim(GameObject pAnimGo, AnimationClip pClip, float pTime)
        {
            if (pAnimGo == null)
                return;

            if (pAnimGo == null)
                return;

            Animation animation = pAnimGo.GetComponent<Animation>();
            if (animation == null)
            {
                animation = pAnimGo.GetComponentInChildren<Animation>();
            }
            
            if (animation != null)
            {
                PlayAnimation(animation, pClip.name, pTime);
                return;
            }

            Animator animator = pAnimGo.GetComponent<Animator>();
            if (animator == null)
            {
                animator = pAnimGo.GetComponentInChildren<Animator>();
            }
            if (animator != null)
            {
                PlayAnimator(animator, pClip, pTime);
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public static void PlayAnimation(Animation anim, string animName, float animTime)
        {
            if (anim == null)
                return;
            AnimationState state = anim[animName];
            if (state == null)
                return;
            
            anim.Play(state.name);
            state.time = animTime;
            state.speed = 0f;
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public static void PlayAnimator(Animator anim, AnimationClip pClip, float pTime)
        {
            if (anim == null)
                return;
            AnimationMode.SampleAnimationClip(anim.gameObject, pClip, pTime);
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        public static void PlayParticle(GameObject pEffectGo, float time)
        {
            ParticleSystem[] particleSystems = pEffectGo.GetComponentsInChildren<ParticleSystem>();
            if (particleSystems == null)
            {
                return;
            }

            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Simulate(time,false);
            }
        }
    }
}