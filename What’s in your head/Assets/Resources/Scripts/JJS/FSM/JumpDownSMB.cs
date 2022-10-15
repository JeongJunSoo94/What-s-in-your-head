using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDownSMB : CharacterBaseSMB
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
            GetPlayerController3D(animator).InputMove();
            GetPlayerController3D(animator).InputDash();
            GetPlayerController3D(animator).InputJump();
            check(animator);
        }
   
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    void check(Animator animator)
    {

        if (GetPlayerController3D(animator).characterState.RayCheck)
        {
            float DistY = -(GetPlayerController3D(animator).moveVec.y) / 10.0f;
            //Debug.Log(DistY);
            if (DistY > 0.2f)
                animator.SetFloat("DistY", DistY);
            //animator.SetFloat("DistY", DistY);
        }
        else
        {
            animator.SetFloat("DistY", 0.1f);
        }

        if (GetPlayerController3D(animator).characterState.IsGrounded)
        {
            animator.SetBool("isAir", false);
            animator.SetBool("isJump", false);
        }
        if (GetPlayerController3D(animator).characterState.IsAirJumping)
        {
            animator.SetBool("isAirJump", true);
            return;
        }
        if (GetPlayerController3D(animator).characterState.IsAirDashing)
        {
            animator.SetBool("isAirDash", true);
            return;
        }

    }
}
