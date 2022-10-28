using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.CharacterSMB;
namespace JJS
{
    public class AirJumpSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                animator.SetBool("wasAirJump", true);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).InputMove();
                GetPlayerController(animator).InputDash();
                check(animator);
            }
        }
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                animator.SetBool("isAirJump", false);
            }
        }
        void check(Animator animator)
        {
            GetPlayerController(animator).GetComponent<KSU.PlayerInteraction>().InputInteract();
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

}
