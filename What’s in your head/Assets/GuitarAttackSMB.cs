using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuitarAttackSMB : StateMachineBehaviour
{
    PlayerController3D player;
    public int index;
    bool onClick;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<PlayerController3D>();
        onClick = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.playerMouse.CheckLeftDownClick();
        player.playerMouse.CheckLeftClick();
        Check(animator);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttack1", false);

        animator.SetBool("isAttack3", false);
    }
    void Check(Animator animator)
    {
        if (!onClick)
        {
            if (player.playerMouse.leftOn)
            {
                switch (index)
                {
                    case 0:
                        animator.SetBool("isAttack1", true);
                        animator.SetTrigger("Attack");
                        break;
                    case 1:
                        animator.SetBool("isAttack2", true);
                        break;
                    case 2:
                        animator.SetBool("isAttack3", true);
                        break;
                }
            }
            else
            {
                animator.SetBool("isAttack1", false);
                animator.SetBool("isAttack2", false);
                animator.SetBool("isAttack3", false);
            }
        }
       
        if (player.playerMouse.ableToLeft)
        {
            if (player.playerMouse.leftDown)
            {
                onClick = true;
                switch (index)
                {
                    case 0:
                        animator.SetBool("isAttack1", true);
                        animator.SetTrigger("Attack");
                        break;
                    case 1:
                        animator.SetBool("isAttack2", true);
                        break;
                    case 2:
                        animator.SetBool("isAttack3", true);
                        break;
                }
            }
        }
        
    }
}
