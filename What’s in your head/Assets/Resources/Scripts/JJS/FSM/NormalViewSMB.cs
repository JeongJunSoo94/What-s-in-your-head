using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
public class NormalViewSMB : CharacterBaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetPlayerController3D(animator).playerMouse.CheckLeftClick(0);
        GetPlayerController3D(animator).playerMouse.CheckRightClick(0);
        if (GetPlayerController3D(animator).enabled)
        {
            animator.SetBool("wasAirJump", false);
            GetPlayerController3D(animator).characterState.aim = false;
        }

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController3D(animator).enabled)
        {
            GetPlayerController3D(animator).InputRun();
            GetPlayerController3D(animator).InputMove();
            GetPlayerController3D(animator).InputJump();
            GetPlayerController3D(animator).InputDash();
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

    void check(Animator animator)
    {
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

        if (!GetPlayerController3D(animator).characterState.isMove)
        {
            GetPlayerController3D(animator).characterState.isRun = false;
        }
        animator.SetBool("isJump", GetPlayerController3D(animator).characterState.IsJumping);
        animator.SetBool("isDash", GetPlayerController3D(animator).characterState.IsDashing);

        if (ITT_KeyManager.Instance.GetKey(PlayerAction.Fire))
        { 
            animator.SetBool("isAttack1", true);
            return;
        }

        if (ITT_KeyManager.Instance.GetKey(PlayerAction.Aim))
        {
            animator.SetBool("Aim",true);
        }
    }
}
