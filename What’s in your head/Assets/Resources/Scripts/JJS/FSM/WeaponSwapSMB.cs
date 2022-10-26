using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
using JCW.UI.InGame;

namespace JJS
{
    public class WeaponSwapSMB : CharacterBaseSMB
    {
        int index=0;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(1, 1);
            index = GetPlayerController(animator).playerMouse.GetUseWeapon();
            GetPlayerController(animator).playerMouse.WeaponSwap();
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).gameObject.GetComponentInChildren<SwapItem>().SetSwap(GetPlayerController(animator).playerMouse.GetUseWeapon());
            }
        }
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if (index!= GetPlayerController(animator).playerMouse.GetUseWeapon())
            //{
            //    animator.SetBool("WeaponSwap", false);
            //}
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).characterState.swap = false;
            animator.SetLayerWeight(1, 0);
            animator.SetBool("WeaponSwap", false);
        }

    }
}
