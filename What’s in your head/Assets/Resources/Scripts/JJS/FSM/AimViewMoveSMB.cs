using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.Options.InputBindings;
public class AimViewMoveSMB : CharacterBaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 1);
        if (GetPlayerController3D(animator) != null)
        {
            GetPlayerController3D(animator).characterState.aim = true;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController3D(animator) != null)
        {
            GetPlayerController3D(animator).AimViewInputMove();
            animator.SetFloat("MoveX", GetPlayerController3D(animator).moveDir.normalized.x * (GetPlayerController3D(animator).characterState.isMove ? 1.0f : 0.0f));
            animator.SetFloat("MoveZ", GetPlayerController3D(animator).moveDir.normalized.z * (GetPlayerController3D(animator).characterState.isMove ? 1.0f : 0.0f));
            GetPlayerController3D(animator).InputMove();
            check(animator);
        }

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 0);
        if (GetPlayerController3D(animator) != null)
        {
            GetPlayerController3D(animator).playerMouse.CheckRightClick(false);
            GetPlayerController3D(animator).characterState.aim = false;
        }
    }
    void check(Animator animator)
    {
        if (!ITT_KeyManager.Instance.GetKey(PlayerAction.Aim))
        {
            animator.SetBool("Aim", false);
        }
    }

}
