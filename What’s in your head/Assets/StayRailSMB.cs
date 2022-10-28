using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;

namespace KSU.FSM
{
    public class StayRailSMB : RailSMB
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isTransferRail", false);
            animator.SetBool("isMoveToRail", false);
            animator.SetBool("isRailJump", false);
            GetRailAction(animator).ReSetRailJump();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CheckState(animator);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        void CheckState(Animator animator)
        {
            animator.SetBool("isAir", !GetPlayerController(animator).characterState.IsGrounded);

            if (KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
            {
                GetRailAction(animator).StartRailJump();
            }

            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
            {
                if (GetRailAction(animator).GetWhetherFoundRail())
                {
                    animator.SetBool("isTransferRail", true);
                    animator.SetFloat("moveToRailSpeed", 1.883f / GetRailAction(animator).SwapRail());
                }
                else
                {
                    GetRailAction(animator).EscapeRailAction();
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
}


