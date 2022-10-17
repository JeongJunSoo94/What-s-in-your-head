using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JCW.UI.Options.InputBindings;
public class AimAttack : CharacterBaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController(animator).enabled)
        {
            check(animator);
        }
    }
    void check(Animator animator)
    {
        //if (!GetPlayerController(animator).characterState.IsGrounded)
        //{
        //    animator.SetBool("isAir", true);
        //    if (!GetPlayerController(animator).characterState.IsJumping)
        //    {
        //        animator.SetTrigger("JumpDown");
        //        return;
        //    }
        //}
        //else
        //{
        //    animator.SetBool("isAir", false);
        //}

        //if (!GetPlayerController(animator).characterState.isMove)
        //{
        //    GetPlayerController(animator).characterState.isRun = false;
        //}

        //GetPlayerController(animator).playerMouse.ableToLeft = true;

        if (!KeyManager.Instance.GetKey(PlayerAction.Fire))
        {
            animator.SetBool("AimAttack", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
