using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class SteadyAimSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).characterState.aim = true;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            WeaponCheck(animator);
            if (GetPlayerController(animator).characterState.isMine)
            {
                check(animator);
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        void WeaponCheck(Animator animator)
        {
            if (GetPlayerController(animator).characterState.top)
                GetPlayerController(animator).playerMouse.ik.enableIK = false;
            if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            {
                if (GetPlayerController(animator).characterState.top)
                {
                    animator.SetLayerWeight(1, 0);
                }
                else
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
            if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            {
                if (GetPlayerController(animator).characterState.aim || GetPlayerController(animator).characterState.top)
                {
                    if (!GetPlayerController(animator).characterState.IsJumping && !GetPlayerController(animator).characterState.IsAirJumping
                        && !GetPlayerController(animator).characterState.IsDashing && !GetPlayerController(animator).characterState.IsAirDashing)
                    {
                        if (GetPlayerController(animator).playerMouse.clickLeft)
                        {
                            animator.SetBool("AimAttack", true);
                        }
                    }
                }
            }
            if (GetPlayerController(animator).playerMouse.GetUseWeapon() == 0)
            {
                if (KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
                {
                    animator.SetBool("isShootingGrapple", true);
                }
            }

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
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }

}
