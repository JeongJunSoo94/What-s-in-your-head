using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
using YC.YC_Camera;
namespace JJS
{
    public class AimAttackSMB : CharacterBaseSMB
    {
        string aimAttack = "AimAttack";
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //GetPlayerController(animator).playerMouse.SetWeaponEnable(GetPlayerController(animator).playerMouse.GetUseWeapon(), true);
 
            // << : 찬 수정 (스테디가 빔으로 공격할시, 공격이 끝날때 까지 마우스 인풋을 막는다)
            if(GetPlayerController(animator))
            {
                if (GetPlayerController(animator).CompareTag("Steady"))
                { 
                    GetPlayerController(animator).GetComponent<CameraController>().SetSteadyBeam(true);
                    GetPlayerController(animator).characterState.isOutOfControl = true;
                }
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).playerMouse.ik.enableIK = true;
            animator.SetLayerWeight(1, 1);

            if (GetPlayerController(animator).characterState.top)
            {
                if (GetPlayerController(animator).CompareTag("Steady"))
                {
                    GetPlayerController(animator).playerMouse.AimUpdate(3);
                }
                else
                {
                    GetPlayerController(animator).playerMouse.AimUpdate(2);
                }
            }
            else
            {
                GetPlayerController(animator).playerMouse.AimUpdate(1);
            }
            if (GetPlayerController(animator).characterState.isMine)
            {
                check(animator);
            }
        }
        void check(Animator animator)
        {
            
            if (!GetPlayerController(animator).playerMouse.clickLeft)
            {
                animator.SetBool(aimAttack, false);
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
            //GetPlayerController(animator).playerMouse.SetWeaponEnable(GetPlayerController(animator).playerMouse.GetUseWeapon(), false);

            // << : 찬 수정 (스테디가 빔으로 공격할시, 공격이 끝날때 까지 마우스 인풋을 막는다)
            if (GetPlayerController(animator))
            {
                if (GetPlayerController(animator).CompareTag("Steady"))
                {
                    GetPlayerController(animator).characterState.isOutOfControl = false;
                    GetPlayerController(animator).GetComponent<CameraController>().SetSteadyBeam(false);
                    
                }
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
