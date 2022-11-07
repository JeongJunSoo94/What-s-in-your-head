using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(RoyaleCharacterMotor))]
    public class RoyaleCharacterAnimator : MonoBehaviour
    {
        private const string TurnAngleParamName = "turn";
        private const string GroundParamName = "ground";
        private const string SpeedParamName = "speed";
        private const string RefreshTrigger = "refresh";

        [SerializeField] Animator animator;

        private RoyaleCharacterMotor motor;
        private AnimatorOverrideController animatorController;
        private AnimationClipVariants clipOverrides;

        private void Start()
        {
            var initialized = InitializeComponents();
            if (!initialized)
            {
                Debug.LogError($"Initialization of {nameof(RoyaleCharacterAnimator)} has been failed.", this);
                return;
            }

            animatorController = animator.runtimeAnimatorController as AnimatorOverrideController;
            clipOverrides = new AnimationClipVariants(animatorController.overridesCount);
            animatorController.GetOverrides(clipOverrides);

            var runtimeController = animatorController.runtimeAnimatorController;
            animatorController = new AnimatorOverrideController();
            animatorController.runtimeAnimatorController = runtimeController;
            animatorController.ApplyOverrides(clipOverrides);
            animator.runtimeAnimatorController = animatorController;
        }

        private bool InitializeComponents()
        {
            motor = GetComponent<RoyaleCharacterMotor>();
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (!animator)
            {
                Debug.LogWarning($"Can't find {nameof(Animator)} componnet", this);
                return false;
            }

            return true;
        }

        public void UpdateAnimation(Vector3 moveDirection, bool isGrounded)
        {
            SetGrounded(isGrounded);

            var speed = moveDirection.sqrMagnitude > 0.001 ? 1 : 0;
            SetSpeed(speed);
        }

        public void SetGrounded(bool isGrounded)
        {
            animator.SetBool(GroundParamName, isGrounded);
        }

        public void SetSpeed(float normalizedSpeed)
        {
            animator.SetFloat(SpeedParamName, normalizedSpeed);
        }

        private class AnimationClipVariants : List<KeyValuePair<AnimationClip, AnimationClip>>
        {
            public AnimationClipVariants(int capacity) : base(capacity) { }

            public AnimationClip this[string name]
            {
                get { return this.Find(x => x.Key.name.Equals(name)).Value; }
                set
                {
                    int index = this.FindIndex(x => x.Key.name.Equals(name));
                    if (index != -1)
                        this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
                }
            }
        }
    }
}