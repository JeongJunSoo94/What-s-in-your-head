using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.CharacterSMB;

namespace JJS
{
    public class AirDashSMB : CharacterBaseSMB
    {

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).InputMove();
                GetPlayerController(animator).InputJump();
                check(animator);
            }

        }
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                animator.SetBool("isAirDash", false);
            }
        }
        void check(Animator animator)
        {
            GetPlayerController(animator).GetComponent<KSU.PlayerInteraction>().InputInteract();
            if (GetPlayerController(animator).characterState.IsGrounded)
            {
                animator.SetBool("isAir", false);
                animator.SetBool("isAirDash", false);
            }
            if (!GetPlayerController(animator).characterState.IsAirDashing)
            {
                animator.SetBool("isAirDash", false);
            }

            if (animator.GetBool("isAirJump"))
            {
                animator.SetBool("isAirJump", false);
            }
            else if (GetPlayerController(animator).characterState.IsAirJumping)
            {
                animator.SetBool("isAirJump", true);
                return;
            }
        }
    }

}
