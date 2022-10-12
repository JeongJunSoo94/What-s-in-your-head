using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
public class GuitarAttackSMB : CharacterBaseSMB
{
    public int index;
    bool onClick;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetPlayerController3D(animator).playerMouse.CheckLeftClick(1);
        if (GetPlayerController3D(animator).enabled)
        {
            onClick = false;

        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController3D(animator).enabled)
        {
            Check(animator);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetPlayerController3D(animator).enabled)
        {
            animator.SetBool("isAttack1", false);

            animator.SetBool("isAttack2", false);
            animator.SetBool("isAttack3", false);
        }
    }
    void Check(Animator animator)
    {
        if (!onClick)
        {
            if (ITT_KeyManager.Instance.GetKey(PlayerAction.Fire))
            {
                switch (index)
                {
                    case 0:
                        animator.SetBool("isAttack1", true); 
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
        
        {
            if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
            {
                onClick = true;
                switch (index)
                {
                    case 0:
                        animator.SetBool("isAttack1", true);
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
