using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSMB : CharacterBaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController3D(animator).enabled)
        {
            GetPlayerController3D(animator).characterState.isRun = false;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController3D(animator).enabled)
        {
             check(animator);
        }
    }
    void check(Animator animator)
    {
        if (!GetPlayerController3D(animator).characterState.IsGrounded)
        {
            animator.SetBool("isAir", true);
        }
        if (!GetPlayerController3D(animator).characterState.IsDashing)
        {
            animator.SetBool("isDash", false);
        }
    }
}
