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

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        void CheckState(Animator animator)
        {
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
            {
                animator.SetBool("isRailJump", true);
                //GetPlayerController(animator)

                if (GetPlayerController(animator).characterState.top)
                {
                    animator.SetBool("Aim", !GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canNoAimAttack);
                    animator.SetBool("Top", true);
                    return;
                }

                animator.SetBool("Aim", KeyManager.Instance.GetKey(PlayerAction.Aim));
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


