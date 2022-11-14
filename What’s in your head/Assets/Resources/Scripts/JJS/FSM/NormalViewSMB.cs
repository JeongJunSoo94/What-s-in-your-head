using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
using KSU;
using YC.Camera_; // << : 찬 추가
namespace JJS
{
    public class NormalViewSMB : CharacterBaseSMB
    {

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("wasAirJump", false);
            animator.SetBool("isJump", false);
            animator.SetBool("isAttack", false);
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).GetComponent<CameraController>().JumpInit(false); // << : 찬 추가
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!GetPlayerController(animator).characterState.swap)
            { 
            }
            if (GetPlayerController(animator).characterState.isMine)
            {
                InputCheck(animator);
            }
            check(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).MoveStop();
        }

        void InputCheck(Animator animator)
        {
            GetPlayerController(animator).GetComponent<KSU.PlayerInteraction>().InputInteract();
            if (GameManager.Instance.isTopView)
            {
                if (GetPlayerController(animator).CompareTag("Steady"))
                {
                    if (!GetPlayerController(animator).playerMouse.clickLeft)
                        GetPlayerController(animator).playerMouse.TopViewUpdate();
                }
                else
                {
                    GetPlayerController(animator).playerMouse.TopViewUpdate();
                }
            }
            else if (GameManager.Instance.isSideView)
            {
                GetPlayerController(animator).playerMouse.SideViewUpdate();
            }

            GetPlayerController(animator).InputRun();
            if (GetPlayerController(animator).CompareTag("Steady"))
            {
                if (!animator.GetBool("AimAttack"))
                { 
                    GetPlayerController(animator).InputMove();
                }
            }
            else
            {
                GetPlayerController(animator).InputMove();
            }

            if (!animator.GetBool("Aim") && !animator.GetBool("isAttack") && !animator.GetBool("AimAttack"))
            {
                GetPlayerController(animator).InputJump();
                GetPlayerController(animator).InputDash();
            }
            if (GetPlayerController(animator).playerMouse != null)
            {
                if (GetPlayerController(animator).playerMouse.clickLeft)
                {
                    if (!animator.GetBool("WeaponSwap"))
                    {
                        if (!GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
                        {
                            if (!GetPlayerController(animator).characterState.IsDashing && !GetPlayerController(animator).characterState.IsJumping)
                            {
                                if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].weapon.name == "ElectricGuitar_st")
                                {
                                    animator.SetBool("isAttackNext", true);
                                    animator.SetBool("isAttack", true);
                                    return;
                                }
                                else if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].weapon.name == "Mic")
                                {
                                    animator.SetBool("isSinging", true);
                                    return;
                                }
                            }
                        }
                    }
                }

                if (GetPlayerController(animator).playerMouse.SwapPossibleCheck() && KeyManager.Instance.GetKeyDown(PlayerAction.Swap))
                {
                    if (!animator.GetBool("isAttack")
                        && !animator.GetBool("AimAttack")
                        && GetPlayerController(animator).playerMouse.canSwap
                        && !GetPlayerController(animator).characterState.aim)
                    {
                        GetPlayerController(animator).characterState.swap = true;
                        animator.SetBool("WeaponSwap", true);
                        return;
                    }
                }

            }

        }

        void check(Animator animator)
        {
            if (GameManager.Instance.isTopView)
            {
                GetPlayerController(animator).characterState.top = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else if (GameManager.Instance.isSideView)
            {
                GetPlayerController(animator).characterState.top = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                GetPlayerController(animator).characterState.top = false;
            }
            animator.SetBool("Top", GetPlayerController(animator).characterState.top);

            if (!GetPlayerController(animator).characterState.isOutOfControl)
            {
                animator.SetFloat("HorizonVelocity", (GetPlayerController(animator).characterState.isMove ? (GetPlayerController(animator).characterState.isRun ? 1.0f : 0.5f) : 0.0f));
            }
            else
            {
                animator.SetFloat("HorizonVelocity",0);
            }
           
            GetPlayerController(animator).GetComponent<PlayerInteraction>().InputInteract();

            if (!GetPlayerController(animator).characterState.IsGrounded)
            {
                animator.SetBool("isAir", true);
                if (!GetPlayerController(animator).characterState.IsJumping)
                {
                    animator.SetBool("JumpDown",true);
                    return;
                }
            }
            else
            {
                animator.SetBool("isAir", false);
            }

     
            if (!GetPlayerController(animator).characterState.isMove)
            {
                GetPlayerController(animator).characterState.isRun = false;
            }
            if (GetPlayerController(animator).characterState.IsJumping)
            {
                animator.SetBool("isJump", true);
                return;
            }

            if (GetPlayerController(animator).characterState.IsDashing)
            {
                animator.SetBool("isDash", true);
                return;
            }
        }
    }
}
