using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSMB : StateMachineBehaviour
{
    PlayerController3D player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
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
             check(animator);
        }
    }
    void check(Animator animator)
    {
        if (!player.characterState.IsGrounded)
        {
            animator.SetBool("isAir", true);
        }
        if (!player.characterState.IsDashing)
        {
            animator.SetBool("isDash", false);
        }
    }
}
