using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class NormalViewSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController3D(animator).enabled)
            {
                animator.SetBool("wasAirJump", false);
                animator.SetBool("isJump", false);
                GetPlayerController3D(animator).characterState.aim = false;
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!GetPlayerController3D(animator).characterState.swap)
            { 
                WeaponCheck(animator);
            GetPlayerController3D(animator).playerMouse.ik.enableIK = false;
            }
            if (GetPlayerController3D(animator).enabled)
            {
                GetPlayerController3D(animator).InputRun();
                GetPlayerController3D(animator).InputMove();
                check(animator);
            }

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController3D(animator).enabled)
            {
                GetPlayerController3D(animator).MoveStop();
            }
        }
        void WeaponCheck(Animator animator)
        {
            if (GetPlayerController3D(animator).playerMouse.GetUseWeapon() == 1)
            {
                if (animator.GetLayerWeight(1) == 0 && GetPlayerController3D(animator).characterState.aim)
                {
                    animator.SetLayerWeight(1, 1);
                }
                else
                {
                    animator.SetLayerWeight(1, 0);
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
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GetPlayerController3D(animator).characterState.top = !GetPlayerController3D(animator).characterState.top;
            }
            if (GetPlayerController3D(animator).characterState.top)
            {
                Cursor.lockState = CursorLockMode.Confined;
                GetPlayerController3D(animator).playerMouse.TopViewUpdate();
               
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }


            animator.SetFloat("HorizonVelocity", (GetPlayerController3D(animator).characterState.isMove ? (GetPlayerController3D(animator).characterState.isRun ? 1.0f : 0.5f) : 0.0f));

            if (!GetPlayerController3D(animator).characterState.IsGrounded)
            {
                animator.SetBool("isAir", true);
                if (!GetPlayerController3D(animator).characterState.IsJumping)
                {
                    animator.SetTrigger("JumpDown");
                    return;
                }
            }
            else
            {
                animator.SetBool("isAir", false);
            }

            if (KeyManager.Instance.GetKey(PlayerAction.Fire))
            {
                if (GetPlayerController3D(animator).playerMouse.GetUseWeapon()==0)
                {
                    animator.SetBool("isAttack1", true);
                    return;
                }
            }
            if (!GetPlayerController3D(animator).characterState.isMove)
            {
                GetPlayerController3D(animator).characterState.isRun = false;
            }
            if (GetPlayerController3D(animator).characterState.IsJumping)
            {
                animator.SetBool("isJump", true);
                return;
            }
            else
            {
                GetPlayerController3D(animator).InputJump();
            }

            if (GetPlayerController3D(animator).characterState.IsDashing)
            {
                animator.SetBool("isDash", true);
                return;
            }
            else
            {
                GetPlayerController3D(animator).InputDash();
            }



            if (KeyManager.Instance.GetKeyDown(PlayerAction.Swap)&& !GetPlayerController3D(animator).characterState.swap)
            {
                if(!animator.GetBool("isAttack1"))
                {
                    GetPlayerController3D(animator).characterState.swap = true;
                    animator.SetBool("WeaponSwap", true);
                }
                //GetPlayerController3D(animator).characterState.swap = true;
                //animator.SetBool("WeaponSwap", true);
                return;
            }
         
            if (GetPlayerController3D(animator).playerMouse.GetUseWeapon() == 1&& !GetPlayerController3D(animator).characterState.top && KeyManager.Instance.GetKey(PlayerAction.Aim))
            {
                animator.SetBool("Aim", true);
                return;
            }

        }
    }

}
