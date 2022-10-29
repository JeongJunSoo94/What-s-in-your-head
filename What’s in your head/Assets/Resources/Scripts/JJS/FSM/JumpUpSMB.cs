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
            animator.SetBool("isJump", false);
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).characterState.isRun = false;
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).InputMove();
                GetPlayerController(animator).InputDash();
                GetPlayerController(animator).InputJump();
                check(animator);
            }
        }
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        void check(Animator animator)
        {
            GetPlayerController(animator).GetComponent<KSU.PlayerInteraction>().InputInteract();
            float DistY = (GetPlayerController(animator).moveVec.y) / 10.0f;
            if (DistY <= 0)
            {
                animator.SetBool("JumpDown", true);
            }
            if (GetPlayerController(animator).characterState.IsGrounded)
            {
                animator.SetBool("isAir", false);
                animator.SetBool("isJump", false);
            }
            if (GetPlayerController(animator).characterState.IsAirJumping)
            {
                animator.SetBool("isAirJump", true);
                return;
            }
            if (GetPlayerController(animator).characterState.IsAirDashing)
            {
                animator.SetBool("isAirDash", true);
                return;
            }

        }
    }

}
