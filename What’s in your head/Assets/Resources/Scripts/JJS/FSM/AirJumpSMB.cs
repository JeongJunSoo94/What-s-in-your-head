using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumpSMB : StateMachineBehaviour
{
    PlayerController3D player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
        if (player.enabled)
        {
            animator.SetBool("wasAirJump", true);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.enabled)
        {
            player.InputMove();
            player.InputDash();
            check(animator);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.enabled)
        {
            animator.SetBool("isAirJump", false);
        }
    }
    void check(Animator animator)
    {
        if (player.characterState.IsGrounded)
        {
            animator.SetBool("isAir", false);
        }
        if (player.characterState.IsAirDashing)
        {
            animator.SetBool("isAirDash", true);
            return;
        }
    }
}
