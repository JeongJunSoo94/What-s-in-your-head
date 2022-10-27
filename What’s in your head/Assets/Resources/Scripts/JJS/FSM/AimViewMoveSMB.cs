using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class AimViewMoveSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(1, 1);
            //if (GetPlayerController(animator).characterState.isMine)
            //{
            //    GetPlayerController(animator).characterState.aim = true;
            //}
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!GetPlayerController(animator).characterState.top)
            {
                GetPlayerController(animator).playerMouse.AimUpdate(1);
            }
            GetPlayerController(animator).playerMouse.ik.enableIK = true;
            if (GetPlayerController(animator).characterState.isMine)
            {

                if (GetPlayerController(animator).CompareTag("Steady"))
                {
                    if (!animator.GetBool("AimAttack"))
                    {
                        GetPlayerController(animator).AimViewInputMove();
                        animator.SetFloat("MoveX", GetPlayerController(animator).moveDir.normalized.x * (GetPlayerController(animator).characterState.isMove ? 1.0f : 0.0f));
                        animator.SetFloat("MoveZ", GetPlayerController(animator).moveDir.normalized.z * (GetPlayerController(animator).characterState.isMove ? 1.0f : 0.0f));
                        GetPlayerController(animator).InputMove();
                    }
                    else
                    {
                        animator.SetFloat("MoveX", 0);
                        animator.SetFloat("MoveZ", 0);
                        GetPlayerController(animator).MoveStop();
                    }
                }
                else
                {
                    GetPlayerController(animator).AimViewInputMove();
                    animator.SetFloat("MoveX", GetPlayerController(animator).moveDir.normalized.x * (GetPlayerController(animator).characterState.isMove ? 1.0f : 0.0f));
                    animator.SetFloat("MoveZ", GetPlayerController(animator).moveDir.normalized.z * (GetPlayerController(animator).characterState.isMove ? 1.0f : 0.0f));
                    GetPlayerController(animator).InputMove();
                }
                check(animator);
            }

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if (GetPlayerController(animator).characterState.isMine)
            //{
            //    GetPlayerController(animator).characterState.aim = false;
            //}
        }
        void check(Animator animator)
        {

            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    GetPlayerController(animator).characterState.top = !GetPlayerController(animator).characterState.top;
            //    animator.SetBool("Top", GetPlayerController(animator).characterState.top);
            //}
            //if (GetPlayerController(animator).characterState.top)
            //{
            //    GetPlayerController(animator).playerMouse.TopViewUpdate();
            //    Cursor.lockState = CursorLockMode.Locked;
            //}
            //if (!KeyManager.Instance.GetKey(PlayerAction.Aim))
            //{
            //    animator.SetBool("Aim", false);
            //}
        }

    }
}
