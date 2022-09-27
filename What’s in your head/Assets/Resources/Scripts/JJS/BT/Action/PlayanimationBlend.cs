using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationBlend : Task
{
    CharacterControlBT _obj;
    string _animationName;
    public PlayAnimationBlend(CharacterControlBT obj, string animName)
    {
        _obj = obj;
        _animationName = animName;
    }
    public override NodeState Evaluate()
    {
        _obj.animator.SetFloat("MoveX", _obj._moveDir.x);
        _obj.animator.SetFloat("MoveZ", _obj._moveDir.z);
        ChangeAnimationState(_animationName);
        return NodeState.SUCCESS;
    }
    void ChangeAnimationState(string newState)
    {
        if (_obj.currentState == newState) return;

        _obj.animator.Play(newState);

        _obj.currentState = newState;

        //_obj.animator.SetFloat("Move", _distFromGround);
        //Com.anim.SetBool(AnimOption.paramGrounded, State.isGrounded);
    }
}
