using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class AimAttackSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController3D(animator).enabled)
            {
              
                check(animator);
            }
        }
        void check(Animator animator)
        {
            GetPlayerController3D(animator).playerMouse.ik.enableIK = true;
            animator.SetLayerWeight(1, 1);
            if (GetPlayerController3D(animator).characterState.top)
            { 
                GetPlayerController3D(animator).playerMouse.AimUpdate(2);
            }
            if (!KeyManager.Instance.GetKey(PlayerAction.Fire))
            {
                animator.SetBool("AimAttack", false);
            }
            //if (!GetPlayerController3D(animator).characterState.IsGrounded)
            //{
            //    animator.SetBool("isAir", true);
            //    if (!GetPlayerController3D(animator).characterState.IsJumping)
            //    {
            //        animator.SetTrigger("JumpDown");
            //        return;
            //    }
            //}
            //else
            //{
            //    animator.SetBool("isAir", false);
            //}

            //if (!GetPlayerController3D(animator).characterState.isMove)
            //{
            //    GetPlayerController3D(animator).characterState.isRun = false;
            //}

            //GetPlayerController3D(animator).playerMouse.ableToLeft = true;

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

}
