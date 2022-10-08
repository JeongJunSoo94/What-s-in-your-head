using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDownSMB : StateMachineBehaviour
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
            player.InputMove();
            player.InputDash();
            player.InputJump();
            check(animator);
        }
   
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    void check(Animator animator)
    {

        if (player.characterState.RayCheck)
        {
            float DistY = -(player.moveVec.y) / 10.0f;
            Debug.Log(DistY);
            if (DistY > 0.2f)
                animator.SetFloat("DistY", DistY);
            //animator.SetFloat("DistY", DistY);
        }
        else
        {
            animator.SetFloat("DistY", 0.1f);
        }

        if (player.characterState.IsGrounded)
        {
            animator.SetBool("isAir", false);
            animator.SetBool("isJump", false);
        }
        if (player.characterState.IsAirJumping)
        {
            animator.SetBool("isAirJump", true);
            return;
        }
        if (player.characterState.IsAirDashing)
        {
            animator.SetBool("isAirDash", true);
            return;
        }

    }
}
