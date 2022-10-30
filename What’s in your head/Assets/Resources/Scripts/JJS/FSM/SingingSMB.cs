using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class SingingSMB : CharacterBaseSMB
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if (GetPlayerController(animator).characterState.isMine)
            //{
            //    Check(animator);
            //}
            Check(animator);
        }

        void Check(Animator animator)
        {
            if (GetPlayerController(animator).playerMouse.clickLeft)
            {
                animator.SetBool("isSinging", true);
            }
            else
            {
                animator.SetBool("isSinging", false);
            }
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
