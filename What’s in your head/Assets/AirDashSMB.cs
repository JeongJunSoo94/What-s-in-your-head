using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashSMB : StateMachineBehaviour
{
    PlayerController3D player;
    CharacterState3D cs3d;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
        cs3d = animator.transform.gameObject.GetComponent<CharacterState3D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.InputJump();
        check(animator);
    }
    void check(Animator animator)
    {
        if (cs3d.IsGrounded)
        {
            animator.SetBool("isAir", false);
            animator.SetBool("isAirDash", false);
        }
        if (!cs3d.IsAirDashing)
        {
            animator.SetBool("isAirDash", false);
        }
        if (cs3d.IsAirJumping)
        {
            animator.SetBool("isAirJump", true);
        }
    }
}
