using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
using KSU;
namespace JJS
{
    public class NormalViewSMB : CharacterBaseSMB
    {
        float AttackDelayTime = 0f;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("wasAirJump", false);
            animator.SetBool("isJump", false);
            animator.SetBool("isAttack", false);
            //GetPlayerController(animator).characterState.aim = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!GetPlayerController(animator).characterState.swap)
            { 
                //WeaponCheck(animator);
                //GetPlayerController(animator).playerMouse.ik.enableIK = false;
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
        //void WeaponCheck(Animator animator)
        //{
        //    if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
        //    {
        //        if(GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canNoAimAttack)
        //        {
        //            animator.SetLayerWeight(1, 1);
        //        }
        //        else
        //        {
        //            if (animator.GetLayerWeight(1) == 0 && GetPlayerController(animator).characterState.aim|| GetPlayerController(animator).characterState.top)
        //            {
        //                animator.SetLayerWeight(1, 1);
        //            }
        //            else
        //            {
        //                animator.SetLayerWeight(1, 0);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (animator.GetLayerWeight(1) == 1)
        //        {
        //            GetPlayerController(animator).playerMouse.ik.enableIK = false;
        //            animator.SetLayerWeight(1, 0);
        //        }
        //    }
        //}

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
            //GetPlayerController(animator).InputJump();
            //if (!GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim
            //    || GetPlayerController(animator).characterState.top) /////////// 3스테이지 전용 코드
            //{
            //    if (!animator.GetBool("isAttack") && !animator.GetBool("AimAttack"))
            //    {
            //        GetPlayerController(animator).InputJump();
            //        GetPlayerController(animator).InputDash();
            //    }
            //}
            //else
            //{
            //    if (!animator.GetBool("Aim")&&!animator.GetBool("isAttack") && !animator.GetBool("AimAttack"))
            //    {
            //        GetPlayerController(animator).InputJump();
            //        GetPlayerController(animator).InputDash();
            //    }
            //}

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
                                if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].weapon.name != "Mic")
                                {
                                    animator.SetBool("isAttackNext", true);
                                    animator.SetBool("isAttack", true);
                                    return;
                                }
                                animator.SetBool("isSinging", true);
                                return;
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
                //GetPlayerController(animator).characterState.top = !GetPlayerController(animator).characterState.top;
                GetPlayerController(animator).characterState.top = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
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
