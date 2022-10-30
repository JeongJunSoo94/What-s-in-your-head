using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class GuitarAttackSMB : CharacterBaseSMB
    {
        public int index;
        bool onClick;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(1, 0);
            onClick = false;
            
                if (GetPlayerController(animator).characterState.isMine)
                {
                    GetPlayerController(animator).InputMove();
                    GetPlayerController(animator).MoveStop();
                }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                Check(animator);
            }
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).playerMouse.afterDelayTime = false;
            animator.SetBool("isAttack", false);
        }
        void Check(Animator animator)
        {
            if (!onClick)
            {
                if (KeyManager.Instance.GetKey(PlayerAction.Fire))
                {
                    animator.SetBool("isAttackNext", true);
                }
                else
                {
                    animator.SetBool("isAttackNext", false);
                }
            }

            {
                if (KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
                {
                    onClick = true;
                    animator.SetBool("isAttackNext", true);
                }
            }
            if (GetPlayerController(animator).playerMouse.afterDelayTime)
            {
                GetPlayerController(animator).InputMove();
                GetPlayerController(animator).InputJump();
                GetPlayerController(animator).InputDash();
                GetPlayerController(animator).MoveStop();
            }
            if (GetPlayerController(animator).characterState.IsJumping)
            {
                animator.SetBool("isJump", true);
                return;
            }

            if (GetPlayerController(animator).characterState.IsDashing)
            {
                animator.SetBool("isDash", true);
                return;
            }
        }
    }
}

