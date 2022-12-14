using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;

namespace KSU.FSM
{
    public class SteadyGrappleMovingSMB : SteadyGrappleSMB
    {
        bool isMovingToTarget = false;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            isMovingToTarget = false;
            animator.SetBool("Aim", false);
            GetPlayerController(animator).characterState.aim = false;
            GetGrappleAction(animator).PlayGrappleFlyingSound();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            check(animator);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).playerMouse.ik.enabled = true;
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

        void check(Animator animator)
        {
            if(!isMovingToTarget)
            {
                if (GetPlayerController(animator).characterState.isRiding)
                {
                    isMovingToTarget = true;
                }
            }
            else
            {
                if (!GetPlayerController(animator).characterState.isRiding)
                {
                    animator.SetBool("isGrappleMoving", false);
                }
            }
        }
    }
}
