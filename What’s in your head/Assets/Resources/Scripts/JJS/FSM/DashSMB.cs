using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.CharacterSMB;
using KSU;
namespace JJS
{
    public class DashSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
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
                check(animator);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isDash", false);
        }
        void check(Animator animator)
        {
            GetPlayerController(animator).gameObject.GetComponent<PlayerInteraction>().InputInteract();
            GetPlayerController(animator).InputJump();
            if (GetPlayerController(animator).characterState.IsJumping)
            {
                animator.SetBool("isJump", true);
                animator.SetBool("isAir", true);
                return;
            }
            animator.SetBool("isAir", !GetPlayerController(animator).characterState.IsGrounded);
            
            animator.SetBool("isDash", GetPlayerController(animator).characterState.IsDashing);
        }
    }

}
