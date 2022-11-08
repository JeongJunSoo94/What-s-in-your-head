using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS.BT
{
    public class AnimationNode : ActionNode
    {
        public string _animationName;
        protected override void OnStart()
        {
            ChangeAnimationState(_animationName);
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }

        void ChangeAnimationState(string newState)
        {
            if (objectInfo.currentAniState == newState) return;

            objectInfo.animator.StopPlayback();
            objectInfo.animator.Play(newState);
            objectInfo.currentAniState = newState;
        }
    }

}