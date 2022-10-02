using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumpSMB : StateMachineBehaviour
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
        player.InputMove();
        player.InputDash();
        check(animator);
    }
    void check(Animator animator)
    {
        //animator.SetFloat("DistY", (player.curGravity > 0 ? 0.0f : (player.curGravity < 0 ? 1.0f : 0.5f)));
        if (cs3d.IsGrounded)
        {
            animator.SetBool("isAir", false);
        }
        if (cs3d.IsAirDashing)
        {
            animator.SetBool("isAirDash", true);
            return;
        }
    }
}
