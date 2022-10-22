using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
using YC.Camera_; // << : 찬 (스테디 빔 마우스 락)
namespace JJS
{
    public class AimAttackSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine) // << : 찬 (스테디 빔 마우스 락)
            {
                GetPlayerController(animator).GetComponent<CameraController>().SetSteadyBeam(true);
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
              
                check(animator);
            }
        }
        void check(Animator animator)
        {
            GetPlayerController(animator).playerMouse.ik.enableIK = true;
            animator.SetLayerWeight(1, 1);
            if (GetPlayerController(animator).characterState.top)
            { 
                GetPlayerController(animator).playerMouse.AimUpdate(2);
            }
            if (!KeyManager.Instance.GetKey(PlayerAction.Fire))
            {
                animator.SetBool("AimAttack", false);
            }
            //if (!GetPlayerController(animator).characterState.IsGrounded)
            //{
            //    animator.SetBool("isAir", true);
            //    if (!GetPlayerController(animator).characterState.IsJumping)
            //    {
            //        animator.SetTrigger("JumpDown");
            //        return;
            //    }
            //}
            //else
            //{
            //    animator.SetBool("isAir", false);
            //}

            //if (!GetPlayerController(animator).characterState.isMove)
            //{
            //    GetPlayerController(animator).characterState.isRun = false;
            //}

            //GetPlayerController(animator).playerMouse.ableToLeft = true;

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine) // << : 찬 (스테디 빔 마우스 락)
            {
                GetPlayerController(animator).GetComponent<CameraController>().SetSteadyBeam(false);
            }
        }

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
