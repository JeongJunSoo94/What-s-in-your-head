using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumpSMB : CharacterBaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController(animator).enabled)
        {
            animator.SetBool("wasAirJump", true);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController(animator).enabled)
        {
            GetPlayerController(animator).InputMove();
            GetPlayerController(animator).InputDash();
            check(animator);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController(animator).enabled)
        {
            animator.SetBool("isAirJump", false);
        }
    }
    void check(Animator animator)
    {
        if (GetPlayerController(animator).characterState.IsGrounded)
        {
            animator.SetBool("isAir", false);
        }
        if (GetPlayerController(animator).characterState.IsAirDashing)
        {
            animator.SetBool("isAirDash", true);
            return;
        }
    }
}
