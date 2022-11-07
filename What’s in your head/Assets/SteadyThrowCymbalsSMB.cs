using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.FSM
{
    public class SteadyThrowCymbalsSMB : SteadyCymbalsSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).characterState.aim = false;
            GetPlayerController(animator).playerMouse.ik.enabled = false;
            animator.SetLayerWeight(1, 1);
            animator.SetBool("WasShootingCymbals", true);
            animator.SetBool("Aim", false);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isShootingCymbals", false);
        }
    }
}
