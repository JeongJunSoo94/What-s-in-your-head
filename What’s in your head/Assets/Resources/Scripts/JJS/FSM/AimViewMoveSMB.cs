using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
public class AimViewMoveSMB : CharacterBaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 1);
        GetPlayerController(animator).playerMouse.CheckRightClick(1);
        if (GetPlayerController(animator).enabled)
        {
            GetPlayerController(animator).characterState.aim = true;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController(animator).enabled)
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
        animator.SetLayerWeight(1, 0);
        if (GetPlayerController(animator).enabled)
        {
            GetPlayerController(animator).playerMouse.CheckRightClick(0);
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
