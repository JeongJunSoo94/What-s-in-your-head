using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalViewSMB : StateMachineBehaviour
{
    PlayerController3D player;
    CharacterState3D cs3d;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
        cs3d = animator.transform.gameObject.GetComponent<CharacterState3D>();
        cs3d.IsJumping = false;
        animator.SetBool("isAirJump", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.InputRun();
        player.InputMove();
        player.InputJump();
        player.InputDash();
        check(animator);
    }
    void check(Animator animator)
    {
        animator.SetFloat("HorizonVelocity", (cs3d.isMove ? (cs3d.isRun ? 1.0f : 0.5f) : 0.0f));
        if (!cs3d.IsGrounded)
        {
            animator.SetBool("isAir", true);
        }
        else
        {
            animator.SetBool("isAir", false);
        }
        if (!cs3d.isMove)
        {
            cs3d.isRun = false;
        }
        if (cs3d.IsJumping)
        {
            animator.SetBool("isJump", true);
            return;
        }
        else
        {
            animator.SetBool("isJump", false);
        }

        if (cs3d.IsDashing)
        {
            animator.SetBool("isDash", true);
            return;
        }
    }
}
