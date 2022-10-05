using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalViewSMB : StateMachineBehaviour
{
    PlayerController3D player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
        animator.SetBool("wasAirJump", false);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.playerMouse.CheckLeftClick();
        player.playerMouse.CheckRightClick();
        player.InputRun();
        player.InputMove();
        player.InputJump();
        player.InputDash();
        check(animator);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.MoveStop();
    }
    void check(Animator animator)
    {
        animator.SetFloat("HorizonVelocity", (player.characterState.isMove ? (player.characterState.isRun ? 1.0f : 0.5f) : 0.0f));

        if (!player.characterState.IsGrounded)
        {
            animator.SetBool("isAir", true);
            if (!player.characterState.IsJumping)
            {
                animator.SetTrigger("JumpDown");
                return;
            }
        }
        else
        {
            animator.SetBool("isAir", false);
        }

        if (!player.characterState.isMove)
        {
            player.characterState.isRun = false;
        }
        animator.SetBool("isJump", player.characterState.IsJumping);
        animator.SetBool("isDash", player.characterState.IsDashing);
        player.playerMouse.ableToLeft = true;
    }
}
