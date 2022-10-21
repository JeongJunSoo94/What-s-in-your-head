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
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).characterState.aim = true;
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).playerMouse.AimUpdate(1);
            GetPlayerController(animator).playerMouse.ik.enableIK = true;
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).AimViewInputMove();
                animator.SetFloat("MoveX", GetPlayerController(animator).moveDir.normalized.x * (GetPlayerController(animator).characterState.isMove ? 1.0f : 0.0f));
                animator.SetFloat("MoveZ", GetPlayerController(animator).moveDir.normalized.z * (GetPlayerController(animator).characterState.isMove ? 1.0f : 0.0f));
                GetPlayerController(animator).InputMove();
                check(animator);
            }

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).characterState.aim = false;
            }
        }
        void check(Animator animator)
        {
            if (!KeyManager.Instance.GetKey(PlayerAction.Aim))
            {
                animator.SetBool("Aim", false);
            }
        }

    }
}
