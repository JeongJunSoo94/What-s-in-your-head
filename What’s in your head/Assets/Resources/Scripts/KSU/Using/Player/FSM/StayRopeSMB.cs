using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using YC.YC_Camera; // << : 찬 추가

namespace KSU.FSM
{
    public class StayRopeSMB : RopeSMB
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isMoveToRope", false);

            if (GetPlayerController(animator))
            {
                GetPlayerController(animator).GetComponent<CameraController>().RidingInit(); // << : 찬 추가
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CheckState(animator);
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

        void CheckState(Animator animator)
        {
            animator.SetBool("isAir", !GetPlayerController(animator).characterState.IsGrounded);

            if(animator.GetBool("isRidingRope"))
            {
                if (KeyManager.Instance.GetKeyDown(PlayerAction.Jump) || KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
                {
                    GetRopeAction(animator).EscapeRope();
                }
            }
        }
    }
}
