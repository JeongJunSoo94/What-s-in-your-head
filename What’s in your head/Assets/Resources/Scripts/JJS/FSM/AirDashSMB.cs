using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashSMB : CharacterBaseSMB
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
     
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController3D(animator).enabled)
        {
            GetPlayerController3D(animator).InputMove();
            GetPlayerController3D(animator).InputJump();
            check(animator);
        }
       
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController3D(animator).enabled)
        {
            animator.SetBool("isAirDash", false);
        }
    }
    void check(Animator animator)
    {
        if (GetPlayerController3D(animator).characterState.IsGrounded)
        {
            animator.SetBool("isAir", false);
            animator.SetBool("isAirDash", false);
        }
        if (!GetPlayerController3D(animator).characterState.IsAirDashing)
        {
            animator.SetBool("isAirDash", false);
        }

        if (animator.GetBool("isAirJump"))
        {
            animator.SetBool("isAirJump", false);
        }
        else if (GetPlayerController3D(animator).characterState.IsAirJumping)
        {
            animator.SetBool("isAirJump", true);
            return;
        }
    }
}
