using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.CharacterSMB;
namespace JJS
{
    public class JumpUpSMB : CharacterBaseSMB
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
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
            float DistY = (GetPlayerController3D(animator).moveVec.y) / 10.0f;
            if (DistY <= 0)
            {
                animator.SetTrigger("JumpDown");
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

}
