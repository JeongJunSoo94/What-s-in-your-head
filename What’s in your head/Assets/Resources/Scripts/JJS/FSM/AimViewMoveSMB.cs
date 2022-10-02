using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimViewMoveSMB : StateMachineBehaviour
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
        player.RotationAim();

        animator.SetFloat("MoveX", player.dir.normalized.x* (player.isRun ? 2.0f:1.0f));
        animator.SetFloat("MoveZ", player.dir.normalized.z * (player.isRun ? 2.0f : 1.0f));
    }
}
