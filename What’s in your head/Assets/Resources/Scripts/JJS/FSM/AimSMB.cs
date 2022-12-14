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
            if (GetPlayerController(animator).characterState.isMine)
            {
                check(animator);
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animator.GetBool("AimAttack"))
                GetPlayerController(animator).playerMouse.ik.enableIK = false;
           
        }

        void WeaponCheck(Animator animator)
        {
            GetPlayerController(animator).playerMouse.ik.enableIK = true;
            if (GetPlayerController(animator).characterState.top)
            {
                if (GetPlayerController(animator).CompareTag("Steady"))
                {
                    GetPlayerController(animator).playerMouse.AimUpdate(3);
                }
                else
                {
                    GetPlayerController(animator).playerMouse.AimUpdate(2);
                }
            }
            else
            {
                GetPlayerController(animator).playerMouse.AimUpdate(1);
            }
            if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            {
                animator.SetLayerWeight(1, 1);
            }
            else
            {
                animator.SetBool("Aim", false);
            }
        }

        void check(Animator animator)
        {
            if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            {
                //if (GetPlayerController(animator).characterState.aim)
                {
                    if (!GetPlayerController(animator).characterState.IsJumping&& !GetPlayerController(animator).characterState.IsAirJumping
                        &&!GetPlayerController(animator).characterState.IsDashing && !GetPlayerController(animator).characterState.IsAirDashing)
                    {
                        if (GetPlayerController(animator).playerMouse.clickLeft)
                        {
                            animator.SetBool("AimAttack", true);
                        }
                    }
                }
            }

            if (GetPlayerController(animator).characterState.isMine)
            {
                if (!GetPlayerController(animator).characterState.IsJumping
                    && !GetPlayerController(animator).characterState.IsAirJumping
                    && !GetPlayerController(animator).characterState.IsDashing
                    && !GetPlayerController(animator).characterState.IsAirDashing
                    && !GetPlayerController(animator).characterState.swap
                    && !animator.GetBool("isAttack"))
                {
                    animator.SetBool("Aim", GetPlayerController(animator).characterState.aim);
                }
                else
                {
                    animator.SetBool("Aim", false);
                }
            }

            //if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            //{
            //    if (GetPlayerController(animator).characterState.top)
            //    {
            //        animator.SetBool("Aim", true);
            //        animator.SetBool("Top", true);
            //        return;
            //    }
            //    animator.SetBool("Aim", KeyManager.Instance.GetKey(PlayerAction.Aim));
            //}
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

    }

}
