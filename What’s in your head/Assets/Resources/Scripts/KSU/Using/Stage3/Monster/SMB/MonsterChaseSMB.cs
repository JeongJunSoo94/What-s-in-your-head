using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object.Monster.FSM
{
    public class MonsterChaseSMB : MonsterSMB
    {
        bool willAttack = false;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            willAttack = false;
            animator.SetBool("WasStunned", false);
            GetMonsterController(animator).StartChasing();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetMonsterController(animator).Chase();
            CheckState(animator);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetMonsterController(animator).StopChasing();
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

        void CheckState(Animator animator)
        {
            if(!willAttack)
            {
                if (GetMonsterController(animator) is TrippleHeadSnake)
                {
                    TrippleHeadSnake trippleHeadSnake = (GetMonsterController(animator) as TrippleHeadSnake);
                    if (trippleHeadSnake.IsReadyToRush())
                    {
                        animator.SetBool("isReadyToRush", true);
                        return;
                    }
                }


                if (GetMonsterController(animator).IsReadyToAttck())
                {
                    willAttack = true;
                    GetMonsterController(animator).PrepareAttack();
                }
            }
        }
    }
}
