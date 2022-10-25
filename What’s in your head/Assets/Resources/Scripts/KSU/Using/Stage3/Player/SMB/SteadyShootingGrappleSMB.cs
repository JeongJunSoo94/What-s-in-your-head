using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;

namespace JJS
{
    public class SteadyShootingGrappleSMB : CharacterBaseSMB
    {
        bool isSuceededInHit = false;
        bool isFired = false;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(1, 1);
            animator.SetBool("Aim", false);
            isSuceededInHit = false;
            isFired = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            check(animator);
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

        void check(Animator animator)
        {
            isSuceededInHit = GetPlayerController(animator).playerMouse.GetCustomInfo();
            //animator.SetBool("isGrappleMoving", GetPlayerController(animator).playerMouse.GetCustomInfo());
            if (!isFired)
            {
                if (!GetPlayerController(animator).playerMouse.weaponInfo[0].weapon.transform.GetChild(0).gameObject.activeSelf)
                {
                    Debug.Log("이벤트 잘됌");
                    isFired = true;
                }
            }
            else
            {
                if(isSuceededInHit)
                {
                    animator.SetBool("isShootingGrapple", false);
                    animator.SetBool("isGrappleMoving", true);
                }
                else if(GetPlayerController(animator).playerMouse.weaponInfo[0].weapon.transform.GetChild(0).gameObject.activeSelf)
                {
                    Debug.Log("안켜짐");
                    animator.SetBool("isShootingGrapple", false);
                    animator.SetBool("isGrappleMoving", false);
                }
            }
        }
    }
}
