using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class UpperIdleSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(1, 0);
            //GetPlayerController(animator).characterState.aim = false;
            if (GetPlayerController(animator).playerMouse != null
                && GetPlayerController(animator).playerMouse.SwapPossibleCheck()
                && !GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            { 
                GetPlayerController(animator).playerMouse.ik.enableIK = false;
            }
            if (GetPlayerController(animator).playerMouse is SteadyMouseController)
            {
                animator.SetBool("isShootingGrapple", false);
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).playerMouse != null&&GetPlayerController(animator).characterState.isMine)
                check(animator);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool("Aim"))
            {
                GetPlayerController(animator).playerMouse.ik.enableIK = true;

                if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
                {
                    animator.SetLayerWeight(1, 1);
                }
            }
            else
            {
                GetPlayerController(animator).playerMouse.ik.enableIK = false;
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

        void check(Animator animator)
        {
            animator.SetLayerWeight(1, 0);
            if (GetPlayerController(animator).playerMouse.SwapPossibleCheck() && GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            {
                //if (GetPlayerController(animator).characterState.top)
                //{
                //    animator.SetBool("Aim", true);
                //    animator.SetBool("Top", true);
                //    return;
                //}

                if (GetPlayerController(animator).characterState.isMine)
                {
                    if (!GetPlayerController(animator).characterState.IsJumping
                        && !GetPlayerController(animator).characterState.IsAirJumping
                        && !GetPlayerController(animator).characterState.IsDashing
                        && !GetPlayerController(animator).characterState.IsAirDashing
                        && !GetPlayerController(animator).characterState.swap
                        && GetPlayerController(animator).characterState.IsGrounded
                        && !animator.GetBool("isDead")
                        && !animator.GetBool("isAttack"))
                    {
                            animator.SetBool("Aim", KeyManager.Instance.GetKey(PlayerAction.Aim));
                    }
                }
            }
        }
    }
}
