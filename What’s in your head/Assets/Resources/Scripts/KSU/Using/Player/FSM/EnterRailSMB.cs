using JCW.UI.Options.InputBindings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_; // << : 찬 추가

namespace KSU.FSM
{
    public class EnterRailSMB : RailSMB
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetRailAction(animator).StartRailAction();

            if (GetPlayerController(animator))
            {
                GetPlayerController(animator).GetComponent<CameraController>().RidingInit(); // << : 찬 추가 (병합 후 StayRail로 옮겨야함)
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //CheckState(animator);
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
             if (GetRailAction(animator).GetWhetherFailedRiding())
            {
                animator.SetBool("isMoveToRail", false);
            }
        }
    }
}

