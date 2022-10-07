using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashSMB : StateMachineBehaviour
{
    PlayerController3D player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
     
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null)
        {
            player.InputMove();
            player.InputJump();
            check(animator);
        }
       
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null)
        {
            animator.SetBool("isAirDash", false);
        }
    }
    void check(Animator animator)
    {
        if (player.characterState.IsGrounded)
        {
            animator.SetBool("isAir", false);
            animator.SetBool("isAirDash", false);
        }
        if (!player.characterState.IsAirDashing)
        {
            animator.SetBool("isAirDash", false);
        }

        if (animator.GetBool("isAirJump"))
        {
            animator.SetBool("isAirJump", false);
        }
        else if (player.characterState.IsAirJumping)
        {
            animator.SetBool("isAirJump", true);
            return;
        }
    }
}
