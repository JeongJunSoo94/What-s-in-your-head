using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class AimSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            WeaponCheck(animator);
            if (GetPlayerController3D(animator).enabled)
            {
                check(animator);
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        void WeaponCheck(Animator animator)
        {
            if (GetPlayerController3D(animator).playerMouse.GetUseWeapon() == 1)
            {
                if (animator.GetLayerWeight(1) == 0&& GetPlayerController3D(animator).characterState.aim)
                {
                    animator.SetLayerWeight(1, 1);
                }
            }
            else
            {
                if (animator.GetLayerWeight(1) == 1)
                {
                    animator.SetLayerWeight(1, 0);
                }
            }
        }

        void check(Animator animator)
        {
            if (GetPlayerController3D(animator).playerMouse.GetUseWeapon() == 1)
            {
                if (GetPlayerController3D(animator).characterState.aim|| GetPlayerController3D(animator).characterState.top)
                {
                    if (KeyManager.Instance.GetKey(PlayerAction.Fire))
                    {
                        animator.SetBool("AimAttack", true);
                    }
                }
                //if (KeyManager.Instance.GetKey(PlayerAction.Fire))
                //{
                //    animator.SetBool("AimAttack", true);
                //}
            }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }

}
