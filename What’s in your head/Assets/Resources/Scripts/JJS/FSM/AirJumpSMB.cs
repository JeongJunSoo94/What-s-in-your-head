using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS.CharacterSMB;
using YC.YC_Camera; // << : 찬 추가
namespace JJS
{
    public class AirJumpSMB : CharacterBaseSMB
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                animator.SetBool("wasAirJump", true);

                GetPlayerController(animator).GetComponent<CameraController>().AirJumpStart(); // << : 찬 추가     
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                GetPlayerController(animator).InputMove();
                GetPlayerController(animator).InputDash();
                check(animator);
            }
        }
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetPlayerController(animator).characterState.isMine)
            {
                animator.SetBool("isAirJump", false);

                //GetPlayerController(animator).GetComponent<CameraController>().NormalJumpCameraInit(false); // << : 찬 추가
            }
        }
        void check(Animator animator)
        {
            GetPlayerController(animator).GetComponent<KSU.PlayerInteraction>().InputInteract();
            if (GetPlayerController(animator).characterState.IsGrounded)
            {
                animator.SetBool("isAir", false);
            }
            if (GetPlayerController(animator).characterState.IsAirDashing)
            {
                animator.SetBool("isAirDash", true);
                return;
            }
        }
    }

}
