using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalViewSMB : StateMachineBehaviour
{
    JJS.PlayerControllerWIYH player;
    JJS.CharacterController3D cC3D;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<JJS.PlayerControllerWIYH>();
        cC3D = animator.transform.gameObject.GetComponent<JJS.CharacterController3D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.InputMove();
        player.InputRun();
        player.InputJump();
        player.RotationNormal();
        animator.SetFloat("HorizonVelocity", (player.isMoved ? (player.isRun ? 1.0f : 0.46f) : 0.0f));
    }
}
