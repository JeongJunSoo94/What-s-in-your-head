using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimSMB : StateMachineBehaviour
{
    PlayerController3D player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
        if (player != null)
        {
            player.characterState.isRun = false;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null)
        {
            player.playerMouse.CheckLeftDownClick();
            player.playerMouse.CheckLeftClick();
            player.InputMove();
            player.RotateAim();
            check(animator);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    void check(Animator animator)
    {
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

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
