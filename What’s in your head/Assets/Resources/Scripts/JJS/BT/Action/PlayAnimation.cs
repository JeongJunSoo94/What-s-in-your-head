using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class PlayAnimation : Task
    {
        CharacterControlBT _obj;
        string _animationName;
        public PlayAnimation(CharacterControlBT obj, string animName)
        {
            _obj = obj;
            _animationName = animName;
        }
        public override NodeState Evaluate()
        {
            ChangeAnimationState(_animationName);
            return NodeState.SUCCESS;
        }
        void ChangeAnimationState(string newState)
        {
            if (_obj.currentState == newState) return;

            _obj.animator.Play(newState);

            _obj.currentState = newState;
        }
    }
}
