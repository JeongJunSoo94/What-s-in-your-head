using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
public class NormalViewSMB : CharacterBaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetPlayerController(animator).playerMouse.CheckLeftClick(0);
        GetPlayerController(animator).playerMouse.CheckRightClick(0);
        if (GetPlayerController(animator).enabled)
        {
            animator.SetBool("wasAirJump", false);
            GetPlayerController(animator).characterState.aim = false;
        }

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController(animator).enabled)
        {
            GetPlayerController(animator).InputRun();
            GetPlayerController(animator).InputMove();
            GetPlayerController(animator).InputJump();
            GetPlayerController(animator).InputDash();
            GetPlayerController(animator).RotateSlerp();
            check(animator);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController(animator).enabled)
        {
            GetPlayerController(animator).MoveStop();
        }
    }

    void check(Animator animator)
    {
        animator.SetFloat("HorizonVelocity", (GetPlayerController(animator).characterState.isMove ? (GetPlayerController(animator).characterState.isRun ? 1.0f : 0.5f) : 0.0f));

        if (!GetPlayerController(animator).characterState.IsGrounded)
        {
            animator.SetBool("isAir", true);
            if (!GetPlayerController(animator).characterState.IsJumping)
            {
                animator.SetTrigger("JumpDown");
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
        animator.SetBool("isJump", GetPlayerController(animator).characterState.IsJumping);
        animator.SetBool("isDash", GetPlayerController(animator).characterState.IsDashing);

        if (KeyManager.Instance.GetKey(PlayerAction.Fire))
        { 
            animator.SetBool("isAttack1", true);
            return;
        }

        if (KeyManager.Instance.GetKey(PlayerAction.Aim))
        {
            animator.SetBool("Aim",true);
        }
    }
}
